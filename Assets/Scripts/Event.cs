using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Event
{
    public string Description;
    public bool IsCompleted;

    public Event(string description)
    {
        Description = description;
        IsCompleted = false;
    }

    public void Complete()
    {
        IsCompleted = true;
    }
}

[Serializable]
public class SavedEventGroup
{
    public string dateString; // Дата в текстовом формате "yyyy-MM-dd"
    public List<Event> tasks;

    public SavedEventGroup(DateTime date, List<Event> tasksList)
    {
        this.dateString = date.ToString("yyyy-MM-dd");
        this.tasks = tasksList;
    }
}

[Serializable]
public class EventCacheContainer
{
    public List<SavedEventGroup> groups = new List<SavedEventGroup>();
}
