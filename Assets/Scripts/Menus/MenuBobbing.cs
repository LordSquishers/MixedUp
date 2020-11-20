using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBobbing : MonoBehaviour
{

    private Vector3 initialPos;
    public float turning = 0.625f;
    public bool bob = false;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (bob) {transform.position = new Vector3(initialPos.x, initialPos.y + Mathf.Sin(Time.time / 2) / 4 + 0.25f, initialPos.z); }
        transform.localEulerAngles += new Vector3(0, turning, 0);
    }
}
