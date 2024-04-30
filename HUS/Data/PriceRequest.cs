using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace HUS.Data;
/// <summary>
/// Class in charge of making api calls on demand for electricity spot prices.
/// Returns the electricity spot price of an hour as a float based on the date time parameter
/// </summary>
public class PriceRequest
{
   private readonly HttpClient _client;
    
   
   private class Rootobject
   {
       public int total { get; set; }
       public int limit { get; set; }
       public string dataset { get; set; }
       public Record[] records { get; set; }
   }

   private class Record
   {
       public DateTime HourUTC { get; set; }
       public DateTime HourDK { get; set; }
       public string PriceArea { get; set; }
       public float SpotPriceDKK { get; set; }
       public float SpotPriceEUR { get; set; }
   }
   
    public PriceRequest()
    {
        _client = new HttpClient();
    }
    
    private async Task<string> GetAsync(string uri)
    {
        using HttpResponseMessage response = _client.GetAsync(uri).Result;
        return await response.Content.ReadAsStringAsync();
    }

    public float GetElectricityPrice(DateTime date)
    {
        var res = GetAsync(
            $"https://api.energidataservice.dk/dataset/Elspotprices?limit=1&start={date.Year}-{date.Month.ToString("00")}-{date.Day.ToString("00")}T{date.Hour.ToString("00")}:{date.Minute.ToString("00")}&filter={{\"PriceArea\":[\"DK1\"]}}&sort=HourDK asc");
        Rootobject? deserializedObject = JsonSerializer.Deserialize<Rootobject>(res.Result);
        return deserializedObject?.records[0].SpotPriceDKK ?? default(float);
    }
}