using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Tobii.Gaming;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;

//using UnityEngine.InputSystem;


// Make sure to have the Script file name the same as "GameTouch" or vice versa
public class ObjectHit : MonoBehaviour
{
    // Initialize Audio Stimulus & Gameplay theme song
    private RaycastHit2D _hit;
    private RaycastHit2D hit;

    // max range of Screen Resolution
    public float Pos_x, Pos_y;

    // Reference
    private string connectionString;
    public float timeElapsed;                                                    // Using for reference to ensure time Elapsed
    static public int updateT;         // Used to increment Time duration array element
    public float startTime;
    public float[] TimeDuration = { 360.0f, 480.0f, 900.0f };           // 3-5-7minutes total in 60.0s float
    public int objHit, objMiss, overallTaps;                               // Counter used for List data 
    public float randomDelayBeforeMeasuring, StimulusAppear, holdTimer, liftTime, recogTime, ProcessTime;
    public float timer = 2f;                                                // Timer for holding validation
    public float TouchTime, firstGazeTime, _lastGazeTime, ObjectDetTime;
    public bool releasedVal;
    public int wrongHand;


    // Data that will be exported into Excel
    [SerializeField] public float totalTime;                                    // Used if Student does not complete the total amount of time
    [SerializeField] public List<int> numTaps;
    [SerializeField] public List<float> responseTime;
    [SerializeField] public List<float> releaseTime;
    [SerializeField] public List<int> missTap;
    [SerializeField] public List<int> hitTap;
    [SerializeField] public List<Vector2> visualCoord;
    [SerializeField] public List<float> visualGaze_time;
    [SerializeField] public List<float> tapTimestamps;
    [SerializeField] public List<float> appStimulus;
    [SerializeField] public float timeStamp_Tap;

    // Serialized Var
    [SerializeField]
    private Text gameText;

    [SerializeField]
    public GameObject StimulusCanvas, PromptCanvas, RewardCanvas, PauseCanvas, StimulusObj;


    // Provide Boolean Logic for Button
    [SerializeField]
    private bool HoldTimeComplete;

    // Provide boolean logic for Stimulus / Prompt 
    [SerializeField]
    public bool StimulusCall, PromptCall, RewardCall, PauseCall;

    public bool ObjectDetect;
    int fileIndex = 0;
    string databaseName = "myDatabase.db";
    
    void Start()
    {
        // Initializing the screen resolution from Start
        Screen.SetResolution(1920, 1080, true);
        updateT = 0;
        wrongHand = 0;
        StartGame();                            // Starting the game
       
        // Random Time generator
        randomDelayBeforeMeasuring = 0f;
        ObjectDetect= false;                    // Boolean for Visual gaze detection
        Debug.Log("The application data path is " + Application.dataPath);


        Invoke("EndGame", TimeDuration[2]);

    }
    void Update()
    {
        // Time Elapsed (Overall)
        timeElapsed = Time.time;

        // Obtaining Gaze Point every frame 
        GazePoint gazePoint = TobiiAPI.GetGazePoint();

        if (Input.GetKeyDown("space"))
        {
            //Beginning of Time also tap
            startTime = Time.time;                
            Debug.Log("Press time is: " + startTime);
            
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
                if (!StimulusCall)
                {
                    Debug.Log("Waiting for Stimulus to appear...");
                }
                // Held is complete but Stimulus hasn't appeared yet
                else
                {
                    StopCoroutine("StartMeasuring");
                    Debug.Log("Routine stops at " + Time.time);
                }

            }
        }
        if (Input.GetKeyUp("space"))        // space up  completed (Lifting up) 
        {
            liftTime = Time.time;                        // Time lifted = overall time - start of tap (Reaction Time)
            StopCoroutine("StartMeasuring");
            if (StimulusCall)
            {
                releasedVal = true;
                recogTime = liftTime - StimulusAppear;
                releaseTime.Add(recogTime);
                
            }
            else if (!StimulusCall)
            {
                releasedVal= false;
                textChange("Please try again and place hands on buttons");
                HoldTimeComplete = false;
            }
        }

        if (Input.GetMouseButtonDown(0) && releasedVal == true)
        {
            // Collect first tap & timestamp
            TouchTime = Time.time;
            overallTaps++;
            tapTimestamps.Add(TouchTime);         
            numTaps.Add(overallTaps);

            // Response Time
            float timeStamp_Tap = Time.time - StimulusAppear;   

            Vector2 _lastGazePoint = gazePoint.Screen;
            visualCoord.Add(_lastGazePoint);                    // Store Visual coord when Tapped

            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.transform.tag == "Stimulus")
            {
                objHit++;
                // Tap count w/ timestamp & response time
                responseTime.Add(timeStamp_Tap);
                hitTap.Add(objHit);
                missTap.Add(0);

                // Switch Reward Canvas
                StartCoroutine("RewardSystem");
            }
            else if (hit.transform.tag == "Background")
            {
                objMiss++;
                // Tap count w/ timestamp & response time
                responseTime.Add(timeStamp_Tap);
                missTap.Add(objMiss);
                hitTap.Add(0);

            }
            else if(hit.collider == null)
            {
                responseTime.Add(timeStamp_Tap);
                objMiss++;
                missTap.Add(objMiss);
                hitTap.Add(0);
            }
        }
        else if(Input.GetMouseButtonDown(0) && releasedVal != true) 
        {
            // Collect first tap & timestamp
            TouchTime = Time.time;
            overallTaps++;
            tapTimestamps.Add(TouchTime);
            numTaps.Add(overallTaps);
            wrongHand++;

            // Response Time
            float timeStamp_Tap = Time.time - StimulusAppear;

            Vector2 _lastGazePoint = gazePoint.Screen;
            visualCoord.Add(_lastGazePoint);                    // Store Visual coord when Tapped
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.transform.tag == "Stimulus")
            {
                objHit++;
                // Tap count w/ timestamp & response time
                responseTime.Add(timeStamp_Tap);
                hitTap.Add(objHit);
                missTap.Add(0);

                // Switch Reward Canvas
                StartCoroutine("RewardSystem");
            }
            else if (hit.transform.tag == "Background")
            {
                objMiss++;
                // Tap count w/ timestamp & response time
                responseTime.Add(timeStamp_Tap);
                missTap.Add(objMiss);
                hitTap.Add(0);

            }
            else if (hit.collider == null)
            {
                responseTime.Add(timeStamp_Tap);
                objMiss++;
                missTap.Add(objMiss);
                hitTap.Add(0);
            }
        }
        if (gazePoint.IsValid && StimulusCall == true)
        {
            float firstGazeTime = Time.time;
            Vector2 gazePointPosition = gazePoint.Screen;
          

            _hit = Physics2D.Raycast(gazePointPosition, Vector2.zero);
            if (_hit.collider != null && _hit.collider.CompareTag("Stimulus"))
            {
                if (ObjectDetect)
                {
                    _lastGazeTime = firstGazeTime;          /// Updates every time it hits if it doesnt

                }
                else
                {
                    // Store Recognition Time
                    recogTime = firstGazeTime - StimulusAppear;
                    
                    visualGaze_time.Add(firstGazeTime);
                    ObjectDetect = true;
                }
            }
            
            
        }
        if (Input.GetKeyDown("escape"))
        {
            // Quit game instead
            // Pausing the Game
            Calculations();
            ExportData();
          
        }
        if (Input.GetKeyDown("p"))
        {
            // Toggle the pause menu
            PauseToggle();
        }
    }
    IEnumerator StartMeasuring()
    {
        // Random time generator
        randomDelayBeforeMeasuring = Random.Range(1f, 5f);
        HoldTimeComplete = true;
        yield return new WaitForSeconds(randomDelayBeforeMeasuring);

        // Switching Canvas
        StimulusToggle();
    }
 
    IEnumerator RewardSystem()
    {
        RewardToggle();
        yield return new WaitForSeconds(3);
        PromptToggle();
        Debug.Log("Im calling Reward...");
    }

    private void PromptToggle()
    {
        // Reset Hold methods
        HoldTimeComplete = false;
        releasedVal = false;

        StimulusCall = false;
        StimulusCanvas.SetActive(false);

        RewardCall = false;
        RewardCanvas.SetActive(false);

        textChange("Hold Down Button to start again");
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
        StimulusAppear = Time.time;                             // The time Stimulus appears
        appStimulus.Add(StimulusAppear);
        GazePoint firstGazePoint = TobiiAPI.GetGazePoint();

    }

    private void RewardToggle()
    {
        ObjectDetect = false;       // Reset object detection
        StimulusCall = false;
        StimulusCanvas.SetActive(false);

        PromptCall = false;
        PromptCanvas.SetActive(false);

        RewardCall = true;
        RewardCanvas.SetActive(RewardCall);
        
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
            Time.timeScale = 1f;    // resume
            PauseCall = !PauseCall;
            PauseCanvas.SetActive(PauseCall);
  
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
 

    public void StartGame()
    {
        Debug.Log("Starting the game now...");
        timeElapsed = Time.time;

        // Text Manipulation
        gameText.text = "Hold Down Buttons to Start";

        // Hold time complete
        HoldTimeComplete = false;

        // Canvas
        RewardCall = false;
        StimulusCall = false;
        PauseCall = false;
        RewardCanvas.SetActive(false);
        StimulusCanvas.SetActive(false); PauseCanvas.SetActive(false);
        PromptCall = true;
        PromptCanvas.SetActive(true);


        // Object detect boolean
        ObjectDetect = false;
        PositionChanger();
    }

    /// <summary>
    /// Work on exporting the data into Database
    /// 
    /// </summary>
    public void EndGame()
    {
        Debug.Log("Game is ending...");
        
        ExportData();
    }

    public void ExportData()           // export at the end of the gameplay 
    {
        Debug.Log("Exporting Data");
        // Check if the database file already exists
        while (File.Exists(Application.dataPath + "/" + databaseName))
        {
            fileIndex++;
            databaseName = "myDatabase" + fileIndex.ToString() + ".db";
        }

        // Set the connection string
        connectionString = "URI=file:" + Application.dataPath + "/" + databaseName;
        var connection = new SqliteConnection(connectionString);
        connection.Open();

        // Create a new table to store the data
        var createTableQuery = "CREATE TABLE IF NOT EXISTS TapData (NumTaps INTEGER PRIMARY KEY, ResponseTime REAL, ReleaseTime REAL, MissTap REAL, HitTap INTEGER, VisualCoord BLOB, VisualGazeTime REAL, TapTimestamps REAL)";
        var createTableCommand = new SqliteCommand(createTableQuery, connection);

        createTableCommand.ExecuteNonQuery();

        // Insert some data into the table
        // Taps per min, Accuracy Rate, Average response Time, Average correct hand
        for (int i = 0; i < tapTimestamps.Count; i++)
        {
            var insertQuery = "INSERT INTO TapData (NumTaps, ResponseTime, ReleaseTime, MissTap, HitTap, VisualCoord, VisualGazeTime, TapTimestamps) VALUES (@NumTaps, @ResponseTime, @ReleaseTime, @MissTap, @HitTap, @VisualCoord, @VisualGazeTime, @TapTimestamps )";
            var insertCommand = new SqliteCommand(insertQuery, connection);
            insertCommand.Parameters.AddWithValue("@NumTaps", numTaps[i]);
           // insertCommand.Parameters.AddWithValue("@ResponseTime", responseTime[i]);
            if (i < responseTime.Count)
            {
                insertCommand.Parameters.AddWithValue("@ResponseTime", responseTime[i]);
            }
            else
            {
                insertCommand.Parameters.AddWithValue("@ResponseTime", null);

            }
            //insertCommand.Parameters.AddWithValue("@ReleaseTime", releaseTime[i]);
            if (i < releaseTime.Count)
            {
                insertCommand.Parameters.AddWithValue("@ReleaseTime", releaseTime[i]);}
            else
            {
                insertCommand.Parameters.AddWithValue("@ReleaseTime", null);}
            if (i < missTap.Count)
            {
                insertCommand.Parameters.AddWithValue("@MissTap", missTap[i]);
            }
            else
            {
                insertCommand.Parameters.AddWithValue("@MissTap", null);

            }
            if (i < hitTap.Count)
            {
                insertCommand.Parameters.AddWithValue("@HitTap", hitTap[i]);
            }
            else
            {
                insertCommand.Parameters.AddWithValue("@HitTap", null);

            }
            insertCommand.Parameters.AddWithValue("@VisualCoord", visualCoord[i]);
            if (i < visualGaze_time.Count)
            {
                insertCommand.Parameters.AddWithValue("@VisualGazeTime", visualGaze_time[i]);
            }
            else
            {
                insertCommand.Parameters.AddWithValue("@VisualGazeTime", null);

            }
            insertCommand.Parameters.AddWithValue("@TapTimestamps", tapTimestamps[i]);
            insertCommand.ExecuteNonQuery();
        }


        // Close the database connection
        connection.Close();
        Application.Quit();
    }

    public void Calculations()
    {
        //Average taps per min
        float val1 = (float) overallTaps;
        float AverageTapsPerMin = val1 / timeElapsed;
        Debug.Log("Average Taps per Min " + AverageTapsPerMin);

        // Accuracy rate
        float val2 = (float)objHit;
        float val3 = (float)overallTaps;
        float AccuracyPercent = (val2 / val3) * 100f;
        Debug.Log("Accuracy rate is " + AccuracyPercent);

        // Average Response Time
        float val4 = (float)tapTimestamps.Count;
        float val5 = 0;
        for (int j = 0; j<responseTime.Count; j++)
        {
            val5 = val5 + (float)responseTime[j];
            Debug.Log("response time total");
        }
        float val6 = val5 / val4;
        Debug.Log("Average Response Time" + val6);


        // Average 
        float val7 = (float)wrongHand / overallTaps;
        Debug.Log("The amount times wrong hand used" + val7);

        // Taps per min, Accuracy Rate, Average response Time, Average correct hand

    }
    public void PauseResume()
    {
        // resume & start time scale again
        PauseCall = false;
        PauseCanvas.SetActive(false);
        Time.timeScale = 1f;

    }

    public void QuitButton_()
    {
        ExportData();
        // include the performance display at the end 

        Application.Quit(); 
    }

    
}




