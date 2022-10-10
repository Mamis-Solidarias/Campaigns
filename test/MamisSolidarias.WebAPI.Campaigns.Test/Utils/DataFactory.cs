using System.Collections.Generic;
using System.Linq;
using Bogus;
using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal static class DataFactory
{
    
    
    public static MochiBuilder GetMochi() => new(db:null);

    public static MochiParticipantBuilder GetMochiParticipant() => new(db: null);

}