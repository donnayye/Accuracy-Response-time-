using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEngine.InputSystem;



public class Prompt1 : MonoBehaviour
{
    // Serialized Vars
    [SerializeField]
    private Text gameText;

    [SerializeField]
    private GameObject StimulusCanvas, PromptCanvas, RewardCanvas;


    [SerializeField]
    public float reactionTime, holdTimer, liftTime, startTime, randomDelayBeforeMeasuring, timeOverall, deltaTimer, timer;

    [SerializeField]
    private float[] TimeDuration = { 3.0f, 5.0f, 7.0f };

    //Help provide game logic
    private bool clockIsTicking, timerCanBeStopped;

    // Provide Boolean Logic for Button
    [SerializeField]
    private bool HoldTimeComplete;

    // Provide boolean logic for Stimulus / Prompt 
    [SerializeField]
    private bool StimulusCall, PromptCall, RewardCall;

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

        // Canvas
        RewardCall = false;
        StimulusCall = false;
        PromptCall = true;
        PromptCanvas.SetActive(true);
    }
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            startTime = Time.time;      //Beginning of Time also tap
            Debug.Log("Press time is: " + startTime);
            gameText.text = "Keep Holding Down Buttons & Wait for shape to appear";
        }
        if (Input.GetKey("space"))
        {
            // Keeps the startTime when first tapped subtracting with Overall time
            holdTimer = Time.time - startTime;

            if (holdTimer > timer && !HoldTimeComplete)
            {
                // If held is complete but Stimulus Canvas not appeared
                Debug.Log("calling routine");
                StartCoroutine("StartMeasuring");
            }
            else if (holdTimer > timer && HoldTimeComplete)
            {
                if(StimulusCall)
                {
                    StopCoroutine("StartMeasuring");
                    Debug.Log("Routine stops at "+ Time.time);
                }
                // Held is complete & Stimulus appeared
                else
                {
                    Debug.Log("Waiting for Stimulus to appear...");
                }
               

            }
        }
        if (Input.GetKeyUp("space"))
        {
            liftTime = Time.time - startTime;   // Time lifted = overall time - start of tap (Reaction Time)
            StopCoroutine("StartMeasuring");
            if(!StimulusCall&& holdTimer > timer)
            {
                Debug.Log("too early!");        // working here 

            }
        }



    }
    IEnumerator StartMeasuring()
    {
        // Random time generator
        randomDelayBeforeMeasuring = Random.Range(1f, 5f);
        Debug.Log("Random time is..." + randomDelayBeforeMeasuring);
        HoldTimeComplete= true;
        yield return new WaitForSeconds(randomDelayBeforeMeasuring);

       // HoldTimeComplete = true;
        // Switching Canvas
        StimulusToggle();

    }

    
    private void PromptToggle(bool NoLogic)
    {
        HoldTimeComplete = false;
        
        if (StimulusCall || RewardCall)
        {
            StimulusCall = false;
            StimulusCanvas.SetActive(false);

            RewardCall = false;
            RewardCanvas.SetActive(false);

            PromptCall = true;
            PromptCanvas.SetActive(PromptCall);

        }

    }

    private void StimulusToggle()
    {
        HoldTimeComplete = true;
        Debug.Log("Changing the canvas now... " + Time.time);
        
        PromptCall = false;
        PromptCanvas.SetActive(false);

        RewardCall = false;
        RewardCanvas.SetActive(false);

        StimulusCall= true;
        StimulusCanvas.SetActive(StimulusCall);

    }

    private void RewardToggle(bool YesLogic)
    {
        HoldTimeComplete = false;
        
        if (StimulusCall || RewardCall)
        {
            StimulusCall = false;
            StimulusCanvas.SetActive(false);

            PromptCall = false;
            PromptCanvas.SetActive(false);

            RewardCall = true;
            RewardCanvas.SetActive(RewardCall);

        }

    }
}

