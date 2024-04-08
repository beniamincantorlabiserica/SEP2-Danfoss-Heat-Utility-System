using System;
using System.Collections.ObjectModel;
using DefaultNamespace;
using HUS.Data;
using HUS.Model;

namespace HUS.ViewModel;

public class DashboardViewModel : ViewModelBase
{
    
    private readonly ResultManager _resultManager;
    
    public ObservableCollection<ResultDataPerHour> Results => new(_resultManager.GetResults());

    public DashboardViewModel()
    {
        Console.WriteLine("Entered in the viewmodel constructor.");
        _resultManager = new ResultManager();
        var excelLoader = new ExcelLoader();
        var optimizerManager = new OptimizerManager(_resultManager, excelLoader);
        
    }
}