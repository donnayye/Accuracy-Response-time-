using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;



public class EyeGaze : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GazePoint gazePoint = TobiiAPI.GetGazePoint();      // Acquiring Gaze point per frame

        if(gazePoint.IsRecent())
        {
            print("Gaze point on screen (X,Y):" + gazePoint.Screen.x + ", " + gazePoint.Screen.y);

        }
    }
}
