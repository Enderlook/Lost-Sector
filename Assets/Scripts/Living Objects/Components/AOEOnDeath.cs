
using UnityEngine;

namespace LivingObjectAddons
{
    public abstract class AOEOnDeath : MonoBehaviour, IBuild, IDie
    {
        [Header("Configuration")]
        [Tooltip("Area of effect radius.")]
        public float radius;

        [Header("Setup")]
        [Tooltip("Used to draw the gizmos.")]
        public RigidbodyHelper rigidbodyHelperGizmos;

        private Rigidbody2D thisRigidbody2D;
        private bool hasAlreadyBeenUsed = false;

        void IBuild.Build(LivingObject livingObject) => thisRigidbody2D = livingObject.rigidbodyHelper.Rigidbody2D;
        void IDie.Die()
        {
            if (!hasAlreadyBeenUsed)
            {
                hasAlreadyBeenUsed = true;

                // https://answers.unity.com/questions/532746/finding-gameobjects-within-a-radius.html
                // https://forum.unity.com/threads/find-gameobjects-in-a-circular-range-of-a-point.36197/
                // We can also do this without Physics2D.OverlapCircleAll...
                Collider2D[] colliders = Physics2D.OverlapCircleAll(thisRigidbody2D.position, radius);
                foreach (Collider2D collider in colliders)
                {
                    // Avoid recursive loop (e.g: TakeDamage -> Die -> Explode -> TakeDamage -> ...)
                    Rigidbody2D rigidbody2D = collider.attachedRigidbody;
                    if (rigidbody2D != thisRigidbody2D && rigidbody2D != null)
                    {
                        // This won't cause NullPointerException because the || clause will only be revised if the explode == null is false, which means there is an explosive to point.
                        RigidbodyHelper target = rigidbody2D.GetComponent<RigidbodyHelper>();
                        if (target != null)
                            AffectTarget(target);
                    }
                }
            }
        }

        protected abstract void AffectTarget(RigidbodyHelper target);

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(rigidbodyHelperGizmos.Position, Vector3.back, radius);
        }
#endif
    }
}
