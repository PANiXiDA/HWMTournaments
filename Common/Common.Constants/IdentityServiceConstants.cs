namespace Common.Constants;

public static class IdentityServiceConstants
{
    public static class DevTemplateAspNetCoreAPI
    {
        public const string DevTemplateAspNetCoreAPIName = "dev_template_asp_net_core_api";
        public const string DevTemplateAspNetCoreAPISecret = "dev_template_asp_net_core_api_secret";
    }

    public static class GrantTypes
    {
        public const string Login = "login";
    }

    public static class Clients
    {
        public const string ReactClient = "react_client";
        public const string ReactAdmin = "react_admin";
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
