using UnityEngine;

namespace LivingObjectAddons
{
    public class ExplosionOnDeath : OnDeath
    {
        [Header("Configuration")]
        [Tooltip("Explosion radius.")]
        public float explosionRadius;
        [Tooltip("Explosion damage.")]
        public float explosionDamage;
        // TODO: Add explosion damage falls-off over distance

        [Header("Setup")]
        [Tooltip("Used to draw the gizmos.")]
        public RigidbodyHelper rigidbodyHelperGizmos;

        private Rigidbody2D thisRigidbody2D;
        private bool hasAlreadyExploded = false;

        public override void OnBuild(LivingObject livingObject) => thisRigidbody2D = livingObject.rigidbodyHelper.GetRigidbody2D();

        public override void Die()
        {
            if (!hasAlreadyExploded)
            {
                hasAlreadyExploded = true;
                // TODO: Add explosion graphics

                // https://answers.unity.com/questions/532746/finding-gameobjects-within-a-radius.html
                // https://forum.unity.com/threads/find-gameobjects-in-a-circular-range-of-a-point.36197/
                // We can also do this without Physics2D.OverlapCircleAll...
                Collider2D[] colliders = Physics2D.OverlapCircleAll(thisRigidbody2D.position, explosionRadius);
                foreach (Collider2D collider in colliders)
                {
                    // Avoid recursive loop (TakeDamage -> Die -> Explode -> TakeDamage -> ...)
                    Rigidbody2D rigidbody2D = collider.attachedRigidbody;
                    if (rigidbody2D != thisRigidbody2D && rigidbody2D != null)
                    {
                        // This won't cause NullPointerException because the || clause will only be revised if the explode == null is false, which means there is an explosive to point.
                        rigidbody2D.GetComponent<RigidbodyHelper>()?.TakeDamage(explosionDamage);
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(rigidbodyHelperGizmos.Position, Vector3.back, explosionRadius);
        }
#endif
    }
}