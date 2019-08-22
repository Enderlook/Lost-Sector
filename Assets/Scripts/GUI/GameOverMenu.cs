using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Subtitle of the menu. Used to show if player won or loose.")]
    public Text subtitle;
    [Tooltip("Text used to show score.")]
    public Text score;
    [Tooltip("Text used to show time player.")]
    public Text timePlayed;

    [Tooltip("Keep playing button.")]
    public GameObject keepPlayingButton;

    [Tooltip("Color used on subtitle if player win.")]
    public Color winColor = Color.green;
    [Tooltip("Color used on subtitle if player lose.")]
    public Color loseColor = Color.red;

    [Tooltip("Sound played if player win.")]
    public Sound winSound;
    [Tooltip("Sound played if player lose.")]
    public Sound loseSound;

    private bool isFinished = false;

    private TextOverTime money;
    private TextOverTime time;

    /// <summary>
    /// Determines whenever the game over menu is displayed or hidden.
    /// </summary>
    /// <param name="show">Whenever that game over menu is displayed or hidden.</param>
    public void SetShown(bool show) => gameObject.SetActive(show);

    /// <summary>
    /// Set values of the menu.
    /// </summary>
    /// <param name="hasWon">Whenever player has won or lose.</param>
    /// <param name="money">Money of the player.</param>
    /// <param name="time">Time survived.</param>
    public void SetConfiguration(bool hasWon, int money, float time)
    {
        if (!isFinished)
        {
            this.money = new TextOverTime(money);
            this.time = new TextOverTime(time);

            keepPlayingButton.SetActive(hasWon);

            Global.menu.PlayMusic(true, false);

            if (hasWon)
            {
                subtitle.text = "Congratulation, you win!";
                subtitle.color = winColor;
                Global.menu.playlistManager.PlaySound(winSound, Settings.IsSoundActive);
            }
            else
            {
                subtitle.text = "You failed the mission!";
                subtitle.color = loseColor;
                Global.menu.playlistManager.PlaySound(loseSound, Settings.IsSoundActive);
            }
            isFinished = true;
        }
    }

    /// <summary>
    /// Keep playing despite had won.
    /// </summary>
    public void KeepPlaying()
    {
        Global.menu.PlayMusic(false, true);
        Global.menu.keepPlaying = true;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isFinished)
        {
            score.text = ((int)money.Update(Time.deltaTime)).ToString();

            float raw = time.Update(Time.deltaTime);
            int minutes = (int)time.Update(Time.deltaTime) / 60;
            float seconds = raw - minutes * 60;
            timePlayed.text = $"{minutes:00}:{seconds:00.00}";
        }
    }
}