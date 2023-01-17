using Microsoft.AspNetCore.Http;

namespace MamisSolidarias.HttpClient.Campaigns.Services;

/// <summary>
///     Service to obtain headers from the current HTTP Context
/// </summary>
internal class HeaderService : IHeaderService
{
    private readonly IHttpContextAccessor? _contextAccessor;

    public HeaderService(IHttpContextAccessor? contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    /// <summary>
    ///     It retrieves the Authorization Header, if it exists
    /// </summary>
    /// <returns>The Authorization header</returns>
    public string? GetAuthorization()
    {
        return _contextAccessor?.HttpContext?.Request.Headers["Authorization"];
    }
}

/// <summary>
///     Service to obtain headers from the current HTTP Context
/// </summary>
internal interface IHeaderService
{
    public string? GetAuthorization();
}