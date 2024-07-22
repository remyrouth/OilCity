using UnityEngine;

public class TestMoneySystem : MonoBehaviour {
    void Start() {
        MoneySystem.AddMoney(100);
        Debug.Log("+100$. Balance: " + MoneySystem.GetMoney());

        if (MoneySystem.BuyItem(50)) {
            Debug.Log("purchased for 50$. Balance: " + MoneySystem.GetMoney());
        } else {
            Debug.Log("Not enough $.");
        }

        if (MoneySystem.BuyItem(100)) {
            Debug.Log("Bought for 100$. Balance: " + MoneySystem.GetMoney());
        } else {
            Debug.Log("Not enough to buy for 100$.");
        }

        MoneySystem.AddMoney(200);
        Debug.Log("+200$. Balance: " + MoneySystem.GetMoney());
    }
}