namespace DefaultNamespace;

/// <summary>
/// Heating asset model
/// </summary>
public class HeatingAsset
{
    
    public string? Name { get; set; }
    public double MaxHeat { get; set; }
    public double MaxElectricity { get; set; }
    public int ProductionCost { get; set; }
    public int Co2Emission { get; set; }
    public double GasConsumption { get; set; }
    public bool IsOperating { get; set; }

    public HeatingAsset(string? name, double maxHeat, double maxElectricity, int productionCost, int co2Emission, double gasConsumption)
    {
        Name = name;
        MaxHeat = maxHeat;
        MaxElectricity = maxElectricity;
        ProductionCost = productionCost;
        Co2Emission = co2Emission;
        GasConsumption = gasConsumption;
        IsOperating = false;
    }
}