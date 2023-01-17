using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.POST;

internal sealed class Request
{
    /// <summary>
    ///     Id of the previous edition of the campaign. Used to import previous benficiaries
    /// </summary>
    [FromRoute]
    public int PreviousCampaignId { get; set; }

    /// <summary>
    ///     New edition of the campaign
    /// </summary>
    public string Edition { get; set; } = string.Empty;

    /// <summary>
    ///     Cause for which the campaign is being created
    /// </summary>
    public string CommunityId { get; set; } = string.Empty;
}

internal class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(t => t.PreviousCampaignId)
            .NotEmpty().WithMessage("La campaña anterior es requerida");

        RuleFor(t => t.CommunityId)
            .NotEmpty().WithMessage("La comunidad es requerida");

        RuleFor(t => t.Edition)
            .NotEmpty().WithMessage("La edición es requerida");
    }
}