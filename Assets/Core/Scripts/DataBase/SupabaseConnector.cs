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
        var request = new HttpRequestMessage(HttpMethod.Get, baseUrl + tableName);
        request.Headers.Add("apikey", apiKey);
        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            Debug.LogError($"Ошибка при получении {tableName}: {response.StatusCode}");
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<T>>(json);
    }

    private async UniTask<bool> InsertRecordAsync<T>(string tableName, T record)
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
            Debug.LogError($"Ошибка при добавлении в {tableName}: {response.StatusCode}");
            return false;
        }

        return true;
    }

    private async UniTask<bool> UpdateRecordAsync<T>(string tableName, int id, T record)
    {
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{baseUrl}{tableName}?Id=eq.{id}");
        request.Headers.Add("apikey", apiKey);
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        request.Headers.Add("Prefer", "return=minimal");

        string json = JsonConvert.SerializeObject(record);
        request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            Debug.LogError($"Ошибка при обновлении {tableName} (id={id}): {response.StatusCode}");
            return false;
        }

        return true;
    }

    // Методы под таблицы
    public UniTask<List<User>> GetUsersAsync() => FetchTableAsync<User>("User");
    public UniTask<List<Category>> GetCategoriesAsync() => FetchTableAsync<Category>("Category");
    public UniTask<List<Lot>> GetLotsAsync() => FetchTableAsync<Lot>("Lot");

    public UniTask<bool> AddUserAsync(User user) => InsertRecordAsync("User", user);
    public UniTask<bool> AddCategoryAsync(Category category) => InsertRecordAsync("Category", category);
    public UniTask<bool> AddLotAsync(Lot lot) => InsertRecordAsync("Lot", lot);

    public UniTask<bool> UpdateUserAsync(User user) => UpdateRecordAsync("User", user.Id ?? 0, user);
    public UniTask<bool> UpdateCategoryAsync(Category category) => UpdateRecordAsync("Category", category.Id ?? 0, category);
    public UniTask<bool> UpdateLotAsync(Lot lot) => UpdateRecordAsync("Lot", lot.Id ?? 0, lot);
}
