using UnityEngine;

public class UserSession : MonoBehaviour
{
    public static UserSession Instance { get; private set; }

    private User CurrentUser { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetUser(User user) => CurrentUser = user;
    public User GetUser() => CurrentUser;
    public void ClearUser() => CurrentUser = null;
}
