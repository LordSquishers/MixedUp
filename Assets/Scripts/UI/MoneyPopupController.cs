using UnityEngine;

public class MoneyPopupController : MonoBehaviour {
    // Color of counters.
    public Color textColor;

    private bool isHigher;
    public float scale, lifespan, driftAmount;
    public AudioClip saleSound;
    public float volume;

    // Start is called before the first frame update
    private void Start() {
        GetComponent<MeshRenderer>().enabled = false;
    }

    void PlaySaleSound()
    {
        transform.parent.gameObject.GetComponents<AudioSource>()[0].pitch = Random.Range(0.8f, 1.1f);
        transform.parent.gameObject.GetComponents<AudioSource>()[0].volume = volume;
        transform.parent.gameObject.GetComponents<AudioSource>()[0].PlayOneShot(saleSound);
    }

    public void CreatePopup(Vector3 initialPos, float moneyGained) {
        // Init GO
        var newPopup = Instantiate(gameObject, initialPos, new Quaternion());
        newPopup.GetComponent<MeshRenderer>().enabled = true;
        newPopup.name = "$$_POPUP_" + moneyGained;

        // Set movement variables.
        var pop = newPopup.AddComponent<PopupMovement>();

        pop.moneyGained = moneyGained;
        pop.initialPos = initialPos + (isHigher ? new Vector3(0, 0.85f, 1.5f) : new Vector3(0, 0.25f, 1.0f));
        pop.drift = driftAmount;
        pop.lifespan = lifespan;
        pop.scale = scale;

        pop.GetComponent<TextMesh>().color = textColor;

        // Sound
        PlaySaleSound();

        // Toggle so counters don't overlap.
        isHigher = !isHigher;
    }

    public void CreatePopup(Vector3 initialPos, string text, Color color) {
        // Init GO
        var newPopup = Instantiate(gameObject, initialPos, new Quaternion());
        newPopup.GetComponent<MeshRenderer>().enabled = true;
        newPopup.name = "$$_POPUP_" + text;

        // Set movement variables.
        var pop = newPopup.AddComponent<PopupMovement>();

        pop.moneyGained = -1;
        pop.text = text;
        pop.initialPos = initialPos + (isHigher ? new Vector3(0, 0.85f, 1.5f) : new Vector3(0, 0.25f, 1.0f));
        pop.drift = driftAmount;
        pop.lifespan = lifespan;
        pop.scale = scale;

        pop.GetComponent<TextMesh>().color = color == null ? textColor : color;

        // Sound
        // PlayErrorSound();

        // Toggle so counters don't overlap.
        isHigher = !isHigher;
    }
}