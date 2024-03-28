using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DefaultNamespace;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using HUS.Data;

namespace HUS;

public class Optimizer
{
    private List<DataPerHour> _dataPerHours;
    private List<HeatingAsset> _heatingAssets;
    private List<HeatingAsset> _optimizedHeatingAssets;
    private List<List<HeatingAssetOptimized>> HeatingAssetOptimizedList = new List<List<HeatingAssetOptimized>>();
    private int _assetIndex = 0;
    public double CurrentDemand { get; set; }
    public double TotalProductionCost { get; set; }
    public double MaxHeatProduced { get; set; }

    private bool IsDeveloping = true; // turn false when project is finalized
    
    public Optimizer(List<DataPerHour> dataPerHour, List<HeatingAsset> heatingAssets)
    {
        _dataPerHours = dataPerHour;
        _heatingAssets = heatingAssets;
        OptimzeAssets(_heatingAssets);
        PrintOptimizedAssets();
        
    }

    public void StartThread()
    {
        Thread timeframeThread = new Thread(() => Optimize());
 
        // start the thread
        timeframeThread.Start();
 
        // do some other work in the main thread
        
 
        // wait for the worker thread to complete
        timeframeThread.Join();
    }

    public void StartOptimizer()
    {
        //start a thread and work through information
        //optimize ( data from an hour )
        
 
        
    }

    public void StopOptimizer()
    {
        
    }

    public void Optimize()
    {
        foreach (var data in _dataPerHours)
        {
            Dev("-------------------------------------------");
            CurrentDemand = data.Demand;
            MaxHeatProduced = 0;

            foreach (var listOfListedAsset in HeatingAssetOptimizedList)
            {
                foreach (var listOfAsset in listOfListedAsset)
                {
                    if (listOfAsset.IsOperating)
                    {
                        MaxHeatProduced += listOfAsset.TotalMaxHeat;
                    }
                }
            }

            Dev("Max heat currently produced: " + MaxHeatProduced);
            Dev("Current demand: " + CurrentDemand);

            if (CurrentDemand > MaxHeatProduced)
            {
                for (int i = 0; i < HeatingAssetOptimizedList.Count; i++)
                {
                    if (HeatingAssetOptimizedList[i][0].TotalMaxHeat >= CurrentDemand)
                    {
                        HeatingAssetOptimizedList[i][0].StartOptimzedAssets();
                        _assetIndex = i;
                        break;
                    }
                }
                
                
            }
            
            
            if (CurrentDemand < MaxHeatProduced  && _assetIndex >= 1 && MaxHeatProduced - HeatingAssetOptimizedList[_assetIndex-1][0].TotalMaxHeat >= CurrentDemand)
            {
                HeatingAssetOptimizedList[_assetIndex][0].StopOptimzedAssets();
                _assetIndex--;
                HeatingAssetOptimizedList[_assetIndex][0].StartOptimzedAssets();
            }

        
            foreach (var listOfListedAsset in HeatingAssetOptimizedList)
            {
                foreach (var listOfAsset in listOfListedAsset) 
                { 
                    if (listOfAsset.IsOperating) 
                        TotalProductionCost += listOfAsset.TotalProductionCost;
                }
            }
            
            
            
            Thread.Sleep(30); //an hour divided by 10000
            
            
            Dev("-------------------------------------------");
            Dev(" ");

        }
        
        
            
        
        
        Console.WriteLine($"total cost: {TotalProductionCost}");
    }

    public void OptimzeAssets(List<HeatingAsset> unoptimizedAssets)
    {
        double count = Math.Pow(2, unoptimizedAssets.Count);
        for (int i = 1; i <= count - 1; i++)
        {
            string str = Convert.ToString(i, 2).PadLeft(unoptimizedAssets.Count, '0');
            List<HeatingAsset> temporaryModel = new List<HeatingAsset>();
            List<HeatingAssetOptimized> temporaryModelList = new List<HeatingAssetOptimized>();
            
                    
            for (int j = 0; j < str.Length; j++)
            {
                //List<model> temporaryModel = new List<model>();
                if (str[j] == '1')
                {
                    //Console.Write(list[j].Name);
                    temporaryModel.Add(new HeatingAsset(unoptimizedAssets[j].Name, unoptimizedAssets[j].MaxHeat, unoptimizedAssets[j].MaxElectricity, unoptimizedAssets[j].ProductionCost, unoptimizedAssets[j].Co2Emission, unoptimizedAssets[j].GasConsumption)); //double check for errors
                    Console.WriteLine($"added {unoptimizedAssets[j].Name}");
                        
                        
                }
                        
            }

            temporaryModelList.Add(new HeatingAssetOptimized(temporaryModel));
            HeatingAssetOptimizedList.Add(temporaryModelList);
            Console.WriteLine($"added that list to model list");
            Console.WriteLine();
        }

        HeatingAssetOptimizedList = HeatingAssetOptimizedList.OrderBy(a => a.Max(x => x.Proficiency)).ToList();
    }
    
    /*dev*/
    
    public void PrintOptimizedAssets()
    {

        foreach (List<HeatingAssetOptimized> data in HeatingAssetOptimizedList)
        {
            foreach (var data2 in data)
            {
                Console.WriteLine($"============================");
                    
                Console.WriteLine(data2.Proficiency);
                data2.GetInfo();
                    
                Console.WriteLine("=============================");
            }
        }
    }

    private void Dev(string str)
    {
        if (IsDeveloping)
            Console.WriteLine($"DEV: {str}");
    }
}

