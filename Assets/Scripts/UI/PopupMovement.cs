using System.Diagnostics;
using UnityEngine;

public class PopupMovement : MonoBehaviour {
    // Initial vars
    public float moneyGained;
    public string text;
    public float drift, scale, lifespan;
    public Vector3 initialPos;

    private Stopwatch timer;

    // Start is called before the first frame update
    private void Start() {
        timer = new Stopwatch();
        timer.Start();

        // Enable rendering because the original copy is hidden during gameplay.
        gameObject.GetComponent<MeshRenderer>().enabled = true;

        // Set damage done.
        if(moneyGained == -1) {
            gameObject.GetComponent<TextMesh>().text = text;
        } else {
            gameObject.GetComponent<TextMesh>().text = string.Format("{0:C2}", moneyGained);
        }
    }

    // Update is called once per frame
    private void Update() {
        // Divide time by drift.
        var amt = timer.ElapsedMilliseconds / drift;

        // This is the "drift" of each health counter. Change + - or 0 for different directions.
        var pos = initialPos + new Vector3(amt, amt, amt);

        // Set position, scale, and set to face camera at all times.
        gameObject.transform.position = pos;
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
        gameObject.transform.rotation =
            Quaternion.RotateTowards(gameObject.transform.rotation, Camera.main.transform.rotation, 360);

        // Remove counter after specified time.
        if (timer.ElapsedMilliseconds > lifespan) Destroy(gameObject);
    }
}