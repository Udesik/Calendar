using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CalendarSystem : MonoBehaviour
{
    [SerializeField] private CalendarGenerater _calendarGenerater;
    private Dictionary<DateTime, string> _yearHolidays = new Dictionary<DateTime, string>();
    private Dictionary<DateTime, List<Event>> _yearEvents = new Dictionary<DateTime, List<Event>>();
    private int _year = DateTime.Now.Year;
    private int _month = DateTime.Now.Month;

    private void Start()
    {
        Init(_year);

        int holidaysInThisYear = _yearHolidays.Keys.Count(date => date.Year == _year);

        if (holidaysInThisYear > 0)
        {
            Debug.Log("[CalendarSystem] Кэш успешно найден на диске. Мгновенно строим календарь!");
            
            if (_calendarGenerater != null)
            {
                _calendarGenerater.BuildCalendar(_year, _month);
            }
        }
    }

    private void OnHolidaysDownloaded(Dictionary<DateTime, string> downloadedData)
    {
        foreach (KeyValuePair<DateTime, string> pair in downloadedData)
        {
            _yearHolidays[pair.Key] = pair.Value;
            Debug.Log($"[CalendarSystem] Добавлен новый праздник: {pair.Key.ToString("dd.MM.yyyy")}, {pair.Value}");
        }

        HolidayCacheStorage.SaveHolidays(_yearHolidays);

        CalendarGenerater generator = FindObjectOfType<CalendarGenerater>();
        
        if (generator != null)
        {
            generator.BuildCalendar(generator._year, generator._month);
        }
    }

    public void AddCustomHoliday(DateTime date, string name)
    {
        _yearHolidays[date.Date] = name;
        HolidayCacheStorage.SaveHolidays(_yearHolidays);
        _calendarGenerater.BuildCalendar(_calendarGenerater._year, _calendarGenerater._month);
    }

    public void AddCustomEvent(DateTime date, List<Event> tasks)
    {
        _yearEvents[date.Date] = tasks;
        EventCacheStorage.SaveTasks(_yearEvents);
        _calendarGenerater.BuildCalendar(_calendarGenerater._year, _calendarGenerater._month);
    }

    public void DeleteHoliday(DateTime date)
    {
        _yearHolidays.Remove(date.Date);
        HolidayCacheStorage.SaveHolidays(_yearHolidays);
        _calendarGenerater.BuildCalendar(_year, _month);
    }

    public void DeleteEvent(DateTime date, List<Event> events)
    {
        if (events.Count == 0)
        {
            _yearEvents.Remove(date.Date);
        }
        else
        {
            _yearEvents[date.Date] = events;
        }

        EventCacheStorage.SaveTasks(_yearEvents);
        _calendarGenerater.BuildCalendar(_year, _month);
    }

    public Dictionary<DateTime, string> GetHolidays()
    {
        return new Dictionary<DateTime, string>(_yearHolidays);
    }

    public Dictionary<DateTime, List<Event>> GetEvents()
    {
        return new Dictionary<DateTime, List<Event>>(_yearEvents);
    }

    public void Init(int year)
    {
        if (_yearHolidays == null || _yearHolidays.Count == 0)
        {
            _yearHolidays = HolidayCacheStorage.LoadHolidays();
        }

        if (_yearEvents == null || _yearEvents.Count == 0)
        {
            _yearEvents = EventCacheStorage.LoadTasks();
        }

        int holidaysInThisYear = _yearHolidays.Keys.Count(date => date.Year == year);

        if (holidaysInThisYear == 0)
        {
            Debug.Log($"Кэша для {year} года нет в словаре. Отправляем запрос к API...");
            StartCoroutine(HolidayLoader.FetchYearHolidays(year, "RU", OnHolidaysDownloaded));
        }
        else
        {
            Debug.Log($"Праздники за {year} год уже успешно находятся в оперативной памяти!");
        }
    }
}
