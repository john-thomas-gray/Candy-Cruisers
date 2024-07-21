using UnityEngine;
using TMPro;

public class TextOnSpot : MonoBehaviour
{

    public TextMeshPro textMesh;
    private float displayTime = .5f;
    private float elapsedTime = 0f;
    private float initialFontSize = 3f;
    private float targetFontSize = 4.3f;

    void Awake() {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.fontSize = initialFontSize;
    }

    void Update() {
        elapsedTime += Time.deltaTime;

        if (elapsedTime <= displayTime) {
            textMesh.fontSize = Mathf.Lerp(initialFontSize, targetFontSize, elapsedTime / displayTime);
        }
        else {
            Destroy(gameObject);
        }

    }

    public void SetMultiplier(int multiplier, Color color) {
        if (multiplier > 1)
        {
            textMesh.text = "x" + multiplier;
            textMesh.color = color;
            textMesh.outlineColor = Color.white;
            textMesh.outlineWidth = 2f;
        }
    }

    // METHOD TO MAKE COLOR BRIGHTER THE HIGHER THE
    // MULTIPLIER IS
}
