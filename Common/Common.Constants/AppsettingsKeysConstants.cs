namespace Common.Constants;

public static class AppsettingsKeysConstants
{
    #region connection strings

    public const string DefaultDbConnectionString = "DefaultConnectionString";

    #endregion

    #region http clients

    public const string ServerAPIBaseAddress = "HttpClients:ServerAPI:BaseAddress";
    public const string IdentityServiceHTTPBaseAddress = "HttpClients:IdentityService:BaseAddress";

    #endregion

    #region gRPC clients

    public const string IdentityServiceBaseAddress = "GrpcClients:IdentityService:BaseAddress";

    #endregion
}
