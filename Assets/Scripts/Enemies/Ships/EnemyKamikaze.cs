using UnityEngine;

public class EnemyKamikaze : Enemy
{
    [Header("Configuration")]
    [Tooltip("Seconds waited before start accelerating.")]
    public float waitSecondsBeferoAccelerate;
    [Tooltip("Acceleration in units per second.")]
    public float acceleration;

    private bool isAccelerating = false;
    private float secondsWaitedBeforeAccelerate = 0;

    private void Update()
    {
        if (isAccelerating)
            rigidbodyHelper.GetRigidbody2D().AddRelativeForce(new Vector2(0, acceleration));
        else
        {
            secondsWaitedBeforeAccelerate += Time.deltaTime;
            isAccelerating = secondsWaitedBeforeAccelerate > waitSecondsBeferoAccelerate;
        }
    }
}
