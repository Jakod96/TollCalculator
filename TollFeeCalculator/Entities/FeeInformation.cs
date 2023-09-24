namespace TollFeeCalculator.Entities;

public class FeeInformation
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal Fee { get; set; }
}