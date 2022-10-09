using System.Collections.Generic;
using System.Linq;
using Bogus;
using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal static class DataFactory
{
    private static readonly Faker<Mochi> UserGenerator = new Faker<Mochi>()
        .RuleFor(t => t.Id, f => f.Random.Int())
        .RuleFor(t => t.Edition, f => f.Name.FindName());
    
    public static Mochi GetUser()
    {
        return UserGenerator.Generate();
    }

    public static IEnumerable<Mochi> GetUsers(int n)
    {
        return Enumerable.Range(0, n).Select(_ => GetUser());
    }
}