using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class Global
{
    /// <summary>
    /// Enemies parent transform. Used to store all the enemies.
    /// </summary>
    public static Transform enemiesParent;
    /// <summary>
    /// Explosions parent transform. Used to store all the explosions.
    /// </summary>
    public static Transform explosionsParent;
    /// <summary>
    /// Projectiles parent transform. Used to store all the projectiles.
    /// </summary>
    public static Transform projectilesParent;
    /// <summary>
    /// Pickups parent transform. Used to store all the pickups.
    /// </summary>
    public static Transform pickupsParent;
    /// <summary>
    /// Boundaries of the screen.
    /// </summary>
    public static Boundary boundary;

    public static CoinMeter coinMeter;
    /// <summary>
    /// Set the <seealso cref="CoinMeter"/> script that controls how money is displayed on canvas.
    /// </summary>
    /// <param name="coinMeterController"><seealso cref="CoinMeter"/> that controls displayed money.</param>
    public static void SetCoinMeter(CoinMeter coinMeterController, int statingMoney)
    {
        coinMeter = coinMeterController;
        coinMeter.ManualUpdate(statingMoney);
    }

    /// <summary>
    /// Current money of the player.
    /// </summary>
    public static int money {
        get => coinMeter.money;
        set => coinMeter.money = value;
    }

    /// <summary>
    /// Instance of the <seealso cref="Menu"/> class used during the scene.
    /// </summary>
    public static Menu menu; // FindObjectOfType<menu>();   Could do this work.

    /// <summary>
    /// Instance of the <seealso cref="EnemySpawner"/> class used to spawn enemies on the scene.
    /// </summary>
    public static EnemySpawner enemySpawner;

    /// <summary>
    /// Amount of money required to win the game.
    /// </summary>
    public static int moneyToWin;

    /// <summary>
    /// Player health bar.
    /// </summary>
    public static HealthBar playerHealthBar;
    /// <summary>
    /// Player shield bar.
    /// </summary>
    public static HealthBar playerShieldBar;
}

public class Dynamic : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Stating money.")]
    public int startingMoney;
    [Tooltip("Amount of money required to win the game.")]
    public int moneyToWin;

    [Header("Setup")]
    [Header("Parents")]
    [Tooltip("Enemies parent transform.")]
    public Transform enemiesParent;
    [Tooltip("Explosion parent transform.")]
    public Transform explosionsParent;
    [Tooltip("Projectiles parent transform.")]
    public Transform projectilesParent;
    [Tooltip("Floating text parent transform.")]
    public Transform floatingTextParent;
    [Tooltip("Pickups parent transform.")]
    public Transform pickupsParent;

    [Header("Menu")]
    [Tooltip("Money controller script.")]
    public CoinMeter coinMeter;
    [Tooltip("Menu script.")]
    public Menu menu;
    [Tooltip("Enemy spawner script.")]
    public EnemySpawner enemySpawner;
    [Tooltip("Text component to show time.")]
    public Text timePlayerText;

    [Header("Player")]
    [Tooltip("Player health bar.")]
    public HealthBar playerHealthBar;
    [Tooltip("Player shield bar.")]
    public HealthBar playerShieldBar;
    [Tooltip("Boundary of screen.")]
    public Boundary boundary;

    public static float playedTime = 0;

    private bool hasWonByCoins = false;

    private void Awake()
    {
        boundary.SetBoundaries();
        StoreGlobals();
    }

    private void StoreGlobals()
    {
        Global.enemiesParent = enemiesParent;
        Global.explosionsParent = explosionsParent;
        Global.projectilesParent = projectilesParent;
        Global.boundary = boundary;
        FloatingTextController.SetFloatingTextParentStatic(floatingTextParent);
        Global.SetCoinMeter(coinMeter, startingMoney);
        Global.pickupsParent = pickupsParent;
        Global.menu = menu;
        Global.enemySpawner = enemySpawner;
        Global.moneyToWin = moneyToWin;
        Global.playerHealthBar = playerHealthBar;
        Global.playerShieldBar = playerShieldBar;
    }
    private void Update()
    {
        if (menu.shouldWork)
        {
            playedTime += Time.deltaTime;
            int minutes = (int)playedTime / 60;
            float seconds = playedTime - minutes * 60;
            timePlayerText.text = $"{minutes:00}:{seconds:00.00}";
        }

        if (Global.coinMeter.showedMoney >= Global.moneyToWin && !hasWonByCoins)
        {
            hasWonByCoins = true;
            Global.menu.GameOver(true);
        }
        Global.menu.canBeShown = Global.playerHealthBar.IsDamageBarPercentHide && Global.playerShieldBar.IsDamageBarPercentHide;
    }
}
