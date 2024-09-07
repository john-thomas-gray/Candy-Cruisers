using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;


public class TextOnSpot : MonoBehaviour
{

    public TextMeshPro textMesh;
    private float displayTime = .5f;
    private float elapsedTime = 0f;
    private float initialFontSize = 3f;
    private float targetFontSize = 4.3f;
    Color purple = new Color(1f, 0f, 1f, 1f);
    public Dictionary<string, Color> colors = new Dictionary<string, Color>();


    void Awake() {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.fontSize = initialFontSize;
        colors = new Dictionary<string, Color>
            {
                { "Red", Color.red },
                { "Yellow", Color.yellow },
                { "Blue", Color.blue },
                { "Green", Color.green },
                { "Purple", purple }
            };
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

    public void SetMultiplier(int multiplier, string color) {
        if (multiplier > 1)
        {
            textMesh.text = "x" + multiplier;
            textMesh.color = colors[color];
            textMesh.outlineColor = Color.white;
            textMesh.outlineWidth = 2f;
        }
    }

    // METHOD TO MAKE COLOR BRIGHTER THE HIGHER THE
    // MULTIPLIER IS
}
