using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Participants.Id.DELETE;

public class Request
{
    /// <summary>
    ///     Participant Id
    /// </summary>
    [FromRoute]
    public int Id { get; set; }
}

internal class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(t => t.Id)
            .NotEmpty().WithMessage("El Id del participante es requerido")
            .GreaterThan(0).WithMessage("El Id del participante debe ser mayor a 0");
    }
}