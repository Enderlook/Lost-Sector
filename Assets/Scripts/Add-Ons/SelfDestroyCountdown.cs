using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyCountdown : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Countdown for self destruction in seconds.")]
    public float countdown;

    private void Start()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(countdown);
        Destroy(gameObject);
    }
}
