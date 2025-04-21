using System;
using UnityEngine;

public enum ControlMethod
{
    Accelerometer,
    Gyroscope,
    Touch
}

public class FloorScript : MonoBehaviour
{
    public GameObject LevelParent;
    public int AxisLock = 0; // 0 = both, 1 = vertical, 2 = horizontal, 4 = disabled
    private GameObject _ballGameObject;
    private Rigidbody _ballRigidbody;
    private GameObject _fadeGameObject;

    public ControlMethod controlMethod = ControlMethod.Accelerometer; // Default method
    private bool gyroEnabled;
    private Vector3 smoothedAcceleration;
    
    [Range(1, 10)] public int sensitivity = 5;
    [Range(0.1f, 1.0f)] public float smoothing = 0.2f;
    public float maxTiltAngle = 25f; // Maximum tilt angle in degrees
    
    public void ChangeSensitivity(int amount)
    {
        sensitivity += amount;
        sensitivity = Mathf.Clamp(sensitivity, 1, 10);
    }
    
    private void Awake()
    {
        _fadeGameObject = GameObject.FindGameObjectWithTag("Fade");
        _ballGameObject = GameObject.FindGameObjectWithTag("Player");
        _ballRigidbody = _ballGameObject.GetComponent<Rigidbody>();
        smoothedAcceleration = Vector3.zero;

        // Check if device has gyroscope
        gyroEnabled = SystemInfo.supportsGyroscope;
        if (gyroEnabled)
        {
            Input.gyro.enabled = true;
        }
        else if (controlMethod == ControlMethod.Gyroscope)
        {
            // Fall back to accelerometer if gyro isn't available
            controlMethod = ControlMethod.Accelerometer;
            Debug.Log("Gyroscope not available, falling back to accelerometer");
        }
    }

    private void FixedUpdate()
    {
        // Get input based on selected control method
        Vector3 inputVector = Vector3.zero;
        
        switch (controlMethod)
        {
            case ControlMethod.Accelerometer:
                inputVector = GetAccelerometerInput();
                break;
            case ControlMethod.Gyroscope:
                if (gyroEnabled)
                    inputVector = GetGyroInput();
                break;
            case ControlMethod.Touch:
                inputVector = GetTouchInput();
                break;
        }

        // Move floor to center on ball
        transform.position = _ballGameObject.transform.position;
        
        // Apply rotation based on input
        if (AxisLock != 4) // If not disabled
        {
            ApplyRotation(inputVector);
        }

        // Limit and apply force to the ball
        _ballRigidbody.AddForce(new Vector3(
            Mathf.Clamp(inputVector.x, -sensitivity, sensitivity),
            0, 
            Mathf.Clamp(inputVector.z, -sensitivity, sensitivity)) * (sensitivity * 0.2f), 
            ForceMode.Acceleration);

        // Handle escape key for menu
        if (Input.GetKey(KeyCode.Escape))
        {
            _fadeGameObject.GetComponent<FunctionsScript>().SceneToLoad = "MainMenu";
            _fadeGameObject.GetComponent<Animator>().Play("FadeOnAnim");
        }
    }
    
    private void ApplyRotation(Vector3 inputVector)
    {
        float targetAngleX = 0;
        float targetAngleZ = 0;
        
        // Calculate target rotation based on input
        if (AxisLock != 2) // Not locked horizontally
            targetAngleX = -inputVector.z * maxTiltAngle;
            
        if (AxisLock != 1) // Not locked vertically
            targetAngleZ = inputVector.x * maxTiltAngle;

        // Create a quaternion with our target rotation
        Quaternion targetRotation = Quaternion.Euler(targetAngleX, 0, targetAngleZ);
        
        // Smoothly rotate towards target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        
        // Make sure level parent stays attached to the floor
        LevelParent.transform.parent = transform;
    }

    private Vector3 GetAccelerometerInput()
    {
        Vector3 rawAcceleration = Input.acceleration;
        
        // For a device lying flat, we need to map differently:
        // X acceleration (device left/right tilt) -> Z force (forward/backward)
        // Y acceleration (device forward/back tilt) -> X force (left/right)
        Vector3 mappedAcceleration = new Vector3(rawAcceleration.x, 0, -rawAcceleration.y);
        
        // Apply smoothing
        smoothedAcceleration = Vector3.Lerp(smoothedAcceleration, mappedAcceleration, smoothing);
        
        return smoothedAcceleration;
    }

    private Vector3 GetGyroInput()
    {
        if (!gyroEnabled) return Vector3.zero;

        // Convert the gyro rotation to device space
        Quaternion gyroAttitude = Input.gyro.attitude;
        Quaternion rotFix = new Quaternion(0, 0, 1, 0); // Fix for different coordinate systems
        Quaternion deviceRotation = gyroAttitude * rotFix;
        
        // Extract the relevant angles for tilting
        Vector3 deviceEuler = deviceRotation.eulerAngles;
        
        // For a device lying flat, adjust readings accordingly
        float tiltX = deviceEuler.x;
        if (tiltX > 180) tiltX -= 360;
        
        float tiltZ = deviceEuler.z;
        if (tiltZ > 180) tiltZ -= 360;
        
        Vector3 inputVector = new Vector3(
            AxisLock != 1 ? tiltZ / 90f : 0,
            0,
            AxisLock != 2 ? -tiltX / 90f : 0
        );
        
        return inputVector;
    }
    
    private Vector3 GetTouchInput()
    {
        Vector3 touchInput = Vector3.zero;
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            // Map touch position relative to screen center
            Vector2 touchPos = new Vector2(
                (touch.position.x / Screen.width) * 2 - 1,
                (touch.position.y / Screen.height) * 2 - 1
            );
            
            touchInput.x = AxisLock != 1 ? touchPos.x : 0;
            touchInput.z = AxisLock != 2 ? touchPos.y : 0;
        }
        
        return touchInput;
    }
}
