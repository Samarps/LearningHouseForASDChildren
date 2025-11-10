using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DoorTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad = "WelcomeScene";

    [Header("UI Hint")]
    public GameObject interactionHint; // assign in Inspector

    bool playerInside = false;

    void Start()
    {
        if (interactionHint != null)
            interactionHint.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered trigger with: " + other.name);
        if (other.GetComponent<CharacterController>() != null)
        {
            playerInside = true;
            if (interactionHint != null)
                interactionHint.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited trigger with: " + other.name);
        if (other.GetComponent<CharacterController>() != null)
        {
            playerInside = false;
            if (interactionHint != null)
                interactionHint.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
