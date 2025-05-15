using UnityEngine;
using UnityEngine.UI;
using System;

public class LotSlot : MonoBehaviour
{
    [SerializeField] private Text _name;
    [SerializeField] private Text _category;
    [SerializeField] private Text _lastBet;
    [SerializeField] private Text _endData;

    private Lot _lot;

    public void Init(string name, string category, string lastBet, DateTime? endTime, Lot lot)
    {
        _name.text = name;
        _category.text = category;
        _lastBet.text = lastBet;
        _lot = lot;

        if (endTime.HasValue)
        {
            TimeSpan remaining = endTime.Value - DateTime.UtcNow;
            int hoursLeft = Mathf.Max((int)Math.Ceiling(remaining.TotalHours), 0);
            _endData.text = hoursLeft > 0 ? $"��������\n {hoursLeft} �." : "����� �����";
        }
        else
        {
            _endData.text = "��� ������";
        }
    }

    public void OpenBetLot()
    {
        BetLot.Instance.SetUserId(UserSession.Instance.GetUser().Id ?? 0);
        BetLot.Instance.Activate(_lot);
    }
}
