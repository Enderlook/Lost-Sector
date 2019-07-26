using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisableBehavioursInvisible : DoWhenInvisible
{
    [Tooltip("Behaviours to disable.")]
    public Behaviour[] behavioursToDisable;

    private IEnumerable<Behaviour> behavioursDisabledByMe;

    protected override void DoEffect(bool visible)
    {
        if (visible)
        {
            if (behavioursDisabledByMe != null)
            {
                foreach (Behaviour behaviour in behavioursDisabledByMe)
                {
                    behaviour.enabled = true;
                }
                behavioursDisabledByMe = null;
            }
        }
        else
        {
            behavioursDisabledByMe = behavioursToDisable.Where(e => Disable(e));
        }
    }

    private bool Disable(Behaviour behaviour)
    {
        if (behaviour.enabled)
        {
            behaviour.enabled = false;
            return true;
        }
        return false;
    }
}

public abstract class DoWhenInvisible : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Time since invisible to do effect.")]
    public float timeToDoEffect = 1;
    [Tooltip("Cameras to check visibility. If null it will use all cameras.")]
    public Camera[] camerasToCheck;
    [Tooltip("Use main camera.")]
    public bool mainCamera;

    protected bool doing;
    private bool wasDoing;
    protected float timeSinceInvisible = 0;

    /* To read:
     * https://answers.unity.com/questions/1230388/how-to-destroy-object-after-it-moves-out-of-screen.html
     */

    private void Update()
    {
        if (mainCamera || camerasToCheck.Length > 0)
            doing = !IsVisibleToCamera(transform, mainCamera, camerasToCheck.Where(e => e != null).ToArray());
        if (doing)
        {
            if (timeSinceInvisible >= timeToDoEffect)
            {
                DoEffect(false);
            }
            else
                timeSinceInvisible += Time.deltaTime;
            wasDoing = true;
        }
        else if (wasDoing)
        {
            wasDoing = false;
            DoEffect(true);
        }
    }

    /// <summary>
    /// Produce effects.
    /// </summary>
    /// <param name="invisible">Whenever it was called because it become visible or invisible.</param>
    protected abstract void DoEffect(bool visible);

    /// <summary>
    /// Check if the position can be seen by at least one <seealso cref="Camera"/> from <paramref name="cameras"/> or by the <paramref name="mainCamera"/> if <see langword="true"/>.
    /// </summary>
    /// <param name="position">position to check if can be seen.</param>
    /// <param name="mainCamera">Whenever it should check the <see cref="Camera.main"/> or not.</param>
    /// <param name="cameras">Additional cameras to check.</param>
    /// <returns>Whenever it's being seen by any of those cameras.</returns>
    public static bool IsVisibleToCamera(Vector3 position, bool mainCamera, params Camera[] cameras)
    {
        // https://forum.unity.com/threads/how-do-i-use-renderer-isvisible.377388/
        bool Test(Vector3 visTest) => (visTest.x >= 0 && visTest.y >= 0) && (visTest.x <= 1 && visTest.y <= 1) && visTest.z >= 0;

        bool main = false;
        bool[] others = new bool[cameras.Length];
        if (mainCamera)
            main = Test(Camera.main.WorldToViewportPoint(position));
        if (cameras.Length > 0)
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                others[i] = Test(cameras[i].WorldToViewportPoint(position));
            }
        }
        return (main && mainCamera) || others.Contains(true);
    }

    /// <summary>
    /// Check if the transform can be seen by at least one <seealso cref="Camera"/> from <paramref name="cameras"/> or by the <paramref name="mainCamera"/> if <see langword="true"/>.
    /// </summary>
    /// <param name="transform">Transform to check if can be seen.</param>
    /// <param name="mainCamera">Whenever it should check the <see cref="Camera.main"/> or not.</param>
    /// <param name="cameras">Additional cameras to check.</param>
    /// <returns>Whenever it's being seen by any of those cameras.</returns>
    public static bool IsVisibleToCamera(Transform transform, bool mainCamera, params Camera[] cameras) => IsVisibleToCamera(transform.position, mainCamera, cameras);

    /// <summary>
    /// Enable self destroy countdown.
    /// </summary>
    private void OnBecameInvisible()
    {
        doing = true;
        timeSinceInvisible = 0f;
    }

    /// <summary>
    /// Disable self destroy countdown.
    /// </summary>
    private void OnBecameVisible()
    {
        doing = false;
        timeSinceInvisible = 0f;
    }
}