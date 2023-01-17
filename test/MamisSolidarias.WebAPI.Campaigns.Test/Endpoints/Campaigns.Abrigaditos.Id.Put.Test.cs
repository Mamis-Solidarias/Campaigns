using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MamisSolidarias.WebAPI.Campaigns.Endpoints.Campaigns.Abrigaditos.Id.PUT;
using MamisSolidarias.WebAPI.Campaigns.Utils;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MamisSolidarias.WebAPI.Campaigns.Endpoints;

internal class Campaigns_Abrigaditos_Id_Put_Test : EndpointTest<Endpoint>
{
    protected override object?[] ConstructorArguments =>
        new object?[] { _dbContext, _mockBus.Object };

    [Test]
    public async Task WhenTheCampaignDoesNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var campaign = DataFactory.GetAbrigaditosCampaign().Build();
        var request = new Request
        {
            Id = campaign.Id,
            Description = "New Description",
            Provider = null,
            FundraiserGoal = 999
        };

        // Act
        await _endpoint.HandleAsync(request, default);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(404);
    }

    [Test]
    public async Task WhenTheCampaignExists_UpdatingOnlyValues_ShouldSucceed()
    {
        // Arrange
        var campaign = _dataFactory.GenerateAbrigaditosCampaign().Build();
        var request = new Request
        {
            Id = campaign.Id,
            Description = "New Description",
            Provider = "New Provider",
            FundraiserGoal = 999
        };

        // Act
        await _endpoint.HandleAsync(request, default);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        var updatedCampaign = await _dbContext.AbrigaditosCampaigns.SingleAsync(t => t.Id == campaign.Id);
        updatedCampaign.Description.Should().Be(request.Description);
        updatedCampaign.Provider.Should().Be(request.Provider);
        updatedCampaign.FundraiserGoal.Should().Be(request.FundraiserGoal);
    }

    [Test]
    public async Task WhenTheCampaignExists_RemovingParticipants_ShouldSucceed()
    {
        // Arrange
        var campaign = _dataFactory.GenerateAbrigaditosCampaign()
            .WithParticipants(DataFactory.GetAbrigaditosParticipants(5))
            .Build();
        var request = new Request
        {
            Id = campaign.Id,
            Description = "New Description",
            Provider = "New Provider",
            FundraiserGoal = 999,
            RemovedBeneficiaries = campaign.Participants.Select(t => t.BeneficiaryId)
        };

        // Act
        await _endpoint.HandleAsync(request, default);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        var updatedCampaign = await _dbContext.AbrigaditosCampaigns
            .Include(t => t.Participants)
            .SingleAsync(t => t.Id == campaign.Id);
        updatedCampaign.Description.Should().Be(request.Description);
        updatedCampaign.Provider.Should().Be(request.Provider);
        updatedCampaign.FundraiserGoal.Should().Be(request.FundraiserGoal);
        updatedCampaign.Participants.Should().BeEmpty();
    }

    [Test]
    public async Task WhenTheCampaignExists_AddingParticipants_ShouldSucceed()
    {
        // Arrange
        var campaign = _dataFactory.GenerateAbrigaditosCampaign()
            .Build();
        var participants = DataFactory.GetAbrigaditosParticipants(5)
            .Select(t => t.Build())
            .ToArray();
        var request = new Request
        {
            Id = campaign.Id,
            Description = "New Description",
            Provider = "New Provider",
            FundraiserGoal = 999,
            AddedBeneficiaries = participants.Select(t => t.BeneficiaryId)
        };

        // Act
        await _endpoint.HandleAsync(request, default);

        // Assert
        _endpoint.HttpContext.Response.StatusCode.Should().Be(200);
        var updatedCampaign = await _dbContext.AbrigaditosCampaigns
            .Include(t => t.Participants)
            .SingleAsync(t => t.Id == campaign.Id);
        updatedCampaign.Description.Should().Be(request.Description);
        updatedCampaign.Provider.Should().Be(request.Provider);
        updatedCampaign.FundraiserGoal.Should().Be(request.FundraiserGoal);
    }
}