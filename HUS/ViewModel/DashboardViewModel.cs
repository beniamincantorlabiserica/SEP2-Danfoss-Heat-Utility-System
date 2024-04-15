using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DynamicData.Binding;
using HUS.Data;
using HUS.Model;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using ReactiveUI;
using SkiaSharp;

namespace HUS.ViewModel;

public class DashboardViewModel : ViewModelBase
{

    #region Proeprties for chart

    public ISeries[] Series { get; set; }

    private ObservableCollection<double> DemandOutput { get; set; }
    private ObservableCollection<double> GasMotorProductionUnitOutput { get; set; }
    private ObservableCollection<double> GasBoilerProductionUnitOutput { get; set; }
    private ObservableCollection<double> OilBoilerProductionUnitOutput { get; set; }
    private ObservableCollection<double> ElectricBoilerProductionUnitOutput { get; set; }
    private ObservableCollection<string> CurrentDateAndTime { get; set; }
    private ObservableCollection<double> ProfitOutput { get; set; }
    private ObservableCollection<double> IncomeOutput { get; set; }

    private Thread thread;
    public Axis[] XAxes { get; set; }

    public Axis[] YAxes { get; set; }
        = new Axis[]
        {
            new Axis
            {
                Name = "MWh",
                NameTextSize = 12,
                MaxLimit = 10,
                MinLimit = 0
            }
        };
    
    public ISeries[] SeriesProfit { get; set; }
    
    public Axis[] XAxesProfit { get; set; }

    public Axis[] YAxesProfit { get; set; }
        = new Axis[]
        {
            new Axis
            {
                Name = "DKK",
                MinLimit = 0,
                NameTextSize = 12,
            }
        };
    #endregion
    
    
    private readonly ResultManager _resultManager;
    private IBrush _elBoilerStateColor;

    public IBrush ElBoilerStateColor
    {
        get { return _elBoilerStateColor; }
        set
        {
            this.RaiseAndSetIfChanged(ref _elBoilerStateColor, value);
        }
    }    
    
    private IBrush _gasBoilerStateColor;

    public IBrush GasBoilerStateColor
    {
        get { return _gasBoilerStateColor; }
        set
        {
            this.RaiseAndSetIfChanged(ref _gasBoilerStateColor, value);
        }
    }    
    
    private IBrush _gasMotorStateColor;

    public IBrush GasMotorStateColor
    {
        get { return _gasMotorStateColor; }
        set
        {
            this.RaiseAndSetIfChanged(ref _gasMotorStateColor, value);
        }
    }    
    
    private IBrush _oilBoilerStateColor;

    public IBrush OilBoilerStateColor
    {
        get { return _oilBoilerStateColor; }
        set
        {
            this.RaiseAndSetIfChanged(ref _oilBoilerStateColor, value);
        }
    }    
    
    public ObservableCollection<ResultDataPerHour> Results { get; set; }

    private bool _systemState = false;
    
    private string _productionProfit = "0";
    public string ProductionProfit
    {
        get => _productionProfit;
        set => this.RaiseAndSetIfChanged(ref _productionProfit, value);
    }
    
    private string _productionIncome = "0";
    public string ProductionIncome
    {
        get => _productionIncome;
        set => this.RaiseAndSetIfChanged(ref _productionIncome, value);
    }
    
    private string _productionCost = "0";
    public string ProductionCost
    {
        get => _productionCost;
        set => this.RaiseAndSetIfChanged(ref _productionCost, value);
    }

    #region  Biindings for view

    

   
    
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
    #endregion

    public DashboardViewModel(ResultManager resultManager)
    {
        DemandOutput = new ObservableCollection<double>();
        GasMotorProductionUnitOutput = new ObservableCollection<double>();
        GasBoilerProductionUnitOutput = new ObservableCollection<double>();
        OilBoilerProductionUnitOutput = new ObservableCollection<double>();
        ElectricBoilerProductionUnitOutput = new ObservableCollection<double>();
        CurrentDateAndTime = new ObservableCollection<string>();
        ProfitOutput = new ObservableCollection<double>();
        IncomeOutput = new ObservableCollection<double>();
        
        ElBoilerStateColor = Brushes.Lavender;
        GasBoilerStateColor = Brushes.Lavender;
        GasMotorStateColor = Brushes.Lavender;
        OilBoilerStateColor = Brushes.Lavender;
        
        Series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = DemandOutput,
                Name = "Demand",

            },
            new ColumnSeries<double>
            {
                Values = GasMotorProductionUnitOutput,
                Name = "Gas Motor Supply"
            },
            new ColumnSeries<double>
            {
                Values = OilBoilerProductionUnitOutput,
                Name = "Oil Boiler Supply"
            },
            new ColumnSeries<double>
            {
                Values = ElectricBoilerProductionUnitOutput,
                Name = "Electric Boiler Supply"
            },
            new ColumnSeries<double>
            {
                Values = GasBoilerProductionUnitOutput,
                Name = "Gas Boiler Supply"
            },
            
        };


        XAxes = new Axis[]
        {
            new Axis
            {
                Name = "Date & Time", 
                NameTextSize = 12,
                TextSize = 10,
                LabelsRotation = -45,
                Labels = CurrentDateAndTime,
                ShowSeparatorLines = true
 
               
            }
        };

        SeriesProfit = new ISeries[] {
            new LineSeries<double>
            {
                Values = ProfitOutput,
                Name = "Profit"
            },
            new LineSeries<double>
            {
                Values = IncomeOutput,
                Name = "Income"
            }
        };
        XAxesProfit = new Axis[]
        {
            new Axis
            {
                Name = "Date & Time", 
                NameTextSize = 12,
                TextSize = 10,
                LabelsRotation = -45,
                Labels = CurrentDateAndTime,
                ShowSeparatorLines = true
 
               
            }
        };
        this._resultManager = resultManager;

        Console.WriteLine("Entered in the viewmodel constructor.");
        _resultManager = new ResultManager();
        var excelLoader = new ExcelLoader();
        
        var optimizerManager = new OptimizerManager(_resultManager, excelLoader);

        Results = new ObservableCollection<ResultDataPerHour>();
        
        var results = new ObservableCollection<ResultDataPerHour>(_resultManager.GetResults().ToList());
        int i = 0;
        thread = new Thread(() =>
        {
            
            Console.WriteLine("Started thread.");
            while (i<results.Count)
            {
                while (!_systemState)
                {
                    Thread.Sleep(1000);
                }
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
        // Update the chart
        
        DemandOutput.Add(Math.Round(Results[index].Demand, 2));
        GasMotorProductionUnitOutput.Add(Math.Round(Results[index].UsedProductionUnits["GasMotor"], 2));
        GasBoilerProductionUnitOutput.Add(Math.Round(Results[index].UsedProductionUnits["GasBoiler"],2));
        OilBoilerProductionUnitOutput.Add(Math.Round(Results[index].UsedProductionUnits["OilBoiler"],2));
        ElectricBoilerProductionUnitOutput.Add(Math.Round(Results[index].UsedProductionUnits["ElectricBoiler"],2));
        CurrentDateAndTime.Add(Results[index].HourStart);
        ProfitOutput.Add(double.Round(Results[index].Demand*Results[index].ElectricityPrice - Results[index].TotalCost));
        IncomeOutput.Add(double.Round(Results[index].Demand*Results[index].ElectricityPrice));
        AdjustListForChartTo10Elements(DemandOutput);
        AdjustListForChartTo10Elements(GasMotorProductionUnitOutput);
        AdjustListForChartTo10Elements(GasBoilerProductionUnitOutput);
        AdjustListForChartTo10Elements(OilBoilerProductionUnitOutput);
        AdjustListForChartTo10Elements(ElectricBoilerProductionUnitOutput);
        AdjustListForChartTo10Elements(CurrentDateAndTime);
        AdjustListForChartTo10Elements(ProfitOutput, 3);
        AdjustListForChartTo10Elements(IncomeOutput, 3);
        
        Console.WriteLine("Product "  +  ProductionProfit); 
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
        Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

        Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberGroupSeparator = ".";
        Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalDigits = 2;

        ProductionProfit = Math.Round(double.Parse(ProductionProfit)  + Math.Round(Results[index].Demand*Results[index].ElectricityPrice - Results[index].TotalCost, 2), 2).ToString("N");
        ProductionIncome = Math.Round(double.Parse(ProductionIncome) + Math.Round(Results[index].Demand*Results[index].ElectricityPrice, 2), 2).ToString("N");
        ProductionCost = Math.Round(double.Parse(ProductionCost) + Math.Round(Results[index].TotalCost, 2), 2).ToString("N");

        
        ActivateProductionUnitStateColor("GasMotor", Results[index].UsedProductionUnits["GasMotor"]);
        ActivateProductionUnitStateColor("GasBoiler", Results[index].UsedProductionUnits["GasBoiler"]);
        ActivateProductionUnitStateColor("OilBoiler", Results[index].UsedProductionUnits["OilBoiler"]);
        ActivateProductionUnitStateColor("ElectricBoiler", Results[index].UsedProductionUnits["ElectricBoiler"]);
    }


    void ActivateProductionUnitStateColor(string name, double output)
    {
        bool sem = output > 0;
        
        Console.WriteLine("Entered in the production unit state color.");
        switch (name)
        {
            case "GasMotor":
                GasMotorStateColor = sem ? Brushes.CornflowerBlue : Brushes.Lavender;
                break;
            case "GasBoiler":
                GasBoilerStateColor = sem ? Brushes.CornflowerBlue : Brushes.Lavender;
                break;
            case "OilBoiler":
                OilBoilerStateColor = sem ? Brushes.CornflowerBlue : Brushes.Lavender;
                break;
            case "ElectricBoiler":
                ElBoilerStateColor = sem ? Brushes.CornflowerBlue : Brushes.Lavender;
                break;
        }
    } 
    void AdjustListForChartTo10Elements(IList list, int elements = 10)
    {
        if (list.Count > elements)
        {
            list.RemoveAt(0);
        }
    }

    public void StartSystem()
    {
        _systemState = true;
    }
    
    public void PauseSystem()
    {
        _systemState = false;
    }
    
    public void ResetSystem()
    {
        Console.WriteLine("afefefesfsefesfsefesfesf");
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