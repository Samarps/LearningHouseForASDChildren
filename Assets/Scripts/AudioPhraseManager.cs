using System.Collections;
using UnityEngine;

public class AudioPhraseManager : MonoBehaviour
{
    [Header("Base Phrase Clips")]
    public AudioClip findTheClip;   // "Find the"
    public AudioClip itsAClip;      // "It's a"
    public AudioClip itsAnClip;     // "It's an"

    [Header("Color Clips")]
    public AudioClip redClip;
    public AudioClip blueClip;
    public AudioClip greenClip;
    public AudioClip orangeClip;

    [Header("Shape Clips")]
    public AudioClip cubeClip;
    public AudioClip sphereClip;
    public AudioClip cylinderClip;
    public AudioClip capsuleClip;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D audio
    }

    // 🔹 Plays "Find the blue cube"
    public IEnumerator PlayFindPhrase(string color, string shape)
    {
        yield return PlayClip(findTheClip);
        yield return PlayColorClip(color);
        yield return PlayShapeClip(shape);
    }

    // 🔹 Plays "It's a red cube" or "It's an orange cube"
    public IEnumerator PlayItsAPhrase(string color, string shape)
    {
        if (StartsWithVowel(color))
            yield return PlayClip(itsAnClip);
        else
            yield return PlayClip(itsAClip);

        yield return PlayColorClip(color);
        yield return PlayShapeClip(shape);
    }

    IEnumerator PlayClip(AudioClip clip)
    {
        if (clip == null) yield break;
        audioSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length + 0.01f); // short pause
    }

    IEnumerator PlayColorClip(string color)
    {
        switch (color.ToLower())
        {
            case "red": yield return PlayClip(redClip); break;
            case "blue": yield return PlayClip(blueClip); break;
            case "green": yield return PlayClip(greenClip); break;
            case "orange": yield return PlayClip(orangeClip); break;
        }
    }

    IEnumerator PlayShapeClip(string shape)
    {
        switch (shape.ToLower())
        {
            case "cube": yield return PlayClip(cubeClip); break;
            case "sphere": yield return PlayClip(sphereClip); break;
            case "cylinder": yield return PlayClip(cylinderClip); break;
            case "capsule": yield return PlayClip(capsuleClip); break;
        }
    }

    bool StartsWithVowel(string word)
    {
        if (string.IsNullOrEmpty(word)) return false;
        char c = char.ToLower(word[0]);
        return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u';
    }
}
