using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Random;

public class Mixer : MonoBehaviour
{
    public float timePerIngredient = 1f;
    public Conveyor outputBelt;
    public AudioClip mixerAudio;

    private List<ItemStack> ingredients;
    private ItemStack outputItem;

    [HideInInspector]
    public bool isComplete = false, isRunning = false;
    private float timeRunning = 0f;
    private GameObject whisk;
    private Vector3 initPos, initScale;

    public void DepositItem(ItemStack item)
    {
        ItemStack deposit = new ItemStack(item);
        deposit.amount = 1;
        ingredients.Add(deposit);
    }

    public bool Activate()
    {
        outputItem = RecipeManager.Lookup(ingredients);
        if(outputItem.amount != 0)
        {
            isComplete = false;
            isRunning = true;
            GetComponent<AudioSource>().Play();
            return true;
        } else if(PlayerPrefs.GetInt("mixerDelete") > 0) { // wrong recipe you fool get rekt
            ingredients.Clear(); // this is so evil
        }
        return false;
    }

    public ItemStack TakeLastItem() {
        if(ingredients.Count == 0) return new ItemStack();
        ItemStack lastItem = ingredients[ingredients.Count - 1];
        ingredients.RemoveAt(ingredients.Count - 1);
        return lastItem;
    }

    public void Deactivate() {
        isRunning = false;
        ingredients = new List<ItemStack>();
        transform.position = initPos;
        transform.localScale = initScale;
    }

    public void Reset() {
        isComplete = false;
    }

    void Update() {
        if(isRunning) {
            if(timeRunning >= timePerIngredient * ingredients.Count) {
                Deactivate();
                timeRunning = 0;
                isComplete = true;
                OutputProduct();
            }
            timeRunning += Time.deltaTime;
            float oldRotY = whisk.transform.localEulerAngles.y;
            whisk.transform.localEulerAngles = new Vector3(-90, oldRotY + 2f, 0);

            if (GetComponent<AudioSource>().time / mixerAudio.length > 0.75f)
            {
                GetComponent<AudioSource>().time = 2.25f;
            }
        }
    }

    void FixedUpdate() {
        if(isRunning) {
            float rand = Range(-0.15f, 0.15f) / 4;
            transform.position += new Vector3(rand * Math.Sign(Range(-1, 1)), 0, rand * Math.Sign(Range(-1, 1)));
            if((transform.position - initPos).sqrMagnitude > 0.02) {
                transform.position = initPos;
            }
            float freq = 4;
            transform.localScale = new Vector3(Mathf.Cos(Time.time * freq) * 0.0625f * initScale.x + 1.8f, initScale.y, Mathf.Sin(Time.time * freq) * 0.0625f * initScale.z + 1.8f);
        }
    }

    void OutputProduct() {
        Conveyor conveyorBelt = transform.parent.GetComponent<Table>().linkedObject.GetComponent<Conveyor>();
        conveyorBelt.passingStack = new ItemStack(outputItem);
    }

    void Start() {
        ingredients = new List<ItemStack>();
        whisk = gameObject.FindInChildren("Whisk");
        initPos = transform.position;
        initScale = transform.localScale;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Deselect();
        }
    }

    public void Select()
    {
        Material[] mats = gameObject.FindInChildren("Base").GetComponent<Renderer>().materials;
        foreach (Material mat in mats)
        {
            mat.SetInt("Bool_TintSelected", 1);
        }
    }

    public void Deselect()
    {
        Material[] mats = gameObject.FindInChildren("Base").GetComponent<Renderer>().materials;
        foreach (Material mat in mats)
        {
            mat.SetInt("Bool_TintSelected", 0);
        }
    }

}
