using ELearningPlatform.Model.Payment.Dtos;
using ELearningPlatform.Service.Payment.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ELearningPlatform.Api.Controllers;

[Authorize]
public class PaymentsController(IPaymentService paymentService) : CustomBaseController
{
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest paymentRequest) =>
        CreateActionResult(await paymentService.ProcessPaymentAsync(paymentRequest));
}
