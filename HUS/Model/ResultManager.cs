using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;

namespace HUS.Model;

public class ResultManager
{
    private List<ResultDataPerHour> Results { get; set; }
    
    private static string connectionString = @"Data source=../HUS/HeatUtilityDB/heatutility.db;Version=3;";

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

    public static void AddData()
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string hourStart = "12:10";
            string hourEnd = "12:20";
            float demand = 12;
            float electricityPrice = 2;
            string timePeriod = "1000";
            float totalCost = 23;
            double prodGas = 2.234;
            double prodOil = 1.3;
            double prodElectric = 0.3;
            double prodGasMotor = 1.1;

            using (SQLiteCommand command = new SQLiteCommand(connection))
            {
                for (int i = 0; i < hourStart.Length; i++)
                {
                    command.CommandText = @"INESERT INTO ProductionUnits (HourStart, HourEnd, Demand, ElectricityPrice, TimePeriod, TotalCost, ProdGas, ProdOil, ProdElectric, ProdGasMotor)
                                        VALUES (@HourStart, @HourEnd, @Demand, @ElectricityPrice, @TimePeriod, @TotalCost, @ProdGas, @ProdOil, @ProdElectric, @ProdGasMotor);";
                    command.Parameters.AddWithValue("@HourStart", hourStart);
                    command.Parameters.AddWithValue("@HourEnd", hourEnd);
                    command.Parameters.AddWithValue("@Demand", demand);
                    command.Parameters.AddWithValue("@ElectricityPrice", electricityPrice);
                    command.Parameters.AddWithValue("@TimePeriod", timePeriod);
                    command.Parameters.AddWithValue("@TotalCost", totalCost);
                    command.Parameters.AddWithValue("@ProdGas", prodGas);
                    command.Parameters.AddWithValue("@ProdOil", prodOil);
                    command.Parameters.AddWithValue("@ProdElectric", prodElectric);
                    command.Parameters.AddWithValue("@ProdGasMotor", prodGasMotor);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
        }
    }

}