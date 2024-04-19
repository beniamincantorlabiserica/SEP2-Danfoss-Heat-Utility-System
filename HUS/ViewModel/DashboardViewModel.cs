using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using HUS.Data;

namespace HUS.ViewModel;

public partial class DashboardViewModel : ViewModelBase
{
    private ResultManager _resultManager;
    public ObservableCollection<double> DemandOutput = new ObservableCollection<double>();
    public ObservableCollection<string> CurrentDate = new ObservableCollection<string>();
    public List<ResultDataPerHour> DemandData = new List<ResultDataPerHour>();

    private bool liveMode = true;
    
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
          
    }

    private void GetData()
    {
        DemandData = _resultManager.GetResults();
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
        thumb.Xi = x.MinLimit;
        thumb.Xj = x.MaxLimit;

        if (thumb.Xj > DemandData.Count - 5)
        {
            Console.WriteLine("Live mode on");
            liveMode = true;
        }
        else
        {
            Console.WriteLine("Live mode off");
            liveMode = false;
        }
        
        StartPoint = thumb.Xj.ToString();
        EndPoint = thumb.Xi.ToString();
        // Console.WriteLine("WHEN chart changing ");
        // Console.WriteLine($"Xi: {thumb.Xi}, Xj: {thumb.Xj}");
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

    private void init(int scenario)
    {
        
        
        OptimizerManager optimizerManager = new(_resultManager, new ExcelLoader());
        
        
        Thread x = new(() =>
        {
            while (true)
            {
                foreach (var resultDataPerHours in _resultManager.GetNewResults())
                {
                    DemandData.Add(resultDataPerHours);
                    DemandOutput.Add(resultDataPerHours.Demand);
                    CurrentDate.Add(resultDataPerHours.HourStart);
                    if (liveMode)
                    {
                        XAxes[0].MaxLimit++;
                    }
                } 
                // GetData();
                Console.WriteLine("Getting data");
                Thread.Sleep(1000);
            }
        });
        
        x.Start();

    }


    public void LoadScenarioOne()
    {
        Console.WriteLine("Loading scenario one");
        init(1);
        
    }
    
    public void LoadScenarioTwo()
    {
        Console.WriteLine("Loading scenario two");
        init(2);
    }

    public void LoadChart()
    {
        #region supply and demand chart
        
        Series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = DemandOutput,
                GeometryStroke = null,
                GeometryFill = null,
                DataPadding = new(0, 1)
            }
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
            new LineSeries<ObservablePoint>
            {
                Values = _values2,
                GeometryStroke = null,
                GeometryFill = null,
                DataPadding = new(0, 1)
            }
        };

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