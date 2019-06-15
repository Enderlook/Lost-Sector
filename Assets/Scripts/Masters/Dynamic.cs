﻿using System.Linq;
using UnityEngine;

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

    private static CoinMeter coinMeter;
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
        get {
            return coinMeter.money;
        }
        set {
            coinMeter.money = value;
        }
    }
}

public class Dynamic : MonoBehaviour
{
    [Header("Setup")]
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
    [Tooltip("Boundary of screen.")]
    public Boundary boundary;
    [Tooltip("Stating money.")]
    public int startingMoney;
    [Tooltip("Money controller script.")]
    public CoinMeter coinMeter;

    private void Awake()
    {
        boundary.SetBoundaries();

        // Workaround to avoid Singleton Pattern because no one likes it :(
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
    }
}

[System.Serializable]
public class Boundary
{
    // Maybe we could just take the parent gameObject and then find its children
    [Tooltip("Boundary. The game object won't be able to cross it.")]
    public Transform[] boundaries;

    // Is fine this hybrid behavior or should nothing be static?
    // Maybe the whole class could be static and be separated from Global...
    private static float xMin, xMax, yMin, yMax;

    /// <summary>
    /// Set boundaries in order to use the Boundary class. Mandatory.
    /// </summary>
    public void SetBoundaries()
    {
        // Using this we can work with 2 diagonals transforms or 4, whatever we have.
        xMin = boundaries.Min(e => e.position.x);
        xMax = boundaries.Max(e => e.position.x);
        yMin = boundaries.Min(e => e.position.y);
        yMax = boundaries.Max(e => e.position.y);
    }

    /// <summary>
    /// Constrains a position to fit inside the boundaries. If the position is outside it will be put inside.
    /// </summary>
    /// <param name="position">Position of the game object to check.</param>
    /// <returns><c>Item1</c> is the new position of the game object.<br/>
    /// <c>Item2</c> is a <see langword="bool"/>, if <see langword="true"/>, the position was clamped inside the boundaries and so the game object position must be updated with the values from <c>Item1</c>.</returns>
    public static System.Tuple<Vector2, bool> CheckForBoundaries(Vector2 position)
    {
        Vector2 newPosition = new Vector2(Mathf.Clamp(position.x, xMin, xMax), Mathf.Clamp(position.y, yMin, yMax));
        return new System.Tuple<Vector2, bool>(newPosition, !position.Equals(newPosition));
    }
}