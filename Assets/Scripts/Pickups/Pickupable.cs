using UnityEngine;

public abstract class Pickupable : MonoBehaviour, ICanBePickedUp
{
    [Header("Configuration")]
    [Tooltip("Initial impulse.")]
    public Vector2RangeTwo impulse;
    [Tooltip("A sound from this list will be picked to play.")]
    public Playlist playlist;

    private Rigidbody2D thisRigidbody2D;
    Rigidbody2D ICanBePickedUp.Rigidbody2D => thisRigidbody2D;

    private void Start()
    {
        transform.parent = Global.pickupsParent;
        thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        thisRigidbody2D.AddRelativeForce((Vector2)impulse * thisRigidbody2D.mass);
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        if (rigidbody2D == null)
            Debug.LogWarning($"Game object {gameObject.name} lacks of an Rigidbody2D Component.");
    }
#endif

    public virtual void Pickup(Player player) => playlist?.PlayAtPoint(thisRigidbody2D.transform.position, Settings.IsSoundActive);
}
