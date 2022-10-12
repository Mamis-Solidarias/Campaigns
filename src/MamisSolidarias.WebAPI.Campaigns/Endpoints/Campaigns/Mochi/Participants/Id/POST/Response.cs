using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Participants.Id.POST;

/// <param name="State">State of the donation</param>
internal sealed record Response(MochiParticipantState State);