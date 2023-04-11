using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;


public class VisualGazePlots : MonoBehaviour
{
    public GameObject gazeRep;
    private RaycastHit2D _hit;
    private GazePoint _lastGazePoint = GazePoint.Invalid;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("The width and height of unity screen is " + UnityEngine.Screen.width + " ," + UnityEngine.Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        GazePoint gazePoint = TobiiAPI.GetGazePoint();      // 
        if(gazePoint.IsRecent() && gazePoint.Timestamp > (_lastGazePoint.Timestamp + float.Epsilon))            // Produces Recent data     The IsValid() property processes old but valid data. 
        {
            Vector2 gazePointPosition = gazePoint.Screen;
            
            gazeRep.transform.position = gazePointPosition;
            Debug.Log(gazePointPosition);

            _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(gazePointPosition), Vector2.zero);

            if (_hit.collider != null && _hit.collider.CompareTag("Stimulus"))
            {
                Debug.Log("Hit " + _hit.collider.name + ", " + gazePoint.Screen.x + ", " + gazePoint.Screen.y);
            }

            _lastGazePoint=gazePoint;       // Saves gaze point to compare timestamp
        }
    }

 
}
