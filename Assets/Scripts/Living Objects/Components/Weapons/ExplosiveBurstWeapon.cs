using UnityEngine;

namespace LivingObjectAddons
{
    public class ExplosiveBurstWeapon : WeaponWithSound
    {
        [Header("Configuration")]
        [Tooltip("Damage on hit.")]
        public float damageOnHit;
        [Tooltip("Push force on hit")]
        public float forceOnHit;
        [Tooltip("Range of effect.")]
        public float radius;
        [Tooltip("Should spawn floating damage text on the enemy on collision?")]
        public bool shouldDisplayDamage;
        [Tooltip("Shockwave sprite.")]
        public Sprite shockwaveSprite;

        [Header("Setup")]
        [Tooltip("Transform point where the area of effect will begin.")]
        public Transform shootingPosition;

        public override void Shoot()
        {
            MakeExplosion();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(rigidbodyHelper.Rigidbody2D.position, radius);
            foreach (Collider2D collider in colliders)
            {
                Rigidbody2D rigidbody2D = collider.attachedRigidbody;
                if (rigidbody2D != rigidbodyHelper.Rigidbody2D)
                {
                    float chargeFactor = CalculateChargeFactor();
                    float distanceFactor = CalculateDistanceFactor(collider);
                    Push(rigidbody2D, distanceFactor * forceOnHit * chargeFactor);
                    Hurt(rigidbody2D, distanceFactor * damageOnHit * chargeFactor);
                }
            }
            base.Shoot();
        }

        private void MakeExplosion()
        {
            GameObject shockwave = new GameObject("Shockwave");
            SpriteRenderer spriteRenderer = shockwave.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = shockwaveSprite;            
            shockwave.transform.position = shootingPosition.position;
            shockwave.AddComponent<Expand>().size = radius * 2;

        }

        private void Hurt(Rigidbody2D target, float amount)
        {
            RigidbodyHelper rigidbodyHelper = target.GetComponent<RigidbodyHelper>();
            rigidbodyHelper?.TakeDamage(amount, shouldDisplayDamage);
        }

        private void Push(Rigidbody2D target, float force) => target.AddForce(-(shootingPosition.position - target.transform.position) * force);

        private float CalculateDistanceFactor(Collider2D target) => (rigidbodyHelper.Rigidbody2D.Distance(target).distance / radius) * .5f + .5f;

        private float CalculateChargeFactor() => CooldownPercent <= 0 ? 1 : (1 - CooldownPercent) * .5f + .25f;

        public override bool CanShoot => cooldownTime <= (1 / firerate) / 2;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(shootingPosition.position, Vector3.back, radius);
        }
#endif    

    private class Expand : MonoBehaviour
        {
            public float size;
            private SpriteRenderer spriteRenderer;
            private float time;

            private void Start()
            {
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.drawMode = SpriteDrawMode.Sliced;
                spriteRenderer.size = Vector2.zero;
                spriteRenderer.color = new Color(1, 1, 0);
            }

            private void Update()
            {
                spriteRenderer.size = Vector2.one * Mathf.Lerp(0, size, time);
                if (spriteRenderer.size.x >= size)
                    Destroy(gameObject);
                time += Time.deltaTime * 3;
            }
        }
    }
}
