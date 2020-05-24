using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private InputField _mapSizeInputField = null;
    [SerializeField]
    private InputField _numObstaclesInputField = null;
    [SerializeField]
    private Toggle _isWeightedToggle = null;
    [SerializeField]
    private Dropdown _tileDebugStyleDropdown = null;

    [SerializeField]
    private Dropdown _algorithmsDropdown = null;
    [SerializeField]
    private Toggle _animateSearchToggle = null;

    [SerializeField]
    private Toggle _cameraOrthographicToggle = null;

    private Type[] _availableAlgorithms;
    private Dictionary<TileDebugStyle, string> _tileDebugStyles;

    public void UpdateFieldsWhenGenerateClicked()
    {
        SetInputFieldValueUnnotified(_mapSizeInputField, Settings.MapSize.ToString());
    }

    private void Awake()
    {
        Settings.SettingsChanged += OnSettingsChanged;
        FillTileDebugStyleDropdown();
        FillAlgorithmsDropdown();
    }

    private void Start()
    {
        _mapSizeInputField.text = Settings.MapSize.ToString();
        _mapSizeInputField.onValueChanged.RemoveAllListeners();
        _mapSizeInputField.onValueChanged.AddListener(OnMapSizeChanged);

        _numObstaclesInputField.text = Settings.NumObstacles.ToString();
        _numObstaclesInputField.onValueChanged.RemoveAllListeners();
        _numObstaclesInputField.onValueChanged.AddListener(OnNumObstaclesChanged);

        _isWeightedToggle.isOn = Settings.IsWeighted;
        _isWeightedToggle.onValueChanged.RemoveAllListeners();
        _isWeightedToggle.onValueChanged.AddListener(OnIsWeightedValueChanged);

        _tileDebugStyleDropdown.value = (int)Settings.TileDebugStyle;
        _tileDebugStyleDropdown.onValueChanged.RemoveAllListeners();
        _tileDebugStyleDropdown.onValueChanged.AddListener(OnTileDebugStyleSelected);

        _algorithmsDropdown.value = Array.IndexOf(_availableAlgorithms, Settings.Pathfinder);
        _algorithmsDropdown.onValueChanged.RemoveAllListeners();
        _algorithmsDropdown.onValueChanged.AddListener(OnAlgorithmSelected);
        if (_availableAlgorithms != null && _availableAlgorithms.Length > 0)
        {
            Settings.SetPathfinder(_availableAlgorithms[0], false);
        }

        _animateSearchToggle.isOn = Settings.AnimateSearch;
        _animateSearchToggle.onValueChanged.RemoveAllListeners();
        _animateSearchToggle.onValueChanged.AddListener(OnAnimateSearchValueChanged);

        //TODO: use as an example for how to set up the other fields
        _cameraOrthographicToggle.isOn = Settings.IsCameraOrthographic;
        _cameraOrthographicToggle.onValueChanged.RemoveAllListeners();
        _cameraOrthographicToggle.onValueChanged.AddListener(OnCameraOrthographicValueChanged);
    }

    private void FillTileDebugStyleDropdown()
    {
        _tileDebugStyleDropdown.options.Clear();

        _tileDebugStyles = new Dictionary<TileDebugStyle, string>();
        _tileDebugStyles.Add(TileDebugStyle.None, "No tile labels");
        _tileDebugStyles.Add(TileDebugStyle.Coords, "Show coordinates");
        _tileDebugStyles.Add(TileDebugStyle.Weight, "Show weight");
        _tileDebugStyles.Add(TileDebugStyle.Cost, "Show pathfinding cost");

        foreach(var kvp in _tileDebugStyles)
        { 
            Dropdown.OptionData option = new Dropdown.OptionData(kvp.Value);
            _tileDebugStyleDropdown.options.Add(option);
        }
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

    private void OnIsWeightedValueChanged(bool value)
    {
        Settings.SetIsWeighted(value, true);
    }

    private void OnTileDebugStyleSelected(int option)
    {
        Settings.SetTileDebugStyle((TileDebugStyle)option, true);
    }

    private void OnAlgorithmSelected(int option)
    {
        Settings.SetPathfinder(_availableAlgorithms[option], false);
    }

    private void OnAnimateSearchValueChanged(bool value)
    {
        Settings.SetAnimateSearch(value, false);
    }

    private void OnCameraOrthographicValueChanged(bool value)
    {
        Settings.SetCameraOrthographic(value, true);
    }

    private void OnMapSizeChanged(string value)
    {
        int size;
        int.TryParse(value, out size);
        size = Mathf.Max(size, 10);

        Settings.SetMapSize(size, false);
    }

    private void OnNumObstaclesChanged(string value)
    {
        int numObstacles;
        int.TryParse(value, out numObstacles);
        if (numObstacles < 0)
            numObstacles = 0;

        Settings.SetNumObstacles(numObstacles, false);
    }


    private void OnSettingsChanged(object sender, EventArgs e)
    {
        //updated UI fields
        SetInputFieldValueUnnotified(_mapSizeInputField, Settings.MapSize.ToString());
        SetInputFieldValueUnnotified(_numObstaclesInputField, Settings.NumObstacles.ToString());
        SetDropdownValueUnnotified(_tileDebugStyleDropdown, (int)Settings.TileDebugStyle);
        SetDropdownValueUnnotified(_algorithmsDropdown, Array.IndexOf(_availableAlgorithms, Settings.Pathfinder));
        SetToggleValueUnnotified(_animateSearchToggle, Settings.AnimateSearch);
        SetToggleValueUnnotified(_cameraOrthographicToggle, Settings.IsCameraOrthographic);
    }

    private void SetInputFieldValueUnnotified(InputField field, string value)
    {
        InputField.OnChangeEvent eventBackup = field.onValueChanged;
        field.onValueChanged = null;
        field.text = value;
        field.onValueChanged = eventBackup;
    }

    private void SetDropdownValueUnnotified(Dropdown field, int value)
    {
        Dropdown.DropdownEvent eventBackup = field.onValueChanged;
        field.onValueChanged = null;
        field.value = value;
        field.onValueChanged = eventBackup;
    }

    private void SetToggleValueUnnotified(Toggle field, bool value)
    {
        Toggle.ToggleEvent eventBackup = field.onValueChanged;
        field.onValueChanged = null;
        field.isOn = value;
        field.onValueChanged = eventBackup;
    }
}
