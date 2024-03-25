using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DefaultNamespace;
using DocumentFormat.OpenXml.Wordprocessing;
using HUS.Data;

namespace HUS;

public class Optimizer
{
    private List<DataPerHour> _dataPerHours;
    private List<HeatingAsset> _heatingAssets;
    private List<HeatingAsset> _optimizedHeatingAssets = new List<HeatingAsset>();
    private int _assetIndex = 0;

    public double CurrentDemand { get; set; } = 0;
    public double TotalProductionCost { get; set; } = 0;
    public double MaxHeatProduced { get; set; } = 0;
    
    public Optimizer(List<DataPerHour> dataPerHour, List<HeatingAsset> heatingAssets)
    {
        _dataPerHours = dataPerHour;
        _heatingAssets = heatingAssets;
        _optimizedHeatingAssets = OptimzeAssets();
        
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
            CurrentDemand = data.Demand;

            if (_assetIndex >= _optimizedHeatingAssets.Count - 1)
                continue;
            /*
            if (CurrentDemand > MaxHeatProduced)
            {
                AssetManager.StartAsset(_optimizedHeatingAssets[_assetIndex]);
                _assetIndex++;
                Console.WriteLine("started another asset");
            }
            else
            {
                AssetManager.StopAsset(_optimizedHeatingAssets[_assetIndex]);
                _assetIndex--;
            }

            foreach (var asset in _optimizedHeatingAssets)
            {
                if (asset.IsOperating)
                    TotalProductionCost += asset.ProductionCost;
            }*/
            Console.WriteLine("an hour passed");
            //Thread.Sleep(3); //an hour divided by 10000
        }
    }

    public List<HeatingAsset> OptimzeAssets()
    {
        /*Calculate net production costs*/
        return _heatingAssets.OrderBy(x => x.ProductionCost).ToList();
    }
    
    /*dev*/
    
    public void PrintOriginAssets()
    {
        foreach (var data in _heatingAssets)
        {
            Console.WriteLine(data.Name);
        }
    }
    
    public void PrintOptimizedAssets()
    {
        foreach (var data in _optimizedHeatingAssets)
        {
            Console.WriteLine(data.Name);
        }
        
    }
}

/*
 *
 *  input: data from csv or database (collection) or update (per timeframe with multithreading)
 *             assets from assetmanager
 *
 *  output: optimezed data for result manager
 *
 *  The class optimizes the asset usings, in account with the data received from Danfoss therefore the costs
 *      of heating are minimized and profits are upsized.
 * 
 */
 
 /*
  * optimize assets and get a base asset then a second one i suppose
  * actually a collection of assets in function of how much heat those can excel and the cost
  * takeing into account
  * deem assets on or off for each case of an hour
  */
  
  /*
   * scenario 1: gas boiler and oil boilergas boiler < oil boiler
   * cenario 2: all together but keep in mind that the gas motor produces electricity whereas the the electri boiler consumes electricity, gas motor should tun when there are high electricity prices and the electric boiler should run when electricity prices are low
   */
   
   /*
    * ways to optimize
    * algorthm to sort together i suppos somehow and give them a price to demand grade and in function of grade use them, grade should be given for groups (scenario1)
    */