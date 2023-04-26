using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Tobii.Gaming;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;


public class ObjectHit : MonoBehaviour
{
    // Initialize Audio Stimulus & Gameplay theme song
    private RaycastHit2D _hit;
    private RaycastHit2D hit;
    // Joypad Initialization
    public string portName = "COM4"; // Change this to the name of the serial port connected to Arduino
    public int baudRate = 9600; // Should match the baud rate defined in Arduino code
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
    public Vector2 previousGaze;
    public Vector2 Nullify = new Vector2(0,0);
    public List<Vector2> StimPosition;          // Store the Stimulus Position 
    private float ThresholdPress = 750;      //Force Sensor Threshold
    private float ThresholdRelease = 100;


    // Latency 
    public float _latency;
    public float _lastTime;
    public List<float> latencyTime;
    public float gazeTimestamp;

    // eye tracker sampling rate
    public float lastgazeTimestamp = 0f;
    private float _gazeUpdateInterval = 0.03f;  // update every 50ms
    
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
    [SerializeField] public List<Vector2> tapCoord; 
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
        firstGazeTime = 0f;
        StartGame();                            // Starting the game
        // Initialize the serial port
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
        serialPort.ReadTimeout = 100;
        // Start reading data from Arduino in a separate thread
        StartCoroutine(ReadDataFromArduino());
        HoldTimeComplete = false;

        // Random Time generator
        randomDelayBeforeMeasuring = 0f;
        ObjectDetect= false;                    // Boolean for Visual gaze detection
        Debug.Log("The application data path is " + Application.dataPath);

        
        Invoke("EndGame", TimeDuration[2]);

    }
    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
    void Update(int buttonState1, int buttonState2, int forceSensor1Value, int forceSensor2Value)
    {
        // Time Elapsed (Overall)
        timeElapsed = Time.time;

        // Obtaining Gaze Point every frame 
        GazePoint gazePoint = TobiiAPI.GetGazePoint();
        if (gazePoint.IsRecent() && gazePoint.IsValid)      // Use IsValid to detect object  & IsRecent for recent visual points 
        {
            // First Gaze
            firstGazeTime = Time.time;
            gazeTimestamp = gazePoint.Timestamp;
            
            if (gazeTimestamp - lastgazeTimestamp >= _gazeUpdateInterval)
            {
                
                Vector2 gazePosition = gazePoint.Screen;
               // lastgazeTimestamp = gazeTimestamp;

                _hit = Physics2D.Raycast(gazePosition, Vector2.zero);

                if (_hit.collider != null && _hit.collider.CompareTag("Stimulus"))
                {
                    Debug.Log("Calling second");
                    if (ObjectDetect)
                    {
                        _lastGazeTime = firstGazeTime;          /// Updates every time it hits if it doesnt

                    }
                    else
                    {
                        // Store Recognition Time
                        recogTime = firstGazeTime - StimulusAppear;
                        visualCoord.Add(gazePosition);
                        visualGaze_time.Add(recogTime);             // Storing the Recognition time (Object detection) 
                        ObjectDetect = true;
                    }

                }
                lastgazeTimestamp = gazeTimestamp;
            }

        }
        // Placing the input and eye gaze here for hieracy execution


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
                if (StimulusCall)
                {
                    StopCoroutine("StartMeasuring");
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

        if ((buttonState1 == 0 && buttonState2 == 0) && (forceSensor1Value >= ThresholdPress && forceSensor1Value >= ThresholdPress))
        {
            //Beginning of Time also tap
            startTime = Time.time;
            Debug.Log("Press time is: " + startTime);
            forceSensor1 = forceSensor1Value;
            forceSensor2 = forceSensor2Value;

        }
        if ((buttonState1 == 1 && buttonState2 == 1) && (forceSensor1Value >= ThresholdPress && forceSensor1Value >= ThresholdPress))
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
                }

            }
        }
        if ((buttonState1 == 0 && buttonState2 == 0) && (forceSensor1Value <= ThresholdPress && forceSensor1Value <= ThresholdPress))   
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
                releasedVal = false;
                textChange("Please try again and place hands on buttons");
                HoldTimeComplete = false;
            }
        }

        // RECONSTRUCTIION
        if (Input.GetMouseButtonDown(0) && gazePoint.IsRecent())
        {

            // initialization
            Vector2 mouseTapper = Input.mousePosition;

            // Collect first tap & timestamp
            TouchTime = Time.time;
            overallTaps++;

            //sql
            tapTimestamps.Add(TouchTime);         
            numTaps.Add(overallTaps);

            // Response Time
            float timeStamp_Tap = Time.time - StimulusAppear;
            if (!ObjectDetect)
            {
               
                visualCoord.Add(previousGaze);                    // Storing since Object isnt detected to see where his gaze is at
                visualGaze_time.Add(0);
            }
            
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.transform.tag == "Stimulus")
            {
                objHit++;
                // Tap count w/ timestamp & response time
                tapCoord.Add(mouseTapper);
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
                tapCoord.Add(mouseTapper);
                responseTime.Add(timeStamp_Tap);
                missTap.Add(objMiss);

                // Nulling list datas
                hitTap.Add(0);
                appStimulus.Add(0);
                StimPosition.Add(Nullify);

            }
            else if(hit.collider.tag == "Prompt")       // If Student hits !Stimulus Canvas count as abrupted tap
            {
                responseTime.Add(timeStamp_Tap);
                tapCoord.Add(mouseTapper);
                objMiss++;
                missTap.Add(objMiss);

                // nulling 
                hitTap.Add(0);
                appStimulus.Add(0);
                StimPosition.Add(Nullify);
            }
            else
            {
                // Hits anything else 
                responseTime.Add(timeStamp_Tap);
                tapCoord.Add(mouseTapper);
                objMiss++;
                missTap.Add(objMiss);

                appStimulus.Add(0);
                hitTap.Add(0);
                StimPosition.Add(Nullify);
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
        _lastTime = timeElapsed;        // used for calculating latency
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
                    Update(buttonState1, buttonState2, forceSensor1Value, forceSensor2Value);
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

    }

    private void StimulusToggle()
    {

        HoldTimeComplete = true;

        PromptCall = false;
        PromptCanvas.SetActive(false);

        RewardCall = false;
        RewardCanvas.SetActive(false);

        StimulusCall = true;
        StimulusCanvas.SetActive(StimulusCall);

        StimulusAppear = Time.time;                             // The time Stimulus appears
        appStimulus.Add(StimulusAppear);                            // ADD me to sqlite
        

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
        StimPosition.Add(ObjPos);                               // ADD me to SQLite Database
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
        var createTableQuery = "CREATE TABLE IF NOT EXISTS TapData (NumTaps INTEGER PRIMARY KEY, ResponseTime REAL, ReleaseTime REAL, MissTap REAL, HitTap INTEGER, VisualCoord BLOB, VisualGazeTime REAL, TapTimestamps REAL, StimulusAppearance REAL, StimulusCoordinate BLOB)";
        var createTableCommand = new SqliteCommand(createTableQuery, connection);

        createTableCommand.ExecuteNonQuery();
        
        // Insert some data into the table
        // Taps per min, Accuracy Rate, Average response Time, Average correct hand
        for (int i = 0; i < tapTimestamps.Count; i++)
        {
            var insertQuery = "INSERT INTO TapData (NumTaps, ResponseTime, ReleaseTime, MissTap, HitTap, VisualCoord, VisualGazeTime, TapTimestamps, StimulusAppearance, StimulusCoordinate) VALUES (@NumTaps, @ResponseTime, @ReleaseTime, @MissTap, @HitTap, @VisualCoord, @VisualGazeTime, @TapTimestamps, @StimulusAppearance, @StimulusCoordinate )";
            var insertCommand = new SqliteCommand(insertQuery, connection);
            insertCommand.Parameters.AddWithValue("@NumTaps", numTaps[i]);
            if (i < responseTime.Count)
            {
                insertCommand.Parameters.AddWithValue("@ResponseTime", responseTime[i]);
            }
            else
            {
                insertCommand.Parameters.AddWithValue("@ResponseTime", null);
            }
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
            if (i < visualCoord.Count)
            {
                insertCommand.Parameters.AddWithValue("@VisualCoord", visualCoord[i]);
            }
            else
            {
                insertCommand.Parameters.AddWithValue("@VisualCoord", null);

            }

            if (i < visualGaze_time.Count)
            {
                insertCommand.Parameters.AddWithValue("@VisualGazeTime", visualGaze_time[i]);
            }
            else
            {
                insertCommand.Parameters.AddWithValue("@VisualGazeTime", null);

            }
            insertCommand.Parameters.AddWithValue("@TapTimestamps", tapTimestamps[i]);
            if (i < appStimulus.Count)
            {
                insertCommand.Parameters.AddWithValue("@StimulusAppearance", appStimulus[i]);
            }
            else
            {
                insertCommand.Parameters.AddWithValue("@StimulusAppearance", null);

            }
            if (i < StimPosition.Count)
            {
                insertCommand.Parameters.AddWithValue("@StimulusCoordinate", StimPosition[i]);
            }
            else
            {
                insertCommand.Parameters.AddWithValue("@StimulusCoordinate", null);

            }
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




