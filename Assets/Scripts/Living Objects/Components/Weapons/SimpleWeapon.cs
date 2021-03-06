using UnityEngine;

namespace LivingObjectAddons
{
    public class SimpleWeapon : WeaponWithShootingAndImpactSound, IProjectileConfiguration
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

        [Header("Setup")]
        [Tooltip("Transform point where projectiles will be spawn.")]
        public Transform shootingPosition;
        [Tooltip("Projectile prefab.")]
        public GameObject projectilePrefab;
        [Tooltip("Layer mask of the projectile")]
        public LayerMask layer;
        int IProjectileConfiguration.Layer => layer.ToLayer();

        /// <summary>
        /// Generate an instance of a projectile an shoot it.<br/>
        /// In addition, cooldown is reseted and a shooting sound is played.<br/>
        /// This method forces to shoot even when the weapon is still on cooldown.
        /// </summary>
        /// <seealso cref="TryShoot(float)"/>
        public override void Shoot()
        {
            GameObject projectile = Instantiate(projectilePrefab, Global.projectilesParent);
            projectile.transform.position = shootingPosition.position;
            // Just to be sure. We don't really need to set rotation for our game
            projectile.transform.rotation = shootingPosition.rotation;
            projectile.GetComponent<Projectile>().SetProjectileProperties(this);
            base.Shoot();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (1 < ((IProjectileConfiguration)this).Layer && ((IProjectileConfiguration)this).Layer < 31)
                Debug.LogWarning($"The field {nameof(layer)} should only contain a single layer.");
        }

        private void OnDrawGizmos() => Gizmos.DrawIcon(shootingPosition.position, "Aim.png");
#endif
    }

    public abstract class WeaponWithShootingAndImpactSound: WeaponWithShootingSound, IImpactSound
    {
        [Tooltip("Impact Sound")]
        public Sound impactSound;
        Sound IImpactSound.ImpactSound { get => impactSound; set => impactSound = value; }
    }

    public abstract class WeaponWithShootingSound : Weapon, IBuild
    {
        [Header("Setup")]
        [Tooltip("Shooting sound.")]
        public Sound shootingSound;

        protected RigidbodyHelper rigidbodyHelper;

        void IBuild.Build(LivingObject livingObject) => rigidbodyHelper = livingObject.rigidbodyHelper;

        public override void Shoot()
        {
            shootingSound.PlayOneShoot(rigidbodyHelper.audioSource, Settings.IsSoundActive, 1);
            base.Shoot();
        }
    }

    public abstract class Weapon : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Firerate (shoots per second).")]
        public float firerate = 1;

        [HideInInspector]
        public float strengthMultiplier = 1;

        /// <summary>
        /// Current cooldown time.
        /// </summary>
        public float CooldownTime => cooldownTime;
        protected float cooldownTime = 0f;

        /// <summary>
        /// Total cooldown time.
        /// </summary>
        public float TotalCooldown => 1 / firerate;

        /// <summary>
        /// Cooldown percent from 0 to 1. When 0, it's ready to shoot.
        /// </summary>
        public float CooldownPercent => Mathf.Clamp01(cooldownTime / TotalCooldown);

        /// <summary>
        /// Whenever it can shoot or is still in cooldown.
        /// </summary>
        public virtual bool CanShoot => cooldownTime <= 0;

        public virtual void Shoot() => ResetCooldown();

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


        /// <summary>
        /// Reset <see cref="cooldownTime"/> time to maximum.
        /// </summary>
        public void ResetCooldown() => cooldownTime = TotalCooldown;

        /// <summary>
        /// Reduce <see cref="cooldownTime"/> time and checks if the weapon's <see cref="cooldownTime"/> is over.
        /// </summary>
        /// <param name="deltaTime"><see cref="Time.deltaTime"/></param>
        /// <returns><see langword="true"/> if the weapon is ready to shoot, <see langword="false"/> if it's on cooldown.</returns>
        public bool Recharge(float deltaTime)
        {
            cooldownTime -= deltaTime;
            return CanShoot;
        }
    }
}