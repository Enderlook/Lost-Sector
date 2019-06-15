using UnityEngine;

public class Coin : MonoBehaviour, IPickup
{    
    private int price;
    private Transform player;
    private Rigidbody2D thisRigidbody2D;
    private Animator animator;
    private void Start() => thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();

    /// <summary>
    /// Destroy this coin and collect its money.
    /// </summary>
    void IPickup.Pickup()
    {
        Global.money += price;
        Destroy(gameObject);
    }

    Rigidbody2D IPickup.Rigidbody2D {
        get {
            return thisRigidbody2D;
        }
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

    private void OnValidate()
    {
        if (gameObject.GetComponent<Animator>() == null)
            Debug.LogWarning($"Game object {gameObject.name} lacks of an Animator Component.");
        if (gameObject.GetComponent<Rigidbody2D>() == null)
            Debug.LogWarning($"Game object {gameObject.name} lacks of an Rigidbody2D Component.");
    }
}

// We may have more pickups in the future
public interface IPickup
{
    /// <summary>
    /// Method executed when the object must be picked up.
    /// </summary>
    void Pickup();
    /// <summary>
    /// <seealso cref="Rigidbody2D"/> of this pickup.
    /// </summary>
    Rigidbody2D Rigidbody2D { get; } 
}