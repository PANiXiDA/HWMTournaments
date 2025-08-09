using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Common.Helpers;

public static class DisplayNameHelper
{
    public static string GetDisplayName<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null)
        {
            return value.ToString();
        }

        var displayAttr = field
            .GetCustomAttributes(typeof(DisplayAttribute), false)
            .OfType<DisplayAttribute>()
            .FirstOrDefault();

        return displayAttr?.GetName() ?? value.ToString();
    }

    public static string GetDisplayName<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expression)
    {
        MemberExpression? member = expression.Body as MemberExpression;

        if (member == null && expression.Body is UnaryExpression unary && unary.Operand is MemberExpression inner)
        {
            member = inner;
        }
        if (member == null)
        {
            return string.Empty;
        }
        if (member.Member is not PropertyInfo prop)
        {
            return member.Member.Name;
        }

        var displayAttr = prop.GetCustomAttribute<DisplayAttribute>();
        return displayAttr?.GetName() ?? prop.Name;
    }
}
