using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SelfDestroyEndAnimation : MonoBehaviour
{
    [Tooltip("Deactivate instead of destroy.")]
    public bool hide;

    private Animator animator;
    private float cooldown;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        if (!hide)
            Destroy(gameObject, animator.runtimeAnimatorController.animationClips[0].length);
        else
        {
            cooldown = animator.runtimeAnimatorController.animationClips[0].length;
        }
    }

    private void Update()
    {
        if (hide && (cooldown -= Time.deltaTime) <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (animator != null)
            cooldown = animator.runtimeAnimatorController.animationClips[0].length;
    }

    private void OnValidate()
    {
        Animator animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning($"Game object {gameObject.name} lacks of an Animator Component.");
    }

}
