using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Web;

namespace Common.Helpers;

public static class QueryStringHelper
{
    public static string ToQueryString(object? parameters)
    {
        var pairs = BuildPairs(parameters);
        return pairs.Count == 0 ? string.Empty : "?" + string.Join("&", pairs);
    }

    public static string Append(string baseUrl, object? parameters)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = string.Empty;
        }

        var pairs = BuildPairs(parameters);
        if (pairs.Count == 0)
        {
            return baseUrl;
        }
        var querySeparator = baseUrl.Contains('?') ? "&" : "?";

        return baseUrl + querySeparator + string.Join("&", pairs);
    }

    public static string AppendMany(string baseUrl, params object?[] parts)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = string.Empty;
        }

        var allPairs = new List<string>();
        foreach (var part in parts)
        {
            allPairs.AddRange(BuildPairs(part));
        }

        if (allPairs.Count == 0)
        {
            return baseUrl;
        }
        var querySeparator = baseUrl.Contains('?') ? "&" : "?";

        return baseUrl + querySeparator + string.Join("&", allPairs);
    }

    private static List<string> BuildPairs(object? obj)
    {
        var pairs = new List<string>();
        if (obj is null)
        {
            return pairs;
        }

        if (obj is IReadOnlyDictionary<string, string?> readOnlyDictionary)
        {
            foreach (var keyValue in readOnlyDictionary)
            {
                pairs.Add($"{Encode(keyValue.Key)}={Encode(keyValue.Value)}");
            }
            return pairs;
        }
        if (obj is IDictionary dictionary)
        {
            foreach (DictionaryEntry keyValue in dictionary)
            {
                var key = Convert.ToString(keyValue.Key, CultureInfo.InvariantCulture) ?? string.Empty;
                var val = Convert.ToString(keyValue.Value, CultureInfo.InvariantCulture);
                pairs.Add($"{Encode(key)}={Encode(val)}");
            }
            return pairs;
        }

        var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var property in properties)
        {
            if (!property.CanRead)
            {
                continue;
            }

            var key = property.Name;
            var value = property.GetValue(obj);

            if (value is null)
            {
                continue;
            }

            if (value is IEnumerable sequence && value is not string)
            {
                foreach (var item in sequence)
                {
                    var s = ScalarToString(item);
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        pairs.Add($"{Encode(key)}={Encode(s)}");
                    }
                }
            }
            else
            {
                var s = ScalarToString(value);
                if (!string.IsNullOrWhiteSpace(s))
                {
                    pairs.Add($"{Encode(key)}={Encode(s)}");
                }
            }
        }

        return pairs;
    }

    private static string? ScalarToString(object? value)
    {
        if (value is null)
        {
            return null;
        }

        var type = Nullable.GetUnderlyingType(value.GetType()) ?? value.GetType();

        if (type == typeof(bool))
        {
            return (bool)value ? "true" : "false";
        }
        if (type == typeof(int) || type == typeof(long) || type == typeof(short) ||
            type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort) ||
            type == typeof(byte) || type == typeof(sbyte))
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
        if (type == typeof(double) || type == typeof(float) || type == typeof(decimal))
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
        if (type == typeof(string))
        {
            return (string)value;
        }
        if (type == typeof(Guid))
        {
            return ((Guid)value).ToString();
        }
        if (type.IsEnum)
        {
            return value.ToString();
        }
        if (type == typeof(DateTime))
        {
            return ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
        }
        if (type == typeof(DateTimeOffset))
        {
            return ((DateTimeOffset)value).ToString("o", CultureInfo.InvariantCulture);
        }
        if (type == typeof(DateOnly))
        {
            return ((dynamic)value).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        if (type == typeof(TimeOnly))
        {
            return ((dynamic)value).ToString("HH:mm:ss.fffffff", CultureInfo.InvariantCulture);
        }

        return Convert.ToString(value, CultureInfo.InvariantCulture);
    }

    private static string Encode(string? s)
    {
        return HttpUtility.UrlEncode(s ?? string.Empty);
    }
}
