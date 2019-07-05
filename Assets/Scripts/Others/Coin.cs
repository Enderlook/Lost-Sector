using UnityEngine;

public class Coin : MonoBehaviour, ICanBePickedUp
{
    private int price;
    private Transform player;
    private Animator animator;
    private Rigidbody2D thisRigidbody2D;

    Rigidbody2D ICanBePickedUp.Rigidbody2D => thisRigidbody2D;

    private void Start() => thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();

    /// <summary>
    /// Collect its money.
    /// </summary>
    void ICanBePickedUp.Pickup(LivingObject livingObject) => Global.money += price;

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

    private void OnValidate()
    {
        if (gameObject.GetComponent<Animator>() == null)
            Debug.LogWarning($"Game object {gameObject.name} lacks of an Animator Component.");
        if (gameObject.GetComponent<Rigidbody2D>() == null)
            Debug.LogWarning($"Game object {gameObject.name} lacks of an Rigidbody2D Component.");
    }
}