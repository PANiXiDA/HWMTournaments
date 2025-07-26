using System;
using System.Text;

namespace Common.Helpers;

public static class RandomGeneratorHelper
{
    private const string DefaultAlphabet = "abcdefghijklmnopqrstuvwxyz0123456789";

    private static readonly Random _randomGenerator = new Random();

    public static string GenerateRandomString(int length = 32, string alphabet = DefaultAlphabet)
    {
        var builder = new StringBuilder();
        for (var i = 0; i < length; ++i)
        {
            builder.Append(alphabet[_randomGenerator.Next(0, alphabet.Length - 1)]);
        }

        return builder.ToString();
    }

    public static string GenerateRandomString(int minLength, int maxLength, string alphabet = DefaultAlphabet)
    {
        return GenerateRandomString(_randomGenerator.Next(minLength, maxLength), alphabet);
    }

    public static int GenerateRandomInt(int minValue, int maxValue)
    {
        return _randomGenerator.Next(minValue, maxValue + 1);
    }
}
