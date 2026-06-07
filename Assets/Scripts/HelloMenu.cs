using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class HelloMenu : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private Text _textForMotivation;
    [SerializeField] private Text _textUntilJune;
    [SerializeField] private Text _textHowMatchUntil;
    [SerializeField] private Text _textDay;
    [SerializeField] private Button _button;

    private string[] _day = { "День", "Дня", "Дней"};
    private string _motivation = "Держись!!";
    private string _untilJune = "До следущего июня:";
    private int _howMatchUntil;

    private void Awake()
    {
        _menu.SetActive(true);
        DateTime nowDate = DateTime.Now.Date;
        DateTime nexJune = new DateTime(nowDate.Year, 6, 1);

        if (nowDate > nexJune)
        {
            nexJune = nexJune.AddYears(1);
        }
        
        TimeSpan timeSpan = nexJune - nowDate;
        _howMatchUntil = timeSpan.Days;
        _button.onClick.AddListener(CloseMenu);
        ShowInfo();
    }

    private void ShowInfo()
    {
        Sequence _seq = DOTween.Sequence();
        _seq.Append(_textForMotivation.DOText(_motivation, 1.5f));
        _seq.Append(_textUntilJune.DOText(_untilJune, 1.5f));
        _seq.Append(_textHowMatchUntil.DOText(_howMatchUntil.ToString(), 1.5f, true, ScrambleMode.All));

        if (_howMatchUntil == 1)
        {
            _seq.Append(_textDay.DOText(_day[0], 1.5f));
        }
        else if (_howMatchUntil > 1 && _howMatchUntil < 5)
        {
            _seq.Append(_textDay.DOText(_day[1], 1.5f));
        }
        else
        {
            _seq.Append(_textDay.DOText(_day[2], 1.5f));
        }
    }

    private void CloseMenu()
    {
        _menu.SetActive(false);
    }
}
