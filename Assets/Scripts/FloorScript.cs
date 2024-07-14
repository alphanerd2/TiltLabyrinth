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
    public int sensitivity = 1;
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

        inputVector = GetGyroInput();

        // _ballRigidbody.AddForce(new Vector3(
        //     Mathf.Clamp(inputVector.x, -1, 1), 0, Mathf.Clamp(inputVector.z, -1, 1)) / 15, ForceMode.Impulse);

        
        if (AxisLock != 4)
        {
            turnVector = new Vector3(
                Mathf.Clamp(inputVector.z, -sensitivity, sensitivity), 0, Mathf.Clamp(inputVector.x, -sensitivity, sensitivity));
            transform.Rotate(turnVector);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            _fadeGameObject.GetComponent<FunctionsScript>().SceneToLoad = "MainMenu";
            _fadeGameObject.GetComponent<Animator>().Play("FadeOnAnim");
        }
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
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

    // Method to switch to Attitude gyroscope method
    public void SwitchGyroMethodAttitude()
    {
        currentGyroMethod = GyroMethod.Attitude;
    }

    // Method to switch to RotationRate gyroscope method
    public void SwitchGyroMethodRotationRate()
    {
        currentGyroMethod = GyroMethod.RotationRate;
    }

    // Method to switch to RotationRateUnbiased gyroscope method
    public void SwitchGyroMethodRotationRateUnbiased()
    {
        currentGyroMethod = GyroMethod.RotationRateUnbiased;
    }

    // Method to switch to UserAcceleration gyroscope method
    public void SwitchGyroMethodUserAcceleration()
    {
        currentGyroMethod = GyroMethod.UserAcceleration;
    }

    public void IncreaseSensitivity()
    {
        sensitivity += 1;
    }
    
    public void DecreaseSensitivity()
    {
        sensitivity -= 1;
    }
}