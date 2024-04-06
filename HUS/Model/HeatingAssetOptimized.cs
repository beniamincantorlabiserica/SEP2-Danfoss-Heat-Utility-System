using System.Collections.Generic;
using HUS;
using HUS.Data;

namespace DefaultNamespace;

/// <summary>
/// Represents an optimized heating asset combination formed from one or more heating assets, alongside the following fields:
///     TotalMaxHeat - the total heat produced by all the assets within the combination
///     TotalProductionCost - the total production cost of all the assets within the combination per hour
///     Efficiency - grade the defines the best approach, achieved by dividing the TotalProductionCost with the TotalMaxHeat,
///  lower grade results in a better fit for a combination of heating assets, in term of demand, production cost and profit. 
/// </summary>
public class HeatingAssetOptimized
{
    public List<HeatingAsset> ModelList { get; set; }
    public double TotalProductionCost { get; set; }
    public double TotalMaxHeat { get; set; }
    public double Efficiency { get; set; }
    
    public bool IsOperating = false;
        
    /// <summary>
    /// Model constructor.
    /// Computes TotalProductionCost, TotalMaxHeat and Efficiency.
    /// </summary>
    /// <param name="listOfModels"> List of heating assets. </param>
    public HeatingAssetOptimized(List<HeatingAsset> listOfModels)
    {
        ModelList = listOfModels;

        foreach (var asset in ModelList)
        {
            TotalProductionCost += asset.ProductionCost - asset.MaxElectricity; // to be changed
            TotalMaxHeat += asset.MaxHeat;
        }

        Efficiency = TotalProductionCost / TotalMaxHeat;

    }
    
    /// <summary>
    /// Starts all the heating assets within the heating asset combination model.
    /// </summary>
    public void StartCombination()
    {
        foreach (var asset in ModelList)
        {
            AssetManager.StartAsset(asset);
            Utils.Dev("Started asset " + asset.Name);
        }

        IsOperating = true;

    }
    
    /// <summary>
    /// Stops all the heating assets within the heating asset combination model.
    /// </summary>
    public void StopCombination()
    {
        foreach (var asset in ModelList)
        {
            AssetManager.StopAsset(asset);
            Utils.Dev("Stopped asset " + asset.Name);
        }
        IsOperating = false;
    }
}
