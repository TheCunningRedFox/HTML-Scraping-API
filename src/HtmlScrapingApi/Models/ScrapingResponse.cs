using System.Text.Json.Serialization;

namespace TestAssignment.Models
{
    public record class ScrapingResponse(
        [property: JsonPropertyName("is_error")] int IsError,
        [property: JsonPropertyName("error_code")] string? ErrorCode = null,
        [property: JsonPropertyName("error_message")] string? ErrorMessage = null,
        [property: JsonPropertyName("elements_count")] int ElementsCount = 0,
        [property: JsonPropertyName("emails_count")] int EmailsCount = 0,
        [property: JsonPropertyName("url")] string? Url = null,
        [property: JsonPropertyName("decrypted_plain_text")] string? DecryptedPlainText = null,
        [property: JsonPropertyName("elements_attr_list")] List<string> ElementsAttrList = null!,
        [property: JsonPropertyName("emails_list")] List<string> EmailsList = null!);
}
