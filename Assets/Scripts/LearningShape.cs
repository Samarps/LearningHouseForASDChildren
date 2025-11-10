using System.Collections;
using UnityEngine;

public class LearningShape : MonoBehaviour
{
    [Header("Shape Settings")]
    public GameObject label; // assign the label in Inspector
    public Material defaultMaterial;
    public Material highlightMaterial;
    public float highlightDuration = 2f;

    private MeshRenderer meshRenderer;
    private bool isHighlighted = false;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // Ensure label starts hidden
        if (label != null)
            label.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!isHighlighted)
            StartCoroutine(HighlightAndShowLabel());
    }

    private IEnumerator HighlightAndShowLabel()
    {
        isHighlighted = true;

        // Show label
        if (label != null)
            label.SetActive(true);

        // Change material to highlight
        if (meshRenderer != null && highlightMaterial != null)
            meshRenderer.material = highlightMaterial;

        // Wait for a few seconds
        yield return new WaitForSeconds(highlightDuration);

        // Revert material
        if (meshRenderer != null && defaultMaterial != null)
            meshRenderer.material = defaultMaterial;

        // Hide label
        if (label != null)
            label.SetActive(false);

        isHighlighted = false;
    }
}
