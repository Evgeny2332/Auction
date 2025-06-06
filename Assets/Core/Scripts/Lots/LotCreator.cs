using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

public class LotCreator : MonoBehaviour
{
    [SerializeField] private GameObject _lotPrefab;
    [SerializeField] private Transform _lotContainer;

    [SerializeField] private BetLot _betLot;

    private DatabaseManager _databaseManager;

    private async void OnEnable()
    {
        _databaseManager = FindObjectOfType<DatabaseManager>();
        await LoadOpenLotsAsync();
    }

    private async UniTask LoadOpenLotsAsync()
    {
        // ������� ��� ������ �����
        foreach (Transform child in _lotContainer)
        {
            Destroy(child.gameObject);
        }

        List<Lot> openLots = await _databaseManager.GetLotsAsync();

        if (openLots == null || openLots.Count == 0)
        {
            Debug.LogWarning("��� �������� �����.");
            return;
        }

        foreach (var lot in openLots)
        {
            if (lot.Status != "������")
                continue;

            if (lot.EndData.HasValue && DateTime.UtcNow >= lot.EndData.Value)
            {
                lot.Status = "������";
                bool updateResult = await _databaseManager.UpdateLotAsync(lot);
                continue;
            }

            CreateLotSlot(lot);
        }
    }


    private async void CreateLotSlot(Lot lot)
    {
        GameObject lotSlotObject = Instantiate(_lotPrefab, _lotContainer);
        LotSlot lotSlot = lotSlotObject.GetComponent<LotSlot>();

        if (lotSlot != null)
        {
            Category category = await _databaseManager.GetCategoryByIdAsync(lot.CategoryId);
            int currentUserId = UserSession.Instance.GetUser().Id ?? 0;

            lotSlot.Init(
                lot.Name,
                "���������: " + category.Name,
                "��������� ������: " + (lot.Bet ?? 0),
                lot.EndData,
                lot,
                currentUserId
            );
        }
        else
        {
            Debug.LogWarning("��������� LotSlot �� ������ �� �������!");
        }
    }


    public void CloseBet()
    {
        LoadOpenLotsAsync();
    }
}
