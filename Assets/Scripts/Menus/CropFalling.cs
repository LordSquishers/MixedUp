using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropFalling : MonoBehaviour
{
    public bool fall = true;

    // Update is called once per frame
    void Update()
    {
        if (fall && transform.position.y < -100)
        {
            transform.position = new Vector3(transform.position.x, 1200, transform.position.z);
        }
    }
}
