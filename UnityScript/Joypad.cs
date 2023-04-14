using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class SerialController : MonoBehaviour
{
    // Variables for serial port communication
    public string portName = "COM3"; // Change to match your Arduino port
    public int baudRate = 9600;
    private SerialPort serialPort;

    void Start()
    {
        // Open the serial port
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
    }

    void Update()
    {
        // Read the serial input
        if (serialPort.IsOpen)
        {
            string serialInput = serialPort.ReadLine();
            Debug.Log(serialInput);
        }
    }

    void OnApplicationQuit()
    {
        // Close the serial port when the application is closed
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
public class ButtonController : MonoBehaviour
{
    // Variables for button input
    public int button1Pin = 2; // Change to match your button pins
    public int button2Pin = 3;
    private bool button1State = false;
    private bool button2State = false;

    // Variables for serial port communication
    public SerialController serialController;

    void Start()
    {
        // Set the button pins to input mode
        pinMode(button1Pin, INPUT);
        pinMode(button2Pin, INPUT);
    }

    void Update()
    {
        // Check the button states
        bool newButton1State = digitalRead(button1Pin);
        bool newButton2State = digitalRead(button2Pin);

        // Send button data to Arduino
        if (newButton1State != button1State || newButton2State != button2State)
        {
            // Construct the data string and send it to the Arduino
            string dataString = "BUTTON|" + (newButton1State ? "1" : "0") + "|" + (newButton2State ? "1" : "0") + "\n";
            serialController.SendSerialMessage(dataString);

            // Update the button states
            button1State = newButton1State;
            button2State = newButton2State;
        }
    }
}
public class ForceSensorController : MonoBehaviour
{
    // Variables for force sensor input
    public int forceSensorPin = 0; // Change to match your force sensor pin
    private int forceSensorValue = 0;

    // Variables for serial port communication
    public SerialController serialController;

    void Start()
    {
        // Set the force sensor pin to input mode
        pinMode(forceSensorPin, INPUT);
    }

    void Update()
    {
        // Read the force sensor value
        int newForceSensorValue = analogRead(forceSensorPin);

        // Send force sensor data to Arduino
        if (newForceSensorValue != forceSensorValue)
        {
            // Construct the data string and send it to the Arduino
            string dataString = "FORCE|" + newForceSensorValue.ToString() + "\n";
            serialController.SendSerialMessage(dataString);

            // Update the force sensor value
            forceSensorValue = newForceSensorValue;
        }

        // Display the force sensor value on the screen
        TextMesh textMesh = GetComponent<TextMesh>();
        textMesh.text = "Force: " + forceSensorValue.ToString();
    }
}
public class ButtonController1 : MonoBehaviour
{
    // Variables for button input
    public KeyCode button1Key;
    public KeyCode button2Key;
    private int button1Value = 0;
    private int button2Value = 0;

    // Variables for serial port communication
    public SerialController serialController;

    void Update()
    {
        // Check if button 1 is pressed or released
        if (Input.GetKeyDown(button1Key))
        {
            button1Value = 1;
            SendButtonData();
        }
        else if (Input.GetKeyUp(button1Key))
        {
            button1Value = 0;
            SendButtonData();
        }

        // Check if button 2 is pressed or released
        if (Input.GetKeyDown(button2Key))
        {
            button2Value = 1;
            SendButtonData();
        }
        else if (Input.GetKeyUp(button2Key))
        {
            button2Value = 0;
            SendButtonData();
        }
    }

    // Sends button data to Arduino
    void SendButtonData()
    {
        // Construct the data string and send it to the Arduino
        string dataString = "BUTTON|" + button1Value.ToString() + "|" + button2Value.ToString() + "\n";
        serialController.SendSerialMessage(dataString);
    }
}