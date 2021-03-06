﻿using UnityEngine;

public class Coin : Pickupable
{
    private int price;
    private Animator animator;

    /// <summary>
    /// Collect its money.
    /// </summary>
    public override void Pickup(Player player)
    {
        Global.money += price;
        base.Pickup(player);
    }

    /// <summary>
    /// Set configuration of the coin.
    /// </summary>
    /// <param name="price">Amount of money this coin is worth.</param>
    /// <param name="animationState">Animation played by the coin.</param>
    public void SetConfiguration(int price, string animationState)
    {
        this.price = price;
        gameObject.GetComponent<Animator>().Play(animationState);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (gameObject.GetComponent<Animator>() == null)
            Debug.LogWarning($"Game object {gameObject.name} lacks of an Animator Component.");
        base.OnValidate();
    }
#endif
}