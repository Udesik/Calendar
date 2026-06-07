using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SavedHolidayItem
{
    public string dateString; // Храним дату как текст "yyyy-MM-dd"
    public string holidayName;

    public SavedHolidayItem(DateTime date, string name)
    {
        this.dateString = date.ToString("yyyy-MM-dd");
        this.holidayName = name;
    }
}

// Класс-контейнер, который Unity превратит в текстовый файл
[Serializable]
public class HolidayCacheContainer
{
    public List<SavedHolidayItem> items = new List<SavedHolidayItem>();
}
