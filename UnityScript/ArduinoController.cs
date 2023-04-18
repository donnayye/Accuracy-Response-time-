using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO.Ports;



public class ArduinoController : MonoBehaviour
{
    public string portName = "COM3"; // Change this to the name of the serial port connected to Arduino
    public int baudRate = 9600; // Should match the baud rate defined in Arduino code

    private SerialPort serialPort;
    private bool leftButtonHeld = false;
    private bool rightButtonHeld = false;
    private float buttonHoldDuration = 0.5f; // Adjust this value to set the desired button hold duration in seconds
    private float leftButtonTimer = 0f;
    private float rightButtonTimer = 0f;

    void Start()
    {
        // Initialize the serial port
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
        serialPort.ReadTimeout = 100;

        // Start reading data from Arduino in a separate thread
        StartCoroutine(ReadDataFromArduino());
    }

    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }

    IEnumerator ReadDataFromArduino()
    {
        while (true)
        {
            try
            {
                string data = serialPort.ReadLine();
                Debug.Log("Received from Arduino: " + data);

                // Parse the received data
                string[] values = data.Split(',');
                if (values.Length >= 4)
                {
                    int buttonState1 = int.Parse(values[0]);
                    int buttonState2 = int.Parse(values[1]);
                    int forceSensor1Value = int.Parse(values[2]);
                    int forceSensor2Value = int.Parse(values[3]);

                    // Detect button actions
                    DetectButtonActions(buttonState1, buttonState2);
                }
            }
            catch (System.TimeoutException)
            {
                // Ignore timeout exceptions
            }

            yield return null;
        }
    }

    void DetectButtonActions(int buttonState1, int buttonState2)
    {
        // Detect left button actions
        if (buttonState1 == 1)
        {
            if (!leftButtonHeld)
            {
                Debug.Log("Left button initial press detected.");
                leftButtonHeld = true;
                leftButtonTimer = Time.time;
            }
            else if (Time.time - leftButtonTimer >= buttonHoldDuration)
            {
                Debug.Log("Left button hold detected.");
            }
        }
        else if (buttonState1 == 0 && leftButtonHeld)
        {
            Debug.Log("Left button release detected.");
            leftButtonHeld = false;
        }

        // Detect right button actions
        if (buttonState2 == 1)
        {
            if (!rightButtonHeld)
            {
                Debug.Log("Right button initial press detected.");
                rightButtonHeld = true;
                rightButtonTimer = Time.time;
            }
            else if (Time.time - rightButtonTimer >= buttonHoldDuration)
            {
                Debug.Log("Right button hold detected.");
            }
        }
        else if (buttonState2 == 0 && rightButtonHeld)
        {
            Debug.Log("Right button release detected.");
            rightButtonHeld = false;
        }
    }
}


