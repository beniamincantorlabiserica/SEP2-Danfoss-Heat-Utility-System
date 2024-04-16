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

    public DashboardViewModel(ResultManager resultManager)
    {
        
    }
}