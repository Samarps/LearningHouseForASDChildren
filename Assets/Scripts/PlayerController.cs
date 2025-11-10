using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 5.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Header("Mouse Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2.0f;
    public float maxLookX = 80f;
    public float minLookX = -80f;
    private float rotX = 0f;

    [Header("Control Lock")]
    public bool controlsEnabled = true;   // ✅ added — toggled by TestManager to lock player

    CharacterController cc;
    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.None; // change to Locked later if you want capture
        Cursor.visible = true; // lets user see cursor for testing; change to false for immersive
    }

    void Update()
    {
        // ✅ prevent all input if controls are disabled (focus mode)
        if (!controlsEnabled) return;

        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, minLookX, maxLookX);

        cameraTransform.localEulerAngles = new Vector3(rotX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        isGrounded = cc.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -1f;

        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * inputX + transform.forward * inputZ;
        bool running = Input.GetKey(KeyCode.LeftShift);
        float speed = running ? runSpeed : walkSpeed;

        cc.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}
