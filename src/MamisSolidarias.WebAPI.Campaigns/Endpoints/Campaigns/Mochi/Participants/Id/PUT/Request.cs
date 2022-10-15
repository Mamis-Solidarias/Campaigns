using FastEndpoints;
using FluentValidation;
using MamisSolidarias.Infrastructure.Campaigns.Models;
using Microsoft.AspNetCore.Mvc;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.PUT;

public class Request
{
    /// <summary>
    /// The id of the participant
    /// </summary>
    [FromRoute] public int Id { get; set; }

    /// <summary>
    /// The assigned donor
    /// </summary>
    public int DonorId { get; set; }

    public string DonationType { get; set; } = string.Empty;
    
    /// <summary>
    /// Donation drop off location
    /// </summary>
    public string? DonationDropOffLocation { get; set; }
}

internal class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(t=> t.Id)
            .NotEmpty().WithMessage("El id del participante no puede ser vacío")
            .GreaterThan(0).WithMessage("El id del participante debe ser mayor a 0");
        
        RuleFor(t=> t.DonorId)
            .NotEmpty().WithMessage("El id del donante no puede ser vacío")
            .GreaterThan(0).WithMessage("El id del donante debe ser mayor a 0");
        
        RuleFor(t=> t.DonationType)
            .NotEmpty().WithMessage("El tipo de donación no puede ser vacío")
            .IsEnumName(typeof(MochiDonationType),false).WithMessage("El tipo de donación no es válido");
        
        RuleFor(t=> t.DonationDropOffLocation)
            .MaximumLength(500).WithMessage("La ubicación de la donación no puede ser mayor a 500 caracteres");
    }
}