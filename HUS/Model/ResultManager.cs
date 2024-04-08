using System.Collections.Generic;
using DefaultNamespace;

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
}