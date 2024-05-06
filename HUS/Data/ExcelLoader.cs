using System;
using System.Collections.Generic;
using System.Globalization;
using ClosedXML.Excel;
using HUS.Model;

namespace HUS.Data;

public class ExcelLoader
{
    // Adapt to your own file path
    // TODO: Change the file path to the correct one
    private const string FilePath = "/Users/beniamin/Documents/SDU/SEM 2/SEP/HUS/HUS/Assets/data.xlsx";
    
    /// <summary>
    /// Constructor for the ExcelLoader class
    /// </summary>
    public ExcelLoader()
    {
        
    }

    
    /// <summary>
    /// Function to get data from the excel file
    /// </summary>
    public List<DataPerHour> GetData()
    {
       
        var data = new List<DataPerHour>();
        using var workbook = new XLWorkbook(FilePath);
        var worksheet = workbook.Worksheet(1); // Assuming data is in the first sheet
        // Load Winter data from B4 to E171
        for (var row = 4; row <= 171; row++)
        {
                
            data.Add(CreateDataPerHourFromRow(worksheet, row, 2, "Winter"));
        }

        // Load Summer data from G4 to J171
        for (var row = 4; row <= 171; row++)
        {
            data.Add(CreateDataPerHourFromRow(worksheet, row, 7, "Summer"));
        }

        return data;
    }

    /// <summary>
    /// Function to create DataPerHour object from a row in the excel file
    /// </summary>
    /// <param name="worksheet"> current working worksheet </param>
    /// <param name="rowNumber"> row number in use </param>
    /// <param name="startColumn"> column number in use</param>
    /// <param name="period"> period of the data (summer/winter) </param>
    /// <returns> A DataPerHour type object to be added to the list</returns>
    private static DataPerHour CreateDataPerHourFromRow(IXLWorksheet worksheet, int rowNumber, int startColumn, string period)
    {
        var hourStartCell = worksheet.Cell(rowNumber, startColumn);
        var hourStart = hourStartCell.TryGetValue(out DateTime startDateTime) ? startDateTime.ToString(CultureInfo.CurrentCulture) : "00:00";

        string hourEnd;
        // Check if processing the last row for the summer period
        if (rowNumber == 171 && startColumn == 7) // Adjusted check for the last row of summer
        {
            // Add only 1 hour to hourStart for this specific case, correcting the previous misunderstanding
            var adjustedEndDateTime = startDateTime.AddHours(1);
            hourEnd = adjustedEndDateTime.ToString(CultureInfo.CurrentCulture);
        }
        else
        {
            var hourEndCell = worksheet.Cell(rowNumber, startColumn + 1);
            if (!hourEndCell.TryGetValue(out DateTime endDateTime))
            {
                // Console.WriteLine($"Warning: Cannot convert hour end in row {rowNumber} to DateTime. Using hour start as fallback.");
                hourEnd = hourStart; // Using hourStart as a fallback or another default value
            }
            else
            {
                hourEnd = endDateTime.ToString(CultureInfo.CurrentCulture);
            }
        }

        var culture = new CultureInfo("de-DE");
        double.TryParse(worksheet.Cell(rowNumber, startColumn + 2).GetValue<string>(), NumberStyles.Any, culture, out var demand);
        double.TryParse(worksheet.Cell(rowNumber, startColumn + 3).GetValue<string>(), NumberStyles.Any, culture, out var electricityPrice);

        return new DataPerHour(hourStart, hourEnd, demand, electricityPrice, period);
    }

    
    /// <summary>
    /// Function to write the data to the console
    /// </summary>
    /// <param name="data"> current list to write to console</param>
    private void WriteDataToConsole(List<DataPerHour> data)
    {
        foreach (var d in data)
        {
            Console.WriteLine($"HourStart: {d.HourStart}, HourEnd: {d.HourEnd}, Demand: {d.Demand}, ElectricityPrice: {d.ElectricityPrice}, Period: {d.Period}");
        }
    }
}