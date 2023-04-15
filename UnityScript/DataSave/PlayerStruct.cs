using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVWriter : MonoBehaviour
{
    string filename = "";

    [System.Serializable]
    public class Player
    {
        public string name;
        public List<float> timeStampList;         // List for Time stamps
        public List<int> objectTaps = new List<int>();                 // List for Object tapped
        public List<int> missTaps = new List<int>();                  // List for Miss Tapped
        public int NumOfTaps = new int();                               // Integer for No. of Taps
        public List<float> releaseTime = new List<float>();           // List for Release time
        public List<Vector2> visualGaze = new List<Vector2>();         // List for Visual Gaze
        public List<float> recognitionTime = new List<float>();         // List for Recog time
        public List<float> reactionTime = new List<float>();          // List for Reaction Time
        // ("Name", "Object Tap", "Miss Tap", "Button Release", "Visual Gaze", "Response Time")
        // (Student, No., No., L/R/Both, Coordinate, ms/s)
        // Boolean T/F for Left and right hand
    }

    [System.Serializable]
    public class PlayerList
    {
        public Player[] player;
    }

    public PlayerList myPlayerList = new PlayerList();

    void Start()
    {
        Debug.Log("App path: " + Application.dataPath);
        filename = Application.dataPath + "/test.csv";

    }

    void Update()
    {
        
        // condition for application quit
        if (Input.GetKeyDown("space"))
        {
            Debug.Log(string.Join(System.Environment.NewLine, myPlayerList.player[1].timeStampList));
            Debug.Log(string.Join(System.Environment.NewLine, myPlayerList.player[0].objectTaps));
            Debug.Log()
            WriteCSV();
        }
        
    }

    public void WriteCSV()
    {
        if(myPlayerList.player.Length > 0 )
        {   // checks if length is valid
            TextWriter tw = new StreamWriter(filename, false);
            tw.WriteLine("Name, Time Stamp, Object Tap, Miss Tap, Button Release, Visual Gaze, Recognition Time", "Response Time");
            tw.Close();

            tw = new StreamWriter(filename, true);

            for (int i= 0; i< myPlayerList.player.Length; i++)
            {
                tw.WriteLine(myPlayerList.player[0].name + "," + myPlayerList.player[i].timeStampList +"," +myPlayerList.player[i].objectTaps + "," + myPlayerList.player[i].missTaps + "," +
                    myPlayerList.player[i].releaseTime + "," + myPlayerList.player[i].visualGaze + "," + myPlayerList.player[i].recognitionTime + "," +
                    myPlayerList.player[i].reactionTime);
            }

            tw.Close();
        }
    }
    
}

