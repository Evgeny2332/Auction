using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    [SerializeField] private Text _firstName;
    [SerializeField] private Text _lastName;

    private void Start()
    {
        ShowUserData();
    }

    private void ShowUserData()
    {
        User user = UserSession.Instance.GetUser();
        _firstName.text = $"Имя: {user.FirstName}";
        _lastName.text = $"Фамилия: {user.LastName}";
    }

    public void LogOut()
    {
        SceneManager.LoadScene(0);
    }
}
