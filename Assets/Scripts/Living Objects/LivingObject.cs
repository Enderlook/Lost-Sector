using System;
using Effects;
using LivingObjectAddons;
using UnityEngine;

/* https://forum.unity.com/threads/make-child-unaffected-by-parents-rotation.461161/
 * https://stackoverflow.com/questions/52179975/make-child-unaffected-by-parents-rotation-unity
 * https://docs.unity3d.com/Manual/class-ParentConstraint.html
 */

public class LivingObject : MonoBehaviour, IRigidbodyHelperConfiguration
{
    // TODO: https://forum.unity.com/threads/exposing-fields-with-interface-type-c-solved.49524/

    [Header("Configuration")]
    [Tooltip("Health.")]
    public HealthPoints healthPoints;

    [Tooltip("Die sound.")]
    public Sound dieSound;
    [Tooltip("Instantiate this explosion prefab on death.")]
    public GameObject onDeathExplosionPrefab;
    [Tooltip("Scale of the explosion prefab.")]
    [Range(0, 100)]
    public float onDeathExplosionPrefabScale = 1;

    [Tooltip("RigidbodyHelper script.")]
    public RigidbodyHelper rigidbodyHelper;

    [Tooltip("FloatingTextController Script")]
    public FloatingTextController floatingTextController;

    [Tooltip("Effect displayer.")]
    public EffectsDisplayer effectsDisplayer;

    private Quaternion? initialRotation = null;

    private IInitialize[] initializes;
    private IDie[] dies;
    private LivingObjectAddons.IUpdate[] updates;
    private IMove move;
    [HideInInspector]
    public Weapon[] weapons;
    private IMelee melee;

    IMelee IRigidbodyHelperConfiguration.Melee => melee;

    private bool hasBeenBuilded = false;
    private EffectManager effectManager;

    protected bool isDead = false;

    public float SpeedMultiplier {
        get => rigidbodyHelper.SpeedMultiplier;
        set => rigidbodyHelper.SpeedMultiplier = value;
    }


    private void Build()
    /* We could have used Awake,
     * but in order to use that we would need to make Initialize public and call it from EnemySpawner through GetComponent.
     * That is because OnEnable is called before Awake.
     */
    {
        rigidbodyHelper.SetProperties(this);
        effectManager = new EffectManager(this);
        Array.ForEach(gameObject.GetComponents<IBuild>(), e => e.Build(this));
        LoadComponents();
        healthPoints.SetDie(Die);
    }

    private void LoadComponents()
    {
        initializes = gameObject.GetComponentsInChildren<IInitialize>();
        dies = gameObject.GetComponentsInChildren<IDie>();
        updates = gameObject.GetComponentsInChildren<LivingObjectAddons.IUpdate>();
        weapons = gameObject.GetComponentsInChildren<Weapon>();
        move = gameObject.GetComponentInChildren<IMove>();
        melee = gameObject.GetComponentInChildren<IMelee>();
    }

    protected virtual void Update()
    {
        if (!Global.menu.ShouldWork)
            return;
        if (isDead)
        {
            gameObject.SetActive(false);
            return;
        }
        healthPoints.Update(Time.deltaTime);
        move?.Move(SpeedMultiplier);
        effectManager.Update(Time.deltaTime);
        Array.ForEach(weapons, e => e.Recharge(Time.deltaTime));
        Array.ForEach(updates, e => e.Update());
        effectsDisplayer?.CheckEffects(effectManager.effects);
    }

    /// <summary>
    /// Initializes some values that are reused during the enemies recycling.
    /// </summary>
    protected virtual void Initialize()
    {
        rigidbodyHelper.transform.position = transform.position;
        if (initialRotation == null)
            initialRotation = rigidbodyHelper.transform.rotation;
        else
            rigidbodyHelper.transform.rotation = (Quaternion)initialRotation;
        healthPoints.Initialize();
        Array.ForEach(initializes, e => e.Initialize());
        isDead = false;
    }

    private void OnEnable()
    {
        if (!hasBeenBuilded)
        {
            hasBeenBuilded = true;
            Build();
        }
        Initialize();
    }

    /// <summary>
    /// Takes healing increasing its <see cref="Health"/>.
    /// </summary>
    /// <param name="amount">Amount of <see cref="Health"/> recovered. Must be positive.</param>
    public void TakeHealing(float amount)
    {
        if (isDead) return;
        healthPoints.TakeHealing(amount);
    }

    /// <summary>
    /// Take damage reducing its <see cref="Health"/>.
    /// </summary>
    /// <param name="amount">Amount of <see cref="Health"/> lost. Must be positive.</param>
    /// <param name="displayText">Whenever the damage taken must be shown in a floating text.</param>
    public virtual void TakeDamage(float amount, bool displayDamage = false)
    {
        if (isDead) return;
        healthPoints.TakeDamage(amount);
        if (displayDamage)
            SpawnFloatingText(amount, Color.Lerp(Color.red, new Color(1, .5f, 0), healthPoints.Ratio));
    }

    /// <summary>
    /// Destroy <see cref="gameObject"/> and spawn an explosion prefab instance on current location.
    /// </summary>
    /// <param name="suicide"><see langword="true"/> if it was a suicide. <see langword="false"/> if it was murderer.</param>
    public virtual void Die(bool suicide = false)
    {
        if (isDead) return;
        isDead = true;
        dieSound.PlayAtPoint(rigidbodyHelper.transform.position, Settings.IsSoundActive && DoWhenInvisible.IsVisibleToCamera(transform, true));
        GameObject explosion = Global.enemySpawner.Spawn(onDeathExplosionPrefab, Global.explosionsParent);
        explosion.transform.position = rigidbodyHelper.transform.position;
        explosion.transform.localScale = Vector3.one * onDeathExplosionPrefabScale;
        Array.ForEach(dies, e => e.Die(suicide));
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Spawn a floating text above the creature.
    /// </summary>
    /// <param name="text">Text to display.</param>
    /// <param name="textColor">Color of the text.</param>
    /// <param name="checkIfPositive">Only display if the number is positive.</param>
    protected void SpawnFloatingText(float text, Color? textColor, bool checkIfPositive = true)
    {
        if (floatingTextController != null && (!checkIfPositive || text > 0))
            floatingTextController.SpawnFloatingText(text, textColor);
    }

    /// <summary>
    /// Add effect to this creature.
    /// </summary>
    /// <param name="effect">Effect to add.</param>
    public void AddEffect(Effect effect) => effectManager.AddEffect(effect);
}