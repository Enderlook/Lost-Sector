using UnityEngine;

public class Menu : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Menu to display on escape key press.")]
    public GameObject menu;
    [Tooltip("Game over menu to display when game finishes.")]
    public GameOverMenu gameOver;
    [Tooltip("Others panels. Used to hide them when press escape.")]
    public GameObject[] panels;
    [Tooltip("Force menu to not be toggleable.")]
    public bool menuNoToggleable = false;
    [Tooltip("Playlist Manager.")]
    public PlaylistManager playlistManager;
    [Tooltip("Name of the playlist to play when menu is shown")]
    public string playlistMenuShow;
    [Tooltip("Name of the playlist to play when menu is hide")]
    public string playlistMenuHide;

    private bool isActive;
    [HideInInspector]
    public bool isGameOver = false;
    [HideInInspector]
    public bool keepPlaying = false;
    /// <summary>
    /// Whenever game should work or stop.
    /// </summary>
    public bool ShouldWork => !isGameOver || (keepPlaying && hasWon);

    private bool hasWon;

    [HideInInspector]
    public bool canBeShown;

    private void Start() => DisplayMenuPause(false);

    private void Update()
    {
        // This should be done in other script and not here...
        if (canBeShown && !ShouldWork)
        {
            gameOver.SetShown(true);
            gameOver.SetConfiguration(hasWon, Global.money, Dynamic.playedTime);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && ShouldWork)
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
        foreach (GameObject panel in panels)
        {
            if (panel.activeSelf)
            {
                panel.SetActive(false);
                return;
            }
        }
        if (menuNoToggleable)
            return;
        isActive = active != null ? (bool)active : !isActive;
        Time.timeScale = isActive ? 0 : 1;
        menu.SetActive(isActive);
        PlayMusic(isActive, true);
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
        if (hasWon != win)
            gameOver.ResetFinished();
        hasWon = win;
        isGameOver = true;
        keepPlaying = false;
    }

    /// <summary>
    /// Play music.
    /// </summary>
    /// <param name="menuMusic">Whenever menu music should be player or game music.</param>
    /// <param name="resetCurrentMusic">Whenever it should reset the current music (not playlist) or wait until it ends.</param>
    public void PlayMusic(bool menuMusic, bool resetCurrentMusic)
    {
        if (menuMusic)
            playlistManager.SetPlaylist(playlistMenuShow);
        else
            playlistManager.SetPlaylist(playlistMenuHide);
        playlistManager.Reset();
    }

    public void OpenURL(string url) => Application.OpenURL(url);
}