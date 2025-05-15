using System;
using UnityEngine;
using UnityEngine.UI;

public class BetLot : MonoBehaviour
{
    public static BetLot Instance;

    [SerializeField] private DatabaseManager _databaseManager;
    [SerializeField] private GameObject _betLot;

    [SerializeField] private Text _name;
    [SerializeField] private Text _description;
    [SerializeField] private Text _endData;
    [SerializeField] private Text _currentBet;
    [SerializeField] private InputField _bet;

    private Lot _currentLot;
    private int _currentUserId;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetUserId(int userId) => _currentUserId = userId;

    public void Activate(Lot lot)
    {
        _betLot.SetActive(true);
        _currentLot = lot;

        _name.text = lot.Name;
        _description.text = lot.Description;

        TimeSpan? timeLeft = _currentLot.EndData - DateTime.UtcNow;

        if (timeLeft.HasValue)
        {
            if (timeLeft.Value.TotalHours >= 1)
                _endData.text = $"�������� {Mathf.FloorToInt((float)timeLeft.Value.TotalHours)} �.";
            else if (timeLeft.Value.TotalSeconds > 0)
                _endData.text = "�������� ����� ����";
            else
                _endData.text = "��� ��������";
        }
        else
        {
            _endData.text = "�� �������";
        }

        _currentBet.text = lot.Bet?.ToString() ?? "0";
    }

    public async void TryPlaceBet()
    {
        if (_currentLot == null)
        {
            Debug.LogError("��� �� ����������");
            return;
        }

        if (_currentUserId == 0)
        {
            Debug.LogError("UserId �� ����������! ������ SetUserId() ����� ����������� ������.");
            return;
        }

        if (!int.TryParse(_bet.text, out int newBet) || newBet <= 0)
        {
            Debug.LogWarning("������������ ������. ������� ������������� �����.");
            return;
        }

        if (_currentLot.Bet.HasValue && newBet <= _currentLot.Bet.Value)
        {
            Debug.Log("������ ������ ���� ���� �������.");
            return;
        }

        _currentLot.Bet = newBet;
        _currentLot.BidderUserId = _currentUserId;

        bool success = await _databaseManager.UpdateLotAsync(_currentLot);
        if (success)
        {
            Debug.Log("������ ������� �������!");
            _currentBet.text = newBet.ToString();

            _bet.text = "";
            Activate(_currentLot);
        }
        else
        {
            Debug.LogError("�� ������� �������� ������");
        }
    }
}
