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

            if (hasWon)
            {
                subtitle.text = "Congratulation, you win!";
                subtitle.color = winColor;
                Global.menu.playlistManager.PlaySound(winSound);
            }
            else
            {
                subtitle.text = "You failed the mission!";
                subtitle.color = loseColor;
                Global.menu.playlistManager.PlaySound(loseSound);
            }
            isFinished = true;
        }
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
public class TextOverTime
{
    private readonly float targetValue;
    private float showed;
    private const float PERCENT_RATE = .35f;

    /// <summary>
    /// Generate a counter that increases until show a <paramref name="targetValue"/>.
    /// </summary>
    /// <param name="targetValue">Value target to show.</param>
    public TextOverTime(float targetValue) => this.targetValue = targetValue;

    /// <summary>
    /// Update counter.
    /// </summary>
    /// <param name="time">Time passed since las update.</param>
    /// <returns>New value to show.</returns>
    public float Update(float time)
    {
        if (showed < targetValue)
        {
            showed += targetValue * PERCENT_RATE * time;
            if (showed > targetValue)
                showed = targetValue;
        }
        return showed;
    }
}