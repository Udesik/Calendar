using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class EventCacheStorage
{
    private static string FilePath => Path.Combine(Application.persistentDataPath, "user_tasks_dictionary.json");

    public static void SaveTasks(Dictionary<DateTime, List<Event>> tasksDictionary)
    {
        EventCacheContainer container = new EventCacheContainer();

        foreach (KeyValuePair<DateTime, List<Event>> pair in tasksDictionary)
        {
            if (pair.Value != null && pair.Value.Count > 0)
            {
                SavedEventGroup group = new SavedEventGroup(pair.Key, pair.Value);
                container.groups.Add(group);
            }
        }

        string jsonText = JsonUtility.ToJson(container, true);
        
        string directoryPath = Path.GetDirectoryName(FilePath);
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        File.WriteAllText(FilePath, jsonText);
        Debug.Log($"[TasksStorage] Словарь задач успешно сохранен на устройство!");
    }

    public static Dictionary<DateTime, List<Event>> LoadTasks()
    {
        if (!File.Exists(FilePath))
        {
            Debug.LogWarning("[TasksStorage] Файл задач не найден. Создан новый пустой словарь.");
            return new Dictionary<DateTime, List<Event>>();
        }

        string jsonText = File.ReadAllText(FilePath);
        EventCacheContainer container = JsonUtility.FromJson<EventCacheContainer>(jsonText);

        Dictionary<DateTime, List<Event>> restoredDictionary = new Dictionary<DateTime, List<Event>>();

        foreach (SavedEventGroup group in container.groups)
        {
            if (DateTime.TryParse(group.dateString, out DateTime parsedDate))
            {
                restoredDictionary[parsedDate.Date] = group.tasks;
            }
        }

        Debug.Log($"[TasksStorage] Словарь задач успешно восстановлен! Дней с задачами: {restoredDictionary.Count}");
        return restoredDictionary;
    }
}
