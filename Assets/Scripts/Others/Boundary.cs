using System.Linq;
using UnityEngine;


[System.Serializable]
public class Boundary
{
    // Maybe we could just take the parent gameObject and then find its children
    [Tooltip("Boundary. The game object won't be able to cross it.")]
    public Transform[] boundaries;

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

    /// <summary>
    /// Get the percentage of the yY axis where <paramref name="y"/> is.
    /// </summary>
    /// <param name="y">Y axis to check.</param>
    /// <returns>Percent from 0 to 1 where the <paramref name="y"/> is located in the Y axis.</returns>
    public static float GetYPercent(float y) => (y - yMin) / (yMax - yMin);
}
