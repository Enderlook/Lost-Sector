﻿using UnityEngine;
using System.Linq;
using LivingObjectAddons;
using System.Collections.Generic;

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

    [Tooltip("Relative damage on impact based on force.")]
    public float relativeImpactDamage;
    float IRigidbodyHelperConfiguration.ImpactDamage => relativeImpactDamage;

    bool IRigidbodyHelperConfiguration.IsImpactDamageRelativeToImpulse => true;

    [Tooltip("Should spawn floating damage text on the enemy on collision?")]
    public bool shouldDisplayDamage;
    bool IShouldDisplayDamage.ShouldDisplayDamage => shouldDisplayDamage;

    [Header("Setup")]
    [Tooltip("Impact sound.")]
    public Sound impactSound;
    Sound IRigidbodyHelperConfiguration.ImpactSound => impactSound;

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

    [Tooltip("Actions executed on initialize.")]
    public OnInitialize[] onInitializes;

    [Tooltip("Actions executed on death.")]
    public OnDeath[] onDeaths;

    [Tooltip("Type of movement.")]
    public Movement movement;

    private Quaternion? initialRotation = null;

    private IEnumerable<OnInitialize> toInitialize;

    private bool hasBeenBuilded = false;

    private void Build()
    /* We could have used Awake,
     * but in order to use that we would need to make Initialize public and call it from EnemySpawner through GetComponent.
     * That is because OnEnable is called before Awake.
     */
    {
        rigidbodyHelper.SetProperties(this);
        toInitialize = onInitializes.Append(movement);
        foreach (IBuild action in onInitializes.Concat(onDeaths.Cast<IBuild>()).Append(movement))
        {
            action?.OnBuild(this);
        }
    }

    protected virtual void Update()
    {
        healthPoints.Update(Time.deltaTime);
        movement?.Move();
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
        healthPoints.SetDie(Die);
        foreach (OnInitialize action in toInitialize)
        {
            action?.Initialize();
        }
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
    public void TakeHealing(float amount) => healthPoints.TakeHealing(amount);

    /// <summary>
    /// Take damage reducing its <see cref="Health"/>.
    /// </summary>
    /// <param name="amount">Amount of <see cref="Health"/> lost. Must be positive.</param>
    /// <param name="displayText">Whenever the damage taken must be shown in a floating text.</param>
    public virtual void TakeDamage(float amount, bool displayDamage = false)
    {
        healthPoints.TakeDamage(amount);
        if (displayDamage)
            SpawnFloatingText(amount, Color.Lerp(Color.red, new Color(1, .5f, 0), healthPoints.Ratio));
    }

    /// <summary>
    /// Destroy <see cref="gameObject"/> and spawn an explosion prefab instance on current location.
    /// </summary>
    protected virtual void Die()
    {
        GameObject explosion = Instantiate(onDeathExplosionPrefab, Global.explosionsParent);
        explosion.transform.position = rigidbodyHelper.Position;
        explosion.transform.localScale = Vector3.one * onDeathExplosionPrefabScale;
        gameObject.SetActive(false);
        foreach (OnDeath action in onDeaths)
        {
            action.Die();
        }
        //Destroy(gameObject);
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
}