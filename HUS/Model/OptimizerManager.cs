using System.Linq;
using DefaultNamespace;
using HUS.Data;

namespace HUS.Model;

public class OptimizerManager
{
    public OptimizerManager(ResultManager resultManager, ExcelLoader excelLoader)
    {
        var dataPerHours = excelLoader.GetData();

        foreach (var result in from data in dataPerHours let optimizer = new Optimizer(data.HourStart, data.HourEnd, data.Demand, data.ElectricityPrice, data.Period) select new ResultDataPerHour(optimizer.GetProductionUnits(), data.HourStart, data.HourEnd, data.Demand, data.ElectricityPrice, data.Period, optimizer.GetTotalCost()))
        {
            resultManager.AddResult(result);
        }
    }
}