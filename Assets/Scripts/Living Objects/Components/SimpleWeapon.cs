using UnityEngine;

namespace LivingObjectAddons
{
    public class SimpleWeapon : MonoBehaviour, IProjectileConfiguration, IBuild, IWeapon
    {
        [Header("Configuration")]
        [Tooltip("Damage on hit.")]
        public float damageOnHit = 1;
        float IMelee.ImpactDamage { get => damageOnHit; set => damageOnHit = value; }

        [Tooltip("Firerate (shoots per second).")]
        public float firerate = 1;

        [Tooltip("Speed.")]
        public float speed = 1;
        float IProjectileConfiguration.Speed => speed;

        [Tooltip("Relative damage on impact based on force.")]
        public bool isDamageRelativeToImpulse;
        bool IMelee.IsImpactDamageRelativeToImpulse { get => isDamageRelativeToImpulse; set => isDamageRelativeToImpulse = value; }

        [Tooltip("Should spawn floating damage text on the enemy on collision?")]
        public bool shouldDisplayDamage;
        bool IShouldDisplayDamage.ShouldDisplayDamage { get => shouldDisplayDamage; set => shouldDisplayDamage = value; }

        [Header("Setup")]
        [Tooltip("Transform point where projectiles will be spawn.")]
        public Transform shootingPosition;
        [Tooltip("Projectile prefab.")]
        public GameObject projectilePrefab;
        [Tooltip("Shooting sound.")]
        public Sound shootingSound;
        [Tooltip("Layer mask of the projectile")]
        public LayerMask layer;
        int IProjectileConfiguration.Layer => layer.ToLayer();

        private RigidbodyHelper rigidbodyHelper;
        
        Vector3 IProjectileConfiguration.SpawnPosition => shootingPosition.position;        
       
        private float cooldownTime = 0f;

        public bool CanShoot {
            get {
                return cooldownTime <= 0f;
            }
        }

        void IBuild.Build(LivingObject livingObject) => rigidbodyHelper = livingObject.rigidbodyHelper;

        /// <summary>
        /// Reduce <see cref="cooldownTime"/> time and checks if the weapon's <see cref="cooldownTime"/> is over.
        /// </summary>
        /// <param name="deltaTime"><see cref="Time.deltaTime"/></param>
        /// <returns><see langword="true"/> if the weapon is ready to shoot, <see langword="false"/> if it's on cooldown.</returns>
        public bool Recharge(float deltaTime)
        {
            return (cooldownTime -= deltaTime) <= 0f;
        }

        /// <summary>
        /// Reset <see cref="cooldownTime"/> time to maximum.
        /// </summary>
        public void ResetCooldown()
        {
            cooldownTime = 1 / firerate;
        }

        /// <summary>
        /// Generate an instance of a projectile an shoot it.<br/>
        /// In addition, cooldown is reseted and a shooting sound is played.<br/>
        /// This method forces to shoot even when the weapon is still on cooldown.
        /// </summary>
        /// <seealso cref="TryShoot(float)"/>
        public void Shoot()
        {
            ResetCooldown();
            GameObject projectile = Instantiate(projectilePrefab, Global.projectilesParent);
            // Just to be sure. We don't really need to set rotation for our game
            projectile.transform.rotation = shootingPosition.rotation;
            projectile.GetComponent<Projectile>().SetProjectileProperties(this);
            shootingSound.Play(rigidbodyHelper.audioSource, 1);
        }

        /// <summary>
        /// Try to shoot a projectile. It will check for the <see cref="cooldownTime"/>, and if possible, shoot.
        /// </summary>
        /// <param name="deltaTime">Time since the last frame. <see cref="Time.deltaTime"/></param>
        /// <returns><see langword="true"/> if the weapon shoot, <see langword="false"/> if it's still on cooldown.</returns>
        public bool TryShoot(float deltaTime = 0)
        {
            if (Recharge(deltaTime))
            {
                Shoot();
                return true;
            }
            return false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (1 < ((IProjectileConfiguration)this).Layer && ((IProjectileConfiguration)this).Layer < 31)
                Debug.LogWarning($"The field {nameof(layer)} should only contain a single layer.");
        }
#endif
    }
    public interface IWeapon
    {
        bool CanShoot { get; }
        void Shoot();
        bool TryShoot(float deltaTime);
        void ResetCooldown();
        bool Recharge(float deltaTime);
    }
}