namespace HUS.Model;

public class ProductionUnit(string name, double maxHeatCapacity, double productionCost)
{
    public string Name = name;
    public double MaxHeatCapacity = maxHeatCapacity;
    public double ProductionCost = productionCost;
}