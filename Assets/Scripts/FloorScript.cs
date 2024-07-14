using System;
using UnityEngine;

public class FloorScript : MonoBehaviour
{
    public GameObject LevelParent;
    public int AxisLock = 0; // 0 = both, 1 = vertical, 2 = horizontal, 4 = disabled
    private GameObject _ballGameObject;
    private Rigidbody _ballRigidbody;
    private GameObject _fadeGameObject;

    public GyroMethod currentGyroMethod = GyroMethod.RotationRateUnbiased; // Default method
    private bool gyroEnabled;
    public Vector3 turnVector;
    public Vector3 inputVector;
    [Range(2,10)][SerializeField] public int sensitivity;
    
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

        gyroEnabled = SystemInfo.supportsGyroscope;
        if (gyroEnabled)
        {
            Input.gyro.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        SetFloorRotation();

       // inputVector = GetGyroInput();
        inputVector = GetAccelerometerInput();

        _ballRigidbody.AddForce(new Vector3(
            Mathf.Clamp(inputVector.x, -sensitivity, sensitivity),
            0, Mathf.Clamp(inputVector.z, -sensitivity, sensitivity)) / 15, ForceMode.Impulse);
        
        if (AxisLock != 4)
        {
            turnVector = new Vector3(
                Mathf.Clamp(inputVector.z, -sensitivity, sensitivity), 0, Mathf.Clamp(inputVector.x, -sensitivity, sensitivity));
            // turnVector = new Vector3(inputVector.y, 0, inputVector.x);
            
            transform.Rotate(turnVector);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            _fadeGameObject.GetComponent<FunctionsScript>().SceneToLoad = "MainMenu";
            _fadeGameObject.GetComponent<Animator>().Play("FadeOnAnim");
        }
    }
    
    private void SetFloorRotation()
    {
        Vector3 rot = new Vector3(transform.rotation.eulerAngles.x < 150
                ? Mathf.Clamp(transform.rotation.eulerAngles.x * 0.95f, -35, 35)
                : Mathf.Clamp(((transform.rotation.eulerAngles.x - 360) * 0.95f) + 360, 360 - 35, 360 + 35), 0,
            transform.rotation.eulerAngles.z < 150
                ? Mathf.Clamp(transform.rotation.eulerAngles.z * 0.95f, -35, 35)
                : Mathf.Clamp(((transform.rotation.eulerAngles.z - 360) * 0.95f) + 360, 360 - 35, 360 + 35));

        transform.rotation = Quaternion.Euler(Vector3.zero);
        LevelParent.transform.parent = null;

        transform.position = _ballGameObject.transform.position;
        LevelParent.transform.parent = transform;
        transform.eulerAngles = rot;
    }

    private Vector3 GetGyroInput()
    {
        if (!gyroEnabled) return Vector3.zero;

        Vector3 inputVector = Vector3.zero;

        switch (currentGyroMethod)
        {
            case GyroMethod.Attitude:
                Quaternion gyroAttitude = Input.gyro.attitude;
                inputVector.x = AxisLock != 1 ? gyroAttitude.eulerAngles.y : 0;
                inputVector.z = AxisLock != 2 ? gyroAttitude.eulerAngles.x : 0;
                break;
            case GyroMethod.RotationRate:
                Vector3 gyroRotationRate = Input.gyro.rotationRate;
                inputVector.x = AxisLock != 1 ? gyroRotationRate.y : 0;
                inputVector.z = AxisLock != 2 ? -gyroRotationRate.x : 0;
                break;
            case GyroMethod.RotationRateUnbiased:
                Vector3 gyroRotationRateUnbiased = Input.gyro.rotationRateUnbiased;
                inputVector.x = AxisLock != 1 ? gyroRotationRateUnbiased.y : 0;
                inputVector.z = AxisLock != 2 ? -gyroRotationRateUnbiased.x : 0;
                break;
            case GyroMethod.UserAcceleration:
                Vector3 gyroAcceleration = Input.gyro.userAcceleration;
                inputVector.x = AxisLock != 1 ? gyroAcceleration.y : 0;
                inputVector.z = AxisLock != 2 ? gyroAcceleration.x : 0;
                break;
        }

        return inputVector;
    }
    
    private Vector3 GetAccelerometerInput()
    {
        Vector3 accelerometerInput = Vector3.zero;

        Vector3 acceleration = Input.acceleration;
        accelerometerInput.x = AxisLock != 1 ? acceleration.y : 0;
        accelerometerInput.z = AxisLock != 2 ? acceleration.x : 0;

        return accelerometerInput;
    }
}