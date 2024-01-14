using UnityEngine;

public class MainMenuData : MonoBehaviour
{
    public static MainMenuData Instance;

    public GameObject loginScreen;
    public GameObject passwordScreen;
    public GameObject mainMenuScreen;

    private void Awake()
    {
        if (Instance == null || Instance == this)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    public void ToggleScreen(GameObject objectToToggle)
    {
        objectToToggle.SetActive(!objectToToggle.activeSelf);
    }

    public void SetScreenActive(GameObject screenToActivate)
    {
        loginScreen.SetActive(false);
        passwordScreen.SetActive(false);
        mainMenuScreen.SetActive(false);

        screenToActivate.SetActive(true);
    }
}
