using UnityEngine;

public class TestZoneTrigger : MonoBehaviour
{
    public GameObject interactionHint;
    public TestManager testManager;
    private bool playerInside = false;

    void Start()
    {
        if (interactionHint != null)
            interactionHint.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            playerInside = true;
            if (interactionHint != null)
                interactionHint.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
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
            if (interactionHint != null)
                interactionHint.SetActive(false);
            testManager.StartTest();
        }
    }
}
