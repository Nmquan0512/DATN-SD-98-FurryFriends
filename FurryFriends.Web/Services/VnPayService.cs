using FurryFriends.API.Library;
using FurryFriends.API.Models.VNPay;
using FurryFriends.Web.Services.IServices;

namespace FurryFriends.Web.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var pay = new VnPayLibrary();

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);

            long amount = (long)(model.Amount * 100);
            pay.AddRequestData("vnp_Amount", amount.ToString());

            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", context.Connection.RemoteIpAddress?.ToString());
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);

            string orderInfo = Uri.EscapeDataString($"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderInfo", orderInfo);

            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:PaymentBackReturnUrl"]);
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

            // === LOG DEBUG - In toàn bộ tham số gửi sang VNPAY ===
            var requestData = pay.GetRequestData(); // Bạn thêm hàm này trong VnPayLibrary để trả về Dictionary<string,string>

            Console.WriteLine("===== VNPAY REQUEST DATA =====");
            foreach (var kv in requestData)
            {
                Console.WriteLine($"{kv.Key} = {kv.Value}");
            }

            // Log dạng JSON đẹp
            var json = System.Text.Json.JsonSerializer.Serialize(
                requestData,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
            );
            Console.WriteLine(json);

            // === Tạo URL thanh toán và log ra ===
            var paymentUrl = pay.CreateRequestUrl(
                _configuration["Vnpay:BaseUrl"],
                _configuration["Vnpay:HashSecret"]
            );

            Console.WriteLine("===== VNPAY PAYMENT URL =====");
            Console.WriteLine(paymentUrl);

            return paymentUrl;
        }




        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

            return response;
        }
    }
}
