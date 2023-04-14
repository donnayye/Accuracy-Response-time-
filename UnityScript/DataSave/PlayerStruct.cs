using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NewBehaviourScript : MonoBehaviour
{
    string filename = "";

    [System.Serializable]
    public class Player
    {
        public string name;
        public List<float> timeStampList = new List<float> ();          // List for Time stamps
        public List<int> objectTaps = new List<int> ();                 // List for Object tapped
        public List<int> missTaps = new List<int> () ;                  // List for Miss Tapped
        public List<float> releaseTime = new List<float> () ;           // List for Release time
        public List<Vector2> visualGaze = new List<Vector2> ();         // List for Visual Gaze
        public List<float> recognitionTime = new List<float>();         // List for Recog time
        public List<float> reactionTime = new List<float>();            // List for Reaction Time


        // Boolean T/F for Left and right hand
    }
}

