using System.Collections.Generic;

namespace HUS.Model;

public class ResultManager
{
    private List<ResultDataPerHour> Results { get; set; }
    
    public ResultManager()
    {
        Results = new List<ResultDataPerHour>();
    }
    
    public void AddResult(ResultDataPerHour resultDataPerHour)
    {
        Results.Add(resultDataPerHour);
    }
    
    public List<ResultDataPerHour> GetResults()
    {
        return Results;
    }
    
    public string GetTotalCost()
    {
        double totalCost = 0;
        foreach (var result in Results)
        {
            totalCost += result.TotalCost;
        }
        return totalCost.ToString();
    }
}