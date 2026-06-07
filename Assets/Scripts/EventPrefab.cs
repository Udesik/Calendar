using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class EventPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _eventText;
    [SerializeField] private Button _eventButton;

    public event Action<Event> DeletedEvent;

    private Event _event;

    private void Awake()
    {
        _eventButton.onClick.AddListener(OnEventButtonClick);
    }

    public void Init(Event eventData)
    {
        _event = eventData;
        _eventText.text = _event.Description;
    }

    public void OnEventButtonClick()
    {
        _event.Complete();
        DeletedEvent?.Invoke(_event);
    }
}
