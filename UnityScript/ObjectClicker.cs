using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClicker : MonoBehaviour
{
// This requires two GameObject, one tagged with a "Stimulus" and the other tagged as "Background" on the Inspector settings of the game object

    // Initializing Serialized Vars
    private int NumTap;
    private int missCount;
    private int hitCount;

    private void Start()
    {
        NumTap= 0;
        missCount= 0;
        hitCount= 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NumTap++;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            Debug.Log("Total No. of Taps: " + NumTap);
            if (hit.transform.tag== "Stimulus")
            {
                hitCount++;
                Debug.Log("I hit: "+ hitCount);
                Debug.Log("Target name: " + hit.collider.gameObject.name);
            }
            else
            {
                missCount++;
                Debug.Log("I missed: " + missCount);
                PrintName(hit.transform.gameObject);
            }



            //RaycastHit hit;
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

           // if(Physics.Raycast(ray, out hit, 100.0f))
            //{
              //  if(hit.transform != null)
               // {
                 //   PrintName(hit.transform.gameObject);

//                }
          //  }
        }
    }
    private void PrintName(GameObject go)
    {
        Debug.Log(go.name + " Called ");
    }
}
