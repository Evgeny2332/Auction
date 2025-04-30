using UnityEngine;

public class NavigationWindowsController : MonoBehaviour
{
    [SerializeField] private GameObject _lastOpenWindow;

    public void OpenWindow(GameObject window)
    {
        _lastOpenWindow.SetActive(false);
        window.SetActive(true);
        _lastOpenWindow = window;
    }
}
