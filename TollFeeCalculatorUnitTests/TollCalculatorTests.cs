using System;
using System.Collections.Generic;
using TollFeeCalculator;
using TollFeeCalculator.Entities;
using TollFeeCalculator.Enums;

namespace TollFeeCalculatorUnitTests;

public class TollCalculatorTests
{

    [Fact]
    public void GetTotalTollFee_MultipleVariousRates_ReturnsCorrectFee()
    {
        // Arrange
        var vehicle = new Vehicle { VehicleType = VehicleType.Car, LastPayment = DateTime.UtcNow.AddDays(-1) };
        var dates = new List<DateTime>
        {
            new (2023, 9, 25, 6, 45, 0), // 13
            new (2023, 9, 25, 8, 45, 0), // 8
            new (2023, 9, 25, 23, 0, 0), // 0
        };

        // Act
        var totalFee = TollCalculator.GetTotalTollFee(vehicle, dates);

        // Assert
        Assert.Equal(21, totalFee); //13 + 8 + 0
    }

    [Fact]
    public void GetTotalTollFee_OnlyWeekendDays_ReturnsNoFee()
    {
        // Arrange
        var vehicle = new Vehicle { VehicleType = VehicleType.Car, LastPayment = DateTime.UtcNow.AddDays(-1) };
        var dates = new List<DateTime>
        {
            new (2023, 9, 23, 6, 15, 0), // Saturday
            new (2023, 9, 24, 8, 45, 0), // Sunday
        };

        // Act
        var totalFee = TollCalculator.GetTotalTollFee(vehicle, dates);

        // Assert
        Assert.Equal(0, totalFee); 
    }
    
    [Fact]
    public void GetTotalTollFee_JulyDate_ReturnsNoFee()
    {
        // Arrange
        var vehicle = new Vehicle { VehicleType = VehicleType.Car, LastPayment = DateTime.UtcNow.AddDays(-1) };
        var dates = new List<DateTime>
        {
            new (2023, 7, 3, 6, 15, 0), // Monday
        };

        // Act
        var totalFee = TollCalculator.GetTotalTollFee(vehicle, dates);

        // Assert
        Assert.Equal(0, totalFee); 
    }

    [Theory]
    [InlineData(VehicleType.Motorbike)]
    [InlineData(VehicleType.Military)]
    [InlineData(VehicleType.Diplomat)]
    [InlineData(VehicleType.Emergency)]
    [InlineData(VehicleType.Foreign)]
    [InlineData(VehicleType.Tractor)]
    public void GetTollFee_TollFreeVehicleType_ReturnsZero(VehicleType type)
    {
        // Arrange
        var vehicle = new Vehicle { VehicleType = type, LastPayment = DateTime.UtcNow.AddDays(-1)};
        var date = new DateTime(2023, 9, 25, 6, 15, 0);

        // Act
        var fee = TollCalculator.GetTollFee(date, vehicle);

        // Assert
        Assert.Equal(0, fee);
    }

}