using UnityEngine;

namespace LivingObjectAddons
{
    public class RadialBurst : WeaponWithSound, IProjectileConfiguration
    {
        [Header("Configuration")]
        [Tooltip("Damage on hit.")]
        public float damageOnHit;
        float IMelee.ImpactDamage { get => damageOnHit * strengthMultiplier; set => damageOnHit = value / strengthMultiplier; }

        [Tooltip("Speed.")]
        public float speed = 1;
        float IProjectileConfiguration.Speed => speed;

        [Tooltip("Relative damage on impact based on force.")]
        public bool isDamageRelativeToImpulse;
        bool IMelee.IsImpactDamageRelativeToImpulse { get => isDamageRelativeToImpulse; set => isDamageRelativeToImpulse = value; }

        [Tooltip("Should spawn floating damage text on the enemy on collision?")]
        public bool shouldDisplayDamage;
        bool IShouldDisplayDamage.ShouldDisplayDamage { get => shouldDisplayDamage; set => shouldDisplayDamage = value; }

        [Tooltip("Radius of shooting circle.")]
        public float radius;
        [Tooltip("Amount of projectiles.")]
        public int pellets;

        [Header("Setup")]
        [Tooltip("Transform point where the area of effect will begin.")]
        public Transform shootingPosition;
        [Tooltip("Projectile prefab.")]
        public GameObject projectilePrefab;
        [Tooltip("Layer mask of the projectile")]
        public LayerMask layer;
        int IProjectileConfiguration.Layer => layer.ToLayer();

        public override void Shoot()
        {
            foreach ((Vector3 position, float angle) in GetCoordinates())
            {
                MakeProjectile(position, angle);
            }
            base.Shoot();
        }

        private void MakeProjectile(Vector3 position, float angle)
        {
            // https://answers.unity.com/questions/27365/radialomni-directional-shooting-problem.html
            GameObject projectile = Instantiate(projectilePrefab, Global.projectilesParent);
            projectile.transform.position = position;
            projectile.transform.rotation = Quaternion.AngleAxis(angle, shootingPosition.forward);
            projectile.GetComponent<Projectile>().SetProjectileProperties(this);
        }

        private System.Collections.Generic.IEnumerable<(Vector3 position, float angle)> GetCoordinates()
        {
            if (pellets > 0)
            {
                float anglePerPellet = 360 / pellets;
                float angle = 0;
                for (int i = 0; i < pellets; i++)
                {
                    angle += anglePerPellet;
                    // https://answers.unity.com/questions/209216/finding-a-circumference-point.html
                    Vector3 circle = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
                    Vector3 worldPos = shootingPosition.TransformPoint(circle * radius);
                    yield return (position: worldPos, angle: angle);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(shootingPosition.position, Vector3.back, radius);

            foreach ((Vector3 position, float angle) in GetCoordinates())
            {
                Gizmos.DrawIcon(position, "Aim.png");
            }
        }
#endif
    }
}