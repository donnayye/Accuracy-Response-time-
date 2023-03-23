using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;



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
    private bool StimulusCall, PromptCall;

    // Start is called before the first frame update
    void Start()
    {
        // Used for equation
        reactionTime = 0f;
        startTime = 0f;
        timer = 3f;
        
        // Random Time generator
        randomDelayBeforeMeasuring = 0f;

        // Text Manipulation
        gameText.text = "Hold Down Buttons to Start";

        // Boolean 
        clockIsTicking = false;
        timerCanBeStopped = true;
        HoldTimeComplete = false;
        StimulusCall= false;
        PromptCall = true;
    }
    void Update()
    {
        // Recieve Constant Hold down button from Arduino Joypad buttons
        // When first placing pressure onto Joypads
        if (Input.GetKeyDown("space"))
        {
            timeOverall = Time.time;
            Debug.Log("Time is: " + timeOverall);
            Debug.Log("The Delta time is: " + deltaTimer);
            Debug.Log("Tapped!");
            
           
            
        }
        // Constant Hold on joypad button
        else if (Input.GetKey("space"))
        {
            holdTimer = Time.time - timeOverall;
            // If button held down for more than 3 seconds, condition is completed.
            if (holdTimer > timer)
            {   
                
                clockIsTicking= true;
                timerCanBeStopped= false;
                HoldTimeComplete = true;

                // Here we want to implement the a function that pauses this routine
                // && disable the Prompt Canvas and enable the stimulus canvas
                // Along with the random generator 
                Debug.Log("Holding time: " + (holdTimer));
               

                // Completed the hold
             
                Debug.Log("Coroutine Starts!");
                StartCoroutine("StartMeasuring");
                Debug.Log("Pausing... for 3 seconds");
        

            }
            else if (HoldTimeComplete)
            {

            }
        }
        // Too early condition 
        else if (Input.GetKeyUp("space"))
        {
            liftTime = Time.time;
            HoldTimeComplete = false;
            Debug.Log("Key has been lifted at "+ liftTime);
            //StopCoroutine
            clockIsTicking= false;
            timerCanBeStopped = true;
            gameText.text = "Too Early!\n" + "\nHold Down button to start again";
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

   // private string textChange(string text1)
    //{

      //  gameText.text = text1;
    //}
    // Function generates the Prompt Canvas to Hold down buttons again
   // private IEnumerator StopMeasuring(){}
    

    
}
