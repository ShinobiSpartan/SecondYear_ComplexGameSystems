using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    private Vector2 snakePos;
    private Vector2 snakeDir;

    private float moveTimer;
    private float moveTimerMax;

    void Awake()
    {
        // Start the Snake in the centre of the screen
        snakePos = new Vector2(10, 10);
        // Snake starts facing right
        snakeDir = new Vector2(1, 0);

        // Set default move timer to 1 second        
        moveTimerMax = 0.25f;
        moveTimer = moveTimerMax;
    }

    void Update()
    {
        SnakeInput();
        SnakeMovement();
    }

    // Controls all of the input and directions for the Snake
    private void SnakeInput()
    {
        // Change direction to UP
        if (Input.GetKeyDown(KeyCode.UpArrow) && snakeDir.y != -1)
        {
            snakeDir = new Vector2(0, 1);
        }

        // Change direction to DOWN
        if (Input.GetKeyDown(KeyCode.DownArrow) && snakeDir.y != 1)
        {
            snakeDir = new Vector2(0, -1);
        }

        // Change direction to RIGHT
        if (Input.GetKeyDown(KeyCode.RightArrow) && snakeDir.x != -1)
        {
            snakeDir = new Vector2(1, 0);
        }

        // Change direction to LEFT
        if (Input.GetKeyDown(KeyCode.LeftArrow) && snakeDir.x != 1)
        {
            snakeDir = new Vector2(-1, 0);
        }
    }

    // Deals with the actual movement of the Snake
    private void SnakeMovement()
    {
        moveTimer += Time.deltaTime;
        if (moveTimer >= moveTimerMax)
        {
            snakePos += snakeDir;
            moveTimer -= moveTimerMax;

            transform.position = new Vector3(snakePos.x, snakePos.y);
        }

    }
}

