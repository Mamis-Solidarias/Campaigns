using System.Collections.Generic;
using System.Linq;
using Bogus;
using MamisSolidarias.Infrastructure.Campaigns.Models;

namespace MamisSolidarias.WebAPI.Campaigns.Utils;

internal static class DataFactory
{
    private static readonly Faker<User> UserGenerator = new Faker<User>()
        .RuleFor(t => t.Id, f => f.Random.Int())
        .RuleFor(t => t.Name, f => f.Name.FindName());
    
    public static User GetUser()
    {
        return UserGenerator.Generate();
    }

    public static IEnumerable<User> GetUsers(int n)
    {
        return Enumerable.Range(0, n).Select(_ => GetUser());
    }
}