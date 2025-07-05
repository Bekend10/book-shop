using System.Net.Http;
using System.Text;
using System.Text.Json;
using book_shop.Enums;
using book_shop.Models;
using book_shop.Repositories.Interfaces;
using book_shop.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace book_shop.Services.Implementations
{
    public class VnpayService : IVnpayService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        public VnpayService(IHttpClientFactory httpClientFactory, IOrderRepository orderRepository, IPaymentRepository paymentRepository)
        {
            _httpClientFactory = httpClientFactory;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<string> CreatePaymentUrl(VnpayPaymentRequest model, HttpContext context)
        {
            var client = _httpClientFactory.CreateClient("VnpayServiceClient");

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/v1/payments/create-by-vnpay", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to create payment URL from vnpay.service");

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);
            return doc.RootElement.GetProperty("paymentUrl").GetString()!;
        }

        public async Task<(bool isValid, string status)> ValidateReturnUrl(IQueryCollection query)
        {
            var client = _httpClientFactory.CreateClient("VnpayServiceClient");

            var queryString = QueryString.Create(query).ToString();
            var response = await client.GetAsync($"/api/v1/payments/vnpay-return{queryString}");

            if (!response.IsSuccessStatusCode)
                return (false, "Failed to validate signature");

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);

            string status = doc.RootElement.GetProperty("status").GetString() ?? "Unknown";
            string orderIdStr = query["vnp_TxnRef"];

            if (!int.TryParse(orderIdStr, out int orderId))
                return (false, "Invalid Order Id");

            var order = await _orderRepository.GetByIdAsync(orderId);
            var payment = await _paymentRepository.GetByOrderId(orderId);

            if (order == null || payment == null)
                return (false, "Order or Payment not found");

            if (status == "Success")
            {
                order.status = OrderEnumStatus.OrderStatus.Delivered;
                payment.payment_status = PaymentEnumStatus.Completed;
            }
            else
            {
                order.status = OrderEnumStatus.OrderStatus.Cancelled;
                payment.payment_status = PaymentEnumStatus.Failed;
            }

            await _orderRepository.UpdateAsync(order);
            await _paymentRepository.UpdateAsync(payment);

            return (true, status);
        }

    }
}
