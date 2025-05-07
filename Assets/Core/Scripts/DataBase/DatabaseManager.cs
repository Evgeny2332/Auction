using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private SupabaseConnector _connector;

    private void Awake()
    {
        _connector = new SupabaseConnector();
    }

    #region Users

    public UniTask<List<User>> GetUsersAsync() => _connector.GetUsersAsync();

    public async UniTask<User> GetUserByIdAsync(int id)
    {
        var list = await GetUsersAsync();
        return list?.Find(u => u.Id == id);
    }

    public UniTask<bool> UpdateUserAsync(User user) => _connector.UpdateUserAsync(user);
    public UniTask<bool> AddUserAsync(User user) => _connector.AddUserAsync(user);

    #endregion

    #region Categories

    public UniTask<List<Category>> GetCategoriesAsync() => _connector.GetCategoriesAsync();

    public async UniTask<Category> GetCategoryByIdAsync(int id)
    {
        var list = await GetCategoriesAsync();
        return list?.Find(c => c.Id == id);
    }

    public UniTask<bool> UpdateCategoryAsync(Category category) => _connector.UpdateCategoryAsync(category);
    public UniTask<bool> AddCategoryAsync(Category category) => _connector.AddCategoryAsync(category);

    #endregion

    #region Lots

    public UniTask<List<Lot>> GetLotsAsync() => _connector.GetLotsAsync();

    public async UniTask<Lot> GetLotByIdAsync(int id)
    {
        var list = await GetLotsAsync();
        return list?.Find(l => l.Id == id);
    }

    public UniTask<bool> UpdateLotAsync(Lot lot) => _connector.UpdateLotAsync(lot);
    public UniTask<bool> AddLotAsync(Lot lot) => _connector.AddLotAsync(lot);

    #endregion
}