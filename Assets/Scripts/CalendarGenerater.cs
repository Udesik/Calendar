using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CalendarGenerater : MonoBehaviour
{
    [SerializeField] private List<DayPrefab> _dayPrefabs;
    [SerializeField] private CalendarSystem _calendarCache;
    [SerializeField] private TMP_Text _dateText;
    
    public int _month = DateTime.Now.Month;
    public int _year = DateTime.Now.Year;

    public void BuildCalendar(int year, int month)
    {
        DateTime firstDay = new DateTime(year, month, 1);
        _dateText.text = $"{month}. {year}";
        int offset = ((int)firstDay.DayOfWeek + 6) % 7;

        Dictionary<DateTime, string> savedHolidays = _calendarCache.GetHolidays();
        Dictionary<DateTime, List<Event>> savedEvents = _calendarCache.GetEvents();

        for (int j = 0; j < offset; j++)
        {
            _dayPrefabs[j].Invis();
        }

        for (int i = 0; i < DateTime.DaysInMonth(year, month); i++)
        {
            DateTime day = firstDay.AddDays(i).Date; 
            int cellIndex = i + offset;

            List<Event> currentTasks = null;
            bool isHoliday = false;
            string holidayName = null;

            if (savedEvents != null && savedEvents.TryGetValue(day, out List<Event> tasks))
            {
                currentTasks = tasks;
            }

            if (savedHolidays != null && savedHolidays.TryGetValue(day, out string name))
            {  
                isHoliday = true;
                holidayName = name;
            }
            else if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
            {
                isHoliday = true;
                holidayName = "Обычный выходной";
            }

            _dayPrefabs[cellIndex].Init(new Day(day, currentTasks, isHoliday, holidayName));
            _dayPrefabs[cellIndex].SetText(day.Day.ToString());
        }

        for (int z = offset + DateTime.DaysInMonth(year, month); z < _dayPrefabs.Count; z++)
        {
            _dayPrefabs[z].Invis();
        }
    }

    private void ChackStorageHolidays()
    {
        Dictionary<DateTime, string> savedHolidays = _calendarCache.GetHolidays();

        if (savedHolidays == null || savedHolidays.Keys.Count(date => date.Year == _year) == 0)
        {
            _calendarCache.Init(_year);
        }
        else
        {
            BuildCalendar(_year, _month);
        }
    }

    public void NextMonth()
    {
        _month++;

        if (_month > 12)
        {
            _month = 1;
            _year++;
        }
        
        ChackStorageHolidays();
    }

    public void PrevMonth()
    {
        _month--;

        if (_month < 1)
        {
            _month = 12;
            _year--;
        }

        ChackStorageHolidays();
    }
}
