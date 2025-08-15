namespace Common.Constants;

public static class IdentityServiceConstants
{
    public static class DevTemplateAspNetCoreAPI
    {
        public const string DevTemplateAspNetCoreAPIName = "hwm_tournaments_api";
        public const string DevTemplateAspNetCoreAPISecret = "hwm_tournaments_api_secret";
        public const string OfflineAccess = "offline_access";
    }

    public static class GrantTypes
    {
        public const string Login = "login";
        public const string Refresh = "refresh_token";
    }

    public static class Clients
    {
        public const string Blazor = "blazor";
    }

    public static class CustomJwtClaimTypes
    {
        public const string UserId = "user_id";
    }

    public static class CorsPolicies
    {
        public const string AllowAll = "AllowAll";
    }

    public static class RequestParameters
    {
        public const string Login = "login";
        public const string Password = "password";
    }

    public static class AuthenticationMethods
    {
        public const string Custom = "custom";
    }
}
