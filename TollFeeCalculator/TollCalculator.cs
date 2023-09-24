using TollFeeCalculator.Entities;
using TollFeeCalculator.Enums;

namespace TollFeeCalculator;
#nullable enable

public static class TollCalculator
{
    #region TableData
    //Both of these would be in a database which you would use a repository to get instead.
    private static readonly List<FeeInformation> FeeInformations = new()
    {
        new FeeInformation
        {
            StartTime = new TimeSpan(6, 0, 0),
            EndTime = new TimeSpan(6, 29, 59),
            Fee = 8,
        },
        new FeeInformation
        {
            StartTime = new TimeSpan(6, 30, 0),
            EndTime = new TimeSpan(6, 59, 59),
            Fee = 13,
        },
        new FeeInformation
        {
            StartTime = new TimeSpan(7, 0, 0),
            EndTime = new TimeSpan(7, 59, 59),
            Fee = 18,
        },
        new FeeInformation
        {
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(8, 29, 59),
            Fee = 13,
        },
        new FeeInformation
        {
            StartTime = new TimeSpan(8, 30, 0),
            EndTime = new TimeSpan(14, 59, 59),
            Fee = 8,
        },
        new FeeInformation
        {
            StartTime = new TimeSpan(15, 0, 0),
            EndTime = new TimeSpan(15, 29, 59),
            Fee = 13,
        },
        new FeeInformation
        {
            StartTime = new TimeSpan(15, 30, 0),
            EndTime = new TimeSpan(16, 59, 59),
            Fee = 18,
        },
        new FeeInformation
        {
            StartTime = new TimeSpan(17, 0, 0),
            EndTime = new TimeSpan(17, 59, 59),
            Fee = 13,
        },
        new FeeInformation
        {
            StartTime = new TimeSpan(18, 0, 0),
            EndTime = new TimeSpan(18, 29, 59),
            Fee = 8,
        },
        new FeeInformation
        {
            StartTime = new TimeSpan(18, 30, 0),
            EndTime = new TimeSpan(23, 59, 59),
            Fee = 0,
        },
        new FeeInformation
        {
            StartTime = new TimeSpan(0, 0, 0),
            EndTime = new TimeSpan(5, 59, 59),
            Fee = 0,
        }
    };
    
    
    //These are considered Holidays in sweden according to https://www.kalender.se/helgdagar
    private static readonly HashSet<DateTime> Holidays = new()
    {
        new DateTime(2023, 1, 1),
        new DateTime(2023, 1, 6),
        new DateTime(2023, 4, 7),
        new DateTime(2023, 4, 9),
        new DateTime(2023, 4, 10),
        new DateTime(2023, 5, 1),
        new DateTime(2023, 5, 18),
        new DateTime(2023, 5, 28),
        new DateTime(2023, 6, 6),
        new DateTime(2023, 6, 24),
        new DateTime(2023, 11, 4),
        new DateTime(2023, 12, 25),
        new DateTime(2023, 12, 26),
    };
    

    #endregion


    private const decimal MaxFee = 60; //Money should be decimal
    private const int TollFreePassageInterval = 60;

    /**
     * Calculate the total toll fee for one day
     * @param vehicle - the vehicle
     * @param dates   - date and time of all passes on one day
     * @return - the total toll fee for that day
     */

    public static decimal GetTotalTollFee(Vehicle vehicle, List<DateTime> dates)
    {
        decimal totalFee = 0;
        
        foreach (var date in dates.OrderBy(x => x))
        {
            var timeSinceLastPayment = date - dates.First();
            
            if (timeSinceLastPayment.TotalMinutes > TollFreePassageInterval || date.Equals(dates.First())) 
            {
                totalFee += GetTollFee(date, vehicle);
            }
        }
        
        return Math.Min(totalFee, MaxFee);
    }
    
    /**
     * Calculate the toll fee for the time
     *
     * @param date   - date and time the pass
     * @param vehicle - the vehicle
     * @return - the fee for that time
     */

    public static decimal GetTollFee(DateTime date, Vehicle vehicle)
    {
        if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle))
        {
            return 0;
        }

        var currentTime = date.TimeOfDay;
        
        foreach (var feeInformation in FeeInformations)
        {
            if (currentTime >= feeInformation.StartTime && currentTime <= feeInformation.EndTime)
            {
                return feeInformation.Fee;
            }
        }

        return 0; 
    }
    
    private static bool IsTollFreeVehicle(Vehicle vehicle)
    {
        return vehicle.VehicleType switch
        {
            VehicleType.Motorbike => true,
            VehicleType.Tractor => true,
            VehicleType.Emergency => true,
            VehicleType.Diplomat => true,
            VehicleType.Foreign => true,
            VehicleType.Military => true,
            VehicleType.Car => false,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static bool IsTollFreeDate(DateTime date)
    {
        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return true;
        }

        if ((Month) date.Month is Month.July)
        {
            return true;
        }
        
        return Holidays.Contains(date.Date) || Holidays.Contains(date.AddDays(-1).Date);
    }
}