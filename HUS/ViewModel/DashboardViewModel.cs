using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using HUS.Model;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Spreadsheet;
using DynamicData;
using HUS.Data;
using LiveChartsCore.SkiaSharpView.VisualElements;
using ReactiveUI;

namespace HUS.ViewModel;

public partial class DashboardViewModel : ViewModelBase
{
    private ResultManager _resultManager;
    public ObservableCollection<double> DemandOutput = new ObservableCollection<double>();
    public ObservableCollection<string> CurrentDate = new ObservableCollection<string>();
    public List<ResultDataPerHour> DemandData = new List<ResultDataPerHour>();
    public ObservableCollection<double> TotalCostList = new ObservableCollection<double>();
    public ObservableCollection<double> TotalIncomeList = new ObservableCollection<double>();
    public ObservableCollection<double> ProductionUnitUsageList = new ObservableCollection<double>();
    public ObservableCollection<double> ElectricityPriceList = new ObservableCollection<double>();


    
    public ObservableCollection<double> ElBoilerProductionUnitOuput = new ObservableCollection<double>();
    public ObservableCollection<double> GasBoilerProductionUnitOuput = new ObservableCollection<double>();
    public ObservableCollection<double> OilBoilerProductionUnitOuput = new ObservableCollection<double>();
    public ObservableCollection<double> GasMotorProductionUnitOuput = new ObservableCollection<double>();

    private int PositionStart { get; set; } = 0;
    private int PositionEnd { get; set; } = 0;

    private double _currentProfit;
    public double CurrentProfit
    {
        get => _currentProfit;
        set => this.RaiseAndSetIfChanged(ref _currentProfit, value);
    }

    private bool _liveMode = true;
    
    private string _startPoint;
    public string StartPoint
    {
        get => _startPoint;
        set
        {
            _startPoint = value;
            
        }
    }
    
    private string _endPoint;
    public string EndPoint
    {
        get => _endPoint;
        set
        {
            _startPoint = value;
        }
    }

    #region cost& profit chart
    
    public ISeries[] CostAndProfitSeries { get; set; }
    
    public ISeries[] ProductionUnitUsageSeries { get; set; }
    
    public ISeries[] ElectricityPriceSeries { get; set; }

    public Axis[] YAxesProdUnits { get; set; } =
    {
        new Axis { MinLimit = 0, MaxLimit = 100 }
    };
    
    public Axis[] XAxesProdUnits { get; set; } =
    {
        new Axis {Labels = new string[] { "El B", "Gas B", "Oil B", "Gas M" }  }
    };
    
    
    public LabelVisual CostAndProfitTitle { get; set; } =
        new LabelVisual
        {
            Text = "Income & Cost",
            TextSize = 20,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.White)
        };
    
    public LabelVisual ElectricityPriceTitle { get; set; } =
        new LabelVisual
        {
            Text = "Electricity Price",
            TextSize = 20,
            Padding = new LiveChartsCore.Drawing.Padding(15),
            Paint = new SolidColorPaint(SKColors.White)
        };
    

    #endregion
    
    #region supply and demand chart

    private bool _isDown = false;
    private readonly ObservableCollection<ResultDataPerHour> _values = new();
    private readonly ObservableCollection<ObservablePoint> _values2 = new();

    public ISeries[] Series { get; set; }
    public Axis[] ScrollableAxes { get; set; }
    public ISeries[] ScrollbarSeries { get; set; }
    public Axis[] InvisibleX { get; set; }
    public Axis[] InvisibleY { get; set; }
    public LiveChartsCore.Measure.Margin Margin { get; set; }
    public RectangularSection[] Thumbs { get; set; }
    
    #endregion
    
    public Axis[] XAxes { get; set; }
   
    public DashboardViewModel(ResultManager resultManager)
    {
        _resultManager = resultManager;
        
        LoadScenarioOne();
        CostAndProfitSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = TotalIncomeList,
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                Name = "Income"
            },
            new LineSeries<double>
            {
                Values = TotalCostList,
                Fill = null,
                GeometryFill = null,
                GeometryStroke = null,
                Name = "Cost"
            }
        };

        ProductionUnitUsageSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                IsHoverable = false, 
                Values = new double[] { 100, 100, 100, 100},
                Stroke = null,
                Fill = new SolidColorPaint(new SKColor(30, 30, 30, 30)),
                IgnoresBarPosition = true
            },
            new ColumnSeries<double>
            {
                Values = ProductionUnitUsageList,
                Stroke = null,
                Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                IgnoresBarPosition = true,
                Name = "Usage (%):"
            }
        };
        ElectricityPriceSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = ElectricityPriceList,
                Fill = new SolidColorPaint(SKColors.CornflowerBlue), 
                Stroke = null,
                GeometryFill = null,
                GeometryStroke = null
            }
        };
    }

    #region supply and demand commands
    
    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var cartesianChart = (ICartesianChartView<SkiaSharpDrawingContext>)args.Chart;

        var x = cartesianChart.XAxes.First();

        // update the scroll bar thumb when the chart is updated (zoom/pan)
        // this will let the user know the current visible range
        var thumb = Thumbs[0];
        

        // if (x.MinLimit < 0 || x.MaxLimit > DemandData.Count - 1)
        // {
        //     Console.WriteLine("NOT IN RANGE");
        //     Console.WriteLine("XJ " + x.MaxLimit);
        //     Console.WriteLine("Count: " + (DemandData.Count - 1));
        //     return;
        // }
        thumb.Xi = x.MinLimit != null ? x.MinLimit : 0;
        thumb.Xj = x.MaxLimit != null ? x.MaxLimit : DemandData.Count - 1;

        if (thumb.Xj > DemandData.Count - 5)
        {
            Console.WriteLine("Live mode on");
            _liveMode = true;
        }
        else
        {
            Console.WriteLine("Live mode off");
            _liveMode = false;
        }
        
        
        StartPoint = thumb.Xj.ToString();
        EndPoint = thumb.Xi.ToString();

        if (thumb.Xi != null)
        {
            PositionStart = (int) thumb.Xi > 0 ? (int) thumb.Xi : 0;
            if (DemandOutput != null)
            {
                PositionEnd = (int) thumb.Xj < DemandData.Count - 1 ? (int) thumb.Xj : DemandData.Count - 1;
                CurrentProfit =
                    Math.Round(
                        DemandData.GetRange(PositionStart, PositionEnd - PositionStart + 1)
                            .Sum(x => (x.ElectricityPrice * x.Demand) - x.TotalCost), 2);
                
                var newTotalCostList = DemandData
                    .GetRange(PositionStart, PositionEnd - PositionStart + 1)
                    .Select(item => item.TotalCost)
                    .ToList();
                TotalCostList.Clear();
                foreach (var t in newTotalCostList)
                {
                    TotalCostList.Add(t);
                }
                Console.WriteLine("list:  " + TotalCostList.Count);
                
                var newTotalIncomeList = DemandData
                    .GetRange(PositionStart, PositionEnd - PositionStart + 1)
                    .Select(item => item.ElectricityPrice * item.Demand)
                    .ToList();
                TotalIncomeList.Clear();
                foreach (var t in newTotalIncomeList)
                {
                    TotalIncomeList.Add(t);
                }
                
                var electricBoilerUsage = DemandData[PositionEnd].UsedProductionUnits["ElectricBoiler"];
                var gasBoilerUsage = DemandData[PositionEnd].UsedProductionUnits["GasBoiler"];
                var oilBoilerUsage = DemandData[PositionEnd].UsedProductionUnits["OilBoiler"];
                var gasMotorUsage = DemandData[PositionEnd].UsedProductionUnits["GasMotor"];
                ProductionUnitUsageList.Clear();
                ProductionUnitUsageList.Add(electricBoilerUsage/8 * 100);
                ProductionUnitUsageList.Add(gasBoilerUsage/5 * 100);
                ProductionUnitUsageList.Add(oilBoilerUsage/4 * 100);
                ProductionUnitUsageList.Add(gasMotorUsage/3.6 * 100);
                
                var newElectricityPriceList = DemandData
                    .GetRange(PositionStart, PositionEnd - PositionStart + 1)
                    .Select(item => item.ElectricityPrice)
                    .ToList();
                ElectricityPriceList.Clear();
                foreach (var t in newElectricityPriceList)
                {
                    ElectricityPriceList.Add(t);
                }
            }
        }
        
        
       
        
        // Console.WriteLine("WHEN chart changing ");
        Console.WriteLine($"Xi: {thumb.Xi}, Xj: {thumb.Xj}");
        Console.WriteLine($"PositionStart: {PositionStart}, PositionEnd: {PositionEnd}");
    }

    [RelayCommand]
    public void PointerDown(PointerCommandArgs args)
    {
        _isDown = true;
    }

    [RelayCommand]
    public void PointerMove(PointerCommandArgs args)
    {
        if (!_isDown) return;

        var chart = (ICartesianChartView<SkiaSharpDrawingContext>)args.Chart;
        var positionInData = chart.ScalePixelsToData(args.PointerPosition);
        

        var thumb = Thumbs[0];
        var currentRange = thumb.Xj - thumb.Xi;
        
        if (positionInData.X < 0 || positionInData.X > DemandData.Count - 1) return;
        
        if (thumb.Xi < 0)
        {
            thumb.Xi = 0;
            if (thumb.Xj > DemandData.Count - 1)
            {
                thumb.Xj = DemandData.Count - 1;
                return;
            }
            return;
        }
        
        if (thumb.Xj > DemandData.Count - 1)
        {
            thumb.Xj = DemandData.Count - 1;
            if (thumb.Xi < 0)
            {
                thumb.Xi = 0;
            }
            return;
        }
        
        // update the scroll bar thumb when the user is dragging the chart
        thumb.Xi = positionInData.X - currentRange / 2;
        thumb.Xj = positionInData.X + currentRange / 2;
        
        Console.WriteLine("\n\nSTART POINT: " + thumb.Xi);
        Console.WriteLine("END POINT: " + thumb.Xj + "\n\n");


        // update the chart visible range
        XAxes[0].MinLimit = thumb.Xi;
        XAxes[0].MaxLimit = thumb.Xj;
        
    }

    [RelayCommand]
    public void PointerUp(PointerCommandArgs args)
    {
        _isDown = false;
    }
    

    #endregion

    private void Init(int scenario)
    {
        
        LoadChart(scenario);

        OptimizerManager optimizerManager = new(_resultManager, new ExcelLoader());
        
        
        Thread x = new(() =>
        {
            _liveMode = true;
            
            while (true)
            {
                foreach (var resultDataPerHours in _resultManager.GetNewResults())
                {
                    DemandData.Add(resultDataPerHours);
                    DemandOutput.Add(resultDataPerHours.Demand);
                    CurrentDate.Add(resultDataPerHours.HourStart);
                    PositionEnd = DemandOutput.Count - 1;
                    Console.WriteLine("Adaugam position end: " + PositionEnd);

                    ElBoilerProductionUnitOuput.Add(resultDataPerHours.UsedProductionUnits["ElectricBoiler"]);
                    GasBoilerProductionUnitOuput.Add(resultDataPerHours.UsedProductionUnits["GasBoiler"]);
                    OilBoilerProductionUnitOuput.Add(resultDataPerHours.UsedProductionUnits["OilBoiler"]);
                    GasMotorProductionUnitOuput.Add(resultDataPerHours.UsedProductionUnits["GasMotor"]);
                    
                    // Console.WriteLine("Added new data");
                    // Console.WriteLine("ElectricBoiler: " + resultDataPerHours.UsedProductionUnits["ElectricBoiler"]);
                    // Console.WriteLine("GasBoiler: " + resultDataPerHours.UsedProductionUnits["GasBoiler"]);
                    // Console.WriteLine("OilBoiler: " + resultDataPerHours.UsedProductionUnits["OilBoiler"]);
                    // Console.WriteLine("GasMotor: " + resultDataPerHours.UsedProductionUnits["GasMotor"]);
                    
                    if (_liveMode)
                    {
                        XAxes[0].MaxLimit++;
                        Console.WriteLine("live mode ii true aici in thread");
                    }
                }

                if (_liveMode)
                {
                    // setting profit for current selected area
                    CurrentProfit =
                        Math.Round(
                            DemandData.GetRange(PositionStart, PositionEnd - PositionStart + 1)
                                .Sum(x => (x.ElectricityPrice * x.Demand) - x.TotalCost), 2);
                    
                    // setting cost and profit graphics for current selected area
                    var newTotalCostList = DemandData
                        .GetRange(PositionStart, PositionEnd - PositionStart + 1)
                        .Select(item => item.TotalCost)
                        .ToList();

                    TotalCostList.Clear();
                    for (int i = 0; i < newTotalCostList.Count; i++)
                    {
                        TotalCostList.Add(newTotalCostList[i]);
                    }
                    Console.WriteLine("list:  " + TotalCostList.Count);
                    
                    // setting total income for current selected area
                    var newTotalIncomeList = DemandData
                        .GetRange(PositionStart, PositionEnd - PositionStart + 1)
                        .Select(item => item.ElectricityPrice * item.Demand)
                        .ToList();
                    TotalIncomeList.Clear();
                    foreach (var t in newTotalIncomeList)
                    {
                        TotalIncomeList.Add(t);
                    }
                    
                    var newElectricityPriceList = DemandData
                        .GetRange(PositionStart, PositionEnd - PositionStart + 1)
                        .Select(item => item.ElectricityPrice)
                        .ToList();
                    ElectricityPriceList.Clear();
                    foreach (var t in newElectricityPriceList)
                    {
                        ElectricityPriceList.Add(t);
                    }
                }

                Console.WriteLine("Current profit: " + CurrentProfit);
                // GetData();
                // Console.WriteLine("Getting data");
                Thread.Sleep(1000);
            }
        });
        
        x.Start();
        
    }


    public void LoadScenarioOne()
    {
        Console.WriteLine("Loading scenario one");
        Init(1);
        
    }
    
    public void LoadScenarioTwo()
    {
        Console.WriteLine("Loading scenario two");
        Init(2);
    }

    public void LoadChart(int scenario)
    {
        #region supply and demand chart


        if (scenario == 1)
        {
            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = DemandOutput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1),
                    Name = "Demand"
                },
                new LineSeries<double>
                {
                    Values = GasMotorProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1),
                    Name = "Gas Motor"
                },
                new LineSeries<double>
                {
                    Values = GasBoilerProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1),
                    Name = "Gas Boiler"
                },
                new LineSeries<double>
                {
                    Values = OilBoilerProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1),
                    Name = "Oil Boiler"
                },
            };
        
            ScrollbarSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = DemandOutput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
                new LineSeries<double>
                {
                    Values = GasMotorProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
                new LineSeries<double>
                {
                    Values = GasBoilerProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
                new LineSeries<double>
                {
                    Values = OilBoilerProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
            };
        }
        else if (scenario == 2)
        {
            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = DemandOutput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
                new LineSeries<double>
                {
                    Values = GasMotorProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
                new LineSeries<double>
                {
                    Values = GasBoilerProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
                new LineSeries<double>
                {
                    Values = OilBoilerProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
                new LineSeries<double>
                {
                    Values = ElBoilerProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
            };
        
            ScrollbarSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = DemandOutput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
                new LineSeries<double>
                {
                    Values = GasMotorProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
                new LineSeries<double>
                {
                    Values = GasBoilerProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
                new LineSeries<double>
                {
                    Values = OilBoilerProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
                new LineSeries<double>
                {
                    Values = ElBoilerProductionUnitOuput,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                },
            };
        }

        ScrollableAxes = new[] { new Axis() };

        Thumbs = new[]
        {
            new RectangularSection
            {
                Fill = new SolidColorPaint(new SKColor(255, 205, 210, 100))
            }
        };

        InvisibleX = new[] { new Axis { IsVisible = false } };
        InvisibleY = new[] { new Axis { IsVisible = false } };

        // force the left margin to be 100 and the right margin 50 in both charts, this will
        // align the start and end point of the "draw margin",
        // no matter the size of the labels in the Y axis of both chart.
        var auto = LiveChartsCore.Measure.Margin.Auto;
        Margin = new(100, auto, 50, auto);
        
        #endregion
        
        XAxes = new Axis[]
        {
            new Axis
            {
                Name = "Date & Time", 
                NameTextSize = 12,
                TextSize = 10,
                LabelsRotation = -45,
                Labels = CurrentDate,
                ShowSeparatorLines = true,
                IsVisible = false
 
               
            }
        };
        
        
      
    }
    
}