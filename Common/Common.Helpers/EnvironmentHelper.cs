using System;

using Common.Constants;

namespace Common.Helpers;

public static class EnvironmentHelper
{
    public static bool IsLocalHost => Environment.GetEnvironmentVariable(EnvironmentConstants.EnvironmentVariableName) == EnvironmentConstants.LocalHost;
    public static bool IsDevelopment => Environment.GetEnvironmentVariable(EnvironmentConstants.EnvironmentVariableName) == EnvironmentConstants.Development;
    public static bool IsProduction => Environment.GetEnvironmentVariable(EnvironmentConstants.EnvironmentVariableName) == EnvironmentConstants.Production;
}
