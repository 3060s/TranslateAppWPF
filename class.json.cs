using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

public class Json
{
    public static Dictionary<string, string> LoadDictionaryFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
        else
        {
            return new Dictionary<string, string>();
        }
    }

    public static void SaveDictionaryToFile(string filePath, Dictionary<string, string> dictionary)
    {
        string json = JsonConvert.SerializeObject(dictionary, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(filePath, json);
    }
}