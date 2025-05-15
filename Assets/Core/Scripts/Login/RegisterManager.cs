using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Linq;

public class RegisterManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private InputField _loginField;
    [SerializeField] private InputField _passwordField;
    [SerializeField] private InputField _firstNameField;
    [SerializeField] private InputField _lastNameField;
    [SerializeField] private Text _messageText;

    private SupabaseConnector _connector;

    private void Awake()
    {
        _connector = new SupabaseConnector();
    }

    public void OnRegisterButtonClick()
    {
        RegisterAsync().Forget();
    }

    private async UniTaskVoid RegisterAsync()
    {
        string login = _loginField.text.Trim();
        string password = _passwordField.text.Trim();
        string firstName = _firstNameField.text.Trim();
        string lastName = _lastNameField.text.Trim();

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            ShowMessage("Заполните все поля.", Color.red);
            return;
        }

        if (login.Length < 5)
        {
            ShowMessage("Логин должен содержать минимум 5 символов.", Color.red);
            return;
        }

        if (password.Length < 6)
        {
            ShowMessage("Пароль должен содержать минимум 6 символов.", Color.red);
            return;
        }

        var users = await _connector.GetUsersAsync();
        if (users == null)
        {
            ShowMessage("Ошибка при подключении к базе.", Color.red);
            return;
        }

        if (users.Any(u => u.Login == login))
        {
            ShowMessage("Пользователь с таким логином уже существует.", Color.red);
            return;
        }

        var newUser = new User
        {
            Login = login,
            Password = password,
            FirstName = firstName,
            LastName = lastName
        };

        bool success = await _connector.AddUserAsync(newUser);

        if (success)
        {
            ShowMessage("Регистрация прошла успешно!", Color.green);

            _loginField.text = "";
            _passwordField.text = "";
            _firstNameField.text = "";
            _lastNameField.text = "";
        }
        else
        {
            ShowMessage("Не удалось зарегистрироваться. Попробуйте позже.", Color.red);
        }
    }

    private void ShowMessage(string text, Color color)
    {
        _messageText.text = text;
        _messageText.color = color;
    }
}
