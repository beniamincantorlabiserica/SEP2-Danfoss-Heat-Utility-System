using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.ExcelAc;

namespace DefaultNamespace;

public class HeatingAssetOptimized
{
    public List<HeatingAsset> ModelList { get; set; }
    
    public double TotalProductionCost { get; set; }
    public double TotalMaxHeat { get; set; }
    
    public double Proficiency { get; set; }
    
    
    //public int TotalAge { get; set; }
        
    public HeatingAssetOptimized(List<HeatingAsset> listOfModels)
    {
        ModelList = listOfModels;

        foreach (var asset in ModelList)
        {
            TotalProductionCost += asset.ProductionCost - asset.MaxElectricity;
            TotalMaxHeat += asset.MaxHeat;
        }

        Proficiency = TotalProductionCost / TotalMaxHeat;

    }

    public void GetInfo()
    {
        foreach (var data in ModelList)
        {
            Console.WriteLine(data.Name);
            Console.WriteLine(" ");
        }
    }
}

/*
 * 1. totalcost = 50
 *      totalheat = 5
 *
 * 2. totalcost = 25
 *      totalheeat = 5
 */