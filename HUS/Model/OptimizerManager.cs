using System.Collections.Generic;
using HUS.Data;

namespace DefaultNamespace;

public class OptimizerManager
{
    public OptimizerManager()
    {
        
        ExcelLoader excelLoader = new ExcelLoader();
        
        List<DataPerHour> Data = excelLoader.GetData();
        
        foreach (var data in Data)
        {
            Optimizer optimizer = new Optimizer(data.HourStart, data.HourEnd, data.Demand, data.ElectricityPrice, data.Period);
        }
        
    }
}