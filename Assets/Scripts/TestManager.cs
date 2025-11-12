using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestManager : MonoBehaviour
{
    [System.Serializable]
    public class TestShape
    {
        public string shapeName;
        public string colorName;
        public GameObject shapeObject;
    }

    [Header("References")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI feedbackText;
    public PlayerController playerController;
    public Transform focusPoint;
    public Transform testStartPosition;
    public AudioPhraseManager phraseManager; // 🎧 link phrase system

    [Header("Audio Clips")]
    public AudioClip correctSound;       // ✅ pleasant chime
    public AudioClip wrongSound;         // ❌ try again sound
    public AudioClip celebrationSound;   // 🎉 end of test sound

    private AudioSource audioSource;

    [Header("Test Shapes")]
    public List<TestShape> testShapes = new List<TestShape>();

    private int currentIndex = 0;
    private int correctCount = 0;
    private int wrongCount = 0;
    private int totalClicks = 0;

    private bool testActive = false;
    private bool waitingForClick = false;

    private float testStartTime;
    private float testEndTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D
    }

    public void StartTest()
    {
        if (testActive) return;

        testActive = true;
        currentIndex = 0;
        correctCount = 0;
        wrongCount = 0;
        totalClicks = 0;

        StartCoroutine(MoveAndFocusPlayer());
    }

    IEnumerator MoveAndFocusPlayer()
    {
        if (playerController != null)
            playerController.controlsEnabled = false;

        Vector3 startPos = playerController.transform.position;
        Vector3 targetPos = new Vector3(testStartPosition.position.x, startPos.y, testStartPosition.position.z);

        float elapsed = 0f;
        float duration = 1.5f;

        while (elapsed < duration)
        {
            playerController.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerController.transform.position = targetPos;

        yield return StartCoroutine(FocusCamera());

        testStartTime = Time.time;

        feedbackText.gameObject.SetActive(false);
        NextQuestion();
    }

    IEnumerator FocusCamera()
    {
        Vector3 direction = (focusPoint.position - playerController.transform.position).normalized;
        direction.y = 0;
        Quaternion targetRot = Quaternion.LookRotation(direction);

        Quaternion startRot = playerController.transform.rotation;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            playerController.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerController.transform.rotation = targetRot;
    }

    void NextQuestion()
    {
        if (currentIndex >= testShapes.Count)
        {
            EndTest();
            return;
        }

        var target = testShapes[currentIndex];
        Debug.Log($"Next question: Find the {target.colorName} {target.shapeName}");

        questionText.text = $"Find the {target.colorName} {target.shapeName}";
        waitingForClick = true;

        // 🗣 Play modular phrase
        if (phraseManager != null)
            StartCoroutine(phraseManager.PlayFindPhrase(target.colorName, target.shapeName));
    }

    public void ShapeClicked(GameObject clickedShape)
    {
        if (!testActive || !waitingForClick) return;
        if (currentIndex >= testShapes.Count)
        {
            EndTest();
            return;
        }

        totalClicks++;

        var target = testShapes[currentIndex];
        bool correct = (clickedShape == target.shapeObject);

        waitingForClick = false;

        if (correct)
        {
            correctCount++;
            PlaySound(correctSound);
            StartCoroutine(ShowFeedback("Correct!", Color.green, true, target));
        }
        else
        {
            wrongCount++;
            PlaySound(wrongSound);
            StartCoroutine(ShowFeedback("Try Again!", Color.red, false, target));
        }
    }

    IEnumerator ShowFeedback(string message, Color color, bool correct, TestShape target)
    {
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = message;
        feedbackText.color = color;

        Renderer rend = (correct ? target.shapeObject.GetComponent<Renderer>() : null);
        if (rend) rend.material.color = Color.yellow;

        yield return new WaitForSeconds(2f);

        feedbackText.gameObject.SetActive(false);
        if (rend) rend.material.color = GetColorFromName(target.colorName);

        if (correct)
        {
            currentIndex++;
            yield return new WaitForSeconds(0.5f);
            NextQuestion();
        }
        else
        {
            waitingForClick = true;
        }
    }

    void EndTest()
    {
        testEndTime = Time.time;
        float totalTimeTaken = testEndTime - testStartTime;

        questionText.text = "Test complete!";
        feedbackText.gameObject.SetActive(true);

        string resultSummary =
            $"You got {correctCount} out of {testShapes.Count} correct!\n" +
            $"Wrong Attempts: {wrongCount}\n" +
            $"Total Clicks: {totalClicks}\n" +
            $"Time Taken: {totalTimeTaken:F1} seconds";

        feedbackText.text = resultSummary;
        feedbackText.color = Color.white;

        PlaySound(celebrationSound);

        StartCoroutine(UnlockPlayerAndClearUI());
        testActive = false;
    }

    IEnumerator UnlockPlayerAndClearUI()
    {
        yield return new WaitForSeconds(4f);

        if (playerController != null)
            playerController.controlsEnabled = true;

        yield return new WaitForSeconds(2f);

        questionText.text = "";
        feedbackText.gameObject.SetActive(false);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    Color GetColorFromName(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "red": return Color.red;
            case "blue": return Color.blue;
            case "green": return Color.green;
            case "orange": return new Color(1f, 0.5f, 0f);
            default: return Color.white;
        }
    }
}
