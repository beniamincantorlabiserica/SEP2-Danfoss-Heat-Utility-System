using System;
using System.Collections.Generic;
using System.Text.Json;

namespace HUS.Data;

public class PriceRequest
{
    private readonly ApiService _api;
    
    public PriceRequest(ApiService? service)
    {
        _api = (service is not null) ? service : new ApiService();
    }

    public float GetElectricityPrice(DateTime date, TimeOnly time)
    {
        var res = _api.GetAsync(
            $"https://api.energidataservice.dk/dataset/DatahubPricelist?start={date.Year}-{date.Month.ToString("00")}-{date.Day.ToString("00")}&end={date.AddDays(1).Year}-{date.AddDays(1).Month.ToString("00")}-{date.AddDays(1).Day.ToString("00")}&limit=1");
        
        Rootobject? deserializedObject = JsonSerializer.Deserialize<Rootobject>(res.Result);

        Dictionary<TimeOnly, float> returnDictionary = new Dictionary<TimeOnly, float>();

        returnDictionary.Add(new TimeOnly(00, 00), deserializedObject?.records[0].Price1 ?? default(float));
        returnDictionary.Add(new TimeOnly(01, 00), deserializedObject?.records[0].Price2 ?? default(float));
        returnDictionary.Add(new TimeOnly(02, 00), deserializedObject?.records[0].Price3 ?? default(float));
        returnDictionary.Add(new TimeOnly(03, 00), deserializedObject?.records[0].Price4 ?? default(float));
        returnDictionary.Add(new TimeOnly(04, 00), deserializedObject?.records[0].Price5 ?? default(float));
        returnDictionary.Add(new TimeOnly(05, 00), deserializedObject?.records[0].Price6 ?? default(float));
        returnDictionary.Add(new TimeOnly(06, 00), deserializedObject?.records[0].Price7 ?? default(float));
        returnDictionary.Add(new TimeOnly(07, 00), deserializedObject?.records[0].Price8 ?? default(float));
        returnDictionary.Add(new TimeOnly(08, 00), deserializedObject?.records[0].Price9 ?? default(float));
        returnDictionary.Add(new TimeOnly(09, 00), deserializedObject?.records[0].Price10 ?? default(float));
        returnDictionary.Add(new TimeOnly(10, 00), deserializedObject?.records[0].Price11 ?? default(float));
        returnDictionary.Add(new TimeOnly(11, 00), deserializedObject?.records[0].Price12 ?? default(float));
        returnDictionary.Add(new TimeOnly(12, 00), deserializedObject?.records[0].Price13 ?? default(float));
        returnDictionary.Add(new TimeOnly(13, 00), deserializedObject?.records[0].Price14 ?? default(float));
        returnDictionary.Add(new TimeOnly(14, 00), deserializedObject?.records[0].Price15 ?? default(float));
        returnDictionary.Add(new TimeOnly(15, 00), deserializedObject?.records[0].Price16 ?? default(float));
        returnDictionary.Add(new TimeOnly(16, 00), deserializedObject?.records[0].Price17 ?? default(float));
        returnDictionary.Add(new TimeOnly(17, 00), deserializedObject?.records[0].Price18 ?? default(float));
        returnDictionary.Add(new TimeOnly(18, 00), deserializedObject?.records[0].Price19 ?? default(float));
        returnDictionary.Add(new TimeOnly(19, 00), deserializedObject?.records[0].Price20 ?? default(float));
        returnDictionary.Add(new TimeOnly(20, 00), deserializedObject?.records[0].Price21 ?? default(float));
        returnDictionary.Add(new TimeOnly(21, 00), deserializedObject?.records[0].Price22 ?? default(float));
        returnDictionary.Add(new TimeOnly(22, 00), deserializedObject?.records[0].Price23 ?? default(float));
        returnDictionary.Add(new TimeOnly(23, 00), deserializedObject?.records[0].Price24 ?? default(float));


        return returnDictionary[time];
    }
}