using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.DELETE;

public sealed class Request
{
    [FromRoute] public int Id { get; set; }
}

internal class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(t => t.Id)
            .NotEmpty().WithMessage("El Id de la campaña no puede ser vacío")
            .GreaterThan(0).WithMessage("El Id de la campaña debe ser mayor a 0");
    }
}