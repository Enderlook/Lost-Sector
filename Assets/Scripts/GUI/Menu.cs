using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Menu to display on escape key press.")]
    public GameObject menu;
    [Tooltip("Game over menu to display when game finishes.")]
    public GameOverMenu gameOver;
    
    private bool isActive;
    private bool isGameOver = false;
    private bool hasWon;
    private void Start() => DisplayMenuPause(false);

    private void Update()
    {
        // This should be done in other script and not here...
        if (Global.coinMeter.showedMoney >= Global.moneyToWin)
            GameOver(true);
        if (isGameOver && Global.playerHealthBar.IsDamageBarPercentHide && Global.playerShieldBar.IsDamageBarPercentHide)
        {
            gameOver.SetShown(true);
            gameOver.SetConfiguration(hasWon, Global.money);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
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
    /// Toggle visibility of the Game Over menu.
    /// </summary>
    public void GameOver(bool win)
    {
        hasWon = win;
        isGameOver = true;
        Global.enemySpawner.StopSpawnWaves();
    }
}
