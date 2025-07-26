﻿using System;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;

using Scriban;

namespace CodeGeneration.ServerCodeGenerator.Extensions;

internal static class TemplatesRenderer
{
    public static string Render(string templatePath, INamedTypeSymbol model)
    {
        var idType = PropertiesExtractor.GetIdType(model);
        var properties = model.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => !p.IsStatic)
            .Select(PropertiesExtractor.GetPropertiesInfo)
            .ToList();

        var searchParams = string.Join(Environment.NewLine, SearchFilterGenerator.GetSearchParams(model));
        var filters = string.Join(Environment.NewLine, SearchFilterGenerator.GetFilters(model));

        var text = File.ReadAllText(templatePath);
        var template = Template.Parse(text);
        if (template.HasErrors)
        {
            foreach (var message in template.Messages)
            {
                Console.Error.WriteLine(message);
            }
            throw new InvalidOperationException("Ошибки в шаблоне Scriban.");
        }

        return template.Render(new
        {
            model_name = model.Name,
            model_camel_name = char.ToLowerInvariant(model.Name[0]) + model.Name[1..],
            id_type = idType,
            properties = properties,
            search_params = searchParams,
            filters = filters
        });
    }
}
