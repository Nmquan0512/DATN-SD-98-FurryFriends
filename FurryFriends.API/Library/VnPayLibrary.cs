using FurryFriends.API.Models.VNPay;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace FurryFriends.API.Library
{
    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }

    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public PaymentResponseModel GetFullResponseData(IQueryCollection collection, string hashSecret)
        {
            try
            {
                if (collection == null || !collection.Any())
                {
                    return new PaymentResponseModel { Success = false, OrderDescription = "Không có dữ liệu phản hồi" };
                }

                if (string.IsNullOrEmpty(hashSecret))
                {
                    throw new ArgumentException("HashSecret không được để trống", nameof(hashSecret));
                }

            var vnPay = new VnPayLibrary();
                
                // Lấy tất cả tham số từ VNPay
            foreach (var (key, value) in collection)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnPay.AddResponseData(key, value);
                }
            }

                // Lấy các tham số quan trọng
                var orderId = vnPay.GetResponseData("vnp_TxnRef");
                var vnPayTranId = vnPay.GetResponseData("vnp_TransactionNo");
            var vnpResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
                var vnpSecureHash = collection.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value;
            var orderInfo = vnPay.GetResponseData("vnp_OrderInfo");
                var amount = vnPay.GetResponseData("vnp_Amount");
                var bankCode = vnPay.GetResponseData("vnp_BankCode");

                // Validate dữ liệu bắt buộc
                if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(vnPayTranId))
                {
                    return new PaymentResponseModel { Success = false, OrderDescription = "Thiếu thông tin giao dịch" };
                }

                // Kiểm tra chữ ký
                var checkSignature = vnPay.ValidateSignature(vnpSecureHash, hashSecret);
            if (!checkSignature)
                {
                    return new PaymentResponseModel { Success = false, OrderDescription = "Chữ ký không hợp lệ" };
                }

                // Kiểm tra mã phản hồi
                bool isSuccess = vnpResponseCode == "00"; // 00 = thành công

                return new PaymentResponseModel
                {
                    Success = isSuccess,
                PaymentMethod = "VnPay",
                    OrderDescription = orderInfo ?? "Không có mô tả",
                    OrderId = orderId,
                    PaymentId = vnPayTranId,
                    TransactionId = vnPayTranId,
                Token = vnpSecureHash,
                VnPayResponseCode = vnpResponseCode
            };
            }
            catch (Exception ex)
            {
                // Log lỗi và trả về response lỗi
                return new PaymentResponseModel
                {
                    Success = false,
                    OrderDescription = $"Lỗi xử lý phản hồi: {ex.Message}",
                    VnPayResponseCode = "ERROR"
                };
            }
        }

        public string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();

                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không throw exception
                return "127.0.0.1";
            }

            return "127.0.0.1";
        }

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                _requestData[key] = value; // Sử dụng indexer để ghi đè nếu key đã tồn tại
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                _responseData[key] = value;
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
        }

        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentException("BaseUrl không được để trống", nameof(baseUrl));
            
            if (string.IsNullOrEmpty(vnpHashSecret))
                throw new ArgumentException("HashSecret không được để trống", nameof(vnpHashSecret));

            var data = new StringBuilder();

            // Lọc và sắp xếp các tham số
            var validParams = _requestData
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .OrderBy(kv => kv.Key, new VnPayCompare());

            foreach (var (key, value) in validParams)
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            var querystring = data.ToString();

            // Loại bỏ dấu & cuối cùng
            if (querystring.Length > 0)
            {
                querystring = querystring.Remove(querystring.Length - 1, 1);
            }

            // Tạo chữ ký
            var vnpSecureHash = HmacSha512(vnpHashSecret, querystring);
            
            // Tạo URL hoàn chỉnh
            var finalUrl = baseUrl + "?" + querystring + "&vnp_SecureHash=" + vnpSecureHash;

            return finalUrl;
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            if (string.IsNullOrEmpty(inputHash) || string.IsNullOrEmpty(secretKey))
                return false;

            try
        {
            var rspRaw = GetResponseData();
            var myChecksum = HmacSha512(secretKey, rspRaw);
                return myChecksum.Equals(inputHash, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private string HmacSha512(string key, string inputData)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(inputData))
                return string.Empty;

            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        public Dictionary<string, string> GetRequestData()
        {
            return _requestData.ToDictionary(k => k.Key, v => v.Value);
        }

        private string GetResponseData()
        {
            var data = new StringBuilder();
            
            // Loại bỏ các tham số không cần thiết cho việc tạo chữ ký
            var validParams = _responseData
                .Where(kv => kv.Key != "vnp_SecureHashType" && kv.Key != "vnp_SecureHash")
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .OrderBy(kv => kv.Key, new VnPayCompare());

            foreach (var (key, value) in validParams)
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            // Loại bỏ dấu & cuối cùng
            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }
    }
}
