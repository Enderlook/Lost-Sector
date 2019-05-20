using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public static class Global {
    public static Transform enemiesParent;
    public static Transform explosionsParent;
}*/

public class Dynamic : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Enemies parent transform.")]
    public Transform enemiesParent;
    [Tooltip("Explosion parent transform.")]
    public Transform explosionsParent;

    [Header("Don't Touch")] // By this way, no one can touch it
    private static Dynamic _instance;
    /// <summary>
    /// Instance to get access of global variables.
    /// </summary>
    public static Dynamic Instance {
        get {
            return _instance;
        }
    }

    private void Awake()
    {
        // Kind of basic singlenton Pattern https://en.wikipedia.org/wiki/Singleton_pattern
        if (_instance != null)
        {
            Debug.LogError("More than one Dynamic class in scene!");
            return;
        }
        _instance = this;
        //StoreGlobal();
    }

    /*private void StoreGlobal()
    {
        Global.enemiesParent = enemiesParent;
        Global.explosionsParent = explosionsParent;
    }*/
}
