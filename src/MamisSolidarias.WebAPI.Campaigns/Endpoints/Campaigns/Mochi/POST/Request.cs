using FastEndpoints;
using FluentValidation;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.POST;

internal sealed class Request
{
    /// <summary>
    /// Which edition the campaign is for.
    /// </summary>
    public string Edition { get; set; } = string.Empty;

    /// <summary>
    /// For which community the campaign is for.
    /// </summary>
    public string CommunityId { get; set; } = string.Empty;

    /// <summary>
    /// List of the beneficiaries Ids that will be added to the campaign.
    /// </summary>
    public IEnumerable<int> Beneficiaries { get; set; } = new List<int>();

    /// <summary>
    /// Description of the campaign.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Data to contact the provider for the campaign
    /// </summary>
    public string? Provider { get; set; }
}


internal class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(t => t.Edition)
            .NotEmpty().WithMessage("La edición es requerida.");
        
        RuleFor(t=>t.CommunityId)
            .NotEmpty().WithMessage("La comunidad es requerida.");

        RuleFor(t => t.Beneficiaries)
            .NotNull().WithMessage("Los beneficiarios son requeridos.");
        
        RuleFor(t=> t.Provider)
            .MaximumLength(300).WithMessage("El proveedor no puede tener más de 300 caracteres.");
        
        RuleFor(t=> t.Description)
            .MaximumLength(500).WithMessage("La descripción no puede tener más de 500 caracteres.");
    }
}