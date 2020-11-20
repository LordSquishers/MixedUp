using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class Character : MonoBehaviour
{   
    // Max velocity
    [Header("Movement Properties")]
    public float maxVelocity;
    [Range(1, 2)]
    public float slowingFactor;
    public float turningSpeed;
    public float playerSpeed;
    public float tiltAngle;
    public float yLevel;
    public float bobbingHeight, frequency;
    public float walkingVolume;
    public Vector3 baseRotation;
    public GameObject playerItemIcon; // both empties!

    // Rigidbody required for 3D physics
    private Rigidbody rbody;
    private Keyboard kboard;
    private AudioSource audioSource;

    private Player player;

    void Start()
    {
        // Get and store the rigidbody component so we can use it later
        rbody = GetComponent<Rigidbody> ();
        player = GetComponent<Player>();
        kboard = Keyboard.current;
        audioSource = GetComponents<AudioSource>()[0];

        if(player.ID == 1) {
            playerItemIcon = GameObject.Find("P1Item");
            // Debug.Log("Found P1 Item!");
        } else if(player.ID == 2) {
            playerItemIcon = GameObject.Find("P2Item");
            // Debug.Log("Found P2 Item!");
        }
    }

    void FixedUpdate()
    {
        MovementPhysics();
    }

    private void Update()
    {
        HandleVisuals();
    }

    public void OnMove(InputValue value) {
        HandleMovement(value.Get<Vector2>());
    }

    public void OnPickDrop(InputValue value) {
        player.PickDrop();
    }

    public void OnActivate(InputValue value) {
        // GameManagement.PollMoney();
        player.Activate();
    }

    void HandleVisuals() {
        if(player.itemHeld.amount == 0) {
           Destroy(player.objectInHand);
           RemoveItemIcon();
        }else if(player.objectInHand == null && player.itemHeld.amount != 0) {
           SetItemInPlayerHand();
            UpdateItemIcon();
        } else if(player.itemHeld.itemID != player.objectInHand.GetComponent<Item>().GetItemStack().itemID) {
            SetItemInPlayerHand();
            UpdateItemIcon();
        }
        
        string itemLabelText = (player.ID == 1 ? " x " + player.itemHeld.amount : player.itemHeld.amount + " x ");
        GameObject.Find("P" + player.ID + "ItemLabel").GetComponent<Text>().text = itemLabelText;
        if(player.objectInHand != null && player.objectInHand.tag == "Item") player.objectInHand.GetComponent<Item>().GetItemStack().amount = player.itemHeld.amount;
    }

    void UpdateItemIcon() {
        foreach(Transform child in playerItemIcon.transform) Destroy(child.gameObject);
        GameObject newItem = Instantiate<GameObject>(LoadPrefabItem(player.itemHeld.itemID), playerItemIcon.transform);
        newItem.transform.localScale *= 3;
        Destroy(newItem.GetComponent<Rigidbody>());
        newItem.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        foreach(BoxCollider coll in newItem.GetComponents<BoxCollider>()) {
            coll.enabled = false;
        }
    }

    void RemoveItemIcon() {
        if(playerItemIcon.GetComponentInChildren<Item>() == null) return;
        playerItemIcon.GetComponentInChildren<Item>().GetItemStack().amount = 0;
    }

    void SetItemInPlayerHand() {
        Destroy(player.objectInHand);
        GameObject newItem = Instantiate<GameObject>(LoadPrefabItem(player.itemHeld.itemID), transform);
        newItem.transform.localPosition = player.itemOffset;
        newItem.transform.localRotation *= Quaternion.Euler(player.itemRotation);
        newItem.transform.localScale *= player.itemScale;
        Destroy(newItem.GetComponent<Rigidbody>());
        foreach(BoxCollider coll in newItem.GetComponents<BoxCollider>()) {
            coll.enabled = false;
        }
        player.objectInHand = newItem;
    }

    public static GameObject LoadPrefabItem(string name) {
        return Resources.Load<GameObject>("Prefabs/Items/" + name);
    }

    void MovementPhysics() {
        SlowMovement();

        rbody.velocity = Vector3.ClampMagnitude(rbody.velocity, maxVelocity);
        rbody.velocity = new Vector3(rbody.velocity.x, 0, rbody.velocity.z);
        rbody.angularVelocity = Vector3.zero;

        float bobbingOffset = Mathf.Sin(Time.time * rbody.velocity.magnitude * frequency) * bobbingHeight;
        if(bobbingOffset < -bobbingHeight * 0.95f) {
            SpawnDustParticles();
        }

        transform.position = new Vector3(transform.position.x, yLevel + bobbingOffset, transform.position.z);

        if (rbody.velocity.sqrMagnitude > 0.01)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(rbody.velocity) * Quaternion.Euler(Vector3.up * -90) * Quaternion.Euler(Vector3.forward * tiltAngle) * Quaternion.Euler(baseRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * turningSpeed);

            // Sound
            if (!audioSource.isPlaying)
            {
                audioSource.volume = walkingVolume;
                audioSource.pitch = Random.Range(1.0f, 1.2f);
                audioSource.Play();
            }
        } else
        {
            audioSource.Stop();
        }

    }

    void SpawnDustParticles() {
        var prefab = Resources.Load<GameObject>("Prefabs/CloudParticle");
        int numberClouds = Random.Range(2, 4);

        for(int i = 0; i < numberClouds; i++) {
            GameObject cloudParticle = Instantiate<GameObject>(prefab);
            float range = 0.3f;
            Vector3 randOffset = new Vector3(Random.Range(-range, range), Random.Range(-range * 1.25f, range * 1.25f), Random.Range(-range, range));
            cloudParticle.transform.position = new Vector3(
                transform.position.x + randOffset.x, transform.position.y - 1 + randOffset.y, transform.position.z + randOffset.z
                );

            CloudParticle controller = cloudParticle.GetComponent<CloudParticle>();
            controller.topScale = Random.Range(1.25f, 1.55f);
            controller.lifetime = Random.Range(1f, 1.33f);
        }
    }

    void SlowMovement() {
        if(player.ID == 1) {
            if(!(kboard[Key.W].isPressed || kboard[Key.S].isPressed || kboard[Key.A].isPressed || kboard[Key.D].isPressed)) {
                rbody.velocity = new Vector3(rbody.velocity.x / slowingFactor, rbody.velocity.y, rbody.velocity.z / slowingFactor);
            }
        } else if(player.ID == 2) {
            if(!(kboard[Key.UpArrow].isPressed || kboard[Key.DownArrow].isPressed || kboard[Key.LeftArrow].isPressed || kboard[Key.RightArrow].isPressed)) {
                rbody.velocity = new Vector3(rbody.velocity.x / slowingFactor, rbody.velocity.y, rbody.velocity.z / slowingFactor);
            }
        }
    }

    void HandleMovement(Vector2 input)
    {
        Vector3 addedVelocity = new Vector3();

        // Add force (but it's called velocity bc old code)
        if (input.y > 0) // w
        {
            addedVelocity.z += playerSpeed;
        }
        if (input.y < 0) // s
        {
            addedVelocity.z -= playerSpeed;
        }
        if (input.x < 0) // a
        {
            addedVelocity.x -= playerSpeed;
        }
        if (input.x > 0) // d
        {
            addedVelocity.x += playerSpeed;
        }

        // prevent player from moving faster diagonally.
        if (input.y > 0 && (input.x < 0 || input.x > 0))
        {
            addedVelocity /= Mathf.Sqrt(2);
        }

        if (input.y < 0 && (input.x < 0 || input.x > 0))
        {
            addedVelocity /= Mathf.Sqrt(2);
        }

        // UnityEngine.Debug.Log(addedVelocity);
        rbody.velocity = addedVelocity * Time.deltaTime * 10;
    }
    
}