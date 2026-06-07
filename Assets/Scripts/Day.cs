using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Day
{
    public DateTime Date { get; private set; }
    public List<Event> Events { get; private set; }
    public bool IsHoliday { get; private set; }
    public string HolidayName { get; private set; }

    public Day(DateTime date, List<Event> events, bool isHoliday, string holidayName)
    {
        Date = date;

        if (events == null)
        {
            Events = new List<Event>(){};
        }
        else
        {
            Events = events;
        }

        IsHoliday = isHoliday;

        if (isHoliday)
        {
            HolidayName = holidayName;
        }
        else
        {
            HolidayName = null;
        }
    }

    public void SetHoliday()
    {
        if (IsHoliday)
        {
            IsHoliday = false;
        }
        else
        {
            IsHoliday = true;
        }
    }

    public void AddEvent(Event eventToAdd)
    {
        Events.Add(eventToAdd);
    }
}
