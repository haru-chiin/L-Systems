using UnityEngine;
using UnityEngine.UI; 

public class CameraRotation : MonoBehaviour
{
    public Transform objectToRotateAround; 
    public float rotationSpeed = 10f; 
    public Slider rotationSlider; 

    void Start()
    {
        rotationSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void Update()
    {
        if (objectToRotateAround != null)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;
            transform.RotateAround(objectToRotateAround.position, Vector3.up, rotationAmount);
        }
    }

    void OnSliderValueChanged(float value)
    {
        rotationSpeed = value;
    }
}
