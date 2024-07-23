using UnityEngine;
using System.Collections;

public class MoneySystem : MonoBehaviour {
    private static MoneySystem _instance;

    public int money;

    public float saveInterval;

    private static MoneySystem instance {
        get {
            if (_instance == null) {
                if (GameObject.Find("MoneySystem")) {
                    GameObject g = GameObject.Find("MoneySystem");
                    if (g.GetComponent<MoneySystem>()) {
                        _instance = g.GetComponent<MoneySystem>();
                    } else {
                        _instance = g.AddComponent<MoneySystem>();
                    }
                } else {
                    GameObject g = new GameObject();
                    g.name = "MoneySystem";
                    _instance = g.AddComponent<MoneySystem>();
                }
            }
            return _instance;
        }

        set {
            _instance = value;
        }
    }

    void Start() {
        gameObject.name = "MoneySystem";

        _instance = this;

        AddMoney(PlayerPrefs.GetInt("MoneySave", 0));

        StartCoroutine("SaveMoney");
    }

    public IEnumerator SaveMoney() {
        while (true) {
            yield return new WaitForSeconds(saveInterval);
            PlayerPrefs.SetInt("MoneySave", instance.money);
        }
    }

    public static bool BuyItem(int cost) {
        if (instance.money - cost >= 0) {
            instance.money -= cost;
            return true;
        } else {
            return false;
        }
    }

    public static int GetMoney() {
        return instance.money;
    }

    public static void AddMoney(int amount) {
        instance.money += amount;
    }
}