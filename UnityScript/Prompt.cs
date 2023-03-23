using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class Prompt : MonoBehaviour
{
    
    // Serialized Vars
    [SerializeField]
    private Text PromptText;

    [SerializeField]
    private GameObject StimulusCanvas;

    [SerializeField]
    private GameObject CurtainCanvas;

    [SerializeField]
    public float reactionTime, startTime, randomDelayBeforeMeasuring, timeOverall, deltaTimer;

    [SerializeField]
    private float[] TimeDuration = { 3.0f, 5.0f, 7.0f };

    //Help provide game logic
    private bool clockIsTicking, timerCanBeStopped;


    // Start is called before the first frame update
    void Start()
    {
        // Used for equation
        reactionTime = 0f;
        startTime = 0f;
        
        // Random Time generator
        randomDelayBeforeMeasuring = 0f;

        // Text Manipulation
        PromptText.text = "Hold Down Buttons to Start";

        // Boolean 
        //clockIsTicking = false;
        //timerCanBeStopped = true;

        // Overall time 
       
        //deltaTimer = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Recieve Constant Hold down button from Arduino Joypad buttons
        if (Input.GetKeyDown("space"))
        {
            timeOverall = Time.time;
            deltaTimer = Time.deltaTime;
            Debug.Log("Time is: " + timeOverall);
            Debug.Log("The Delta time is: " + deltaTimer);
        }
    }
}
