using System.Collections.Generic;
using System.Linq;
using MamisSolidarias.Infrastructure.Campaigns;
using MamisSolidarias.WebAPI.Campaigns.Utils.Abrigaditos;
using MamisSolidarias.WebAPI.Campaigns.Utils.JuntosALaPar;
using MamisSolidarias.WebAPI.Campaigns.Utils.Mochi;

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
        return new MochiBuilder(_dbContext);
    }

    public static MochiBuilder GetMochiCampaign()
    {
        return new MochiBuilder(db: null);
    }

    public MochiParticipantBuilder GenerateMochiParticipant()
    {
        return new MochiParticipantBuilder(_dbContext);
    }

    public static MochiParticipantBuilder GetMochiParticipant()
    {
        return new MochiParticipantBuilder(db: null);
    }

    public static IEnumerable<MochiParticipantBuilder> GetMochiParticipants(int n)
    {
        return Enumerable.Range(0, n).Select(_ => new MochiParticipantBuilder());
    }

    public JuntosBuilder GenerateJuntosCampaign()
    {
        return new JuntosBuilder(_dbContext);
    }

    public static JuntosBuilder GetJuntosCampaign()
    {
        return new JuntosBuilder(db: null);
    }

    public JuntosParticipantBuilder GenerateJuntosParticipant()
    {
        return new JuntosParticipantBuilder(_dbContext);
    }

    public static JuntosParticipantBuilder GetJuntosParticipant()
    {
        return new JuntosParticipantBuilder(db: null);
    }
    public static IEnumerable<JuntosParticipantBuilder> GetJuntosParticipants(int n)
    {
        return Enumerable.Range(0, n).Select(_ => new JuntosParticipantBuilder());
    }

    public AbrigaditosCampaignBuilder GenerateAbrigaditosCampaign()
    {
        return new AbrigaditosCampaignBuilder(_dbContext);
    }

    public static AbrigaditosCampaignBuilder GetAbrigaditosCampaign()
    {
        return new AbrigaditosCampaignBuilder(db: null);
    }

    public AbrigaditosParticipantBuilder GenerateAbrigaditosParticipant()
    {
        return new AbrigaditosParticipantBuilder(_dbContext);
    }

    public static IEnumerable<AbrigaditosParticipantBuilder> GetAbrigaditosParticipants(int n)
    {
        return Enumerable.Range(0, n).Select(_ => new AbrigaditosParticipantBuilder());
    }
}