using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class MobileDebugVector : MonoBehaviour
{
    [SerializeField] private GameObject floorObject;
    [SerializeField] private TextMeshProUGUI textTurnVector;
    [SerializeField] private TextMeshProUGUI textInputVector;
    [SerializeField] private TextMeshProUGUI textSensitivity;

    private FloorScript _floorScript;

    // Variables to store previous values
    private Vector3 _previousTurnVector;
    private Vector3 _previousInputVector;
    private int _previousSensitivity;

    private void Awake()
    {
        _floorScript = floorObject.GetComponent<FloorScript>();
    }

    void Start()
    {
        // Initialize the previous values
        _previousTurnVector = _floorScript.turnVector;
        _previousInputVector = _floorScript.inputVector;
        _previousSensitivity = _floorScript.sensitivity;

        // Set initial text values
        textTurnVector.text = _floorScript.turnVector.ToString();
        textInputVector.text = _floorScript.inputVector.ToString();
        textSensitivity.text = _floorScript.sensitivity.ToString(CultureInfo.InvariantCulture);
    }

    void Update()
    {
        // Compare the current values directly with the previous values
        if (_floorScript.turnVector != _previousTurnVector)
        {
            textTurnVector.text = _floorScript.turnVector.ToString();
            _previousTurnVector = _floorScript.turnVector;
        }

        if (_floorScript.inputVector != _previousInputVector)
        {
            textInputVector.text = _floorScript.inputVector.ToString();
            _previousInputVector = _floorScript.inputVector;
        }

        if (_floorScript.sensitivity != _previousSensitivity)
        {
            textSensitivity.text = _floorScript.sensitivity.ToString(CultureInfo.InvariantCulture);
            _previousSensitivity = _floorScript.sensitivity;
        }
    }

    public void IncreaseSensitivity()
    {
        _floorScript.IncreaseSensitivity();
    }

    public void DecreaseSensitivity()
    {
        _floorScript.DecreaseSensitivity();
    }
}
