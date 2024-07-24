using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoneyManager : Singleton<MoneyManager> {

    public float money { get; private set; }
    public float minMoneyAmount => 0;

    public float saveInterval;
    
    public float startingMoneyAmount = 100f;

    [SerializeField]
    private Text moneyText; 

    void Start() {
        money = startingMoneyAmount;
        UpdateMoneyUI();

        float savedMoney = PlayerPrefs.GetFloat("MoneySave", money);
        AddMoney(savedMoney - money);

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
        UpdateMoneyUI();
    }
    public void ReduceMoney(float amount)
    {
        money = Mathf.Round(money * 100f) / 100f - Mathf.Round(amount * 100f) / 100f;
        if (money < minMoneyAmount)
            gameOver();
            UpdateMoneyUI();
    }
    public void gameOver()
    {
        Debug.Log("You lost...");
    }

    private void UpdateMoneyUI() {
        if (moneyText != null) {
            moneyText.text = $"Money: {money.ToString("F2)}";
        }
    }
}