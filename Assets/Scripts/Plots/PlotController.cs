using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotController : MonoBehaviour
{
    public enum PlotState { Empty, Growing, Complete };

    [Header("Plot Properties")]
    public GameObject itemToGrow;

    [Range(0, 60)]
    public float timeToGrow = 5;

    [Range(0, 1)]
    public float initialScale = 0.1f;
    public int minYield, maxYield;
    public bool startAutomatically;

    [Header("UI Elements")]
    public Color emptyColor;
    public Color growthColor, completeColor;
    [Tooltip("Values to change the offset of the Item. Required so it 'touches' the dirt.")]
    public float bottomPosition = 0.3f, topPosition = 0.67f;
    public bool isSelected;

    public AudioClip plotSound;

    private PlotState currentState = PlotState.Empty;
    private GameObject itemGrowing;

    private float currentScale = 0f, currentTime = 0f;
    MeshRenderer rend;
    GameObject statusRing;

    // Start is called before the first frame update
    void Start()
    {
        // BeginGrowth();
        statusRing = gameObject.FindInChildren("Status");
        rend = statusRing.GetComponent<MeshRenderer> ();
        if(startAutomatically) BeginGrowth();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == PlotState.Growing) {
            GrowthTick();
        } else if (currentState == PlotState.Complete) {
            CompleteGrowth();
        } else {
            EmptyHandling();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player") {
            Deselect();
        }
    }

    public void Select() {
        isSelected = true;
        Material[] mats = gameObject.FindInChildren("Plot Model").FindInChildren("Main").GetComponent<Renderer>().materials;
        foreach(Material mat in mats) {
            mat.SetInt("Bool_TintSelected", 1);
        }
    }

    public void Deselect() {
        isSelected = false;
        Material[] mats = gameObject.FindInChildren("Plot Model").FindInChildren("Main").GetComponent<Renderer>().materials;
        foreach(Material mat in mats) {
            mat.SetInt("Bool_TintSelected", 0);
        }
    }

    void EmptyHandling() {
        // set ring to Empty Color
        rend.material.SetColor("LineColor", emptyColor);
        // BeginGrowth();
    }

    void CompleteGrowth() {
        // set ring to Complete Color
        rend.material.SetColor("LineColor", completeColor);
        // ClearPlot();
    }

    void GrowthTick() {
        // set ring to Growing Color
        rend.material.SetColor("LineColor", Color.Lerp(growthColor, completeColor, currentScale));

        // Update time and scale values.
        currentTime += Time.deltaTime;
        currentScale = currentTime / timeToGrow; // Range [0, 1]

        // Update position and scale.
        itemGrowing.transform.localScale = currentScale * itemToGrow.transform.localScale;
        itemGrowing.transform.position = transform.position + new Vector3(0, Mathf.Lerp(bottomPosition, topPosition, currentScale), 0);

        // Transition if necessary.
        if(currentTime >= timeToGrow) {
            currentTime = 0;
            currentState = PlotState.Complete;
            rend.material.SetInt("IsGrowing", 0);
        }
    }

    void BeginGrowth() {
        // Set plot variables.
        currentState = PlotState.Growing;
        currentScale = initialScale;
        currentTime = 0;

        // Create new Item Object and remove Physics components.
        itemGrowing = (GameObject)Object.Instantiate(itemToGrow, transform.position + new Vector3(0, bottomPosition, 0), transform.rotation, transform);
        // UnityEngine.Debug.Log(itemGrowing.GetComponent<Item>().itemType.itemName);
        Destroy(itemGrowing.GetComponent<Rigidbody>());
        Destroy(itemGrowing.GetComponent<Collider>());

        // Set Rotation and Scale
        itemGrowing.transform.eulerAngles = itemGrowing.GetComponent<Item>().GetItemStack().itemID == "Egg" ? new Vector3(0, 0, 0) : new Vector3(-90, 0, 0);
        itemGrowing.transform.localScale = initialScale * itemToGrow.transform.localScale;
    }

    void PlaySound(float pitch)
    {
        gameObject.GetComponent<AudioSource>().pitch = pitch + Random.Range(-0.25f, 0.25f);
        gameObject.GetComponent<AudioSource>().PlayOneShot(plotSound);
        gameObject.GetComponent<AudioSource>().time = 1f;
    }

    public ItemStack HarvestPlot() {
        ItemStack harvestedItem =  new ItemStack(itemToGrow.GetComponent<Item>().GetItemStack());
        harvestedItem.amount = Random.Range(minYield, maxYield + 1);
        PlaySound(4.0f);
        ClearPlot();
        return harvestedItem;
    }

    public string GetItemID() {
        return itemToGrow.GetComponent<Item>().GetItemStack().itemID;
    }

    public void SetItem(ItemStack item) {
        if(item.itemID == "Egg" || item.itemID == "Blueberry" || item.itemID == "Sugar") {
            itemToGrow = (GameObject) Resources.Load<GameObject>("Models/Plots/" + item.itemID + "_Prefab");
            Item newItem = itemToGrow.AddComponent<Item>();
            newItem.SetItemStack(item);
            newItem.inert = true;
            newItem.transform.localScale *= item.itemID == "Egg" ? 0.75f : 0.65f;
        } else {
            itemToGrow = (GameObject) Resources.Load<GameObject>("Prefabs/Items/" + item.itemID);
        }
        currentState = PlotState.Growing;
        PlaySound(2f);
    }

    public void StartGrowing() {
        rend.material.SetInt("IsGrowing", 1);
        BeginGrowth();
    }

    public void ClearPlot() {
        /* 
        I considered setting itemToGrow to null, but that will cause errors and
        I *assume* that a new Item will be set anyway. 
        I also assume that ClearPlot will be called. 
        */
        currentState = PlotState.Empty;
        Destroy(itemGrowing);
    }

    public PlotState GetGrowthStatus() {
        return currentState;
    }

    void OnDrawGizmos() {
        Gizmos.color = currentState == PlotState.Complete ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1.85f, 0.75f, 1.85f));
    }

}
