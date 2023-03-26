using UnityEngine;

public class FreeFlight : MonoBehaviour
{
    public float speed = 10.0f;
    public float turboSpeed = 20.0f;
    public float mouseSensitivity = 3.0f;
    public float pitchRange = 60.0f;

    private float pitch = 0.0f;
    private float yaw = 0.0f;

    private bool mouseCaptured = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Capture/release mouse on click
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            mouseCaptured = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            mouseCaptured = false;
        }

        if (!mouseCaptured)
        {
            return;
        }

        // Movement
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = turboSpeed;
        }

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("UpDown"), Input.GetAxis("Vertical"));
        movement = transform.TransformDirection(movement) * currentSpeed * Time.deltaTime;
        transform.position += movement;

        // Rotation
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -pitchRange, pitchRange);

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}