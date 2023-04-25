using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO.Ports;
using Tobii.Gaming;

public class GameJoypad001 : MonoBehaviour
{
    // Joypad Initialization
    public string portName = "COM4"; // Change this to the name of the serial port connected to Arduino
    public int baudRate = 9600; // Should match the baud rate defined in Arduino code

    private SerialPort serialPort;
    private bool ButtonHeld = false;
    private float ThresholdPress = 750;
    private float ThresholdRelease = 100;
    private float buttonHoldDuration = 1; // Adjust this value to set the desired button hold duration in seconds
    private float ButtonTimer = 0f;

    // Include Boolean logic for L / R  Hand release tap
    public bool LeftHandJoy;
    public bool RightHandJoy;

    public int LeftCount;
    public int RightCount;

    // Physics Ray cast for Cam and Eye Tracker
    private RaycastHit2D _hit;
    private RaycastHit2D hit;

    // max range (900,475
    public float Pos_x, Pos_y;

    // Serialized Vars
    [SerializeField]
    private Text gameText;

    [SerializeField]
    public GameObject StimulusCanvas, PromptCanvas, RewardCanvas, PauseCanvas, StimulusObj;

    [SerializeField]
    public float reactionTime, holdTimer, liftTime, startTime, randomDelayBeforeMeasuring, timeOverall, StimulusAppear, timer, miss_Reaction, touchTime, forceSensor1, forceSensor2, visualGaze, recogTime;

    [SerializeField]
    private float[] TimeDuration = { 3.0f, 5.0f, 7.0f };

    // Provide Boolean Logic for Button
    [SerializeField]
    private bool HoldTimeComplete;

    // Provide boolean logic for Stimulus / Prompt 
    [SerializeField]
    public bool StimulusCall, PromptCall, RewardCall, PauseCall;

    [SerializeField]
    public int NumOfTaps, missTap, hitTap;

    [SerializeField]
    public bool ObjectDetect;
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        // Used for equation
        reactionTime = 0f;
        startTime = 0f;
        timer = 3f;
        touchTime = 0f;
        miss_Reaction = 0f;

        // Random Time generator
        randomDelayBeforeMeasuring = 0f;

        // Text Manipulation
        gameText.text = "Hold Down Buttons to Start";
        // Initialize the serial port
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
        serialPort.ReadTimeout = 100;

        // Start reading data from Arduino in a separate thread
        StartCoroutine(ReadDataFromArduino());

        HoldTimeComplete = false;
        LeftHandJoy = false;
        RightHandJoy = false;

        LeftCount = 0;
        RightCount= 0;

        // Canvas
        RewardCall = false;
        StimulusCall = false;
        PauseCall = false;
        StimulusCanvas.SetActive(false); PauseCanvas.SetActive(false);
        PromptCall = true;
        PromptCanvas.SetActive(true);


        // Object detect boolean
        ObjectDetect = false;
        PositionChanger();

    }
    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
    void UpdateSensor(int buttonState1, int buttonState2, int forceSensor1Value, int forceSensor2Value)
    {
        if ((buttonState1 == 1 && buttonState2 == 1) && (forceSensor1Value >= ThresholdPress && forceSensor1Value >= ThresholdPress))
        {
            if (!ButtonHeld && (forceSensor1Value >= ThresholdPress && forceSensor1Value >= ThresholdPress)) // press
            {
                startTime = Time.time;      //Beginning of Time also tap
                Debug.Log("Press time is: " + startTime);
                gameText.text = ("Keep Holding Down Buttons...");
                ButtonTimer = Time.time;
                ButtonHeld = true;
                forceSensor1 = forceSensor1Value;
                forceSensor2 = forceSensor2Value;
            }
            else if (Time.time - ButtonTimer >= buttonHoldDuration) //hold
            {
                // Keeps the startTime when first tapped subtracting with Overall time
                holdTimer = Time.time - startTime;

                if (holdTimer > timer && !HoldTimeComplete)
                {
                    // If held isnt complete but Stimulus Canvas not appeared
                    Debug.Log("calling routine");
                    StartCoroutine("StartMeasuring");
                }
                else if (holdTimer > timer && HoldTimeComplete)
                {
                    // Registering as HoldTimeComplete
                    if (StimulusCall)
                    {
                        StopCoroutine("StartMeasuring");
                        Debug.Log("Routine stops at " + Time.time);
                    }
                    // Held is complete but Stimulus hasn't appeared yet
                    else
                    {
                        Debug.Log("Waiting for Stimulus to appear...");
                    }
                }
            }
        }
        if((buttonState1 == 0 && buttonState2 == 0) && (forceSensor1Value >= ThresholdPress && forceSensor1Value >= ThresholdPress))
        {
            Debug.Log("HERE");
        }
        // release
        if (((buttonState1 == 0 && buttonState2 == 0) && ButtonHeld) && (ThresholdRelease <= forceSensor1Value && forceSensor1Value <= ThresholdPress)) //release
        {
            liftTime = Time.time;   // Time lifted = overall time - start of tap (Reaction Time)
            StopCoroutine("StartMeasuring");
            if (!StimulusCall)
            {
                Debug.Log("too early!");        // working here 
                textChange("Please try again and place hands on buttons");
                HoldTimeComplete = false;
            }
            else if (StimulusCall)
            {
                recogTime = liftTime - StimulusAppear;      // Recognition time
                Debug.Log("Good job you did it");
            }
            ButtonHeld = false;
            ButtonTimer = 0f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            NumOfTaps++;
            touchTime = Time.time;
            // Retrieve visual gaze

            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.transform.tag == "Stimulus")
            {
                reactionTime = touchTime - StimulusAppear;
                // Tap Count
                hitTap++;
                Debug.Log("Object Hit is: " + hit.collider.gameObject.name);

                // Switch Reward Canvas
                StartCoroutine("RewardSystem");
            }
            else if (hit.transform.tag == "Background")
            {
                //
                miss_Reaction = touchTime - StimulusAppear;
                // Missed tap
                missTap++;
                Debug.Log("Object hit is: " + hit.collider.gameObject.name);

            }
            else
            {

                missTap++;
                Debug.Log("Too early buddy");
            }
        }

        
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        if (gazePoint.IsValid && StimulusCall == true)
        {
            Vector2 gazePointPosition = gazePoint.Screen;


            _hit = Physics2D.Raycast(gazePointPosition, Vector2.zero);
            if (_hit.collider != null && _hit.collider.CompareTag("Stimulus"))
            {
                Debug.Log("Hit Object: " + _hit.collider.name + " , " + gazePoint.Screen.x + ", " + gazePoint.Screen.y);
                if (ObjectDetect)
                {

                    Debug.Log("Its true you hit it!");
                }
                else
                {
                    // Save first time here
                    visualGaze = Time.time;     // Time first detected
                    Debug.Log("You hit it now,time to store");
                    ObjectDetect = true;
                }
            }
            else
            {

            }
        }
        if (Input.GetKeyDown("escape"))
        {
            // Pausing the Game
            PauseToggle();

        }


    }

    IEnumerator ReadDataFromArduino()
    {
        while (true)
        {
            try
            {
                string data = serialPort.ReadLine();
                //Debug.Log("Received from Arduino: " + data);

                // Parse the received data
                string[] values = data.Split(',');
                if (values.Length >= 4)
                {
                    int buttonState1 = int.Parse(values[0]);
                    int buttonState2 = int.Parse(values[1]);
                    int forceSensor1Value = int.Parse(values[2]);
                    int forceSensor2Value = int.Parse(values[3]);

                    // Detect button actions
                    UpdateSensor(buttonState1, buttonState2, forceSensor1Value, forceSensor2Value);
                }
            }
            catch (System.TimeoutException)
            {
                // Ignore timeout exceptions
            }

            yield return null;
        }
    }

    IEnumerator StartMeasuring()
    {
        // Random time generator
        randomDelayBeforeMeasuring = Random.Range(1f, 5f);
        Debug.Log("Random time is..." + randomDelayBeforeMeasuring);
        HoldTimeComplete = true;
        yield return new WaitForSeconds(randomDelayBeforeMeasuring);

        // HoldTimeComplete = true;
        // Switching Canvas
        StimulusToggle();

    }

    IEnumerator RewardSystem()
    {
        RewardToggle();
        yield return new WaitForSeconds(6);
        PromptToggle();
        Debug.Log("Im calling Reward...");
    }

    private void PromptToggle()
    {
        HoldTimeComplete = false;

        StimulusCall = false;
        StimulusCanvas.SetActive(false);

        RewardCall = false;
        RewardCanvas.SetActive(false);

        PromptCall = true;
        PromptCanvas.SetActive(PromptCall);

        Debug.Log("Prompt is activating...");
    }

    private void StimulusToggle()
    {

        HoldTimeComplete = true;
        Debug.Log("Changing the canvas now... " + Time.time);

        PromptCall = false;
        PromptCanvas.SetActive(false);

        RewardCall = false;
        RewardCanvas.SetActive(false);

        StimulusCall = true;
        StimulusCanvas.SetActive(StimulusCall);
        StimulusAppear = Time.time;         // The time Stimulus appears

    }

    private void RewardToggle()
    {

        StimulusCall = false;
        StimulusCanvas.SetActive(false);

        PromptCall = false;
        PromptCanvas.SetActive(false);

        RewardCall = true;
        RewardCanvas.SetActive(RewardCall);
        Debug.Log("YAY REWARDINGGGG!");

        // generates random stimulus position
        PositionChanger();
    }
    private void textChange(string _text)
    {
        Debug.Log("textChange is being called!");
        gameText.text = _text;
    }

    private void textPrompt()
    {
        gameText.text = "Hold Down buttons again!";
    }

    public void PauseToggle()
    {
        // Same applies for Audio
        if (!PauseCall)     // False
        {
            PauseCall = !PauseCall;
            PauseCanvas.SetActive(PauseCall);

            Time.timeScale = 0f; //Time is paused
        }
        else
        {
            Time.timeScale = 1f;
            PauseCall = !PauseCall;
            PauseCanvas.SetActive(PauseCall);

            //AudioListener.pause = true;
        }
    }
    public void PositionChanger()
    {
        Pos_x = Random.Range(60.0f, 1800.0f);
        Pos_y = Random.Range(0.0f, 1000.0f);
        Debug.Log("Position are: " + Pos_x + ", " + Pos_y);
        var ObjPos = new Vector2(Pos_x, Pos_y);
        StimulusObj.transform.position = ObjPos;
    }
}

