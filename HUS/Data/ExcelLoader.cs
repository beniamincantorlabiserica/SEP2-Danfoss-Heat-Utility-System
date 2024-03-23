namespace DefaultNamespace;

public class ExcelLoader
{
    private const string filePath = "C:\\Users\\User\\Desktop\\Data.xlsx";
    
    public ExcelLoader()
    {
        WriteDataToConsole();
    }

    public List<DataPerHour> GetData()
    {
        var HourlyData = new List<DataPerHour>();
        using (var workbook = new XLWorkbook(filePath))
        {
            var worksheet = workbook.Worksheet(1); // Assuming the data is in the first worksheet
            var rows = worksheet.RangeUsed().RowsUsed(); // Skip header row
            
            foreach (var row in rows)
            {
                var hourStart = row.Cell(1).GetValue<string>();
                var hourEnd = row.Cell(2).GetValue<string>();
                var demand = row.Cell(3).GetValue<double>();
                var electricityPrice = row.Cell(4).GetValue<double>();
                var period = row.Cell(5).GetValue<string>();

                HourlyData.Add(new DataPerHour(hourStart, hourEnd, demand, electricityPrice, period));
            }
        }

        return HourlyData;
    }
    
    private void WriteDataToConsole()
    {
        var HourlyData = GetData();
        foreach (var data in HourlyData)
        {
            Console.WriteLine($"HourStart: {data.HourStart}, HourEnd: {data.HourEnd}, Demand: {data.Demand}, ElectricityPrice: {data.ElectricityPrice}, Period: {data.Period}");
        }
    }
}