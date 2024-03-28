using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DefaultNamespace;
using DocumentFormat.OpenXml.Office2010.ExcelAc;


namespace HUS;

public class Optimizer
{
    
    
    private List<List<HeatingAssetOptimized>> _heatingAssetOptimizedList;
    
    public List<ReturnOptimizedData> OptimizerOutput;
    
    private Thread _timeframeThread;
    
    private int _assetIndex;
    public double CurrentDemand { get; set; }
    public double TotalProductionCost { get; set; }
    public double MaxHeatProduced { get; set; }
    
    
    public Optimizer(List<DataPerHour> dataPerHour, List<HeatingAsset> heatingAssets, int sleepTime = 360)
    {
        _heatingAssetOptimizedList = new List<List<HeatingAssetOptimized>>();
        OptimizerOutput = new List<ReturnOptimizedData>();
        OptimzeAssets(heatingAssets);
        PrintOptimizedAssets();
        StartOptimizer(dataPerHour, sleepTime);
        
    }

    public void StartOptimizer(List<DataPerHour> dataPerHours, int sleepTime)
    {
        _timeframeThread = new Thread(() => Optimize(dataPerHours: dataPerHours, sleepTime: sleepTime));
        _timeframeThread.Start();
        _timeframeThread.Join();
    }

    public void StopOptimizer()
    {
        _timeframeThread.Abort(); //unsafe
    }

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
            
            
            
            Thread.Sleep(sleepTime); //an hour divided by 10000
            
            
            
            
            Utils.Dev("-------------------------------------------");
            Utils.Dev(" ");

        }

        PrintOutputData();
        Utils.Dev("Total cost: " + TotalProductionCost);
    }

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

