using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Menu to display on escape key press.")]
    public GameObject menu;

    private bool isActive;

    private void Start() => DisplayMenuPause(false);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            DisplayMenuPause();
    }

    /// <summary>
    /// Toggle visibility of the menu using the opposite value of <seealso cref="isActive"/>. <seealso cref="isActive"/> is set as its opposite value.<br/>
    /// If <paramref name="active"/> isn't null this value will override the toggle.
    /// </summary>
    /// <param name="active">Whenever the menu is visible or not.</param>
    public void DisplayMenuPause(bool? active = null)
    {
        isActive = active != null ? (bool)active : !isActive;
        Time.timeScale = isActive ? 0 : 1;
        menu.SetActive(isActive);
    }

    /// <summary>
    /// Hide the menu and set to <see langword="false"/> <seealso cref="isActive"/>.
    /// </summary>
    public void HideMenu() => DisplayMenuPause(false);

    /// <summary>
    /// Reload the current scene.
    /// </summary>
    private void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

}
