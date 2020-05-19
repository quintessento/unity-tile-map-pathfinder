using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private Dropdown _algorithmsDropdown = null;
    [SerializeField]
    private Toggle _animateSearchToggle = null;

    private Type[] _availableAlgorithms;

    private void Awake()
    {
        FillAlgorithmsDropdown();

        _algorithmsDropdown.onValueChanged.RemoveAllListeners();
        _algorithmsDropdown.onValueChanged.AddListener(OnAlgorithmSelected);
        if (_availableAlgorithms != null && _availableAlgorithms.Length > 0)
        {
            Settings.Pathfinder = _availableAlgorithms[0];
        }

        _animateSearchToggle.onValueChanged.RemoveAllListeners();
        _animateSearchToggle.onValueChanged.AddListener(OnAnimateSearchValueChanged);
        Settings.AnimateSearch = _animateSearchToggle.isOn;
    }

    private void FillAlgorithmsDropdown()
    {
        _algorithmsDropdown.options.Clear();

        _availableAlgorithms = PathfindersFactory.GetAvailablePathfinderTypes();

        if (_availableAlgorithms != null)
        {
            for (int i = 0; i < _availableAlgorithms.Length; i++)
            {
                Dropdown.OptionData option = new Dropdown.OptionData(_availableAlgorithms[i].ToString());
                _algorithmsDropdown.options.Add(option);
            }
        }

    }

    private void OnAlgorithmSelected(int option)
    {
        Settings.Pathfinder = _availableAlgorithms[option];
    }

    private void OnAnimateSearchValueChanged(bool value)
    {
        Settings.AnimateSearch = value;
    }
}
