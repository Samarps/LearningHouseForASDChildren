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
    public Transform testStartPosition;   // 🆕 Add this reference!

    [Header("Test Shapes")]
    public List<TestShape> testShapes = new List<TestShape>();

    private int currentIndex = 0;
    private int correctCount = 0;
    private bool testActive = false;
    private bool waitingForClick = false;

    public void StartTest()
    {
        if (testActive) return;
        testActive = true;

        StartCoroutine(MoveAndFocusPlayer());   // 🆕 new coroutine
    }

    IEnumerator MoveAndFocusPlayer()  // 🆕 handles both move + focus
    {
        playerController.controlsEnabled = false;

        // STEP 1: Smoothly move player to start position
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

        // STEP 2: Rotate player toward focus point
        yield return StartCoroutine(FocusCamera());

        // STEP 3: Begin test after player is settled
        feedbackText.gameObject.SetActive(false);
        currentIndex = 0;
        correctCount = 0;
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
        Debug.Log("Next question: Find the " + testShapes[currentIndex].colorName + " " + testShapes[currentIndex].shapeName);

        if (currentIndex >= testShapes.Count)
        {
            EndTest();
            return;
        }

        var target = testShapes[currentIndex];
        questionText.text = "Find the " + target.colorName + " " + target.shapeName;
        waitingForClick = true;
    }

    public void ShapeClicked(GameObject clickedShape)
    {
        if (!testActive || !waitingForClick) return;

        var target = testShapes[currentIndex];
        bool correct = (clickedShape == target.shapeObject);

        waitingForClick = false;

        if (correct)
        {
            correctCount++;
            StartCoroutine(ShowFeedback("✅ Correct!", Color.green, true, target));
        }
        else
        {
            StartCoroutine(ShowFeedback("❌ Try Again!", Color.red, false, target));
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
        questionText.text = "Test complete!";
        feedbackText.gameObject.SetActive(true);
        feedbackText.text = "You got " + correctCount + " out of " + testShapes.Count + " correct!";
        feedbackText.color = Color.white;
        StartCoroutine(UnlockPlayer());
    }

    IEnumerator UnlockPlayer()
    {
        yield return new WaitForSeconds(3f);
        playerController.controlsEnabled = true;
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
