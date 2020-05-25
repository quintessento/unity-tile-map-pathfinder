using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadMenu : MonoBehaviour
{
    [SerializeField]
    private TileMap _tileMap = null;

    [SerializeField]
    private SaveItem _saveItemPrefab = null;

    [SerializeField]
    private RectTransform _savesList = null;

    [SerializeField]
    private InputField _selectedSaveInputField = null;

    [SerializeField]
    private string _saveExtension = "sav";


    private List<SaveItem> _saveItems = new List<SaveItem>();
    private string _savesDirectory;
    private string _dotExtension;
    private string _searchPattern;

    private SaveItem _selectedItem;

    public string SavePath
    {
        get
        {
            if(!string.IsNullOrEmpty(_selectedSaveInputField.text))
            {
                return Path.Combine(Application.persistentDataPath, _selectedSaveInputField.text) + _dotExtension;
            }
            return null;
        }
    }

    public void Save()
    {
        if (_tileMap.Map != null)
        {
            string savePath = SavePath;
            if (savePath != null)
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
                {
                    writer.Write(Settings.NumObstacles);
                    _tileMap.Map.Save(writer);
                    MessagePanel.ShowMessage("Saved map to " + savePath);
                }

                RefreshSavesList();
            }
        }
    }

    public void Load()
    {
        string savePath = SavePath;
        if (savePath != null)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
            {
                int numObstacles = reader.ReadInt32();
                Settings.SetNumObstacles(numObstacles, true);
                Map map = new Map(reader);

                _tileMap.Load(map);
            }
        }
    }

    public void Delete()
    {
        if(_selectedItem != null)
        {
            string path = Path.Combine(Application.persistentDataPath, _selectedItem.SaveName) + _dotExtension;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        RefreshSavesList();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        _savesDirectory = Application.persistentDataPath;
        _dotExtension = "." + _saveExtension;
        _searchPattern = "*" + _dotExtension;
    }

    private void OnEnable()
    {
        RefreshSavesList();
    }

    private void OnDisable()
    {
        
    }

    private void RefreshSavesList()
    {
        _selectedItem = null;

        for (int i = 0; i < _saveItems.Count; i++)
        {
            _saveItems[i].Selected -= OnSaveItemSelected;
            Destroy(_saveItems[i].gameObject);
        }
        _saveItems.Clear();

        if (Directory.Exists(_savesDirectory))
        {
            foreach (var file in Directory.GetFiles(_savesDirectory, _searchPattern))
            {
                SaveItem item = Instantiate(_saveItemPrefab, _savesList);
                string name = file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1).Replace(_dotExtension, "");
                item.SetText(name);
                item.Deselect();
                item.Selected += OnSaveItemSelected;
                _saveItems.Add(item);
            }
        }
    }

    private void OnSaveItemSelected(object sender, EventArgs e)
    {
        _selectedItem?.Deselect();
        _selectedItem = sender as SaveItem;
        _selectedSaveInputField.text = _selectedItem.SaveName;
    }
}
