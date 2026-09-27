using System.Text.Json.Serialization;

namespace TestAssignment.Models
{
    /// <summary>
    /// DTO входящего запроса
    /// </summary>
    /// <param name="Selector">CSS-селектор для поиска html-элементов на веб-странице.</param>
    /// <param name="Attribute">Наименования HTML атрибута.</param>
    /// <param name="UrlB64">Строка URL, закодированная в Base64</param>
    /// <param name="EncryptedTextBytesB64">Результат шифрования строки алгоритмом AES256 в режиме ECB с PaddingMode.None.</param>
    /// <param name="KeyBytesB64">Ключ для AES256, который использовался для шифрования текста.</param>
    /// <param name="PageB64">Код HTML-страницы, закодированный в Base64.</param>
    public record class ScrapingRequest(
        [property: JsonPropertyName("selector")] string Selector,
        [property: JsonPropertyName("attribute")] string Attribute,
        [property: JsonPropertyName("url_b64")] string UrlB64,
        [property: JsonPropertyName("encrypted_text_bytes_b64")] string EncryptedTextBytesB64,
        [property: JsonPropertyName("key_bytes_b64")] string KeyBytesB64,
        [property: JsonPropertyName("page_b64")] string PageB64);
}
