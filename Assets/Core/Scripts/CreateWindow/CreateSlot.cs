using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class CreateSlot : MonoBehaviour
{
    [SerializeField] private Dropdown _categories;

    private DatabaseManager _databaseManager;

    private async void Start()
    {
        _databaseManager = FindObjectOfType<DatabaseManager>();
        await LoadCategoriesAsync();
    }

    private async UniTask LoadCategoriesAsync()
    {
        var categories = await _databaseManager.GetCategoriesAsync();
        if (categories == null || categories.Count == 0)
        {
            Debug.LogWarning("Категории не найдены");
            return;
        }

        // Очистим текущие опции
        _categories.ClearOptions();

        // Сформируем список строк
        List<string> options = new List<string>();
        foreach (var category in categories)
        {
            options.Add(category.Name);
        }

        _categories.AddOptions(options);
    }
}
