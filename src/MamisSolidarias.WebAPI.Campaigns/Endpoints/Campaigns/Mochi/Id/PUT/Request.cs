using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.PUT;

public sealed class Request
{
    /// <summary>
    ///     Id of the campaign
    /// </summary>
    [FromRoute]
    public int Id { get; set; }

    /// <summary>
    ///     Provider data assigned to the campaign
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    ///     Description of the campaing
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Beneficiaries to add as participants
    /// </summary>
    public IEnumerable<int> AddedBeneficiaries { get; set; } = new List<int>();

    /// <summary>
    ///     Participants to remove from the campaign
    /// </summary>
    public IEnumerable<int> RemovedBeneficiaries { get; set; } = new List<int>();
}

internal class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(t => t.Id)
            .NotEmpty().WithMessage("El Id de la campaña no puede ser vacío")
            .GreaterThan(0).WithMessage("El Id de la campaña debe ser mayor a 0");

        RuleFor(t => t.Description)
            .MaximumLength(500).WithMessage("La descripción de la campaña no puede superar los 500 caracteres");

        RuleFor(t => t.Provider)
            .MaximumLength(300).WithMessage("El proveedor de la campaña no puede superar los 300 caracteres");
    }
}