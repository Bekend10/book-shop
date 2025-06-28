using payment_service.Models;
using payment_service.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace payment_service.Services.Implementations
{
    public class VnpayService : IVnpayService
    {
        private readonly IConfiguration _config;

        public VnpayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(VnpayPaymentRequest model, HttpContext context)
        {
            string vnp_Url = _config["Vnpay:vnp_Url"];
            string tmnCode = _config["Vnpay:vnp_TmnCode"];
            string hashSecret = _config["Vnpay:vnp_HashSecret"];
            string returnUrl = _config["Vnpay:vnp_ReturnUrl"];

            var vnp_Params = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", tmnCode },
                { "vnp_Amount", ((long)(model.Amount * 100)).ToString() },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", GetIpAddress(context) },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", model.OrderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_TxnRef", model.OrderId }
            };

            string signData = string.Join("&", vnp_Params.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            string hash = HmacSHA512(hashSecret, signData);

            var queryParams = new List<string>(vnp_Params.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            queryParams.Add($"vnp_SecureHashType=HMACSHA512");
            queryParams.Add($"vnp_SecureHash={hash}");

            return vnp_Url + "?" + string.Join("&", queryParams);
        }

        public bool ValidateReturnUrl(IQueryCollection query, out string status)
        {
            string hashSecret = _config["Vnpay:vnp_HashSecret"];
            string vnp_SecureHash = query["vnp_SecureHash"];

            var vnpData = query
                .Where(q => q.Key.StartsWith("vnp_") && q.Key != "vnp_SecureHash")
                .OrderBy(q => q.Key)
                .ToDictionary(k => k.Key, v => v.Value.ToString());

            var rawData = string.Join('&', vnpData.Select(x => $"{x.Key}={x.Value}"));
            string calculatedHash = HmacSHA512(hashSecret, rawData);

            if (calculatedHash == vnp_SecureHash)
            {
                status = query["vnp_ResponseCode"] == "00" ? "Success" : "Fail";
                return true;
            }

            status = "Invalid signature";
            return false;
        }

        public string HmacSHA512(string key, string data)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public string GetIpAddress(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();
            return (ip == "::1" || string.IsNullOrEmpty(ip)) ? "127.0.0.1" : ip;
        }
    }
}
