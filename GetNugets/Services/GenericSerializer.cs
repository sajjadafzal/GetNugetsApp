using GetNugets.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GetNugets.Services
{
    internal static class GenericSerializer
    {
        public static void Serialize<T>(T TObject, string filePath)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize<T>(TObject, options);
            File.WriteAllText(filePath, jsonString);
        }

        public static T Deserialize<T>(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            T? obj =  JsonSerializer.Deserialize<T>(jsonString);
            return obj;
        }
    }
}
