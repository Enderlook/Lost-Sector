using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingObject
{
    [Header("Configurable")]
    [Tooltip("Movement speed.")]
    public float moveSpeed;
    /*public float turnSpeed;
    public float rotationOffset;*/

    [Tooltip("Maximum shield.")]
    public float startingMaxShield = 100;
    private float _maxShield;
    protected float MaxShield {
        get {
            return _maxShield;
        }
        set {
            _maxShield = value;
            shieldBar.UpdateValues(Shield, MaxShield);
        }
    }
    [Tooltip("Starting shield. Set -1 to use Max shield value.")]
    public float startingShield = -1;
    private float _shield;
    protected float Shield {
        get {
            return _shield;
        }
        set {
            _shield = value;
            shieldBar.UpdateValues(Shield, MaxShield);
        }
    }

    [Tooltip("Shield recharge rate (points per second).")]
    public float shieldRechargeRate = 10;
    [Tooltip("Amount of time in seconds after receive damage in order to start recharging shield.")]
    public float shieldRechargeDelay = 3f;
    private bool isRechargingShield = false;
    //private Coroutine shieldRechargeCoroutine;

    [Header("Build")]
    [Tooltip("Shield bar script.")]
    public HealthBar shieldBar;

    protected override void Start()
    {        
        /*float shield = startingShield == -1 ? startingMaxShield : startingShield;
        shieldBar.ManualUpdate(startingMaxHealth, shield);*/
        Shield = InitializeBar(shieldBar, startingMaxShield, startingShield); ;
        MaxShield = startingMaxShield;

        base.Start();
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        /*// Rotate https://answers.unity.com/questions/798707/2d-look-at-mouse-position-z-rotation-c.html
        Vector3 direction = mousePosition - transform.position;
        float angleTarget = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;       
        Quaternion lookAt = Quaternion.Euler(0, 0, angleTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, turnSpeed * Time.deltaTime);*/

        // Move
        transform.position = Vector3.MoveTowards(transform.position, mousePosition, moveSpeed * Time.deltaTime);

        // Recharge shield
        if (isRechargingShield && Shield < MaxShield)
            Shield = ChangeValue(shieldRechargeRate * Time.deltaTime, Shield, MaxShield, true, "shield");
        /*// To avoid over-shield
        if (Shield + shieldRechargeRate * Time.deltaTime < MaxShield)
            Shield += shieldRechargeRate * Time.deltaTime;
        else
            Shield = MaxShield;*/

        Debug.Log(Shield);
    }

    public override void TakeDamage(float amount)
    {
        StopCoroutine(InitializeRechargeShieldDelay());
        /*shieldRechargeCoroutine = */StartCoroutine(InitializeRechargeShieldDelay());
        (float value, float rest) = ChangeValueWithRemain(amount, Shield, MaxShield, false, "shield");
        Shield = value;
        if (rest != 0)
            Health = ChangeValue(rest, Health, MaxHealth, false, "health");



        /*if (Shield - amount < 0)
        {
            Shield = 0;
            float remainingDamage = amount - Shield;
            base.TakeDamage(remainingDamage);
        } else
        {
            Shield -= amount;
        }*/

        /*
        // Damage reduction on low health
        if (Health - amount < MaxHealth * 0.35f)
            amount = (Health - amount) + (amount - Health) * (10 + Mathf.Exp(1) / Mathf.Exp(-amount / 10));
        base.TakeDamage(amount);*/
    }

    protected override void Die()
    {
        GameObject explosion = Instantiate(onDeathExplosionPrefab, Dynamic.Instance.explosionsParent);
        explosion.transform.position = transform.position;
        base.Die();
    }

    private IEnumerator InitializeRechargeShieldDelay()
    {
        isRechargingShield = false;
        yield return new WaitForSeconds(shieldRechargeDelay);
        isRechargingShield = true;
    }
}
