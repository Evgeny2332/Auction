using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private InputField _loginInput;
    [SerializeField] private InputField _passwordInput;
    [SerializeField] private Text _resultText;

    [SerializeField] private DatabaseManager _dataBase;

    private List<User> _users;

    private async void Start()
    {
        _users = await _dataBase.GetUsersAsync();
    }

    public void OnLoginButtonPressed()
    {
        string login = _loginInput.text.Trim();
        string password = _passwordInput.text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            ShowMessage("Введите логин и пароль.", Color.red);
            return;
        }

        var user = _users.FirstOrDefault(u => u.Login == login);

        if (user == null)
        {
            ShowMessage("Пользователь с таким логином не найден.", Color.red);
            return;
        }

        if (user.Password != password)
        {
            ShowMessage("Неверный пароль.", Color.red);
            return;
        }

        UserSession.Instance.SetUser(user);
        ShowMessage("Вход выполнен успешно!", Color.green);
        SceneManager.LoadScene(1);
    }

    private void ShowMessage(string text, Color color)
    {
        _resultText.text = text;
        _resultText.color = color;
    }
}
