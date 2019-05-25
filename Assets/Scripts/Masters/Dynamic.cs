using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global {
    public static Transform enemiesParent;
    public static Transform explosionsParent;
    public static Transform projectilesParent;
}

public class Dynamic : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Enemies parent transform.")]
    public Transform enemiesParent;
    [Tooltip("Explosion parent transform.")]
    public Transform explosionsParent;
    [Tooltip("Projectiles parent transform.")]
    public Transform projectilesParent;

    private void Awake()
    {
        // Workaround to avoid Singleton Pattern because no one likes it :(
        StoreGlobals();
    }

    private void StoreGlobals()
    {
        Global.enemiesParent = enemiesParent;
        Global.explosionsParent = explosionsParent;
        Global.projectilesParent = projectilesParent;
    }
}
