using CodeGeneration.ServerCodeGenerator.Enums;

namespace CodeGeneration.ServerCodeGenerator.Extensions;

internal static class NamingProvider
{
    internal static string GetPrefix(GeneratedFile layer) => layer switch
    {
        GeneratedFile.IDAL => "I",
        GeneratedFile.IBL => "I",
        _ => string.Empty
    };

    internal static string GetSuffix(GeneratedFile layer) => layer switch
    {
        GeneratedFile.Entity => string.Empty,
        GeneratedFile.SearchParams => "sSearchParams",
        GeneratedFile.ConvertParams => "sConvertParams",
        GeneratedFile.DALFilters => "sFilters",
        GeneratedFile.DALIncludes => "sIncludes",
        GeneratedFile.IDAL => "sDAL",
        GeneratedFile.DAL => "sDAL",
        GeneratedFile.IBL => "sBL",
        GeneratedFile.BL => "sBL",
        GeneratedFile.APIController => "sController",
        GeneratedFile.APIModel => "Model",
        _ => string.Empty
    };
}
