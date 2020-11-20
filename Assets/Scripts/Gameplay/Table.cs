using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{

    [Tooltip("Point relative to center of table where items and machines are anchored.")]
    public Vector3 anchorPoint = new Vector3(0, 0.5f, 0);
    public Vector3 rotation;
    public GameObject heldObject;
    public GameObject refObject;
    [Tooltip("This is a shoddy workaround for not having a Mixer instantiated in Editor Mode.")]
    public GameObject linkedObject;
    // Start is called before the first frame update
    void Start()
    {
       SetupObjectByReference();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player") {
            Deselect();
        }
    }

    bool ContainsChildWithTag(GameObject go, string tag) {
        foreach(Transform child in go.transform) {
            // UnityEngine.Debug.Log(child.gameObject.tag);
            if(child.gameObject.tag == tag) return true;
        }
        return false;
    }

    public void Select() {
        if(ContainsChildWithTag(gameObject, "Merchant") || ContainsChildWithTag(gameObject, "Mixer")) return;
        Material[] mats = gameObject.FindInChildren("TableModel").GetComponent<Renderer>().materials;
        foreach(Material mat in mats) {
            mat.SetInt("Bool_TintSelected", 1);
        }
    }

    public void Deselect() {
        if(ContainsChildWithTag(gameObject, "Merchant") || ContainsChildWithTag(gameObject, "Mixer")) return;
        Material[] mats = gameObject.FindInChildren("TableModel").GetComponent<Renderer>().materials;
        foreach(Material mat in mats) {
            mat.SetInt("Bool_TintSelected", 0);
        }
    }
    void SetupObjectByReference() {
        if(refObject != null) {
            // UnityEngine.Debug.Log("Loading " + refObject.name);
            heldObject = GameObject.Instantiate(refObject, transform.position + anchorPoint, refObject.transform.rotation * Quaternion.Euler(rotation.x, rotation.y, rotation.z), transform);
            if(heldObject.GetComponent<Rigidbody>() != null)
                heldObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void InsertItem(ItemStack item) {
        refObject = Resources.Load<GameObject>("Prefabs/Items/" + item.itemID);
        SetupObjectByReference();
        heldObject.GetComponent<Item>().SetItemStack(new ItemStack(item));
        if(heldObject.GetComponent<Item>().GetItemStack().amount > 1) {
            heldObject.transform.position = transform.position + anchorPoint + new Vector3(0f, 0f, 0f);
        }
        // UnityEngine.Debug.Log("[TABLE] Inserted " + item.itemID);
    }

    public ItemStack TakeItem() {
        if(!isItemPresent()) return new ItemStack();
        ItemStack item = heldObject.GetComponent<Item>().GetItemStack();
        Destroy(heldObject);
        return item;
    }

    public GameObject GetObjectHeld() {
        return heldObject;
    }

    public bool isEmpty() {
        return heldObject == null;
    }

    public bool isItemPresent() { return heldObject != null && heldObject.tag == "Item"; }

    public bool isObjectPresent() { return heldObject != null; }
}
