using UnityEngine;
using UnityEngine.UI;

public class CoinMeter : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Text where money will be displayed")]
    public Text moneyText;

    private float showedMoney;
    private int oldMoney;
    private int realMoney;

    public int money {
        get {
            return realMoney;
        }
        set {
            oldMoney = (int)showedMoney;
            realMoney = value;
        }
    }

    public void ManualUpdate(int amount)
    {
        realMoney = oldMoney = amount;
        showedMoney = amount;
        moneyText.text = amount.ToString();
    }

    private void Update()
    { 
        if (showedMoney != realMoney)
        {
            showedMoney = Mathf.Clamp(showedMoney + (realMoney - oldMoney) * Time.deltaTime, Mathf.Min(realMoney, oldMoney), Mathf.Max(realMoney, oldMoney));
            moneyText.text = ((int)showedMoney).ToString();
        }    
    }
}
