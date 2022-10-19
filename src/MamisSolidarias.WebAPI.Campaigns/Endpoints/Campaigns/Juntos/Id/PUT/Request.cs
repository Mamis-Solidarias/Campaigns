using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Id.PUT;

internal sealed class Request
{
    /// <summary>
    ///     Campaign Id
    /// </summary>
    [FromRoute]
    public int Id { get; set; }

    /// <summary>
    ///     Description of the campaign edition
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Provider for the campaign edition
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    ///     Expected amount of money collected for the campaign edition
    /// </summary>
    public decimal FundraiserGoal { get; set; }

    /// <summary>
    ///     Participants of the campaign edition
    /// </summary>
    public IEnumerable<int> AddedBeneficiaries { get; set; } = new List<int>();

    /// <summary>
    ///     Participants of the campaign edition
    /// </summary>
    public IEnumerable<int> RemovedBeneficiaries { get; set; } = new List<int>();
}

internal class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(t => t.Id)
            .GreaterThan(0).WithMessage("El Id de la campaña debe ser mayor a 0");

        RuleFor(t => t.Description)
            .MaximumLength(500).WithMessage("La descripción no puede tener más de 500 caracteres");

        RuleFor(t => t.Provider)
            .MaximumLength(300).WithMessage("El proveedor no puede tener más de 300 caracteres");

        RuleFor(t => t.FundraiserGoal)
            .GreaterThan(0).WithMessage("El objetivo de recaudación debe ser mayor o igual a 0")
            .NotEmpty().WithMessage("El objetivo de recaudación no puede ser vacío");

        RuleFor(t => t.Provider)
            .NotEmpty().WithMessage("El proveedor no puede ser vacío");
    }
}