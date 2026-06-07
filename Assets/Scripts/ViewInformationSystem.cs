using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ViewInformationSystem : MonoBehaviour
{
    [SerializeField] private AddEventMenu _addEventMenu;
    [SerializeField] private CalendarSystem _calendarSystem;
    [Header("Настройки UI Текста")]
    [SerializeField] private TextMeshProUGUI _dayNameText;
    [SerializeField] private TextMeshProUGUI _isHolidayText;
    [SerializeField] private TextMeshProUGUI _dateText;

    [Header("Настройки Списков и Пулинга")]
    [SerializeField] private GameObject _eventPrefab;     // Префаб одной задачи (с компонентом EventPrefab)
    [SerializeField] private Transform _eventContainer;
    [SerializeField] private List<DayPrefab> _days;

    private Day _day;
    private List<EventPrefab> _eventPrefabs = new List<EventPrefab>();

    public event Action AddEvent;

    private void Awake()
    {
        if (_eventPrefabs == null)
        {
            _eventPrefabs = new List<EventPrefab>();
        }
    }

    private void OnEnable()
    {
        _addEventMenu.CreatedEvent += AddEventToStore;

        if (_days != null)
        {
            foreach (DayPrefab day in _days)
            {
                day.OnDayClick += UpdateViewInformation;
            }
        }
    }

    private void OnDisable()
    {
        _addEventMenu.CreatedEvent -= AddEventToStore;

        if (_days != null)
        {
            foreach (DayPrefab day in _days)
            {
                day.OnDayClick -= UpdateViewInformation;
            }
        }
    }

    public void UpdateViewInformation(Day day)
    {
        _day = day;
        
        _dateText.text = _day.Date.ToString("dd MMMM yyyy");

        if (_day.IsHoliday)
        {
            _isHolidayText.text = "Выходной";
            
            if (_day.HolidayName != null)
            {
                _dayNameText.text = _day.HolidayName;
            }
            else
            {
                _dayNameText.text = _day.Date.ToString("dddd");
            }
        }
        else
        {
            _dayNameText.text = _day.Date.ToString("dddd");
            _isHolidayText.text = "Рабочий день";
        }

        UpdateEventInformation();
    }

    public void UpdateEventInformation()
    {
        DestroyEventPrefabs();
        _eventPrefabs.Clear();
        int countNeeded = _day.Events.Count;

        for (int i = 0; i < countNeeded; i++)
        {
            GameObject eventObject = Instantiate(_eventPrefab, _eventContainer);
            EventPrefab eventPrefabScript = eventObject.GetComponent<EventPrefab>();
            
            eventPrefabScript.Init(_day.Events[i]);
            _eventPrefabs.Add(eventPrefabScript);

            eventPrefabScript.DeletedEvent += DeleteEventFromStore;
        }
    }

    private void DestroyEventPrefabs()
    {
        foreach (EventPrefab eventPrefab in _eventPrefabs)
        {
            eventPrefab.DeletedEvent -= DeleteEventFromStore;
            Destroy(eventPrefab.gameObject);
        }
    }

    private void AddEventToStore(Event eventToAdd)
    {
        if (_day == null) return;

        _day.Events.Add(eventToAdd);
        _calendarSystem.AddCustomEvent(_day.Date.Date, _day.Events);

        UpdateEventInformation();
    }

    private void DeleteEventFromStore(Event eventToDelete)
    {
        if (_day == null) return;

        _day.Events.Remove(eventToDelete);
        _calendarSystem.DeleteEvent(_day.Date.Date, _day.Events);
    }

    public void AddEventToAction()
    {
        Debug.Log("Добавить событие");
        AddEvent?.Invoke();
    }

    public void SetHoliday()
    {
        if (!_day.IsHoliday)
        {
            _calendarSystem.AddCustomHoliday(_day.Date, null);
        }
        else
        {
            _calendarSystem.DeleteHoliday(_day.Date);
        }
    }
}
