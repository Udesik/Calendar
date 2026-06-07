using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Colors", menuName = "Colors/Create new colors")]
public class Colors : ScriptableObject
{
    [SerializeField] private Color _holidayColor;
    [SerializeField] private Color _basicColor;
    [SerializeField] private Color _eventColor;
    [SerializeField] private Color _invisibleColor;
    [SerializeField] private Color _nowColor;

    public Color HolidayColor => _holidayColor;
    public Color BasicColor => _basicColor;
    public Color EventColor => _eventColor;
    public Color InvisibleColor => _invisibleColor;
    public Color NowColor => _nowColor;
}
