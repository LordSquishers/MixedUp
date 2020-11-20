using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    public int ID;

    [Header("Inventory")]
    public ItemStack itemHeld;
    public GameObject objectInHand;
    public Vector3 itemOffset, itemRotation;
    public float itemScale;
    public AudioClip pickUpSound, dropSound, tableSound;
    private bool isItemInHand, marketInRange, plotInRange, itemInRange, mixerInRange, tableInRange;

    private PlotController plotController;
    private Merchant market;
    private Mixer mixer;
    private Table table;
    private Item itemInWorld;

    // Collision for interact key
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Plot")
        {
            plotController = other.gameObject.GetComponent<PlotController>();
            plotInRange = true;
            plotController.Select();
        }

        if (other.gameObject.tag == "Merchant")
        {
            market = other.gameObject.GetComponent<Merchant>();
            marketInRange = true;
            market.Select();
        }

        if (other.gameObject.tag == "Mixer")
        {
            mixer = other.gameObject.GetComponent<Mixer>();
            mixerInRange = true;
            mixer.Select();
        }
        if (other.gameObject.tag == "Table" && (other.gameObject.transform.childCount == 1 || 
        (other.gameObject.transform.childCount > 1 && (!ContainsChildWithTag(other.gameObject, "Merchant") || !ContainsChildWithTag(other.gameObject, "Mixer")))))
        {
            table = other.gameObject.GetComponent<Table>();
            tableInRange = true;
            table.Select();
        }
        if (other.gameObject.tag == "Item" && other.gameObject.transform.parent == null)
        {
            itemInWorld = other.gameObject.GetComponent<Item>();
            itemInRange = true;
            itemInWorld.Select();
        }
    }

    bool ContainsChildWithTag(GameObject go, string tag) {
        foreach(Transform child in go.transform) {
            // UnityEngine.Debug.Log(child.gameObject.tag);
            if(child.gameObject.tag == tag) return true;
        }
        return false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Plot")
        {
            plotInRange = false;
            plotController.Deselect();
        }

        else if (other.gameObject.tag == "Merchant")
        {
            marketInRange = false;
            market.Deselect();
        }

        else if (other.gameObject.tag == "Mixer")
        {
            mixerInRange = false;
            mixer.Deselect();
        }
        else if (other.gameObject.tag == "Table")
        {
            tableInRange = false;
            table.gameObject.GetComponent<Table>().Deselect();
        }
        else if (other.gameObject.tag == "Item" && other.gameObject.transform.parent == null)
        {
            itemInRange = false;
            itemInWorld.Deselect();
        }
    }

    public ItemStack PickDrop()
    {
        GetComponents<AudioSource>()[1].volume = 0.25f;
        // ITEM IN WORLD CODE
        if (itemInRange)
        {
            ItemStack stackInWorld = itemInWorld.GetItemStack();
            // pickup item from world (despawn item + set in player)
            if(itemHeld.amount == 0) { // not holding anything!
                isItemInHand = true;
                itemInRange = false;
                itemHeld = new ItemStack(stackInWorld);
                Destroy(itemInWorld.gameObject);
            } else if(itemHeld.amount != 0 && itemHeld.itemID == stackInWorld.itemID && (itemHeld.amount + stackInWorld.amount <= 3)) { // items stack!
                // if the stack size > 3 then it will swap stacks which is the same as maxxing the player's hand.
                itemInRange = false;
                itemHeld.amount += stackInWorld.amount;

                Destroy(itemInWorld.gameObject);
            } else if(itemHeld.amount != 0) { // items don't stack -- swap!
                Transform oldTransform = itemInWorld.gameObject.transform;
                var newItem = Instantiate<GameObject>(Character.LoadPrefabItem(itemHeld.itemID), oldTransform.position + Vector3.up, oldTransform.rotation);
                newItem.GetComponent<Rigidbody>().isKinematic = false;
                newItem.GetComponent<Item>().GetItemStack().amount = itemHeld.amount;
                itemHeld = new ItemStack(stackInWorld);
                Destroy(itemInWorld.gameObject);
            }

            PlaySound(pickUpSound);
        }
        // TABLE CODE
        else if (tableInRange && table.isItemPresent())
        {
            ItemStack itemInTable = table.TakeItem();
            if (itemHeld.amount != 0 && itemInTable.amount != 0 && (itemHeld.amount + itemInTable.amount <= 3) && itemInTable.itemID == itemHeld.itemID) // items stack
            {
                itemHeld.amount += itemInTable.amount;
                isItemInHand = true;

                // UnityEngine.Debug.Log("[PLAYER] Added item from table");
                PlaySound(tableSound);
            }
            else if (itemHeld.amount != 0 && itemInTable.amount != 0) // items dont stack
            {
                table.InsertItem(itemHeld);
                itemHeld = new ItemStack(itemInTable);
                isItemInHand = true;

                // UnityEngine.Debug.Log("[PLAYER] Swapped item from table");
                PlaySound(tableSound);
            }
            else if (itemHeld.amount == 0 && itemInTable.amount != 0) // take items
            {
                itemHeld = new ItemStack(itemInTable);
                isItemInHand = true;

                // UnityEngine.Debug.Log("[PLAYER] Taken item from table: " + itemHeld.itemID);
                PlaySound(tableSound);
            }
        }
        else if (tableInRange && !table.isItemPresent() && table.isEmpty())
        {
            if (itemHeld.amount != 0) // give to table
            {
                isItemInHand = false;
                table.InsertItem(itemHeld);
                itemHeld.amount = 0;

                // UnityEngine.Debug.Log("[PLAYER] Given item to table");
                PlaySound(tableSound);
            }
        }
        // PLOT CODE
        else if (plotInRange && plotController.GetGrowthStatus() == PlotController.PlotState.Complete)
        {
            if(isItemInHand) {
                if(itemHeld.itemID == plotController.GetItemID()) { // combine stacks, otherwise don't do anything unless empty
                    ItemStack harvest = plotController.HarvestPlot();
                    if(harvest.amount + itemHeld.amount > 3) {
                        itemHeld.amount = 3;
                        var refer = Character.LoadPrefabItem(harvest.itemID);
                        var newItem = Instantiate<GameObject>(refer, objectInHand.transform.position, transform.rotation * refer.transform.rotation);
                        newItem.GetComponent<Rigidbody>().isKinematic = false;
                        newItem.GetComponent<Item>().GetItemStack().amount = harvest.amount + itemHeld.amount - 3;
                    } else {
                        itemHeld.amount += harvest.amount;
                    }
                    plotController.StartGrowing();
                }
                // no code here! this is only if the item being held is not stackable.
            } else { // yoink
                itemHeld = plotController.HarvestPlot();
                plotController.StartGrowing();
                isItemInHand = true;
            }
        }
        // MIXER CODE
        else if (mixerInRange)
        {
            if(isItemInHand && !mixer.isRunning) {
                mixer.DepositItem(itemHeld);
                itemHeld.amount--;
                if (itemHeld.amount < 1)
                {
                    isItemInHand = false;
                    itemHeld.amount = 0;
                }
                PlaySound(dropSound);
            } else {
                // MIXER RETRIEVAL CODE
                ItemStack itemFromMixer = mixer.TakeLastItem();
                if (itemFromMixer.amount > 0)
                {
                    isItemInHand = true;
                    itemHeld = itemFromMixer;
                    PlaySound(pickUpSound);
                }
            }
        }
        else if(isItemInHand && !plotInRange)
        {
            // DROP CODE
            var refer = Character.LoadPrefabItem(itemHeld.itemID);
            var newItem = Instantiate<GameObject>(refer, objectInHand.transform.position + itemOffset, transform.rotation * refer.transform.rotation * Quaternion.Euler(itemRotation));
            newItem.GetComponent<Rigidbody>().isKinematic = false;
            foreach(BoxCollider coll in newItem.GetComponents<BoxCollider>()) {
                coll.enabled = true;
            }
            newItem.GetComponent<Item>().GetItemStack().amount = itemHeld.amount;
            itemHeld.amount = 0;
            isItemInHand = false;

            PlaySound(dropSound);
        }


        return itemHeld; // just in case (for info)
    }

    void PlaySound(AudioClip sound)
    {
        // Sound
        GetComponents<AudioSource>()[1].pitch = Random.Range(0.9f, 1.15f);
        GetComponents<AudioSource>()[1].PlayOneShot(sound);
    }

    public void Activate() // for activate button
    {
        if(mixerInRange) {
            // UnityEngine.Debug.Log("Activate");
            if(!mixer.Activate())
                GameObject.Find("UI ELEMENTS").FindInChildren("MoneyPopups")
                .GetComponent<MoneyPopupController>().CreatePopup(mixer.gameObject.transform.position + Vector3.up * 2f, "X", new Color(234f / 255f, 35f / 255f, 35f / 255f, 1));
        }
        else if(marketInRange) {
            // UnityEngine.Debug.Log("Activate");
            if(market.SellInMarket(this, itemHeld)) {
                isItemInHand = false;
                itemHeld.amount = 0;
            } else {
                GameObject.Find("UI ELEMENTS").FindInChildren("MoneyPopups")
                .GetComponent<MoneyPopupController>().CreatePopup(market.gameObject.transform.position + Vector3.up * 2f, "X", new Color(234f / 255f, 35f / 255f, 35f / 255f, 1));
            }
        }
        else if(plotInRange) {
            if(isItemInHand) {
                plotController.ClearPlot();
                plotController.SetItem(itemHeld);
                plotController.StartGrowing();
                if(--itemHeld.amount < 1) {
                    isItemInHand = false;
                    itemHeld.amount = 0; // just in case
                }
            } else {
                GameObject.Find("UI ELEMENTS").FindInChildren("MoneyPopups")
                .GetComponent<MoneyPopupController>().CreatePopup(plotController.gameObject.transform.position + Vector3.up * 2f, "X", new Color(234f / 255f, 35f / 255f, 35f / 255f, 1));
            }
        }
    }

}
