using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    Vector3 sDirection;

	// Use this for initialization
	void Start () {
        sDirection = transform.right;
	}
	
	// Update is called once per frame
	void Update () {
        
        

        
        if (Input.GetKeyDown(KeyCode.UpArrow) && sDirection != -transform.up)
        {
            sDirection = transform.up;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && sDirection != transform.up)
        {
            sDirection = -transform.up;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && sDirection != -transform.right)
        {
            sDirection = transform.right;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && sDirection != transform.right)
        {
            sDirection = -transform.right;
        }
    }
}
