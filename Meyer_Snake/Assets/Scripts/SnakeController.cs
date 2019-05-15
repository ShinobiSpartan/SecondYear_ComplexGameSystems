using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour {

    [SerializeField]
    private Vector3 snakeDirection;

    public float moveTimer = 1.0f;
    private float timer = 0.0f;

	// Use this for initialization
	void Start () {
        snakeDirection = transform.right;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if(timer > moveTimer)
        {
            transform.position += snakeDirection;
            timer = 0;
        }

		if(Input.GetKeyDown(KeyCode.RightArrow) && snakeDirection != -transform.right)
        {
            snakeDirection = transform.right;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && snakeDirection != transform.right)
        {
            snakeDirection = -transform.right;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && snakeDirection != -transform.up)
        {
            snakeDirection = transform.up;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && snakeDirection != transform.up)
        {
            snakeDirection = -transform.up;
        }

        if (transform.position.x >= 5 || transform.position.x >= -5 || transform.position.y <= 5 || transform.position.y >= -5)
        {
            
        }
    }
}
