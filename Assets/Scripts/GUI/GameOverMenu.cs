using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Subtitle of the menu. Used to show if player won or loose.")]
    public Text subtitle;
    [Tooltip("Text used to show score.")]
    public Text score;

    [Tooltip("Color used on subtitle if player won.")]
    public Color winColor = Color.green;
    [Tooltip("Color used on subtitle if player lose.")]
    public Color loseColor = Color.red;

    private bool isFinished = false;
    private int money;
    private float showedMoney = 0;
    private float MONEY_RATE_PERCENT = 0.35f;
    
    /// <summary>
    /// Determines whenever the game over menu is displayed or hidden.
    /// </summary>
    /// <param name="show">Whenever that game over menu is displayed or hidden.</param>
    public void SetShown(bool show) => gameObject.SetActive(show);

    public void SetConfiguration(bool hasWon, int money)
    {
        this.money = money;
        if (hasWon)
        {
            subtitle.text = "Congratulation, you win!";
            subtitle.color = winColor;
        } else
        {
            subtitle.text = "You failed the mission!";
            subtitle.color = loseColor;
        }
        isFinished = true;       
    }

    private void Update()
    {
        if (isFinished)
        {
            showedMoney += money * MONEY_RATE_PERCENT * Time.unscaledDeltaTime; // Just to be sure...
            if (showedMoney > money)
                score.text = money.ToString();
            else
                score.text = Mathf.Floor(showedMoney).ToString();
        }
    }
}
