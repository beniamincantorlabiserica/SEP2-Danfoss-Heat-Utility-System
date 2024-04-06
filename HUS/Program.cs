using Avalonia;
using System;
using HUS.Data;

namespace HUS;

class Program
{
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    
    
    public static void Main(string[] args)
    {
        
        ExcelLoader loader = new();
        AssetManager assets = new AssetManager();
        /*
        List<ReturnOptimizedData> listOfReturnData = new List<ReturnOptimizedData>();
        Optimizer optimizer = new Optimizer(dataPerHour: loader.GetData(), heatingAssets: assets.GetAssets(), optimizerOutput: listOfReturnData, sleepTime: 20);
        */
        Optimizer optimizer = new Optimizer(dataPerHour: loader.GetData(), heatingAssets: assets.GetAssets(), sleepTime: 20);
        
        

        while (true)
        {
            if (!optimizer.IsOperating)
            {
                optimizer.PrintOptimizedAssets();
                optimizer.PrintOutputData(optimizer.ProcessedData);
                break;
            }
            
        }
        








        if (!Utils.IsUnderDevelopment)
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }

    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

}
