using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DefaultNamespace;



namespace HUS;

/// <summary>
/// Optimizer class
/// </summary>
public class Optimizer
{
    private List<HeatingAssetOptimized> _heatingAssetOptimizedList;
    private Thread? _timeframeThread;
    private int _assetIndex;
    public double CurrentDemand { get; set; }
    public double TotalProductionCost { get; set; }
    public double MaxHeatProduced { get; set; }
    public List<ReturnOptimizedData> ProcessedData { get; set; }
    public bool IsOperating { get; set; } = false;
    
    /// <summary>
    /// Class constructor
    /// Usage:
    ///     Without a given list the optimizer will store the processed data within the internal ProcessedData ReturnOptimizedData list
    ///     WIthout a given sleepTime in milliseconds, the default thread sleep time is set  to 360 milliseconds
    /// </summary>
    /// <param name="dataPerHour"> List containing all the data in hours. </param>
    /// <param name="heatingAssets"> List of the available heating assets. </param>
    /// <param name="sleepTime"> Thread's sleep time in milliseconds, an hour's passing pseudo time. </param>
    /// <param name="optimizerOutput"> List to be populated with the optimizers output instead of the internal "ProcessedData" list, if given a parameter </param>
    public Optimizer(List<DataPerHour> dataPerHour, List<HeatingAsset> heatingAssets, List<ReturnOptimizedData> optimizerOutput = null!, int sleepTime = 360)
    {
        IsOperating = true;
        _heatingAssetOptimizedList = new List<HeatingAssetOptimized>();
        ProcessedData = new List<ReturnOptimizedData>();
        OptimizeAssets(heatingAssets);
        StartOptimizer(dataPerHour, sleepTime, optimizerOutput);
        
    }
    
    /// <summary>
    /// Starts the optimizer thread.
    /// </summary>
    /// <param name="dataPerHours"> List containing all the data in hours. </param>
    /// <param name="sleepTime"> Thread's sleep time in milliseconds, an hour's passing pseudo time. </param>
    /// <param name="optimizerOutput"> List to be populated with the optimizer output </param>
    private void StartOptimizer(List<DataPerHour> dataPerHours, int sleepTime, List<ReturnOptimizedData> optimizerOutput)
    {
        _timeframeThread = new Thread(() => Optimize(dataPerHours: dataPerHours, sleepTime: sleepTime, optimizerOutput: optimizerOutput));
        _timeframeThread.Start();
        //_timeframeThread.Join();
    }

    /*
    /// <summary>
    /// Stops the optimizer thread, unsafe
    /// </summary>
    public void StopOptimizer()
    {
        try
        {
            if(_timeframeThread is not null)
                _timeframeThread.Abort();  // thread.Abort() is unsafe, according to microsoft 
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    */

    /// <summary>
    /// For each data in hours, in function of demand, starts or stops a set of heating assets.
    /// </summary>
    /// <param name="dataPerHours"> List containing all the data in hours. </param>
    /// <param name="sleepTime"> Thread's sleep time in milliseconds, an hour's passing pseudo time. </param>
    /// <param name="optimizerOutput"> List to be populated with the optimizer output </param>
    private void Optimize(List<DataPerHour> dataPerHours, int sleepTime, List<ReturnOptimizedData> optimizerOutput)
    {
        
        foreach (var data in dataPerHours)
        {
            Utils.Dev("-------------------------------------------");
            CurrentDemand = data.Demand;
            MaxHeatProduced = 0;

            foreach (var listOfAsset in _heatingAssetOptimizedList)
            {
                    if (listOfAsset.IsOperating)
                    {
                        MaxHeatProduced += listOfAsset.TotalMaxHeat;
                    }
            }

            Utils.Dev("Max heat currently produced: " + MaxHeatProduced);
            Utils.Dev("Current demand: " + CurrentDemand);

            if (CurrentDemand > MaxHeatProduced)
            {
                for (int i = 0; i < _heatingAssetOptimizedList.Count; i++)
                {
                    if (_heatingAssetOptimizedList[i].TotalMaxHeat >= CurrentDemand)
                    {
                        _heatingAssetOptimizedList[i].StartCombination();
                        _assetIndex = i;
                        break;
                    }
                }
            }
            
            if (CurrentDemand < MaxHeatProduced  && _assetIndex >= 1 && MaxHeatProduced - _heatingAssetOptimizedList[_assetIndex-1].TotalMaxHeat >= CurrentDemand)
            {
                _heatingAssetOptimizedList[_assetIndex].StopCombination();
                _assetIndex--;
                _heatingAssetOptimizedList[_assetIndex].StartCombination();
            }
            
            foreach (var listOfAsset in _heatingAssetOptimizedList)
            {
                    if (listOfAsset.IsOperating) 
                        TotalProductionCost += listOfAsset.TotalProductionCost;
            }
            
            if(optimizerOutput is not null)
                optimizerOutput.Add(new ReturnOptimizedData(data,  _heatingAssetOptimizedList[_assetIndex].ModelList));
            else
                ProcessedData.Add(new ReturnOptimizedData(data,  _heatingAssetOptimizedList[_assetIndex].ModelList));
            
            
            
            Thread.Sleep(sleepTime);
            
            Utils.Dev("-------------------------------------------");
            Utils.Dev(" ");

        }

        //PrintOutputData(optimizerOutput);
        Utils.Dev("Total cost: " + TotalProductionCost);
        IsOperating = false;
        
    }

    /// <summary>
    /// Creates all the possible combinations of heating assets and loads them into _heatingAssetsOptimized, then sorts the list ascending by the proficiency level of each combination.
    /// </summary>
    /// <param name="unoptimizedAssets"> List of unoptimized assets. </param>
    private void OptimizeAssets(List<HeatingAsset> unoptimizedAssets)
    {
        double count = Math.Pow(2, unoptimizedAssets.Count);
        for (int i = 1; i <= count - 1; i++)
        {
            string str = Convert.ToString(i, 2).PadLeft(unoptimizedAssets.Count, '0');
            List<HeatingAsset> temporaryModel = new List<HeatingAsset>();
            
            for (int j = 0; j < str.Length; j++)
            {
                if (str[j] == '1')
                {
                    temporaryModel.Add(new HeatingAsset(unoptimizedAssets[j].Name, unoptimizedAssets[j].MaxHeat, unoptimizedAssets[j].MaxElectricity, unoptimizedAssets[j].ProductionCost, unoptimizedAssets[j].Co2Emission, unoptimizedAssets[j].GasConsumption));
                }
            }
            _heatingAssetOptimizedList.Add(new HeatingAssetOptimized(temporaryModel));
        }
        _heatingAssetOptimizedList = _heatingAssetOptimizedList.OrderBy(x => x.Efficiency).ToList();
    }

    
    /*Development purpose*/
    public void PrintOptimizedAssets()
    {
        
        Utils.Dev("The following groups of heating assets were created: \n");

        foreach (var optimizedAssetsList in _heatingAssetOptimizedList)
        {
            Utils.Dev("----------------------------------");
            
                Utils.Dev("Group Efficiency: " + optimizedAssetsList.Efficiency);

               foreach(var asset in optimizedAssetsList.ModelList)
                   Utils.Dev("Asset name: " + asset.Name);
               
            Utils.Dev("----------------------------------");
            
        }
    }

    public void PrintOutputData(List<ReturnOptimizedData> optimizerOutput)
    {
        if (Utils.IsUnderDevelopment)
        {
            Utils.Dev("output for optimizer is ");
            foreach (var test in optimizerOutput)
            {
                Console.Write("for date " + test.Hour.HourStart + " to " + test.Hour.HourEnd + "  the demand is " +
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

