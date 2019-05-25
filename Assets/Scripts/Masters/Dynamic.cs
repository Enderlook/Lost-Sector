﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Global {
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
    /// Boundaries of the screen.
    /// </summary>
    public static Boundary boundary;
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
    [Tooltip("Boundary of screen.")]
    public Boundary boundary;

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
    }
}

[System.Serializable]
public class Boundary {
    [Tooltip("Boundary. The game object won't be able to cross it.")]
    public Transform[] boundaries;

    // Is fine this hybrid behaivor or should nothing be static
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
    /// <returns>Item1 is the new position of the game object. Item2 is a boolean, if true, the position was clamped inside the boundaries and so the game object position must be updated with the values from Item1.</returns>
    public static System.Tuple<Vector2, bool> CheckForBoundaries(Vector2 position)
    {
        Vector2 newPosition = new Vector2(Mathf.Clamp(position.x, xMin, xMax), Mathf.Clamp(position.y, yMin, yMax));
        return new System.Tuple<Vector2, bool>(newPosition, !position.Equals(newPosition));
    }
}