using System.Collections.Generic;

namespace HUS.Model;

public class AssetManager
{
    public List<ProductionUnit> ProductionUnits;
    
    public AssetManager()
    {
        ProductionUnits = new List<ProductionUnit>();        
        CreateProductionUnits();
    }

    private void CreateProductionUnits()
    {
        ProductionUnits.Add(new ProductionUnit("GasBoiler", 5.0, 500));
        ProductionUnits.Add(new ProductionUnit("OilBoiler", 4.0, 700));
        ProductionUnits.Add(new ProductionUnit("GasMotor", 3.6, 1100));
        ProductionUnits.Add(new ProductionUnit("ElectricBoiler", 8.0, 50));
    }
}