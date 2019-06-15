using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyEndAnimation : MonoBehaviour
{
    private void Start()
    {
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator != null)
            Destroy(gameObject, animator.runtimeAnimatorController.animationClips[0].length);
    }

    private void OnValidate()
    {
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning($"Game object {gameObject.name} lacks of an Animator Component.");
    }
}
