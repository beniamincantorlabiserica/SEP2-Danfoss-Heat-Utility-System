using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HUS.Model;

public class ResultManager
{
    private List<ResultDataPerHour> Results { get; set; }
    private List<ResultDataPerHour> ResultsAsync { get; set; }

    private List<ResultDataPerHour> ResultsSent = new();

    
    public ResultManager()
    {
        Results = new List<ResultDataPerHour>();
       
    }
    
    public void AddResult(ResultDataPerHour resultDataPerHour)
    {
        Results.Add(resultDataPerHour);
    }
    
    public void OnFinishAddingResults()
    {
        Console.WriteLine("All results added to ResultManager.");
        ResultsAsync = new List<ResultDataPerHour>();
        Thread thread = new Thread(() =>
        {
            int i = 0;
            while (i < Results.Count - 1)
            {
                Console.WriteLine("Getting data in result manager async..." + i);
                ResultsAsync.Add(Results[i]);
                Thread.Sleep(163);
                i++;
            }
        });
        
        thread.Start();
    }
    
    public List<ResultDataPerHour> GetNewResults()
    {
        List<ResultDataPerHour> newResults = new();
        // The following foreach loop will add to the newResults list all the results that are not already in the ResultsAsync list.
        foreach (var item in ResultsAsync)
        {
            if (!ResultsSent.Contains(item))
            {
                newResults.Add(item);
                ResultsSent.Add(item);
            }
        }

        return newResults;
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