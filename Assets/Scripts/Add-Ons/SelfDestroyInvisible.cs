using System.Linq;
using UnityEngine;

namespace LivingObjectAddons
{
    public class SelfDestroyInvisible : MonoBehaviour, IInitialize
    {
        [Header("Configuration")]
        [Tooltip("Game Object to destroy. If null current game object will be destroyed.")]
        public GameObject gameObjectToDestroy;
        [Tooltip("Time since invisible to destroy.")]
        public float timeToDestroy = 1;
        [Tooltip("Deactivate instead of destroy.")]
        public bool hide;
        [Tooltip("Cameras to check visibility. If null it will use all cameras.")]
        public Camera[] camerasToCheck;
        [Tooltip("Use main camera.")]
        public bool mainCamera;

        private bool destroying;
        private float timeSinceInvisible = 0;

        /* To read:
         * https://answers.unity.com/questions/1230388/how-to-destroy-object-after-it-moves-out-of-screen.html
         */

        private void Start()
        {
            if (gameObjectToDestroy == null)
                gameObjectToDestroy = gameObject;
        }

        private void Update()
        {
            if (mainCamera || camerasToCheck.Length > 0)
                destroying = !IsVisibleToCamera(transform, mainCamera, camerasToCheck.Where(e => e != null).ToArray());
            if (destroying)
            {
                if (timeSinceInvisible >= timeToDestroy)
                {
                    if (hide)
                        gameObjectToDestroy.SetActive(false);
                    else
                        Destroy(gameObjectToDestroy);
                }
                else
                    timeSinceInvisible += Time.deltaTime;
            }
        }

        /// <summary>
        /// Reset <see cref="destroying"/> to <see langword="false"/> and <see cref="timeSinceInvisible"/> to 0.
        /// </summary>
        void IInitialize.Initialize()
        {
            destroying = false;
            timeSinceInvisible = 0;
        }

        /// <summary>
        /// Check if the position can be seen by at least one <seealso cref="Camera"/> from <paramref name="cameras"/> or by the <paramref name="mainCamera"/> if <see langword="true"/>.
        /// </summary>
        /// <param name="position">position to check if can be seen.</param>
        /// <param name="mainCamera">Whenever it should check the <see cref="Camera.main"/> or not.</param>
        /// <param name="cameras">Additional cameras to check.</param>
        /// <returns>Whenever it's being seen by any of those cameras.</returns>
        public static bool IsVisibleToCamera(Vector3 position, bool mainCamera, params Camera[] cameras)
        {
            // https://forum.unity.com/threads/how-do-i-use-renderer-isvisible.377388/
            bool Test(Vector3 visTest) => (visTest.x >= 0 && visTest.y >= 0) && (visTest.x <= 1 && visTest.y <= 1) && visTest.z >= 0;

            bool main = false;
            bool[] others = new bool[cameras.Length];
            if (mainCamera)
                main = Test(Camera.main.WorldToViewportPoint(position));
            if (cameras.Length > 0)
            {
                for (int i = 0; i < cameras.Length; i++)
                {
                    others[i] = Test(cameras[i].WorldToViewportPoint(position));
                }
            }
            return (main && mainCamera) || others.Contains(true);
        }

        /// <summary>
        /// Check if the transform can be seen by at least one <seealso cref="Camera"/> from <paramref name="cameras"/> or by the <paramref name="mainCamera"/> if <see langword="true"/>.
        /// </summary>
        /// <param name="transform">Transform to check if can be seen.</param>
        /// <param name="mainCamera">Whenever it should check the <see cref="Camera.main"/> or not.</param>
        /// <param name="cameras">Additional cameras to check.</param>
        /// <returns>Whenever it's being seen by any of those cameras.</returns>
        public static bool IsVisibleToCamera(Transform transform, bool mainCamera, params Camera[] cameras) => IsVisibleToCamera(transform.position, mainCamera, cameras);

        /// <summary>
        /// Enable self destroy countdown.
        /// </summary>
        private void OnBecameInvisible()
        {
            destroying = true;
            timeSinceInvisible = 0f;
        }

        /// <summary>
        /// Disable self destroy countdown.
        /// </summary>
        private void OnBecameVisible()
        {
            destroying = false;
            timeSinceInvisible = 0f;
        }
    }
}