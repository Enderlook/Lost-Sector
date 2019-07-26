using UnityEngine;

public class Menu : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Menu to display on escape key press.")]
    public GameObject menu;
    [Tooltip("Game over menu to display when game finishes.")]
    public GameOverMenu gameOver;
    [Tooltip("How to play menu.")]
    public GameObject howToPlay;
    [Tooltip("Force menu to not be toggleable.")]
    public bool menuNoToggleable = false;

    private bool isActive;
    [HideInInspector]
    public bool isGameOver = false;
    private bool hasWon;

    [HideInInspector]
    public bool canBeShown;

    private void Start() => DisplayMenuPause(false);

    private void Update()
    {
        // This should be done in other script and not here...        
        if (isGameOver && canBeShown)
        {
            gameOver.SetShown(true);
            gameOver.SetConfiguration(hasWon, Global.money, Dynamic.playedTime);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
            DisplayMenuPause();
    }

    /// <summary>
    /// Toggle visibility of the <see cref="menu"/> using the opposite value of <seealso cref="isActive"/>. <seealso cref="isActive"/> is set as its opposite value.<br/>
    /// If <paramref name="active"/> isn't null this value will override the toggle.<br/>
    /// If <c><see cref="howToPlay"/>.activeSelf</c> is <see langword="true"/>, that panel will be hidden instead.
    /// </summary>
    /// <param name="active">Whenever the menu is visible or not.</param>
    public void DisplayMenuPause(bool? active = null)
    {
        if (menuNoToggleable)
            return;
        if (howToPlay.activeSelf)
            ShowHowToPlay(false);
        else
        {
            isActive = active != null ? (bool)active : !isActive;
            Time.timeScale = isActive ? 0 : 1;
            menu.SetActive(isActive);
        }
    }

    /// <summary>
    /// Hide the menu and set to <see langword="false"/> <seealso cref="isActive"/>.
    /// </summary>
    public void HideMenu() => DisplayMenuPause(false);


    /// <summary>
    /// Toggle visibility of the Game Over menu.
    /// </summary>
    public void GameOver(bool win)
    {
        hasWon = win;
        isGameOver = true;
        Global.enemySpawner.StopSpawnWaves();
    }

    /// <summary>
    /// Show how to play panel.
    /// </summary>
    /// <param name="active"></param>
    public void ShowHowToPlay(bool active) => howToPlay.SetActive(active);
}
