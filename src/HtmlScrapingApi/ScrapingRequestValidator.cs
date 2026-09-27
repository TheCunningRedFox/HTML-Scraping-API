using FluentValidation;
using TestAssignment.Models;

namespace TestAssignment.Validation;

public class ScrapingRequestValidator : AbstractValidator<ScrapingRequest>
{
    public ScrapingRequestValidator()
    {
        RuleFor(x => x.Selector)
            .NotEmpty().WithError("VALIDATION_ERROR", "Селектор не может быть пустым.");

        RuleFor(x => x.Attribute)
            .NotEmpty().WithError("VALIDATION_ERROR", "Атрибут не может быть пустым.");

        RuleFor(x => x.UrlB64)
            .NotEmpty().WithError("VALIDATION_ERROR", "Параметр url_b64 не может быть пустым.");

        RuleFor(x => x.PageB64)
            .NotEmpty().WithError("VALIDATION_ERROR", "Параметр page_b64 не может быть пустым.");

        RuleFor(x => x.EncryptedTextBytesB64)
            .NotEmpty().WithError("VALIDATION_ERROR", "Параметр encrypted_text_bytes_b64 не может быть пустым.");

        RuleFor(x => x.KeyBytesB64)
            .NotEmpty().WithError("VALIDATION_ERROR", "Параметр key_bytes_b64 не может быть пустым.");
    }
}

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, string code, string message)
    {
        return rule.WithErrorCode(code).WithMessage(message);
    }
}