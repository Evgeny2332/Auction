using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SupabaseConnector
{
    private const string baseUrl = "https://wavdgesrgojjnxutiilb.supabase.co/rest/v1/";
    private const string apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6IndhdmRnZXNyZ29qam54dXRpaWxiIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDY2MjE2NTIsImV4cCI6MjA2MjE5NzY1Mn0.5xuiHTjLBart_USl5Q_u3GbP0T3KFwsz4AxzgXZAtWg";

    private static readonly HttpClient httpClient = new HttpClient();

    private async UniTask<List<T>> FetchTableAsync<T>(string tableName)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + tableName);
            request.Headers.Add("apikey", apiKey);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"Ошибка при получении данных из таблицы {tableName}: {response.StatusCode}. Ответ: {await response.Content.ReadAsStringAsync()}");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка при запросе данных из таблицы {tableName}: {ex.Message}");
            return null;
        }
    }

    private async UniTask<bool> InsertRecordAsync<T>(string tableName, T record)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, baseUrl + tableName);
            request.Headers.Add("apikey", apiKey);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Headers.Add("Prefer", "return=minimal");

            string json = JsonConvert.SerializeObject(record);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"Ошибка при добавлении записи в таблицу {tableName}: {response.StatusCode}. Ответ: {await response.Content.ReadAsStringAsync()}");
                return false;
            }

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка при добавлении записи в таблицу {tableName}: {ex.Message}");
            return false;
        }
    }

    private async UniTask<bool> UpdateRecordAsync<T>(string tableName, int id, object updatePayload)
    {
        try
        {
            if (id <= 0)
            {
                Debug.LogError("Некорректный Id для обновления.");
                return false;
            }

            var url = $"{baseUrl}{tableName}?id=eq.{id}";
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
            request.Headers.Add("apikey", apiKey);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Headers.Add("Prefer", "return=minimal");

            string json = JsonConvert.SerializeObject(updatePayload);
            Debug.Log($"PATCH -> {url}\nPayload: {json}");

            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"Ошибка при обновлении записи в {tableName} (id={id}): {response.StatusCode}. Ответ: {await response.Content.ReadAsStringAsync()}");
                return false;
            }

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Ошибка при обновлении записи в таблице {tableName}: {ex.Message}");
            return false;
        }
    }

    // Методы под таблицы
    public UniTask<List<User>> GetUsersAsync() => FetchTableAsync<User>("Users");
    public UniTask<List<Category>> GetCategoriesAsync() => FetchTableAsync<Category>("Category");
    public UniTask<List<Lot>> GetLotsAsync() => FetchTableAsync<Lot>("Lots");

    public UniTask<bool> AddUserAsync(User user) => InsertRecordAsync("Users", user);
    public UniTask<bool> AddCategoryAsync(Category category) => InsertRecordAsync("Category", category);
    public UniTask<bool> AddLotAsync(Lot lot) => InsertRecordAsync("Lots", lot);

    public UniTask<bool> UpdateUserAsync(User user) =>
        UpdateRecordAsync<User>("Users", user.Id ?? 0, user);

    public UniTask<bool> UpdateCategoryAsync(Category category) =>
        UpdateRecordAsync<Category>("Category", category.Id ?? 0, category);

    public UniTask<bool> UpdateLotAsync(Lot lot)
    {
        if (lot.Id == null || lot.Id <= 0)
        {
            Debug.LogError("Лот не содержит корректного Id для обновления.");
            return UniTask.FromResult(false);
        }

        var payload = new
        {
            lot.Name,
            lot.Description,
            lot.Bet,
            lot.Status,
            lot.EndData,
            lot.CreatorUserId,
            lot.BidderUserId,
            lot.CategoryId
        };

        return UpdateRecordAsync<Lot>("Lots", lot.Id.Value, payload);
    }
}
