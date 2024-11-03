using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 100f;
    public float mouseSensitivity = 2f; 
    public float verticalMoveSpeed = 2f;

    private float rotationX = 0f;

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical) * moveSpeed * Time.deltaTime;
        movement = Camera.main.transform.TransformDirection(movement); 
        movement.y = 0; 
        transform.position += movement;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); 
        transform.localEulerAngles = new Vector3(rotationX, transform.localEulerAngles.y + mouseX, 0f);

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += Vector3.up * verticalMoveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.position -= Vector3.up * verticalMoveSpeed * Time.deltaTime;
        }
    }
}
