using System.Reflection;
using Voyara.Domain.Entities.Culture_Rules;
using Voyara.Domain.Entities.Geography;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Entities.Transportation;
using Voyara.Domain.Entities.TripAndJourney;
using Voyara.Domain.Enums;
using Xunit;

namespace Voyara.Application.UnitTests.Domain;

public sealed class CityAndCountryIdentityTests
{
    [Fact]
    public void RuleScope_ForCity_WithValidGuid_SucceedsAndSetsTargetId()
    {
        // Arrange
        var cityId = Guid.NewGuid();

        // Act
        var result = RuleScope.ForCity(cityId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ScopeType.City, result.Value.Type);
        Assert.Equal(cityId.ToString(), result.Value.TargetId);
    }

    [Fact]
    public void RuleScope_ForCity_WithEmptyGuid_Fails()
    {
        // Act
        var result = RuleScope.ForCity(Guid.Empty);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(LocalRuleErrors.InvalidScopeId, result.Error);
    }

    [Fact]
    public void RuleScope_ForCountry_WithValidGuid_SucceedsAndSetsTargetId()
    {
        // Arrange
        var countryId = Guid.NewGuid();

        // Act
        var result = RuleScope.ForCountry(countryId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ScopeType.Country, result.Value.Type);
        Assert.Equal(countryId.ToString(), result.Value.TargetId);
    }

    [Fact]
    public void RuleScope_ForCountry_WithEmptyGuid_Fails()
    {
        // Act
        var result = RuleScope.ForCountry(Guid.Empty);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(LocalRuleErrors.InvalidScopeId, result.Error);
    }

    [Fact]
    public void City_Entities_WithSameId_AreEqual_AndNotShadowed()
    {
        // Arrange
        var country = Country.Create("EG", "EGP", "Africa/Cairo");
        var city1 = country.AddCity(30.0444, 31.2357);
        var city2 = country.AddCity(31.2001, 29.9187);

        // Assert: standard inequality
        Assert.NotEqual(city1, city2);
        Assert.False(city1 == city2);

        // Set city2's Id to city1's Id via reflection to verify BaseEntity equality
        var idProp = typeof(City).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)!;
        idProp.SetValue(city2, city1.Id);

        // Assert: equality holds when Id is the same
        Assert.Equal(city1.Id, city2.Id);
        Assert.Equal(city1, city2);
        Assert.True(city1 == city2);
        Assert.Equal(city1.GetHashCode(), city2.GetHashCode());
    }

    [Fact]
    public void GuideProfile_Create_AcceptsGuidCityId_AndRejectsEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cityId = Guid.NewGuid();

        // Act
        var guide = GuideProfile.Create(userId, cityId, "Experienced local tour guide in Cairo.", null, 5);

        // Assert
        Assert.Equal(cityId, guide.CityId);

        // Act & Assert Guid.Empty
        var ex = Assert.Throws<ArgumentException>(() =>
            GuideProfile.Create(userId, Guid.Empty, "Experienced local tour guide in Cairo.", null, 5));
        Assert.Equal("cityId", ex.ParamName);
    }

    [Fact]
    public void CityTransport_Create_AcceptsGuidCityId_AndRejectsEmpty()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        int transportTypeId = 1;

        // Act
        var transportResult = CityTransport.Create(
            cityId,
            transportTypeId,
            "City metro network connecting all major districts.",
            10.0m,
            "EGP",
            "05:00 - 00:00",
            "Keep tickets until exiting the station.");

        // Assert
        Assert.True(transportResult.IsSuccess);
        Assert.Equal(cityId, transportResult.Value.CityId);

        // Act & Assert Guid.Empty
        var failureResult = CityTransport.Create(
            Guid.Empty,
            transportTypeId,
            "City metro network connecting all major districts.",
            10.0m,
            "EGP",
            "05:00 - 00:00",
            null);

        Assert.True(failureResult.IsFailure);
        Assert.Equal(CityTransportErrors.InvalidCityId, failureResult.Error);
    }

    [Fact]
    public void JourneyItem_Create_AcceptsGuidCityId()
    {
        // Arrange
        var journey = Journey.Create("Cairo Adventure", 3);
        var destinationId = Guid.NewGuid();
        var cityId = Guid.NewGuid();

        // Act
        var item = journey.AddItem(1, destinationId, cityId, TimeSpan.FromHours(9), TimeSpan.FromHours(11), TransportMode.Walking);

        // Assert
        Assert.NotNull(item);
        Assert.Equal(cityId, item.CityId);
        Assert.Equal(destinationId, item.DestinationId);
    }
}
