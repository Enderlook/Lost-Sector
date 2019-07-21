using System.Linq;
using UnityEngine;

namespace LivingObjectAddons
{
    public class DropOnDeath : OnDeath
    {
        [Header("Configuration")]
        [Tooltip("Configuration of gameObjects to spawn on death.")]
        public SpawneableGameObjects spawnsOnDeath;

        private void Awake() => spawnsOnDeath.SetSpawnedInstructions(SpawnedInstructions);
        public override void OnBuild(LivingObject livingObject) { }
        public override void Die() => spawnsOnDeath.SpawnPrefabs(Instantiate);

        /// <summary>
        ///  Set additional spawn instructions used when a prefab is instantiated.
        /// </summary>
        /// <param name="spawned"><seealso cref="GameObject"/> recently spawned.</param>
        /// <param name="spawningTransform"><seealso cref="Transform"/> point where it's being spawned</param>
        protected virtual void SpawnedInstructions(GameObject spawned, Transform spawningTransform) { }
    }

    [System.Serializable]
    public class SpawneableGameObjects
    {
        [Tooltip("Spawning points for prefabs.")]
        public SpawningPoint[] spawningPoints;
        [Tooltip("Prefabs types able spawn.")]
        public SpawneableGameObject[] prefabsToSpawn;
        [Tooltip("Amount of prefabs to spawn. Decimal numbers will be used as a chance of spawning.")]
        public FloatRangeTwo amountToSpawn;
        [Tooltip("If false, it will be spawn a number of game objects equal to Amount To Spawn value, randomly distributed by chance. On true, Amount To Spawn will be ignored and weight will be used to determine the amount of each prefab (instead of their chance), non whole amounts will their decimal part be treated as chance of increasing by one the amount spawned.")]
        public bool notRandom;

        /// <summary>
        /// Instructions executed on a newly instantiate prefab.
        /// </summary>
        private System.Action<GameObject, Transform> SpawningInstructions;

        /// <summary>
        /// Get a random prefab from the <see cref="prefabsToSpawn"/>.
        /// </summary>
        /// <returns>Random prefab to spawn.</returns>
        private GameObject GetRandomPrefab()
        {
            float totalWeight = prefabsToSpawn.Sum(prefabs => prefabs.weight);
            float chosenWeight = Random.value * totalWeight;

            float currentWeight = 0;
            foreach (SpawneableGameObject prefab in prefabsToSpawn)
            {
                currentWeight += prefab.weight;
                if (currentWeight >= chosenWeight)
                {
                    return prefab.prefab;
                }
            }
            throw new System.Exception("This shouldn't be happening!!!");
        }

        /// <summary>
        /// Set additional spawn instructions used when a prefab is instantiated.
        /// </summary>
        /// <param name="SpawningInstructions"></param>
        public void SetSpawnedInstructions(System.Action<GameObject, Transform> SpawningInstructions) => this.SpawningInstructions = SpawningInstructions;

        /// <summary>
        /// Spawn the defined game objects.
        /// </summary>
        /// <param name="Instantiate">Instantiate UnityEngine method.</param>
        public void SpawnPrefabs(System.Func<GameObject, Transform, GameObject> Instantiate)
        {
            void Spawn(GameObject gameObjectToSpawn)
            {
                GameObject spawned = Instantiate(gameObjectToSpawn, Global.enemiesParent);
                Transform spawningTransform = (Transform)spawningPoints[Random.Range(0, spawningPoints.Length - 1)];
                Transform spawnedTransform = spawned.transform;
                spawnedTransform.position = spawningTransform.position;
                spawnedTransform.rotation = spawningTransform.rotation;
                SpawningInstructions?.Invoke(spawned, spawningTransform);
            }

            if (notRandom)
            {
                foreach (SpawneableGameObject spawneableGameObject in prefabsToSpawn)
                {
                    int amount = FloatRangeTwo.FloatToIntByChance(spawneableGameObject.weight);
                    for (int i = 0; i < amount; i++)
                    {
                        Spawn(spawneableGameObject.prefab);
                    }
                }
            }
            else
            {
                int total = (int)amountToSpawn;
                for (int i = 0; i < total; i++)
                {
                    Spawn(GetRandomPrefab());
                }
            }
        }
    }

    /// <summary>
    /// Stores an spawneable game object and its weight.
    /// </summary>
    [System.Serializable]
    public struct SpawneableGameObject
    {
        [Tooltip("GameObject prefab.")]
        public GameObject prefab;

        [Tooltip("Weight rarity.")]
        public float weight;
    }

    [System.Serializable]
    public class SpawningPoint
    {
        [Tooltip("Spawning point.")]
        public Transform spawnPoint;

        [Tooltip("Random distance from point.")]
        public FloatRangeTwo distance;

        /// <summary>
        /// Return a random <seealso cref="SpawningPoint"/> within a random circle centered <c><paramref name="x"/>.spawnPoint</c> with a random radius of <c><paramref name="x"/>.distance</c>.
        /// </summary>
        /// <param name="x"><see cref="SpawningPoint"/> instance used to determine the random <seealso cref="Vector3"/>.</param>
        public static explicit operator Transform(SpawningPoint x)
        {
            float distance = (float)x.distance;
            Transform point = x.spawnPoint;
            if (distance > 0)
                point.position += (Vector3)Random.insideUnitCircle * distance;
            return point;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.DrawWireDisc(spawnPoint.position, Vector3.back, distance.Min);
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(spawnPoint.position, Vector3.back, distance.Max);
        }
#endif
    }
}
