using MamisSolidarias.Infrastructure.Campaigns.Models.Base;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.PUT;

/// <param name="State">State of the donation</param>
internal sealed record Response(ParticipantState State);