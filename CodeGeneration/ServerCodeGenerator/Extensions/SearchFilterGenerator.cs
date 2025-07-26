using System.Collections.Generic;

using Common.Enums;

using Microsoft.CodeAnalysis;

namespace CodeGeneration.ServerCodeGenerator.Extensions;

internal static class SearchFilterGenerator
{
    internal static IEnumerable<string> GetSearchParams(INamedTypeSymbol model)
    {
        foreach (var property in model.GetMembers().OfType<IPropertySymbol>())
        {
            var def = AttributesParser.TryParseSearchParams(property);
            if (def is null) continue;

            var propertyType = property.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Replace("?", ""); ;

            if (def.Kind == SearchParamType.Value)
            {
                yield return $"public {propertyType}? {def.Name} {{ get; set; }}";
            }
            else
            {
                yield return $"public {propertyType}? {def.Left} {{ get; set; }}";
                yield return $"public {propertyType}? {def.Right} {{ get; set; }}";
            }
        }
    }

    internal static IEnumerable<string> GetFilters(INamedTypeSymbol model)
    {
        foreach (var property in model.GetMembers().OfType<IPropertySymbol>())
        {
            var def = AttributesParser.TryParseFilter(property);
            if (def is null) continue;

            var isString = property.Type.SpecialType == SpecialType.System_String;
            var isValType = property.Type.IsValueType;
            string P(string n) => $"searchParams.{n}";
            string Val(string n) => isValType ? n + ".Value" : n;
            string Check(string n) => isString
                ? $"!string.IsNullOrEmpty({n})"
                : isValType
                    ? $"{n}.HasValue"
                    : $"{n} != null";

            var itemPrefix = "item";
            var item = itemPrefix + "." + def.BaseName;
            var dbQuery = "dbObjects";

            switch (def.Kind)
            {
                case FilterType.AccurateComparison:
                    yield return $"if ({Check(P(def.Name))})";
                    yield return "{";
                    yield return isString
                        ? $"    {dbQuery} = {dbQuery}.Where({itemPrefix} => {item}.ToLower() == {P(def.Name)}.ToLower().Trim());"
                        : $"    {dbQuery} = {dbQuery}.Where({itemPrefix} => {item} == {Val(P(def.Name))});";
                    yield return "}";
                    break;

                case FilterType.InaccurateComparison:
                    yield return $"if ({Check(P(def.Name))})";
                    yield return "{";
                    yield return isString
                        ? $"    {dbQuery} = {dbQuery}.Where({itemPrefix} => {item}.ToLower().Contains({P(def.Name)}.ToLower().Trim()));"
                        : $"    {dbQuery} = {dbQuery}" + $".Where({itemPrefix} => {item} == {Val(P(def.Name))});"; ;
                    yield return "}";
                    break;

                case FilterType.InRange:
                    yield return $"if ({Check(P(def.Left))})";
                    yield return "{";
                    yield return $"    {dbQuery} = {dbQuery}.Where({itemPrefix} => {item} >= {Val(P(def.Left))});";
                    yield return "}";
                    yield return $"if ({Check(P(def.Right))})";
                    yield return "{";
                    yield return $"    {dbQuery} = {dbQuery}.Where({itemPrefix} => {item} <= {Val(P(def.Right))});";
                    yield return "}";
                    break;

                case FilterType.OutRange:
                    yield return $"if ({Check(P(def.Left))} && {Check(P(def.Right))})";
                    yield return "{";
                    yield return $"    {dbQuery} = {dbQuery}.Where({itemPrefix} => {item} < {Val(P(def.Left))} || {item} > {Val(P(def.Right))});";
                    yield return "}";
                    break;

                case FilterType.GreaterThan:
                    yield return $"if ({Check(P(def.Name))})";
                    yield return "{";
                    yield return $"    {dbQuery} = {dbQuery}.Where({itemPrefix} => {item} > {Val(P(def.Name))});";
                    yield return "}";
                    break;

                case FilterType.GreaterThanOrEqual:
                    yield return $"if ({Check(P(def.Name))})";
                    yield return "{";
                    yield return $"    {dbQuery} = {dbQuery}.Where({itemPrefix} => {item} >= {Val(P(def.Name))});";
                    yield return "}";
                    break;

                case FilterType.LessThan:
                    yield return $"if ({Check(P(def.Name))})";
                    yield return "{";
                    yield return $"    {dbQuery} = {dbQuery}.Where({itemPrefix} => {item} < {Val(P(def.Name))});";
                    yield return "}";
                    break;

                case FilterType.LessThanOrEqual:
                    yield return $"if ({Check(P(def.Name))})";
                    yield return "{";
                    yield return $"    {dbQuery} = {dbQuery}.Where({itemPrefix} => {item} <= {Val(P(def.Name))});";
                    yield return "}";
                    break;
            }
        }
    }
}
