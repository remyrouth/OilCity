using UnityEngine;

public class TestMoneySystem : MonoBehaviour {
    void Start() {
        MoneyManager.Instance.AddMoney(100);
        Debug.Log("+100$. Balance: " + MoneyManager.Instance.money);

        if (MoneyManager.Instance.BuyItem(50)) {
            Debug.Log("purchased for 50$. Balance: " + MoneyManager.Instance.money);
        } else {
            Debug.Log("Not enough $.");
        }

        if (MoneyManager.Instance.BuyItem(100)) {
            Debug.Log("Bought for 100$. Balance: " + MoneyManager.Instance.money);
        } else {
            Debug.Log("Not enough to buy for 100$.");
        }

        MoneyManager.Instance.AddMoney(200);
        Debug.Log("+200$. Balance: " + MoneyManager.Instance.money);
    }
}