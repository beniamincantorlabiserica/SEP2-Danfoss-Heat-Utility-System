using System.Collections.Generic;

namespace DefaultNamespace;

public class ResultDataPerHour
{
    public Dictionary<string, double> UsedProductionUnits { get; set; }
    public string HourStart { get; set; }
    public string HourEnd { get; set; }
    public double Demand { get; set; }
    public double ElectricityPrice { get; set; }
    public string Period { get; set; }
    public double TotalCost { get; set; }
    
    public ResultDataPerHour(Dictionary<string, double> usedProductionUnits, string hourStart, string hourEnd, double demand, double electricityPrice, string period, double totalCost)
    {
        UsedProductionUnits = usedProductionUnits;
        HourStart = hourStart;
        HourEnd = hourEnd;
        Demand = demand;
        ElectricityPrice = electricityPrice;
        Period = period;
        TotalCost = totalCost;
    }
}