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
    }
}