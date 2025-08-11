using FurryFriends.API.Library;
using FurryFriends.API.Models.VNPay;
using FurryFriends.Web.Services.IServices;
using Microsoft.Extensions.Logging;

namespace FurryFriends.Web.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<VnPayService> _logger;

        public VnPayService(IConfiguration configuration, ILogger<VnPayService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            try
            {
                var pay = new VnPayLibrary();

                // Thêm các tham số bắt buộc
                pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
                pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
                pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);

                // Tính toán tổng tiền (VNPay yêu cầu số tiền tính bằng VNĐ, không có phần thập phân)
                long amount = (long)(model.Amount * 100);
                pay.AddRequestData("vnp_Amount", amount.ToString());

                // Thêm thông tin thời gian và địa chỉ
                pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
                pay.AddRequestData("vnp_IpAddr", GetClientIpAddress(context));
                pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);

                // Thông tin đơn hàng
                string orderInfo = Uri.EscapeDataString($"{model.Name} - {model.OrderDescription}");
                pay.AddRequestData("vnp_OrderInfo", orderInfo);

                // Loại hàng hóa và URL callback
                pay.AddRequestData("vnp_OrderType", model.OrderType);
                pay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:PaymentBackReturnUrl"]);
                
                // Mã giao dịch duy nhất
                string txnRef = $"FF_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                pay.AddRequestData("vnp_TxnRef", txnRef);

                // Log thông tin request
                _logger.LogInformation("Creating VNPay payment URL for order: {OrderInfo}, Amount: {Amount}, TxnRef: {TxnRef}", 
                    orderInfo, amount, txnRef);

                // Lấy dữ liệu request để log
                var requestData = pay.GetRequestData();
                _logger.LogDebug("VNPay Request Data: {@RequestData}", requestData);

                // Tạo URL thanh toán
                var paymentUrl = pay.CreateRequestUrl(
                    _configuration["Vnpay:BaseUrl"],
                    _configuration["Vnpay:HashSecret"]
                );

                _logger.LogInformation("VNPay payment URL created successfully: {PaymentUrl}", paymentUrl);

                return paymentUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPay payment URL");
                throw new InvalidOperationException("Không thể tạo URL thanh toán VNPay", ex);
            }
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            try
            {
                var pay = new VnPayLibrary();
                var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

                // Log response để debug
                _logger.LogInformation("VNPay Payment Response - Success: {Success}, TransactionId: {TransactionId}, OrderId: {OrderId}, ResponseCode: {ResponseCode}", 
                    response.Success, response.TransactionId, response.OrderId, response.VnPayResponseCode);

                // Log tất cả query parameters
                var queryParams = collections.ToDictionary(k => k.Key, v => v.Value.ToString());
                _logger.LogDebug("VNPay Callback Query Parameters: {@QueryParams}", queryParams);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay payment response");
                
                // Trả về response lỗi
                return new PaymentResponseModel
                {
                    Success = false,
                    OrderDescription = "Lỗi xử lý phản hồi thanh toán",
                    VnPayResponseCode = "ERROR"
                };
            }
        }

        private string GetClientIpAddress(HttpContext context)
        {
            try
            {
                // Lấy IP từ header X-Forwarded-For (nếu có proxy)
                var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedHeader))
                {
                    return forwardedHeader.Split(',')[0].Trim();
                }

                // Lấy IP từ header X-Real-IP
                var realIpHeader = context.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIpHeader))
                {
                    return realIpHeader;
                }

                // Lấy IP trực tiếp từ connection
                var remoteIp = context.Connection.RemoteIpAddress;
                if (remoteIp != null)
                {
                    // Xử lý IPv6
                    if (remoteIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        remoteIp = System.Net.Dns.GetHostEntry(remoteIp).AddressList
                            .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    }
                    
                    if (remoteIp != null)
                    {
                        return remoteIp.ToString();
                    }
                }

                return "127.0.0.1";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error getting client IP address");
                return "127.0.0.1";
            }
        }
    }
}