using UnityEngine;
using UnityEngine.UI;
using System;

public class LotSlot : MonoBehaviour
{
    [SerializeField] private Text _name;
    [SerializeField] private Text _category;
    [SerializeField] private Text _lastBet;
    [SerializeField] private Text _endData;

    public void Init(string name, string category, string lastBet, DateTime? endTime)
    {
        _name.text = name;
        _category.text = category;
        _lastBet.text = lastBet;

        if (endTime.HasValue)
        {
            TimeSpan remaining = endTime.Value - DateTime.UtcNow;
            int hoursLeft = Mathf.Max((int)Math.Ceiling(remaining.TotalHours), 0);
            _endData.text = hoursLeft > 0 ? $"Осталось\n {hoursLeft} ч." : "Время вышло";
        }
        else
        {
            _endData.text = "Нет данных";
        }
    }
}
