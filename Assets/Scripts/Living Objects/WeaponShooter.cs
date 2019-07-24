using LivingObjectAddons;
using UnityEngine;

namespace PlayerAddons
{
    public class WeaponShooter : MonoBehaviour
    {
        [Tooltip("Weapons configuration")]
        public WeaponKeyPair[] weapons;

        private void Update()
        {
            foreach (WeaponKeyPair weapon in weapons)
            {
                if (weapon.ShouldShoot)
                    weapon.weapon.TryShoot();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                WeaponKeyPair weapon = weapons[i];
                if (weapon.weapon == null)
                    Debug.LogWarning($"Gameobject {gameObject.name} has the {nameof(weapons)} element at index {i} without a {nameof(WeaponKeyPair.weapon)} configured on it.\nIt will raise error if it's tried to shoot in-game.");
                if (weapon.key == KeyCode.None && weapon.button == WeaponKeyPair.MouseButton.None)
                    Debug.LogWarning($"Gameobject {gameObject.name} has the {nameof(weapons)} element at index {i} without an assigned {nameof(WeaponKeyPair.key)} nor {nameof(WeaponKeyPair.button)}. It can't be shooted in-game.");
            }
        }
#endif

    }

    [System.Serializable]
    public class WeaponKeyPair
    {
        public enum MouseButton { None = -1, Left = 0, Right = 1, Middle = 2 }
        [Tooltip("Weapon.")]
        public Weapon weapon;
        [Tooltip("Key to shoot.")]
        public KeyCode key;
        [Tooltip("Mouse button to shoot.")]
        public MouseButton button;
        [Tooltip("Can be hold down.")]
        public bool canBeHoldDown;

        public bool ShouldShoot => canBeHoldDown
            ? Input.GetKey(key) || (button != MouseButton.None && Input.GetMouseButton((int)button))
            : Input.GetKeyDown(key) || (button != MouseButton.None && Input.GetMouseButtonDown((int)button));
    }
}