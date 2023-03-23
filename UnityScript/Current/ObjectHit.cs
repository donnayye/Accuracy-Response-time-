using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHit : MonoBehaviour
{
    // Serialized Field data vars
    [SerializeField]
    public int NumOfTaps, missTap, hitTap;

    // Start is called before the first frame update
    void Start()
    {
        NumOfTaps= 0;
        missTap= 0; 
        hitTap= 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NumOfTaps++;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.transform.tag == "Stimulus")
            {
                hitTap++;
                //PrintName(hit.transform.gameObject);
                Debug.Log("Target name: "+ hit.collider.gameObject.name);
            }
            else
            {
                missTap++;
                PrintName(hit.transform.gameObject);
            }
        }
    }

    private void PrintName(GameObject go)
    {
        string name = go.name;
        Debug.Log(name + "Called");
    }
}
