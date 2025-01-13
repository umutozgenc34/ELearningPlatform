namespace ELearningPlatform.Model.Payment.Dtos;

public class PaymentRequest
{
    public string CardHolderName { get; set; }
    public string CardNumber { get; set; }
    public string ExpiryDate { get; set; }
    public string Cvv { get; set; }
    public string OrderId { get; set; } 
}