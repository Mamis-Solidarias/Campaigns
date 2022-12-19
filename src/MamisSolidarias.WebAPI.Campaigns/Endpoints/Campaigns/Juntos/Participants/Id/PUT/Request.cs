using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Participants.Id.PUT;


internal sealed class Request
{
    /// <summary>
    /// Id of the edition of the participant in a Juntos campaign edition
    /// </summary>
    [FromRoute] public int Id { get; init; }
    
    /// <summary>
    /// Id of the donor
    /// </summary>
    public int DonorId { get; init; }
    
}

internal sealed class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El Id del participante no puede ser menor o igual a 0");
        
        RuleFor(x => x.DonorId)
            .GreaterThan(0).WithMessage("El Id del donante no puede ser menor o igual a 0");
    }
} 