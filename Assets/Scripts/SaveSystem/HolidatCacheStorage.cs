using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class HolidayCacheStorage
{
    private static string FilePath => Path.Combine(Application.persistentDataPath, "holidays_dictionary_cache.json");

    public static void SaveHolidays(Dictionary<DateTime, string> dictionaryToSave)
    {
        HolidayCacheContainer container = new HolidayCacheContainer();

        foreach (var pair in dictionaryToSave)
        {
            SavedHolidayItem item = new SavedHolidayItem(pair.Key, pair.Value);
            container.items.Add(item);
        }

        string jsonText = JsonUtility.ToJson(container, true);
        string directoryPath = Path.GetDirectoryName(FilePath);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(FilePath, jsonText);
        Debug.Log($"[Storage] Словарь праздников сохранен в: {FilePath}");
    }

    public static Dictionary<DateTime, string> LoadHolidays()
    {
        if (File.Exists(FilePath) == false)
        {
            Debug.LogWarning("[Storage] Файл кэша не найден. Возвращаем пустой словарь.");
            return new Dictionary<DateTime, string>();
        }

        string jsonText = File.ReadAllText(FilePath);
        HolidayCacheContainer container = JsonUtility.FromJson<HolidayCacheContainer>(jsonText);
        
        Dictionary<DateTime, string> restoredDictionary = new Dictionary<DateTime, string>();

        foreach (var item in container.items)
        {
            if (DateTime.TryParse(item.dateString, out DateTime parsedDate))
            {
                restoredDictionary[parsedDate.Date] = item.holidayName;
            }
        }

        Debug.Log($"[Storage] Словарь успешно восстановлен! Загружено праздников: {restoredDictionary.Count}");
        return restoredDictionary;
    }
}
