using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveItem : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Text _label = null;

    [SerializeField]
    private Image _background = null;

    private bool _isSelected;

    public event EventHandler Selected;

    public string SaveName { get; private set; }

    public void SetText(string text)
    {
        _label.text = text;
        SaveName = text;
    }

    public void Select()
    {
        _isSelected = true;
        _background.enabled = true;
    }

    public void Deselect()
    {
        _isSelected = false;
        _background.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isSelected)
        {
            Select();
            Selected?.Invoke(this, null);
        }
    }
}
