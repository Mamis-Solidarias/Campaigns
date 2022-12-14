using FastEndpoints;
using FluentValidation;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.DELETE;

internal sealed class Request
{
    /// <summary>
    /// Campaigns Id
    /// </summary>
    public int Id { get; init; }
}

internal sealed class RequestValidator: Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(t => t.Id)
            .GreaterThan(0).WithMessage("El Id de la campaña debe ser mayor a 0");
    }
}