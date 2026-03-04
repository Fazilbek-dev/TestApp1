using System.IO;
using UnityEngine;
using App1.Models;

namespace App1.Services
{
    public class JsonSaveSystem
    {
        private string filePath => Application.persistentDataPath + "/inventory_save.json";

        public void Save(InventorySaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
        }

        public InventorySaveData Load()
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<InventorySaveData>(json);
        }
    }
}