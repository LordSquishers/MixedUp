using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public static float collabMoney;
    void Start() {
    }

    void Update() {
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player") {
            Deselect();
        }
    }

    public void Select() {
        if(gameObject.GetComponent<Renderer>() == null) return;
        Material[] mats = gameObject.GetComponent<Renderer>().materials;
        foreach(Material mat in mats) {
            mat.SetInt("Bool_TintSelected", 1);
        }
    }

    public void Deselect() {
        if(gameObject.GetComponent<Renderer>() == null) return;
        Material[] mats = gameObject.GetComponent<Renderer>().materials;
        foreach(Material mat in mats) {
            mat.SetInt("Bool_TintSelected", 0);
        }
    }

    // Sell item in market
    public bool SellInMarket(Player player, ItemStack item)
    {
        if(item.amount == 0) return false;
        float sale = item.amount * item.marketValue;
        GameObject.Find("UI ELEMENTS").GetComponent<GameManagement>().AddIndividualMoney(player, sale);
        GameObject.Find("UI ELEMENTS").FindInChildren("MoneyPopups").GetComponent<MoneyPopupController>().CreatePopup(transform.position + Vector3.up * 2f, sale);
        return true;
    }

    // Attempt to sell ingredients in a bakery
    public void SellInBakery(ItemStack item)
    {   
        if (item.amount != 0)
        {
            float sale = item.marketValue;
            GameObject.Find("UI ELEMENTS").GetComponent<GameManagement>().AddCollabMoney(sale);
            GameObject.Find("UI ELEMENTS").GetComponent<GameManagement>().DistributeMoney(GameObject.Find("Player 1").GetComponent<Player>(), GameObject.Find("Player 2").GetComponent<Player>(),sale);
            GameObject.Find("UI ELEMENTS").FindInChildren("MoneyPopups").GetComponent<MoneyPopupController>().CreatePopup(transform.position + Vector3.up * 2f, sale);
            Debug.Log("Sale: " + sale);
            collabMoney += sale;
        }
    }

}
