using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Juntos.Participants.Id.PUT;

/// <param name="State">State of the donation</param>
internal sealed record Response(ParticipantState State);