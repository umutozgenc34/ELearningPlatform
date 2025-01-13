using ELearningPlatform.Model.Payment.Dtos;
using ELearningPlatform.Service.Payment.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;
public class PaymentsController(IPaymentService paymentService) : CustomBaseController
{
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest paymentRequest)
    {
        var paymentResult = await paymentService.ProcessPaymentAsync(paymentRequest);

        if (paymentResult.IsSuccessful)
        {
            return Ok(paymentResult.Message); 
        }

        return BadRequest(paymentResult.Message); 
    }

}
