using System.ComponentModel;

namespace Common.Enums;

public enum FilterType
{
    [Description("Точное сравнение")]
    AccurateComparison,

    [Description("Неточное сравнение")]
    InaccurateComparison,

    [Description("В диапазоне")]
    InRange,

    [Description("Вне диапазона")]
    OutRange,

    [Description("Больше")]
    GreaterThan,

    [Description("Больше или равно")]
    GreaterThanOrEqual,

    [Description("Меньше")]
    LessThan,

    [Description("Меньше или равно")]
    LessThanOrEqual,
}
