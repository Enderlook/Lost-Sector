using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class StoppableRigidbody : MonoBehaviour
{
    private Rigidbody2D Rigidbody2D {
        get {
            if (thisRigidbody2D == null)
                thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            return thisRigidbody2D;
        }
    }
    private Rigidbody2D thisRigidbody2D;

    private float angularVelocity;
    private Vector2 velocity;

    private bool paused = false;
    /// <summary>
    /// Stop the <see cref="Rigidbody2D"/> movement.
    /// </summary>
    public bool Paused {
        get => paused;
        set {
            if (value == paused)
                return;
            paused = value;
            if (value)
            {
                velocity = Rigidbody2D.velocity;
                angularVelocity = Rigidbody2D.angularVelocity;
                Rigidbody2D.velocity = Vector2.zero;
                Rigidbody2D.angularVelocity = 0;
                Rigidbody2D.isKinematic = true;
            }
            else
            {
                Rigidbody2D.isKinematic = false;
                Rigidbody2D.velocity = velocity;
                Rigidbody2D.angularVelocity = angularVelocity;
            }
        }

    }
}
