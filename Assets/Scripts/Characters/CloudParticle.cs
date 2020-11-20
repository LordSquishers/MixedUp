using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudParticle : MonoBehaviour
{
    [Range(0, 2)]
    public float lifetime;
    public AnimationCurve shape;
    public float topScale;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float percentComplete = (Time.time - startTime) / lifetime;
        if(percentComplete >= 1) End();

        float scale = shape.Evaluate(percentComplete) * topScale;
        transform.localScale = new Vector3(scale, scale, scale);
        gameObject.GetComponent<Renderer>().material.SetFloat("Lifetime", percentComplete);
    }

    void End() {
        Destroy(gameObject);
    }
}
