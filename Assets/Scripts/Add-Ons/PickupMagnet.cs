using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupMagnet : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Magnet radius.")]
    public float magnetRadius;
    private float magnetRadiusSquared;
    [Tooltip("Magnet strength.")]
    public float magnetStrength;
    [Tooltip("Pickup radius")]
    public float pickupRadius;

    [Header("Setup")]
    [Tooltip("Transform where pickups should be pulled.")]
    public Transform magnetTransform;

    // TODO: If we change magnetStrength, _magnetStrenght isn't updated... in the future it may be a problem...
    private void Start() => magnetRadiusSquared = magnetRadius * magnetRadius;

    private void FixedUpdate()
    {
        foreach(Transform item in Global.pickupsParent)
        {
            // We don't use Vector3.Distance because that is (a - b).magnitude and that is expensive.
            float distance = (item.position - magnetTransform.position).sqrMagnitude;

            if (distance <= pickupRadius)
            {
                // https://forum.unity.com/threads/getcomponents-possible-to-use-with-c-interfaces.60596/
                IPickup pickup = item.GetComponent<IPickup>();
                if (pickup != null)
                    pickup.Pickup();
            } else if (distance <= magnetRadiusSquared)
            {
                float pullingSpeed = (magnetRadiusSquared / distance) * magnetRadius * Time.fixedDeltaTime;
                item.position = Vector3.MoveTowards(item.position, magnetTransform.position, pullingSpeed);
            }
        }
    }

    private void OnValidate()
    {
        if (magnetTransform == null)
            Debug.LogWarning($"Game object {gameObject.name} lacks of an {nameof(magnetTransform)} Component.");
        if (magnetRadius <= 0)
            Debug.LogWarning($"Game object {gameObject.name} has a {nameof(magnetRadius)} of {magnetRadius}. It must be greater than 0.");            
        if (pickupRadius <= 0)
            Debug.LogWarning($"Game object {gameObject.name} has a {nameof(pickupRadius)} of {pickupRadius}. It must be greater than 0.");
    }

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.DrawWireDisc(magnetTransform.position, Vector3.back, magnetRadius);
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(magnetTransform.position, Vector3.back, pickupRadius);
    }
}
