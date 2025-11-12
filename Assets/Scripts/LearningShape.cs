using System.Collections;
using UnityEngine;

public class LearningShape : MonoBehaviour
{
    [Header("Shape Settings")]
    public GameObject label; // Assign label (text) in Inspector
    public Material defaultMaterial;
    public Material highlightMaterial;
    public float fallbackHighlightDuration = 2f; // used if no audio

    [Header("Audio Settings")]
    public AudioClip shapeAudio; // Assign audio in Inspector
    private AudioSource audioSource;

    private MeshRenderer meshRenderer;
    private bool isHighlighted = false;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // Ensure label starts hidden
        if (label != null)
            label.SetActive(false);

        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound (non-positional)
    }

    private void OnMouseDown()
    {
        if (!isHighlighted)
            StartCoroutine(HighlightLabelAndAudio());
    }

    private IEnumerator HighlightLabelAndAudio()
    {
        isHighlighted = true;

        // Show label
        if (label != null)
            label.SetActive(true);

        // Change material to highlight
        if (meshRenderer != null && highlightMaterial != null)
            meshRenderer.material = highlightMaterial;

        // Play audio (if available)
        float waitTime = fallbackHighlightDuration;
        if (shapeAudio != null)
        {
            audioSource.clip = shapeAudio;
            audioSource.Play();
            waitTime = shapeAudio.length;
        }

        // Wait for audio to finish or fallback duration
        yield return new WaitForSeconds(waitTime);

        // Revert material
        if (meshRenderer != null && defaultMaterial != null)
            meshRenderer.material = defaultMaterial;

        // Hide label
        if (label != null)
            label.SetActive(false);

        isHighlighted = false;
    }
}
