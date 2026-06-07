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
            // Если данные были на диске, принудительно строим календарь СЕЙЧАС
            if (_calendarGenerater != null)
            {
                _calendarGenerater.BuildCalendar(_year, _month);
            }
        }
    }

    // Этот метод автоматически выполнится САМ, как только интернет-запрос завершится и расшифруется
    // Этот метод сработает ТОЛЬКО если мы качали данные из интернета
    private void OnHolidaysDownloaded(Dictionary<DateTime, string> downloadedData)
    {
        foreach (KeyValuePair<DateTime, string> pair in downloadedData)
        {
            // Используем квадратные скобки: 
            // Если такой даты еще не было — она добавится.
            // Если дата уже существовала — она обновится.
            _yearHolidays[pair.Key] = pair.Value;
            Debug.Log($"[CalendarSystem] Добавлен новый праздник: {pair.Key.ToString("dd.MM.yyyy")}, {pair.Value}");
        }

        // 3. ЗАШИФРОВЫВАЕМ и сохраняем скачанный словарь в файл на будущее
        HolidayCacheStorage.SaveHolidays(_yearHolidays);

        CalendarGenerater generator = FindObjectOfType<CalendarGenerater>();
        if (generator != null)
        {
            // Передаем текущие год и месяц из генератора, чтобы он нарисовал свежие данные
            generator.BuildCalendar(generator._year, generator._month);
        }
    }

    // Если пользователь вручную добавил свой праздник — просто пересохраните словарь:
    public void AddCustomHoliday(DateTime date, string name)
    {
        _yearHolidays[date.Date] = name;
        HolidayCacheStorage.SaveHolidays(_yearHolidays);
    
        // Берем год и месяц, которые пользователь видит на экране прямо сейчас!
        _calendarGenerater.BuildCalendar(_calendarGenerater._year, _calendarGenerater._month);
    }

    public void AddCustomEvent(DateTime date, List<Event> tasks)
    {
        _yearEvents[date.Date] = tasks;
        EventCacheStorage.SaveTasks(_yearEvents);
    
        // Перерисовываем именно тот месяц, на котором находится пользователь
        _calendarGenerater.BuildCalendar(_calendarGenerater._year, _calendarGenerater._month);
    }

    public void DeleteHoliday(DateTime date)
    {
        _yearHolidays.Remove(date.Date);
        HolidayCacheStorage.SaveHolidays(_yearHolidays); // Перезапишет файл
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

        EventCacheStorage.SaveTasks(_yearEvents); // Перезапишет файл
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
        // 1. Пытаемся загрузить всё, что есть на диске (если словарь еще не был загружен при старте)
        if (_yearHolidays == null || _yearHolidays.Count == 0)
        {
            _yearHolidays = HolidayCacheStorage.LoadHolidays();
        }

        if (_yearEvents == null || _yearEvents.Count == 0)
        {
            _yearEvents = EventCacheStorage.LoadTasks();
        }

        // 2. ИСПРАВЛЕНИЕ: Проверяем, есть ли элементы КОНКРЕТНО ДЛЯ ЭТОГО ГОДА
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
