using AngleSharp.Html.Parser;
using Dapper;
using Npgsql;
using System.Security.Cryptography;
using System.Text;
using TestAssignment.Exceptions;
using TestAssignment.Interfaces;
using TestAssignment.Models;

namespace TestAssignment.Services
{
    public class ScrapingService(IHtmlParser parser, NpgsqlDataSource dataSource) : IScrapingService
    {
        public async Task<ScrapingResponse> Process(ScrapingRequest request, CancellationToken ct)
        {
            var decodedUrl = Encoding.UTF8.GetString(DecodeBase64(request.UrlB64, nameof(request.UrlB64)));
            var pageBytes = DecodeBase64(request.PageB64, nameof(request.PageB64));

            using var htmlStream = new MemoryStream(pageBytes);
            using var document = await parser.ParseDocumentAsync(htmlStream, ct);

            var htmlElements = document.QuerySelectorAll(request.Selector);
            var attributeValues = new List<string>(htmlElements.Length);
            var dbElements = new List<ElementDbModel>(htmlElements.Length);

            var rawSourceText = document.Source.Text;

            var emailList = EmailExtractor.Extract(rawSourceText);

            var cipherTextBytes = DecodeBase64(request.EncryptedTextBytesB64, nameof(request.EncryptedTextBytesB64));

            var keyBytes = DecodeBase64(request.KeyBytesB64, nameof(request.KeyBytesB64));

            var decryptedText = DecryptAes256EcbNoPadding(cipherTextBytes, keyBytes);

            foreach (var element in htmlElements)
            {
                var attrValue = element.GetAttribute(request.Attribute) ?? string.Empty;

                attributeValues.Add(attrValue);
                dbElements.Add(new ElementDbModel(attrValue, element.OuterHtml));
            }

            if (dbElements.Count > 0)
            {
                await using var connection = await dataSource.OpenConnectionAsync(ct);

                const string sql = """
            INSERT INTO elements (value, html_code)
            VALUES (@Value, @HtmlCode);
            """;

                var command = new CommandDefinition(sql, dbElements, cancellationToken: ct);
                await connection.ExecuteAsync(command);
            }

            return new ScrapingResponse(
                IsError: 0,
                ElementsCount: htmlElements.Length,
                EmailsCount: emailList.Count,
                Url: decodedUrl,
                DecryptedPlainText: decryptedText,
                ElementsAttrList: attributeValues,
                EmailsList: emailList);
        }

        private string DecryptAes256EcbNoPadding(byte[] cipherText, byte[] key)
        {
            if (cipherText == null || cipherText.Length == 0)
                throw new CryptographicException("Шифротекст пуст или null.");

            if (key == null || key.Length != 32)
                throw new CryptographicException("Ключ AES-256 должен быть строго 32 байта.");

            if (cipherText.Length % 16 != 0)
                throw new CryptographicException("Длина шифротекста должна быть кратна 16 байтам.");

            using Aes aes = Aes.Create();
            aes.Key = key;

            byte[] decryptedBytes = aes.DecryptEcb(
                cipherText,
                PaddingMode.None
            );

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        private static byte[] DecodeBase64(
            string value,
            string parameterName)
        {
            try
            {
                return Convert.FromBase64String(value);
            }
            catch (FormatException ex)
            {
                throw new Base64DecodeException(
                    "BASE64_DECODE_ERROR",
                    $"Некорректный Base64 в параметре {parameterName}.",
                    ex);
            }
        }
    }
}
