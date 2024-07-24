using UnityEngine;
using System.Collections;

public class MoneyManager : Singleton<MoneyManager> {

    public float money { get; private set; }
    public float minMoneyAmount => 0;

    public float saveInterval;

    void Start() {
        AddMoney(PlayerPrefs.GetFloat("MoneySave", 0));

        StartCoroutine("SaveMoney");
    }

    public IEnumerator SaveMoney() {
        while (true) {
            yield return new WaitForSeconds(saveInterval);
            PlayerPrefs.SetFloat("MoneySave", Mathf.Round(money*100f)/100f);
        }
    }

    public bool BuyItem(float cost) {
        if (money - cost >= 0) {
            ReduceMoney(cost);
            return true;
        } else {
            return false;
        }
    }
    public void AddMoney(float amount) {
        money = Mathf.Round(money * 100f) / 100f + Mathf.Round(amount * 100f) / 100f;
    }
    public void ReduceMoney(float amount)
    {
        money = Mathf.Round(money * 100f) / 100f - Mathf.Round(amount * 100f) / 100f;
        if (money < minMoneyAmount)
            gameOver();
    }
    public void gameOver()
    {
        Debug.Log("You lost...");
    }
}