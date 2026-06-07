using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AddEventMenu : MonoBehaviour
{
    [SerializeField] private GameObject _eventMenu;
    [SerializeField] private TMPro.TMP_InputField _eventNameInput;
    [SerializeField] private ViewInformationSystem _viewInformationSystem;
    
    public event Action<Event> CreatedEvent;
    
    private void Awake()
    {
        _eventMenu.SetActive(false);
    }

    private void OnEnable()
    {
        _viewInformationSystem.AddEvent += OpenMenu;
    }

    private void OnDisable()
    {
        _viewInformationSystem.AddEvent -= OpenMenu;
    }

    private void OpenMenu()
    {
        _eventMenu.SetActive(true);
    }

    public void CreateEvent()
    {
        Event newEvent = new Event(_eventNameInput.text);

        CreatedEvent?.Invoke(newEvent);
        _eventNameInput.text = "";
        _eventMenu.SetActive(false);
    }

    public void CloseMenu()
    {
        _eventNameInput.text = "";
        _eventMenu.SetActive(false);
    }
}
