using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public float transportTime = 1f, transportLength;
    public Vector3 itemScale, transportDirection;
    public Merchant bakery;

    [HideInInspector]
    public ItemStack passingStack;
    private GameObject stackInWorld;
    private float timeStarted;
    private bool currentlyInert = true; // how verbose!

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if(passingStack.amount != 0) { // let's get moving!
            if(currentlyInert) {
                currentlyInert = false;
                timeStarted = Time.time;
                InstantiateItem();
                return;
            }

            if(Time.time - timeStarted > transportTime) {
                DropItem();
                passingStack.amount = 0;
            }

            float speed = Time.deltaTime * transportLength / transportTime;
            stackInWorld.transform.localPosition += new Vector3(transportDirection.x * speed, transportDirection.y * speed, transportDirection.z * speed);
        }

        if(Time.time - timeStarted > transportTime + 0.75f && !currentlyInert) {
            Destroy(stackInWorld);
            currentlyInert = true;
        }
    }

    void DropItem() {
        stackInWorld.GetComponent<Rigidbody>().isKinematic = false;
        stackInWorld.transform.parent = null;
        bakery.SellInBakery(passingStack);
    }

    void InstantiateItem() {
        string itemID = passingStack.itemID.ToLower();
        string modelName = "";
        // WARNING: Yandere Dev incoming
        if(itemID.Contains("blueberry")) { // blue box (blueberry)
            modelName = "BlueBox2"; // yes, this was the second one.
        } else if(itemID.Contains("banana")) { // yellow (banana)
            modelName = "BananaBox";
        } else if(itemID.Contains("carrot")) { // orange (carrot)
            modelName = "CarrotBox";
        } else if(itemID.Contains("rhubarb")) { // red (rhubarb)
            modelName = "RhubarbBox";
        } else if(itemID == "cake") { // cake (purple) (it's all cake)
            modelName = "CakeBox";
        } else if(itemID.Contains("fruit") || itemID.Contains("smoothie")) { // green (fruit)
            modelName = "GreenBox";
        } else if(itemID.Contains("bread")) { // "tan" (bread)
            modelName = "TanBox";
        }

        var boxModel = Instantiate<GameObject>(Resources.Load<GameObject>("Models/Products/" + modelName));
        boxModel.AddComponent<Rigidbody>().isKinematic = true;
        boxModel.transform.localScale = itemScale;
        boxModel.transform.position = transform.position + new Vector3(0f, 1f, 0f);
        boxModel.transform.localPosition -= transportDirection / 2;
        boxModel.name = "SpecialBox";
        stackInWorld = boxModel;
    }
}
