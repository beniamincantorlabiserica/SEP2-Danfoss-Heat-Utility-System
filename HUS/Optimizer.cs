using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DefaultNamespace;
using DocumentFormat.OpenXml.Office2010.ExcelAc;


namespace HUS;

/// <summary>
/// Optimizer class
/// </summary>
public class Optimizer
{
    
    
    private List<List<HeatingAssetOptimized>> _heatingAssetOptimizedList;  //to be fixed
    private Thread _timeframeThread;
    private int _assetIndex;
    public List<ReturnOptimizedData> OptimizerOutput { get; set; }
    public double CurrentDemand { get; set; }
    public double TotalProductionCost { get; set; }
    public double MaxHeatProduced { get; set; }
    
    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="dataPerHour"> List containing all the data in hours. </param>
    /// <param name="heatingAssets"> List of the available heating assets. </param>
    /// <param name="sleepTime"> Thread's sleep time in milliseconds, an hour's passing pseudo time. </param>
    public Optimizer(List<DataPerHour> dataPerHour, List<HeatingAsset> heatingAssets, int sleepTime = 360)
    {
        _heatingAssetOptimizedList = new List<List<HeatingAssetOptimized>>();
        OptimizerOutput = new List<ReturnOptimizedData>();
        OptimzeAssets(heatingAssets);
        PrintOptimizedAssets();
        StartOptimizer(dataPerHour, sleepTime);
        
    }
    
    /// <summary>
    /// Starts the optimizer thread.
    /// </summary>
    /// <param name="dataPerHours"> List containing all the data in hours. </param>
    /// <param name="sleepTime"> Thread's sleep time in milliseconds, an hour's passing pseudo time. </param>
    public void StartOptimizer(List<DataPerHour> dataPerHours, int sleepTime)
    {
        _timeframeThread = new Thread(() => Optimize(dataPerHours: dataPerHours, sleepTime: sleepTime));
        _timeframeThread.Start();
        _timeframeThread.Join();
    }

    /// <summary>
    /// Stops the optimizer thread, unsafe
    /// </summary>
    public void StopOptimizer()
    {
        _timeframeThread.Abort();
    }

    /// <summary>
    /// For each data in hours, in function of demand, starts or stops a set of heating assets.
    /// </summary>
    /// <param name="dataPerHours"> List containing all the data in hours. </param>
    /// <param name="sleepTime"> Thread's sleep time in milliseconds, an hour's passing pseudo time. </param>
    private void Optimize(List<DataPerHour> dataPerHours, int sleepTime)
    {
        foreach (var data in dataPerHours)
        {
            Utils.Dev("-------------------------------------------");
            CurrentDemand = data.Demand;
            MaxHeatProduced = 0;

            foreach (var listOfListedAsset in _heatingAssetOptimizedList)
            {
                foreach (var listOfAsset in listOfListedAsset)
                {
                    if (listOfAsset.IsOperating)
                    {
                        MaxHeatProduced += listOfAsset.TotalMaxHeat;
                    }
                }
            }

            Utils.Dev("Max heat currently produced: " + MaxHeatProduced);
            Utils.Dev("Current demand: " + CurrentDemand);

            if (CurrentDemand > MaxHeatProduced)
            {
                for (int i = 0; i < _heatingAssetOptimizedList.Count; i++)
                {
                    if (_heatingAssetOptimizedList[i][0].TotalMaxHeat >= CurrentDemand)
                    {
                        _heatingAssetOptimizedList[i][0].StartOptimzedAssets();
                        _assetIndex = i;
                        break;
                    }
                }
                
                
            }
            
            
            if (CurrentDemand < MaxHeatProduced  && _assetIndex >= 1 && MaxHeatProduced - _heatingAssetOptimizedList[_assetIndex-1][0].TotalMaxHeat >= CurrentDemand)
            {
                _heatingAssetOptimizedList[_assetIndex][0].StopOptimzedAssets();
                _assetIndex--;
                _heatingAssetOptimizedList[_assetIndex][0].StartOptimzedAssets();
            }

        
            foreach (var listOfListedAsset in _heatingAssetOptimizedList)
            {
                foreach (var listOfAsset in listOfListedAsset) 
                { 
                    if (listOfAsset.IsOperating) 
                        TotalProductionCost += listOfAsset.TotalProductionCost;
                }
            }
            
            OptimizerOutput.Add(new ReturnOptimizedData(data,  _heatingAssetOptimizedList[_assetIndex][0].ModelList));
            
            
            
            Thread.Sleep(sleepTime);
            
            
            
            
            Utils.Dev("-------------------------------------------");
            Utils.Dev(" ");

        }

        PrintOutputData();
        Utils.Dev("Total cost: " + TotalProductionCost);
    }

    /// <summary>
    /// Creates all the possible combinations of heating assets and loads them into _heatingAssetsOptimized, then sorts the list ascending by the proficiency level of each combination.
    /// </summary>
    /// <param name="unoptimizedAssets"> List of unoptimized assets. </param>
    private void OptimzeAssets(List<HeatingAsset> unoptimizedAssets)
    {
        double count = Math.Pow(2, unoptimizedAssets.Count);
        for (int i = 1; i <= count - 1; i++)
        {
            string str = Convert.ToString(i, 2).PadLeft(unoptimizedAssets.Count, '0');
            List<HeatingAsset> temporaryModel = new List<HeatingAsset>();
            List<HeatingAssetOptimized> temporaryModelList = new List<HeatingAssetOptimized>();
            
                    
            for (int j = 0; j < str.Length; j++)
            {
                
                if (str[j] == '1')
                {
                    
                    temporaryModel.Add(new HeatingAsset(unoptimizedAssets[j].Name, unoptimizedAssets[j].MaxHeat, unoptimizedAssets[j].MaxElectricity, unoptimizedAssets[j].ProductionCost, unoptimizedAssets[j].Co2Emission, unoptimizedAssets[j].GasConsumption)); //double check for errors
                    
                        
                        
                }
                        
            }

            temporaryModelList.Add(new HeatingAssetOptimized(temporaryModel));
            _heatingAssetOptimizedList.Add(temporaryModelList);
        }

        _heatingAssetOptimizedList = _heatingAssetOptimizedList.OrderBy(a => a.Max(x => x.Proficiency)).ToList();
    }
    
    /*Utils.Dev*/
    private ReturnOptimizedData ReturnOptimizedData(DataPerHour hour, List<HeatingAsset> assets)
    {
        return new ReturnOptimizedData(hour, assets);
    }
    private void PrintOptimizedAssets()
    {
        
        Utils.Dev("The following groups of heating assets were created: \n");

        foreach (var listOfOptimizedAssets in _heatingAssetOptimizedList)
        {
            Utils.Dev("----------------------------------");
            foreach (var optimizedAssetsList in listOfOptimizedAssets)
            {
                Utils.Dev("Group Proficiency: " + optimizedAssetsList.Proficiency.ToString());

               foreach(var asset in optimizedAssetsList.ModelList)
                   Utils.Dev("Asset name: " + asset.Name);
            }
            Utils.Dev("----------------------------------");
            
        }
    }

    private void PrintOutputData()
    {
        if (Utils.IsUnderDevelopment)
        {
            Utils.Dev("output for optimizer is ");
            foreach (var test in OptimizerOutput)
            {
                Console.Write("for date " + test.Hour.HourStart + " to " + test.Hour.HourEnd + " / the demand is " +
                              test.Hour.Demand + " , and the following assets were running: ");
                foreach (var asset in test.AssetsUsed)
                {
                    Console.Write($" {asset.Name}");
                }

                Console.WriteLine("\n");
            }
        }
    }
}

