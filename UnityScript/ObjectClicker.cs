using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Tobii.Gaming;
//using UnityEngine.InputSystem;


// Make sure to have the Script file name the same as "GameTouch" or vice versa
public class GameTouch : MonoBehaviour
{
    // Initialize Audio Stimulus & Gameplay theme song
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
    public float reactionTime, holdTimer, liftTime, startTime, randomDelayBeforeMeasuring, timeOverall, StimulusAppear, timer, miss_Reaction, touchTime, visualGaze, recogTime;

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



        HoldTimeComplete = false;

        // Canvas
        RewardCall = false;
        StimulusCall = false;
        PauseCall = false;
        StimulusCanvas.SetActive(false); PauseCanvas.SetActive(false);
        PromptCall = true;
        PromptCanvas.SetActive(true);
        

        // Object detect boolean
        ObjectDetect= false;
        PositionChanger();

    }
    void Update()
    {
        
        if (Input.GetKeyDown("space"))
        {
            startTime = Time.time;      //Beginning of Time also tap
            Debug.Log("Press time is: " + startTime);
            gameText.text = ("Keep Holding Down Buttons...");
        }
        if (Input.GetKey("space"))
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
        if (Input.GetKeyUp("space"))
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
        }

        ///////////////////////////////////////////////////////////
        ///////////////////MOUSE INPUT ///////////////////////////
        //////////////////////////////////////////////////////////

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

        ////////////////////////////////////
        ///     EYE TRACKING    ///////////
        //////////////////////////////////
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
        Pos_x = Random.Range(-900.0f, 900.0f);
        Pos_y = Random.Range(-475.0f, 475.0f);
        Debug.Log("Position are: "+ Pos_x+", "+ Pos_y);
        var ObjPos = new Vector2(Pos_x, Pos_y);
        StimulusObj.transform.position = ObjPos;
    }
}



