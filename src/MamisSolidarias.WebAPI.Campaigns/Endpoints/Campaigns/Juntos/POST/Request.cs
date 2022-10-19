using FastEndpoints;
using FluentValidation;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.POST;

public class Request
{
    /// <summary>
    /// Description of the campaign edition
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Provider for the campaign edition
    /// </summary>
    public string? Provider { get; set; }

    /// <summary>
    /// Edition of the campaign
    /// </summary>
    public string Edition { get; set; } = string.Empty;

    /// <summary>
    /// Community assigned to the campaign edition
    /// </summary>
    public string CommunityId { get; set; } = string.Empty;
    
    /// <summary>
    /// Expected amount of money collected for the campaign edition
    /// </summary>
    public decimal FundraiserGoal { get; set; }
    
    /// <summary>
    /// Participants of the campaign edition
    /// </summary>
    public IEnumerable<int> Beneficiaries { get; set; } = new List<int>();
}

internal class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(t=> t.Description)
            .MaximumLength(500).WithMessage("La descripción no puede tener más de 500 caracteres");
        
        RuleFor(t=> t.Provider)
            .MaximumLength(300).WithMessage("El proveedor no puede tener más de 300 caracteres");
        
        RuleFor(t=> t.Edition)
            .NotEmpty().WithMessage("La edición no puede estar vacía")
            .MaximumLength(10).WithMessage("La edición no puede tener más de 10 caracteres");
        
        RuleFor(t=> t.CommunityId)
            .NotEmpty().WithMessage("La comunidad no puede estar vacía");
        
        RuleFor(t=> t.FundraiserGoal)
            .GreaterThanOrEqualTo(0).WithMessage("El objetivo de recaudación debe ser mayor o igual a 0");
        
    }
}