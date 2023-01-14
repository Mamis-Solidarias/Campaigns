using FastEndpoints;
using FluentValidation;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Abrigaditos.Id.Delete;

public sealed class Request
{
    /// <summary>
    /// Campaign's id
    /// </summary>
    public int Id { get; set; }
}

internal sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(t => t.Id)
            .GreaterThan(0);
    }
}