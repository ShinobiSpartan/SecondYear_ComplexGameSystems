using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{

    private enum State
    {
        Alive,
        Dead
    }

    private State state;

    public bool isAIControlled = false;

    private Vector2Int snakePos;
    private Vector2Int snakeDir;

    private float moveTimer;
    private float moveTimerMax;

    private bool inputQueued = false;

    private MapGrid mapGrid;

    private int currentSnakeSize;
    private List<Vector2Int> snakePastPositionList;

    private List<SnakeBodyPart> snakeBodyPartList;

    public void Setup(MapGrid mapGrid)
    {
        this.mapGrid = mapGrid;
    }

    void Awake()
    {
        if(!isAIControlled)
        {
            // Start the Snake in the centre of the screen
            snakePos = new Vector2Int(0, 0);
            // Snake starts facing right
            snakeDir = new Vector2Int(1, 0);

            // Set default move timer to 1 second        
            moveTimerMax = 0.15f;
            moveTimer = moveTimerMax;

            snakePastPositionList = new List<Vector2Int>();
            currentSnakeSize = 0;

            snakeBodyPartList = new List<SnakeBodyPart>();

            state = State.Alive;
        }
        else
        {
            // Start the Snake in the centre of the screen
            snakePos = new Vector2Int(0, 0);

            snakePastPositionList = new List<Vector2Int>();
            currentSnakeSize = 0;

            snakeBodyPartList = new List<SnakeBodyPart>();

            state = State.Alive;
        }
    }

    void Update()
    {
        if (!isAIControlled)
        {
            switch (state)
            {
                case State.Alive:
                    SnakeInput();
                    SnakeMovement();
                    break;
                case State.Dead:
                    break;
            }
        }
        else
        {
            switch(state)
            {
                case State.Alive:
                    SnakeAI();
                    break;
                case State.Dead:
                    break;
            }
        }
    }

    // Controls all of the input and directions for the Snake
    private void SnakeInput()
    {
        if (inputQueued)
            return;
        // Change direction to UP
        if (Input.GetKey(KeyCode.UpArrow) && snakeDir.y != -1)
        {
            snakeDir = new Vector2Int(0, 1);
            inputQueued = true;
        }

        // Change direction to DOWN
       else if (Input.GetKey(KeyCode.DownArrow) && snakeDir.y != 1)
        {
            snakeDir = new Vector2Int(0, -1);
            inputQueued = true;
        }

        // Change direction to RIGHT
       else if (Input.GetKey(KeyCode.RightArrow) && snakeDir.x != -1)
        {
            snakeDir = new Vector2Int(1, 0);
            inputQueued = true;
        }

        // Change direction to LEFT
       else if (Input.GetKey(KeyCode.LeftArrow) && snakeDir.x != 1)
        {
            snakeDir = new Vector2Int(-1, 0);
            inputQueued = true;
        }
    }

    // Deals with the actual movement of the Snake
    private void SnakeMovement()
    {
        moveTimer += Time.deltaTime;
        if (moveTimer >= moveTimerMax)
        {
            inputQueued = false;
            moveTimer -= moveTimerMax;
            snakePastPositionList.Insert(0, snakePos);
            snakePos += snakeDir;

            bool foodConsumed = mapGrid.HasSnakeEatenFood(snakePos);
            if (foodConsumed)
            {
                // If food is eaten, grow body
                currentSnakeSize++;
                CreateSnakeBodyPart();
            }

            if (snakePastPositionList.Count >= currentSnakeSize + 1)
            {
                snakePastPositionList.RemoveAt(snakePastPositionList.Count - 1);
            }

            foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList)
            {
                Vector2Int snakeBodypartGridPosition = snakeBodyPart.GetSnakePosition();
                if(snakePos == snakeBodypartGridPosition)
                {
                    Debug.Log("You dead");
                    state = State.Dead;
                }
            }

            if (snakePos.x < -10 || snakePos.x > 10 || snakePos.y < -10 || snakePos.y > 10)
            {
                Debug.Log("You dead");
                state = State.Dead;
            }

            for (int i = 0; i < snakePastPositionList.Count; i++)
            {
                Vector2Int snakePastPosition = snakePastPositionList[i];
            }

            transform.position = new Vector3(snakePos.x, snakePos.y);

            UpdateSnakeBodyParts();
        }
    }

    // Allows the snake to function without player input
    private void SnakeAI()
    {
       snakePastPositionList.Insert(0, snakePos);

       bool foodConsumed = mapGrid.HasSnakeEatenFood(snakePos);
       if (foodConsumed)
       {
           // If food is eaten, grow body
           currentSnakeSize++;
           CreateSnakeBodyPart();
       }

       if (snakePastPositionList.Count >= currentSnakeSize + 1)
       {
           snakePastPositionList.RemoveAt(snakePastPositionList.Count - 1);
       }

       foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList)
       {
           Vector2Int snakeBodypartGridPosition = snakeBodyPart.GetSnakePosition();
           if (snakePos == snakeBodypartGridPosition)
           {
               Debug.Log("You dead");
               state = State.Dead;
           }
       }

       if (snakePos.x < -10 || snakePos.x > 10 || snakePos.y < -10 || snakePos.y > 10)
       {
           Debug.Log("You dead");
           state = State.Dead;
       }

       for (int i = 0; i < snakePastPositionList.Count; i++)
       {
           Vector2Int snakePastPosition = snakePastPositionList[i];
       }

       transform.position = new Vector3(snakePos.x, snakePos.y);

       UpdateSnakeBodyParts();
       
    }

    private void CreateSnakeBodyPart()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }

    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            snakeBodyPartList[i].SetSnakePosition(snakePastPositionList[i]);
        }
    }

    // Get the full list of the Snake positions (Head + Body)
    public List<Vector2Int> GetFullSnakePositionList()
    {
        List<Vector2Int> snakePosList = new List<Vector2Int>() { snakePos };
        snakePosList.AddRange(snakePastPositionList);
        return snakePosList;
    }

    private class SnakeBodyPart
    {
        private Vector2Int snakePosition;
        private Transform transform;

        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyObject = new GameObject("SnakeBody", typeof(BoxCollider2D), typeof(SpriteRenderer));
            snakeBodyObject.GetComponent<BoxCollider2D>().size = new Vector2(0.9f, 0.9f);
            snakeBodyObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.bodySprite;
            snakeBodyObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            snakeBodyObject.gameObject.layer = 8;
            transform = snakeBodyObject.transform;
        }

        public void SetSnakePosition(Vector2Int snakePos)
        {
            this.snakePosition = snakePos;
            transform.position = new Vector3(snakePos.x, snakePos.y);
        }

        public Vector2Int GetSnakePosition()
        {
            return snakePosition;
        }
    }
}

