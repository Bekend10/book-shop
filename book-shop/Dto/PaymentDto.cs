using book_shop.Enums;

namespace book_shop.Dto
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public int MethodId { get; set; }
        public DateTime PaymentDate { get; set; }
        public int Amount { get; set; }
        public PaymentEnumStatus PaymentStatus { get; set; }
        public UserDto User { get; set; }
    }

    public class CreatePaymentDto
    {
        public int OrderId { get; set; }
        public int MethodId { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentEnumStatus PaymentStatus { get; set; }
    }

    public class PaymentStatus
    {
        public int PaymentId { get; set; }
        public PaymentEnumStatus Status { get; set; }
    }
}