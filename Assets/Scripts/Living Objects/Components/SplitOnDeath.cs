using UnityEngine;

namespace LivingObjectAddons
{
    public class SplitOnDeath : DropOnDeath
    {
        protected override void SpawnedInstructions(GameObject spawned, Transform spawningTransform)
        {
            spawned.transform.parent = Global.enemiesParent;
            RigidbodyHelper spawnedRigidbodyHelper = spawned.GetComponent<Enemy>().rigidbodyHelper; // Or spawned.GetComponentInChildren<RigidbodyHelper>(); ?
            spawnedRigidbodyHelper.transform.position = spawningTransform.position;
            spawnedRigidbodyHelper.transform.transform.rotation = spawningTransform.rotation;
            // To make them a bit slower
            spawned.GetComponent<Enemy>().impulse *= 0.4f;
        }
    }
}