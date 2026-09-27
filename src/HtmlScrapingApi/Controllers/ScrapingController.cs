using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using TestAssignment.Exceptions;
using TestAssignment.Interfaces;
using TestAssignment.Models;

namespace TestAssignment.Controllers
{
    [ApiController]
    [Route("api/elements")]
    public class ScrapingController : ControllerBase
    {
        private readonly IValidator<ScrapingRequest> _validator;

        private readonly IScrapingService _service;

        public ScrapingController(IValidator<ScrapingRequest> validator, IScrapingService service)
        {
            _validator = validator;
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ScrapingRequest request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First();
                return BadRequest(new ScrapingResponse(
                    IsError: 1,
                    ErrorCode: firstError.ErrorCode,
                    ErrorMessage: firstError.ErrorMessage
                ));
            }

            try
            {
                var result = await _service.Process(request, ct);
                return Ok(result);
            }
            catch (Base64DecodeException ex)
            {
                return BadRequest(new ScrapingResponse(
                    IsError: 1,
                    ErrorCode: ex.ErrorCode,
                    ErrorMessage: ex.Message
                ));
            }
            catch (CryptographicException ex)
            {
                return BadRequest(new ScrapingResponse(
                    IsError: 1,
                    ErrorCode: "DECRYPTION_ERROR",
                    ErrorMessage: $"Ошибка дешифрования данных: {ex.Message}"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ScrapingResponse(
                    IsError: 1,
                    ErrorCode: "INTERNAL_SERVER_ERROR",
                    ErrorMessage: ex.Message
                ));
            }
        }
    }
}
