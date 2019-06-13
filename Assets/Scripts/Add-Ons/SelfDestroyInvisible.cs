using UnityEngine;

public class SelfDestroyInvisible : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Game Object to destroy. If null current game object will be destroyed.")]
    public GameObject gameObjectToDestroy;
    [Tooltip("Time since invisible to destroy.")]
    public float timeToDestroy = 1;

    private bool destroying;
    private float timeSinceInvisible = 0;

    /* To read:
     * https://answers.unity.com/questions/1230388/how-to-destroy-object-after-it-moves-out-of-screen.html
     */

    private void Update()
    {
        if (destroying)
        {
            timeSinceInvisible += Time.deltaTime;
            if (timeSinceInvisible >= timeToDestroy)
                Destroy(gameObjectToDestroy != null ? gameObjectToDestroy : gameObject);
        }
    }

    /// <summary>
    /// Enable self destroy countdown.
    /// </summary>
    private void OnBecameInvisible()
    {
        destroying = true;
        timeSinceInvisible = 0f;
    }

    /// <summary>
    /// Disable self destroy countdown.
    /// </summary>
    private void OnBecameVisible()
    {
        destroying = false;
        timeSinceInvisible = 0f;
    }
}
