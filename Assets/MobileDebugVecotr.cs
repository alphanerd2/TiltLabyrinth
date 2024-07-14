using TMPro;
using UnityEngine;

public class MobileDebugVector : MonoBehaviour
{
    [SerializeField] private GameObject floorObject;
    [SerializeField] private TextMeshProUGUI textTurnVector;
    [SerializeField] private TextMeshProUGUI textInputVector;
    [SerializeField] private TextMeshProUGUI textSensitivity;

    private FloorScript _floorScript;

    private void Awake()
    {
        _floorScript = floorObject.GetComponent<FloorScript>();
    }

    void Start()
    {
        textTurnVector.text = _floorScript.turnVector.ToString();
        textInputVector.text = _floorScript.inputVector.ToString();
        textSensitivity.text = _floorScript.sensitivity.ToString();
    }

    void Update()
    {
        textTurnVector.text = _floorScript.turnVector.ToString();
        textInputVector.text = _floorScript.inputVector.ToString();
        textSensitivity.text = _floorScript.sensitivity.ToString();
    }
    
    public void ChangeSensitivity(int amount)
    {
        floorObject.GetComponent<FloorScript>().ChangeSensitivity(amount);
    }
    
    // Method to switch to Attitude gyroscope method
    public void SwitchGyroMethodAttitude()
    {
        floorObject.GetComponent<FloorScript>().currentGyroMethod = GyroMethod.Attitude;
    }

    // Method to switch to RotationRate gyroscope method
    public void SwitchGyroMethodRotationRate()
    {
        floorObject.GetComponent<FloorScript>().currentGyroMethod = GyroMethod.RotationRate;
    }

    // Method to switch to RotationRateUnbiased gyroscope method
    public void SwitchGyroMethodRotationRateUnbiased()
    {
        floorObject.GetComponent<FloorScript>().currentGyroMethod = GyroMethod.RotationRateUnbiased;
    }

    // Method to switch to UserAcceleration gyroscope method
    public void SwitchGyroMethodUserAcceleration()
    {
        floorObject.GetComponent<FloorScript>().currentGyroMethod = GyroMethod.UserAcceleration;
    }
}