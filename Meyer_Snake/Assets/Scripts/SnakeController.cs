using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SnakeController : MonoBehaviour
{ 
    // Enum to determine the life state of the Snake
    private enum State
    {
        Alive,
        Dead
    }

    private State state;

    // Position of the Snake
    private Vector2 snakePos;
    // Direction of the Snake (only for User Playing)
    private Vector2 snakeDir;

    // Movement delay timer variables
    private float moveTimer;
    private float moveTimerMax;

    // Has an input already been pressed (only for User playing)
    private bool inputQueued = false;

    // Reference to Snake Game Grid script
    private MapGrid mapGrid;

    // Current size of the Snake
    private int currentSnakeSize;
    // List for tracking the previous positions of the snake to make sure the tail follows the same path
    private List<Vector2> snakePastPositionList;

    // List of the Snakes' body parts
    private List<SnakeBodyPart> snakeBodyPartList;

    // Connects the pathfinding NodeGrid script
    private NodeGrid nodeGrid;
    // Determines whether the Pathfinding player, or the User does
    public bool isAIControlled = false;

    public int currentScore = 0;
    public Text currentScoreText;

    public float resetTimer;


    // Setup function for the Snake game and Food
    public void Setup(MapGrid mapGrid)
    {
        this.mapGrid = mapGrid;
    }

    void Awake()
    {
        // User Controlled
        if(!isAIControlled)
        {
            // Start the Snake in the centre of the screen
            snakePos = new Vector2(0, 0);
            // Snake starts facing right
            snakeDir = new Vector2(1, 0);

            // Set default move timer to 1 second        
            moveTimerMax = 0.15f;
            moveTimer = moveTimerMax;

            snakePastPositionList = new List<Vector2>();
            currentSnakeSize = 0;

            snakeBodyPartList = new List<SnakeBodyPart>();

            // Set the Snake state to playable
            state = State.Alive;
        }
        // Pathfinding
        else
        {
            // Start the Snake in the centre of the screen
            snakePos = new Vector2(0, 0);

            // Get reference to pathfinding Node Grid
            nodeGrid = GameObject.FindGameObjectWithTag("GameController").GetComponent<NodeGrid>();

            // Set default move timer to 1 second        
            moveTimerMax = 0.15f;
            moveTimer = moveTimerMax;

            snakePastPositionList = new List<Vector2>();
            currentSnakeSize = 0;

            snakeBodyPartList = new List<SnakeBodyPart>();

            state = State.Alive;
        }
    }

    void Update()
    {
        currentScoreText.text = currentScore.ToString();

        // User Controlled
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
        // Pathfinding
        else
        {
            switch(state)
            {
                case State.Alive:
                    SnakeAI();
                    break;
                case State.Dead:
                    SceneManager.LoadScene("Main");
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
                Vector2 snakeBodypartGridPosition = snakeBodyPart.GetSnakePosition();
                if(snakePos == snakeBodypartGridPosition)
                {
                    Debug.Log("You died");
                    state = State.Dead;
                }
            }

            if (snakePos.x < -10 || snakePos.x > 10 || snakePos.y < -10 || snakePos.y > 10)
            {
                Debug.Log("You died");
                state = State.Dead;
            }

            for (int i = 0; i < snakePastPositionList.Count; i++)
            {
                Vector2 snakePastPosition = snakePastPositionList[i];
            }

            transform.position = new Vector3(snakePos.x, snakePos.y);

            UpdateSnakeBodyParts();
        }
    }

    // Snake movement (Assisted by Pathfinding)
    private void SnakeAI()
    {
        // If path exists..
        if(nodeGrid.path != null)
        {
            // If the path is greater than 0..
            if(nodeGrid.path.Count > 0)
            {
                // Start movement timer
                moveTimer += Time.deltaTime;
                // When the timer reaches trhe threshold..
                if (moveTimer >= moveTimerMax)
                {
                    // If the snakes position is not the same as the first position in the path..
                    if(snakePos != nodeGrid.path[0].worldPosition)
                    {
                        // Insert current position into lis of previous positions
                        snakePastPositionList.Insert(0, snakePos);
                        // Move the Snake to that position
                        snakePos = nodeGrid.path[0].worldPosition;
                    }

                    // Check if food has been eaten
                    bool foodConsumed = mapGrid.HasSnakeEatenFood(snakePos);
                    if (foodConsumed)
                    {
                        // If food is eaten, grow body
                        currentSnakeSize++;
                        CreateSnakeBodyPart();
                        currentScore++;
                    }

                    // Once the the whole snake has moved past a position, remove that position from the list
                    if (snakePastPositionList.Count >= currentSnakeSize + 1)
                    {
                        snakePastPositionList.RemoveAt(snakePastPositionList.Count - 1);
                    }

                    // Move the Snake body parts through the list of previous postions
                    for (int i = 0; i < snakePastPositionList.Count; i++)
                    {
                        Vector2 snakePastPosition = snakePastPositionList[i];
                    }

                    transform.position = new Vector3(snakePos.x, snakePos.y);

                    UpdateSnakeBodyParts();

                    moveTimer -= moveTimerMax;

                }
            }
            else
            {
                resetTimer += Time.deltaTime;

                if (resetTimer >= 4f)
                    SceneManager.LoadScene("Main");
            }
        }
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
    public List<Vector2> GetFullSnakePositionList()
    {
        List<Vector2> snakePosList = new List<Vector2>() { snakePos };
        snakePosList.AddRange(snakePastPositionList);
        return snakePosList;
    }

    private class SnakeBodyPart
    {
        private Vector2 snakePosition;
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

        public void SetSnakePosition(Vector2 snakePos)
        {
            this.snakePosition = snakePos;
            transform.position = new Vector3(snakePos.x, snakePos.y);
        }

        public Vector2 GetSnakePosition()
        {
            return snakePosition;
        }
    }
}

