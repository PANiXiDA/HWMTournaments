using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

using DTOs.Core;

namespace Common.Helpers;

public static class ApiErrorHelper
{
    private static readonly Regex DetailRegex = new(@"Detail\s*=\s*""(?<d>.*?)""", RegexOptions.Compiled);

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static string? TryExtractDetail(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var response = JsonSerializer.Deserialize<RestApiResponse<object>>(json, JsonOpts);
            if (response?.Failure?.Errors is { Count: > 0 })
            {
                var first = response.Failure.Errors.Values.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(first))
                {
                    return null;
                }

                var match = DetailRegex.Match(first);
                return match.Success ? match.Groups["d"].Value : first;
            }

            using var document = JsonDocument.Parse(json);
            if (document.RootElement.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.String)
            {
                return msg.GetString();
            }
            if (document.RootElement.TryGetProperty("error", out var err) && err.ValueKind == JsonValueKind.String)
            {
                return err.GetString();
            }
        }
        catch
        {
        }

        return null;
    }
}
