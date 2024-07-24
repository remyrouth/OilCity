using UnityEngine;
using System.Collections;

public class MoneyManager : Singleton<MoneyManager> {

    public int money;

    public float saveInterval;



    void Start() {
        AddMoney(PlayerPrefs.GetInt("MoneySave", 0));

        StartCoroutine("SaveMoney");
    }

    public IEnumerator SaveMoney() {
        while (true) {
            yield return new WaitForSeconds(saveInterval);
            PlayerPrefs.SetInt("MoneySave", money);
        }
    }

    public bool BuyItem(int cost) {
        if (money - cost >= 0) {
            money -= cost;
            return true;
        } else {
            return false;
        }
    }

    public int GetMoney() {
        return money;
    }

    public void AddMoney(int amount) {
        money += amount;
    }
}