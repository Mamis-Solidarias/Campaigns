using System.Collections.Generic;
using System.Linq;
using MamisSolidarias.Infrastructure.Campaigns;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal class DataFactory
{
    private readonly CampaignsDbContext? _dbContext;

    public DataFactory(CampaignsDbContext? dbContext)
    {
        _dbContext = dbContext;
    }

    public MochiBuilder GenerateMochiCampaign()
    {
        return new(_dbContext);
    }

    public static MochiBuilder GetMochiCampaign()
    {
        return new(db: null);
    }

    public MochiParticipantBuilder GenerateMochiParticipant()
    {
        return new(_dbContext);
    }

    public static MochiParticipantBuilder GetMochiParticipant()
    {
        return new(db: null);
    }

    public JuntosBuilder GenerateJuntosCampaign()
    {
        return new(_dbContext);
    }

    public static JuntosBuilder GetJuntosCampaign()
    {
        return new(db: null);
    }

    public JuntosParticipantBuilder GenerateJuntosParticipant()
    {
        return new(_dbContext);
    }

    public static JuntosParticipantBuilder GetJuntosParticipant()
    {
        return new(db: null);
    }

    public AbrigaditosCampaignBuilder GenerateAbrigaditosCampaign()
    {
        return new(_dbContext);
    }

    public static AbrigaditosCampaignBuilder GetAbrigaditosCampaign()
    {
        return new(db: null);
    }

    public static IEnumerable<AbrigaditosParticipantBuilder> GetAbrigaditosParticipants(int n)
    {
        return Enumerable.Range(0, n).Select(_ => new AbrigaditosParticipantBuilder());
    }
}
