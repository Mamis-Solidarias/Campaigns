namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal static class DataFactory
{
    public static MochiBuilder GetMochi() => new(db:null);

    public static MochiParticipantBuilder GetMochiParticipant() => new(db: null);
    
    public static JuntosBuilder GetJuntosCampaign() => new(db:null);

    public static JuntosParticipantBuilder GetJuntosParticipant() => new(db: null);

}