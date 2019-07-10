using UnityEngine;

public class EnemyAsteroidExplosive : Enemy
{
    [Header("Configuration")]
    [Tooltip("Explosion radius.")]
    public float explosionRadius;
    [Tooltip("Explosion damage.")]
    public float explosionDamage;
    // TODO: Add explosion damage falls-off over distance

    private bool hasAlreadyExploded = false;

    protected override void Die()
    {
        if (!hasAlreadyExploded)
        {
            hasAlreadyExploded = true;
            // TODO: Add explosion graphics

            // https://answers.unity.com/questions/532746/finding-gameobjects-within-a-radius.html
            // https://forum.unity.com/threads/find-gameobjects-in-a-circular-range-of-a-point.36197/
            // We can also do this without Physics2D.OverlapCircleAll...
            Collider2D[] colliders = Physics2D.OverlapCircleAll(rigidbodyHelper.Position, explosionRadius);
            foreach (Collider2D collider in colliders)
            {
                // Avoid recursive loop (TakeDamage -> Die -> Explode -> TakeDamage -> ...)
                Rigidbody2D rigidbody2D = collider.attachedRigidbody;
                if (rigidbody2D != rigidbodyHelper.GetRigidbody2D() && rigidbody2D != null)
                {
                    // This won't cause NullPointerException because the || clause will only be revised if the explode == null is false, which means there is an explosive to point.
                    rigidbody2D.GetComponent<RigidbodyHelper>()?.TakeDamage(explosionDamage);
                }
            }
        }
        base.Die();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(rigidbodyHelper.Position, Vector3.back, explosionRadius);
    }
#endif
}
