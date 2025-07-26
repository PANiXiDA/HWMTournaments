using System.Linq;

namespace Common.Helpers;

public static class PhoneHelper
{
    public static string CleanPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone))
        {
            return phone;
        }

        phone = new string(phone.Where(char.IsDigit).ToArray());
        if (phone.Length < 10 || phone.StartsWith("+"))
        {
            return phone;
        }
        if (phone.Length == 10)
        {
            phone = "7" + phone;
        }
        else if (phone.StartsWith("8"))
        {
            phone = "7" + phone.Substring(1);
        }

        return '+' + phone;
    }
}
