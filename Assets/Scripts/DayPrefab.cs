using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DayPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private Image _dayImage;
    [SerializeField] private Button _cellButton;

    [SerializeField] private Colors _colors;

    private Color _holidayColor;
    private Color _basicColor;
    private Color _eventColor;
    private Color _invisibleColor;
    private Color _nowColor;

    public event Action<Day> OnDayClick;

    public Day Day { get; private set; }

    private void Awake()
    {
        _holidayColor = _colors.HolidayColor;
        _basicColor = _colors.BasicColor;
        _eventColor = _colors.EventColor;
        _invisibleColor = _colors.InvisibleColor;
        _nowColor = _colors.NowColor;

        if (_cellButton != null)
        {
            _cellButton.onClick.AddListener(HandleClick);
        }
    }

    private void HandleClick()
    {
        Debug.Log("Day click");

        if (Day != null && _dayText.text != "")
        {
            OnDayClick?.Invoke(Day);
        }
    }

    public void Init(Day day)
    {
        _dayImage.color = _invisibleColor;

        Day = day;
        
        if (Day.Date.Date == DateTime.Now.Date)
        {
            SetDayNow();
            OnDayClick?.Invoke(Day);
        }
        else if (Day.Events.Count != 0)
        {
            SetDayEvent();
        }
        else if (Day.IsHoliday)
        {
            SetDayHoliday();
        }
        else
        {
            SetDayBasic();
        }
    }

    public void SetDayHoliday()
    {
        _dayText.color = _holidayColor;
    }

    public void SetDayBasic()
    {
        _dayText.color = _basicColor;
    }

    public void SetDayEvent()
    {
        _dayText.color = _eventColor;
    }

    public void SetText(string text)
    {
        _dayText.text = text;
    }

    public void SetDayNow()
    {
        _dayText.color = Color.black;
        _dayImage.color = _nowColor;
        OnDayClick?.Invoke(Day);
    }

    public void Invis()
    {
        SetText("");
        _dayImage.color = _invisibleColor;
    }
}
