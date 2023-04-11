using UnityEngine;
using Tobii.Gaming;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class EyeGaze:MonoBehaviour
{
    private RaycastHit2D _hit;
    public GameObject gazeIndicator;
    public GameObject gazeObject;
 //   public GameObject gazeIndic;

    void Start()
    {
        Debug.Log("Screen resolution: " + Screen.currentResolution + ", DPI: " + Screen.dpi + ", Screen Orientation " + Screen.orientation);
        Debug.Log("Screen Main window Display info: " + Screen.mainWindowDisplayInfo + ", main window position: " + Screen.mainWindowPosition);

        
        Debug.Log("The screen (W,H): " + Screen.width + ", " + Screen.height);
    //  Debug.Log("The display layout is: " + displayLayout);
    }
    void Update()
    {
        GazePoint gazePoint = TobiiAPI.GetGazePoint();      // Data being returned into gazePoint 
      //  GazePoint gazePointer = TobiiAPI.GetNormalizedPoint2D;
        if (gazePoint.IsValid )
        {

            // Initializing 2D vector 
            Vector2 gazePointPosition = gazePoint.Screen;
            
            gazeIndicator.transform.position = gazePointPosition;
         //   gazeIndic.transform.position = gazePointer; 
            // _hit = Physics2D.Raycast(gazePointPosition, Vector2.zero);
           

             _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(gazePointPosition), Vector2.zero);
            if (_hit.collider != null && _hit.collider.CompareTag("Stimulus"))
            {
                Debug.Log("Hit " + _hit.collider.name + ", " + gazePoint.Screen.x + ", " + gazePoint.Screen.y);
            }
            
        }
    }
}


