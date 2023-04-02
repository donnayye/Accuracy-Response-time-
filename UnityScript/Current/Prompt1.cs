using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;      // Isnt required yet



public class Prompt : MonoBehaviour
{


    // Serialized Vars
    [SerializeField]
    private Text gameText;

    [SerializeField]
    private GameObject StimulusCanvas;

    [SerializeField]
    private GameObject CurtainCanvas;

    [SerializeField]
    public float reactionTime, holdTimer, liftTime, startTime, randomDelayBeforeMeasuring, timeOverall, deltaTimer, timer;

    [SerializeField]
    private float[] TimeDuration = { 3.0f, 5.0f, 7.0f };

    //Help provide game logic
    private bool clockIsTicking, timerCanBeStopped;

    // Provide Boolean Logic for Button
    private bool HoldTimeComplete;

    // Provide boolean logic for Stimulus / Prompt 
    private bool StimulusCall, PromptCall, stimulusCheck, promptCheck;

    [SerializeField]
    public int NumOfTaps, missTap, hitTap;

    // Start is called before the first frame update
    void Start()
    {
        // Initializing the beginnging of the Game
        stimulusCheck = false;
        StimulusCanvas.SetActive(false);
        promptCheck = true;
        CurtainCanvas.SetActive(true);

        // Used for equation
        reactionTime = 0f;
        startTime = 0f;
        timer = 3f;
        
        // Random Time generator
        randomDelayBeforeMeasuring = 0f;

        // Text Manipulation
        gameText.text = "Hold Down Buttons to Start";

        // Boolean 
        clockIsTicking = false;     //  for reaction time Boolean
        timerCanBeStopped = true;   // 
        HoldTimeComplete = false;   // If user completes holding for certain amount of time
        StimulusCall= false;        // Canvas enable/disable boolean
        PromptCall = true;

        NumOfTaps = 0;
        missTap = 0;
        hitTap = 0;


    }
    void Update()
    {
        // BUTTON INPUT
        if (Input.GetKeyDown("space"))      // Initialize KeyDown
        {
            timeOverall = Time.time;
            Debug.Log("Time is: " + timeOverall);
            Debug.Log("The Delta time is: " + deltaTimer);
            Debug.Log("Tapped!");
            gameText.text = "Hold Down Buttons to start...";
        }
        // Constant Hold on joypad button
        if(Input.GetKey("space"))     //Successful hold or not
        {
            holdTimer = Time.time - timeOverall;
            Debug.Log("Holder timer: "+ holdTimer);
            // If button held down for more than 3 seconds, condition is completed.
            if (holdTimer > timer && !HoldTimeComplete)
            {   
                // Boolean Logic  NEED TO DO SOMETHING WITH THIS
                clockIsTicking= true;
                timerCanBeStopped= false;

                // If held longer than 3s
                HoldTimeComplete = true;
            }
            else if (HoldTimeComplete == true)
            {
                Debug.Log("Hold Complete");
                if (promptCheck == true)    //coroutine call
                {
                    // Checks if the canvas is enabled or disable
                    promptCheck = TogglePromptCanvas(promptCheck);
                    stimulusCheck = ToggleStimulusCanvas(stimulusCheck);
                }
                else // If Prompt Scene is Disabled and Still holding
                {
                    Debug.Log("Prompt is false now..");
                }
               
            }
        }
             // Too early condition 
        else if (Input.GetKeyUp("space") && !HoldTimeComplete)
        {
            liftTime = Time.time;
            HoldTimeComplete = false;
            Debug.Log("Key has been lifted at "+ liftTime);

            // Reset holdTimer
            holdTimer = 0f;

                 //StopCoroutine
            clockIsTicking= false;
            timerCanBeStopped = true;
            gameText.text = "Too Early!\n" + "\nHold Down button to start again";
        }
        else if(Input.GetKeyUp("space") && HoldTimeComplete == true)
        {
            liftTime = Time.time;
            HoldTimeComplete = false;   // Boolean to false to ensure Hold resets

           


        }
        /////////////////////////////////////////////////////////////////////
        ///     MOUSE INPUT 

        if (Input.GetMouseButtonDown(0))
        {
            NumOfTaps++;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if(hit.transform.tag == "Stimulus")
            {
                // HIT
                hitTap++;
                Debug.Log("Target name: "+ hit.collider.gameObject.name);

                // Start the Prompting Scene again 
                Debug.Log("Starting the Prompt Scene again...");
                promptCheck= TogglePromptCanvas(promptCheck);
                stimulusCheck = ToggleStimulusCanvas(stimulusCheck);

            }
            else if(hit.transform.tag == "Background")
            {
                // Missed
                missTap++;
                Debug.Log("Target name: " + hit.collider.gameObject.name);

            }
            else 
            {
                missTap++;
                // Hitting anything else like Prompt scene
                Debug.Log("too early buddy");
            }
        }
    }

    // Function Generates when the Stimulus is displayed
        // Records the timer of Stimulus 
    private IEnumerator StartMeasuring()
    {
        randomDelayBeforeMeasuring = Random.Range(0.5f, 8f);
        yield return new WaitForSeconds(randomDelayBeforeMeasuring);
        
        // manipulate Prompt Canvas to Stimulus Canvas 

        clockIsTicking= true;
        timerCanBeStopped = true;
        //Debug.Log("Finish with Coroutine!");
    }
    private void RandomTimeGenerator()
    {
        // Time generator for Stimulus appearance

    }

    public bool TogglePromptCanvas(bool val2)
    {
        val2 = !val2;
        CurtainCanvas.SetActive(val2);
        return val2;
    }

    public bool ToggleStimulusCanvas(bool val)
    {
        val = !val;
        StimulusCanvas.SetActive(val);
        return val;
    }




}
