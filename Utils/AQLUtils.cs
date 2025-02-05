using System;
using System.Collections.Generic;
using EHRJs.Models;
using Newtonsoft.Json;

namespace EHRJs.Utils;
public class AQLUtils
{
    public static List<VitalsModel> GetVitalRecords(string json)
    {
        var openEhrResponse = JsonConvert.DeserializeObject<OpenEhrResponse>(json);
        var vitalRecords = new List<VitalsModel>();
        foreach (var row in openEhrResponse.Rows)
        {
            var vitalRecord = new VitalsModel()
            {
                Time = row[0].ToString(),
                Uid = row[1].ToString(),
                Systolic = Convert.ToDouble(row[2]),
                Diastolic = Convert.ToDouble(row[3]),
                Height = Convert.ToDouble(row[4]),
                Weight = Convert.ToDouble(row[5]),
                Spo2 = Convert.ToDouble(row[6])
            };
            vitalRecords.Add(vitalRecord);
        }
        return vitalRecords;
    }
}
public class OpenEhrResponse
{
    public MetaData Meta { get; set; }
    public string Q { get; set; }
    public List<Column> Columns { get; set; }
    public List<List<object>> Rows { get; set; }
}

public class MetaData
{
    public string _type { get; set; }
    public string _schema_version { get; set; }
    public string _created { get; set; }
    public string _executed_aql { get; set; }
    public int resultsize { get; set; }
}

public class Column
{
    public string Path { get; set; }
    public string Name { get; set; }
}
