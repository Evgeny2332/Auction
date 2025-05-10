using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;

public class CreateSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Dropdown _categories;
    [SerializeField] private InputField _nameField;
    [SerializeField] private InputField _descriptionField;
    [SerializeField] private InputField _startBidField;
    [SerializeField] private InputField _durationField;
    [SerializeField] private Text _messageText;

    private DatabaseManager _databaseManager;
    private List<Category> _loadedCategories;

    private async void Start()
    {
        _databaseManager = FindObjectOfType<DatabaseManager>();
        await LoadCategoriesAsync();
    }

    private async UniTask LoadCategoriesAsync()
    {
        _loadedCategories = await _databaseManager.GetCategoriesAsync();
        if (_loadedCategories == null || _loadedCategories.Count == 0)
        {
            SetMessage("Категории не найдены", Color.red);
            return;
        }

        _categories.ClearOptions();
        List<string> options = new List<string>();
        foreach (var category in _loadedCategories)
        {
            options.Add(category.Name);
        }

        _categories.AddOptions(options);
    }

    public async void OnCreateSlotButtonPressed()
    {
        string name = _nameField.text.Trim();
        string description = _descriptionField.text.Trim();
        string bidText = _startBidField.text.Trim();
        string durationText = _durationField.text.Trim();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) ||
            string.IsNullOrEmpty(bidText) || string.IsNullOrEmpty(durationText))
        {
            SetMessage("Заполните все поля", Color.red);
            return;
        }

        if (!float.TryParse(bidText, out float bid) || bid <= 0)
        {
            SetMessage("Некорректная ставка", Color.red);
            return;
        }

        if (!int.TryParse(durationText, out int durationHours) || durationHours <= 0)
        {
            SetMessage("Некорректный срок", Color.red);
            return;
        }

        int categoryIndex = _categories.value;
        if (_loadedCategories == null || categoryIndex < 0 || categoryIndex >= _loadedCategories.Count)
        {
            SetMessage("Категория не выбрана", Color.red);
            return;
        }

        var categoryId = _loadedCategories[categoryIndex].Id;
        var userId = UserSession.Instance.GetUser().Id;

        DateTime endDate = DateTime.UtcNow.AddHours(durationHours);

        Lot newLot = new Lot
        {
            Name = name,
            Description = description,
            Status = "Открыт",
            CreatorUserId = userId ?? 0,
            BidderUserId = null,
            CategoryId = categoryId ?? 0,
            Bet = (int)bid,
            EndData = endDate
        };

        bool result = await _databaseManager.AddLotAsync(newLot);
        if (result)
        {
            SetMessage("Лот успешно создан", Color.green);
            _nameField.text = "";
            _descriptionField.text = "";
            _startBidField.text = "";
            _durationField.text = "";
        }
        else
        {
            SetMessage("Не удалось создать лот", Color.red);
        }
    }



    private void SetMessage(string text, Color color)
    {
        _messageText.text = text;
        _messageText.color = color;
    }
}
