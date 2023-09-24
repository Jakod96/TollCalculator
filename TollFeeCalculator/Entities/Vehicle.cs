using TollFeeCalculator.Enums;

namespace TollFeeCalculator.Entities;
#nullable enable
public class Vehicle
{
    public VehicleType VehicleType { get; set; }
    public DateTime? LastPayment { get; set; }
}