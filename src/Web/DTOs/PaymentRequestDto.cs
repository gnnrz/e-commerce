namespace CleanArchitecture.Web.DTOs;

public class PaymentRequestDto
{
    public string PaymentMethod { get; set; }

    public PaymentRequestDto(string paymentMethod)
    {
        PaymentMethod = paymentMethod;
    }
}
