using UnityEngine;

namespace LivingObjectAddons
{
    public class SelfDestroyInvisible : DoWhenInvisible, IInitialize
    {
        [Tooltip("Game Object to destroy. If null current game object will be destroyed.")]
        public GameObject gameObjectToDestroy;
        [Tooltip("Deactivate instead of destroy.")]
        public bool hide;

        private void Start()
        {
            if (gameObjectToDestroy == null)
                gameObjectToDestroy = gameObject;
        }

        protected override void DoEffect(bool visible)
        {
            if (visible)
                return;
            if (hide)
                gameObjectToDestroy.SetActive(false);
            else
                Destroy(gameObject);
        }

        /// <summary>
        /// Reset <see cref="destroying"/> to <see langword="false"/> and <see cref="timeSinceInvisible"/> to 0.
        /// </summary>
        void IInitialize.Initialize()
        {
            doing = false;
            timeSinceInvisible = 0;
        }

        /// <summary>
        /// Enable self destroy countdown.
        /// </summary>
        private void OnBecameInvisible()
        {
            doing = true;
            timeSinceInvisible = 0f;
        }

        /// <summary>
        /// Disable self destroy countdown.
        /// </summary>
        private void OnBecameVisible()
        {
            doing = false;
            timeSinceInvisible = 0f;
        }
    }
}