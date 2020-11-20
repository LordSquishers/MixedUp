using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject reference;
    [HideInInspector]
    public bool inert;

    [SerializeField]
    private ItemStack itemStack;
    private int oldAmount = 0;

    public Item(ItemStack stack)
    {
        this.itemStack = stack;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Select() {
        SetTintShader(gameObject, 1);
        foreach(Transform child in gameObject.transform) {
            SetTintShader(child.gameObject, 1);
        }
    }

    public void Deselect() {
        SetTintShader(gameObject, 0);
        foreach(Transform child in gameObject.transform) {
            SetTintShader(child.gameObject, 0);
        }
    }

    public static void SetTintShader(GameObject go, int value) {
        Material[] mats = go.GetComponent<Renderer>().materials;
        foreach(Material mat in mats) {
            mat.SetInt("Bool_TintSelected", value);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player") {
            Deselect();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(inert) return;
        if (itemStack.amount == 0)
        { // don't render anything, but keep the object
            gameObject.GetComponent<Renderer>().enabled = false;
            foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
            {
                r.enabled = false;
            }
            DestroyItemChildren(gameObject);
        }
        else if (oldAmount != itemStack.amount) DisplayQuantity(itemStack.amount);

        oldAmount = itemStack.amount;
    }

    void DisplayQuantity(int amount)
    { // this method should run once.
        // Don't want to forget to turn on the renderer.
        if (!gameObject.GetComponent<Renderer>().enabled && amount != 0)
        {
            foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
            {
                r.enabled = true;
            }

            gameObject.GetComponent<Renderer>().enabled = true;
        }

        if (amount == 1)
        { // destroy children
            // UnityEngine.Debug.Log(NumberOfItemChildren(gameObject));
            DestroyItemChildren(gameObject);
        }
        else if (amount == 2)
        { // create second carrot and move it a bit (and delete rigidbody)
            if (NumberOfItemChildren(gameObject) < 1)
            {
                GameObject secondItem = Instantiate<GameObject>(reference, transform.position + (Vector3.right / 2), transform.rotation, transform);
                secondItem.transform.localScale = Vector3.one;
                Vector3 lpos = secondItem.transform.localPosition;
            }
            else if (NumberOfItemChildren(gameObject) > 1)
            {
                DestroyItemChildren(gameObject);
            }
        }
        else if (amount == 3)
        { // create 2 and 3
            if (NumberOfItemChildren(gameObject) < 2)
            {
                GameObject secondItem = Instantiate<GameObject>(reference, transform.position + new Vector3(0.5f, 0f, 0f), transform.rotation, transform);
                secondItem.transform.localScale = Vector3.one;

                GameObject thirdItem = Instantiate<GameObject>(reference, transform.position + new Vector3(0.25f, 0.5f, 0f), transform.rotation, transform);
                thirdItem.transform.localScale = Vector3.one;
            }
            else if (NumberOfItemChildren(gameObject) > 2)
            {
                DestroyItemChildren(gameObject);
            }
        }
    }

    int NumberOfItemChildren(GameObject go) {
        int total = 0;
        foreach(Transform child in go.transform) {
            NumberOfItemChildren(child.gameObject);
            if(!ExemptChildrenExist(child.gameObject)) total++;
        }
        return total;
    }

    public Item DestroyItemChildren(GameObject go) {
        foreach(Transform child in go.transform) {
            DestroyItemChildren(child.gameObject);
            if(!ExemptChildrenExist(child.gameObject)) Destroy(child.gameObject);
            // UnityEngine.Debug.Log("Deleted " + child.gameObject.name);
        }
        return this;
    }

    bool ExemptChildrenExist(GameObject obj) {
        return obj.name.Contains("cleaf") || obj.name.Contains("Cylinder") || obj.name.Contains("Icosphere");
    }

    public ItemStack GetItemStack() {
        return itemStack;
    }

    public void SetItemStack(ItemStack stack) {
        itemStack = stack;
    }

}

[System.Serializable]
public class ItemStack {
    public string itemID, displayName;
    public float marketValue;
   
    public int amount;

    public ItemStack(string itemID, string displayName, float marketValue, int amount) {
        this.itemID = itemID;
   
        this.displayName = displayName;
        this.marketValue = marketValue;
        this.amount = Mathf.Min(amount, 3);
    }

    public ItemStack(ItemStack oldStack) {
        this.itemID = oldStack.itemID;
        this.displayName = oldStack.displayName;
        this.marketValue = oldStack.marketValue;
        this.amount = Mathf.Min(oldStack.amount, 3);
    }

    public ItemStack() : this("", "", 0, 0) {}
}
