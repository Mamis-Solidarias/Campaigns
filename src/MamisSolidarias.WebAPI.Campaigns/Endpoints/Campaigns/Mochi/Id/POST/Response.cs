namespace MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Mochi.Id.POST;


/// <param name="Edition">Created Edition</param>
/// <param name="Community">For which community this campaign is</param>
internal sealed record Response(string Edition, string Community);