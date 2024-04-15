using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using HUS.Data;
using HUS.Model;
using ReactiveUI;

namespace HUS.ViewModel;

public class DashboardViewModel : ViewModelBase
{
    
    private readonly ResultManager _resultManager;
    
    
    public ObservableCollection<ResultDataPerHour> Results { get; set; }

    
    private string _hourStart = string.Empty;
    public string HourStart
    {
        get => _hourStart;
        set => this.RaiseAndSetIfChanged(ref _hourStart, value);
    }
    
    private string _hourEnd = string.Empty;
    public string HourEnd
    {
        get => _hourEnd;
        set => this.RaiseAndSetIfChanged(ref _hourEnd, value);
    }
    
    private string _demand = string.Empty;
    public string Demand
    {
        get => _demand;
        set => this.RaiseAndSetIfChanged(ref _demand, value);
    }
    
    private string _electricityPrice = string.Empty;
    public string ElectricityPrice
    {
        get => _electricityPrice;
        set => this.RaiseAndSetIfChanged(ref _electricityPrice, value);
    }
    
    private string _period = string.Empty;
    public string Period
    {
        get => _period;
        set => this.RaiseAndSetIfChanged(ref _period, value);
    }
    
    private string _currentCost = string.Empty;
    public string CurrentCost
    {
        get => _currentCost;
        set => this.RaiseAndSetIfChanged(ref _currentCost, value);
    }
    
    private string _totalCost = string.Empty;
    public string TotalCost
    {
        get => _totalCost;
        set => this.RaiseAndSetIfChanged(ref _totalCost, value);
    }
    
    private string _totalIncome = string.Empty;
    public string TotalIncome
    {
        get => _totalIncome;
        set => this.RaiseAndSetIfChanged(ref _totalIncome, value);
    }
    
    private string _totalProfit = string.Empty;
    public string TotalProfit
    {
        get => _totalProfit;
        set => this.RaiseAndSetIfChanged(ref _totalProfit, value);
    }
    
    private string _totalDaysPassed = string.Empty;
    public string TotalDaysPassed
    {
        get => _totalDaysPassed;
        set => this.RaiseAndSetIfChanged(ref _totalDaysPassed, value);
    }
    

    public DashboardViewModel()
    {
        
        Console.WriteLine("Entered in the viewmodel constructor.");
        _resultManager = new ResultManager();
        var excelLoader = new ExcelLoader();
        
        var optimizerManager = new OptimizerManager(_resultManager, excelLoader);

        Results = new ObservableCollection<ResultDataPerHour>();
        
        var results = new ObservableCollection<ResultDataPerHour>(_resultManager.GetResults().ToList());
        int i = 0;
        Thread thread = new Thread(() =>
        {
            Console.WriteLine("Started thread.");
            while (i<results.Count)
            {
                Console.WriteLine("Entered in the while loop with i = " + i);
                Results.Add(results[i]);
                LoadData(i);
                SetContability(i);
                Thread.Sleep(1000);
                i++;
            }         
        });
        thread.Start();

    }
    
    private void LoadData(int index)
    {
        HourStart = "Starting Hour: " + Results[index].HourStart;
        HourEnd = "Ending Hour: " + Results[index].HourEnd;
        Demand = "Demand: " + Results[index].Demand;
        ElectricityPrice = "El Price: " + Results[index].ElectricityPrice;
        Period = "Period: " + Results[index].Period;
        CurrentCost = "Cost: " +  Results[index].TotalCost.ToString();
    }
    
    private void SetContability(int index)
    {
        double totalCost = 0;
        double totalIncome = 0;
        double totalProfit = 0;

        for (int i = 0; i < index; i++)
        {
            totalCost = totalCost + Results[i].TotalCost;
            totalIncome = totalIncome + Results[i].Demand * Results[i].ElectricityPrice;
        }
        
        totalProfit = totalIncome - totalCost;
        
        TotalCost = "Total Cost: " + totalCost.ToString();
        TotalIncome = "Total Income: " + totalIncome.ToString();
        TotalProfit = "Total Profit: " + totalProfit.ToString();
    }
    
    
}