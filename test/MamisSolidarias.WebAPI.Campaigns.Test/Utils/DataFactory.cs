using MamisSolidarias.Infrastructure.Campaigns;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal class DataFactory
{
    private readonly CampaignsDbContext? _dbContext;

    public DataFactory(CampaignsDbContext? dbContext)
    {
        _dbContext = dbContext;
    }

    public MochiBuilder GenerateMochiCampaign() => new(db:_dbContext);
    public static MochiBuilder GetMochiCampaign() => new(db:null);
    
    public MochiParticipantBuilder GenerateMochiParticipant() => new(db:_dbContext);
    public static MochiParticipantBuilder GetMochiParticipant() => new(db: null);
    
    public JuntosBuilder GenerateJuntosCampaign() => new(db: _dbContext);
    public static JuntosBuilder GetJuntosCampaign() => new(db:null);
    
    public JuntosParticipantBuilder GenerateJuntosParticipant() => new(db: _dbContext);
    public static JuntosParticipantBuilder GetJuntosParticipant() => new(db: null);

}