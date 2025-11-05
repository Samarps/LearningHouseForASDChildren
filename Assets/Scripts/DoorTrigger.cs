using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public string sceneToLoad = "ColorShapeScene"; // change to actual scene name
    public string hintText = "Press E to enter";

    bool playerInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            playerInside = true;
            // Optional: show UI hint using your UI Manager
            Debug.Log("Player near door. " + hintText);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            playerInside = false;
            // Optional: hide UI hint
            Debug.Log("Left door area.");
        }
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            // Use SceneManager directly to load
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }
}
