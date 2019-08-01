using UnityEngine;

public class Follow : MonoBehaviour
{
    [Header("Build")]
    [Tooltip("Following transform")]
    public Transform transformToFollow;

    private Vector3 originalLocalPosition;

    /* TODO:
     * https://forum.unity.com/threads/exposing-fields-with-interface-type-c-solved.49524/
     * https://forum.unity.com/threads/c-interface-wont-show-in-inspector.383886/
     * https://forum.unity.com/threads/understanding-iserializationcallbackreceiver.383757/
     */

    private void Awake() => originalLocalPosition = transform.localPosition;

    // Do? https://forum.unity.com/threads/subtracting-quaternions.317649/
    private void LateUpdate() => transform.localPosition = originalLocalPosition + transformToFollow.localPosition;
}
