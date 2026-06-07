using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class HolidayCacheStorage
{
    // Путь к файлу на компьютере или телефоне
    private static string FilePath => Path.Combine(Application.persistentDataPath, "holidays_dictionary_cache.json");

    // 1. ЗАШИФРОВАТЬ (Сохранить словарь в JSON)
    public static void SaveHolidays(Dictionary<DateTime, string> dictionaryToSave)
    {
        HolidayCacheContainer container = new HolidayCacheContainer();

        // Переводим Dictionary в List элементов SavedHolidayItem
        foreach (var pair in dictionaryToSave)
        {
            SavedHolidayItem item = new SavedHolidayItem(pair.Key, pair.Value);
            container.items.Add(item);
        }

        // Превращаем C# объект в текст JSON
        string jsonText = JsonUtility.ToJson(container, true);

        // 1. Получаем путь к папке, в которой должен лежать файл
        string directoryPath = Path.GetDirectoryName(FilePath);

        // 2. Если этой папки на компьютере еще нет — принудительно создаем её
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 3. Теперь безопасно записываем текст в файл, ошибки не будет!
        File.WriteAllText(FilePath, jsonText);

        Debug.Log($"[Storage] Словарь праздников сохранен в: {FilePath}");
    }

    // 2. РАСШИФРОВАТЬ (Загрузить JSON в словарь)
    public static Dictionary<DateTime, string> LoadHolidays()
    {
        // Если файла еще нет (первый запуск игры), возвращаем пустой словарь
        if (File.Exists(FilePath) == false)
        {
            Debug.LogWarning("[Storage] Файл кэша не найден. Возвращаем пустой словарь.");
            return new Dictionary<DateTime, string>();
        }

        // Читаем текст из файла
        string jsonText = File.ReadAllText(FilePath);

        // Расшифровываем JSON обратно в список
        HolidayCacheContainer container = JsonUtility.FromJson<HolidayCacheContainer>(jsonText);

        Dictionary<DateTime, string> restoredDictionary = new Dictionary<DateTime, string>();

        // Переносим элементы из списка обратно в быстрый Dictionary
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
