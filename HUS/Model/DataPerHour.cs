namespace DefaultNamespace;

public class DataPerHour
{
    public string HourStart { get; set; }
    public string HourEnd { get; set; }
    public double Demand { get; set; }
    public double ElectricityPrice { get; set; }
    public string Period { get; set; }
    
    public DataPerHour(string hourStart, string hourEnd, double demand, double electricityPrice, string period)
    {
        HourStart = hourStart;
        HourEnd = hourEnd;
        Demand = demand;
        ElectricityPrice = electricityPrice;
        Period = period;
    }
}