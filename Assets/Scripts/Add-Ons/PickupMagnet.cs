using UnityEngine;

public class PickupMagnet : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Magnet radius.")]
    public float magnetRadius;
    public float MagnetRadius {
        get {
            return magnetRadius;
        }
        set {
            magnetRadius = value;
            magnetRadiusSquared = magnetRadius * magnetRadius;
        }
    }
    private float magnetRadiusSquared;
    [Tooltip("Magnet strength.")]
    public float magnetStrength;
    [Tooltip("Pickup radius")]
    public float pickupRadius;

    [Header("Setup")]
    [Tooltip("Transform where pickups should be pulled.")]
    public Transform magnetTransform;

    private Player player;

    private void Start()
    {
        magnetRadiusSquared = magnetRadius * magnetRadius;
        player = gameObject.GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        foreach (Transform item in Global.pickupsParent)
        {
            // We don't use Vector3.Distance because that is (a - b).magnitude and that is expensive.
            float distance = (item.position - magnetTransform.position).sqrMagnitude;

            if (distance <= pickupRadius)
            {
                Pickup(item);
            }
            else if (distance <= magnetRadiusSquared)
            {
                float pullingSpeed = (magnetRadiusSquared / distance) * magnetRadius * Time.fixedDeltaTime;
                item.position = Vector3.MoveTowards(item.position, magnetTransform.position, pullingSpeed);
            }
        }
    }

    /// <summary>
    /// Pickup the <paramref name="item"/>.<br/>
    /// Execute the <see cref="Pickupable.Pickup"/> from <paramref name="item"/> method using the implemented overload with the correct parameters.
    /// </summary>
    /// <param name="item">Item to be picked up.</param>
    private void Pickup(Transform item)
    {
        ICanBePickedUp pickup = item.GetComponent<ICanBePickedUp>();
        if (pickup != null)
        {
            pickup.Pickup(player);
            Destroy(item.gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (magnetTransform == null)
            Debug.LogWarning($"Game object {gameObject.name} lacks of an {nameof(magnetTransform)} Component.");
        if (magnetRadius <= 0)
            Debug.LogWarning($"Game object {gameObject.name} has a {nameof(magnetRadius)} of {magnetRadius}. It must be greater than 0.");
        if (pickupRadius <= 0)
            Debug.LogWarning($"Game object {gameObject.name} has a {nameof(pickupRadius)} of {pickupRadius}. It must be greater than 0.");
    }
#endif
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.DrawWireDisc(magnetTransform.position, Vector3.back, magnetRadius);
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(magnetTransform.position, Vector3.back, pickupRadius);
    }
#endif
}

public interface ICanBePickedUp
{
    /// <summary>
    /// <seealso cref="Rigidbody2D"/> of this pickup.
    /// </summary>
    Rigidbody2D Rigidbody2D { get; }
    /// <summary>
    /// Method executed when a pickup is used.
    /// </summary>
    /// <param name="player">Instance of the <see cref="Player"/> which picked it up.</param>
    void Pickup(Player player);
}