using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Setup variables for Snake Game
    [SerializeField]
    private SnakeController snake;
    private MapGrid mapGrid;

    // Snake Game grid size
    public int gridX = 20, gridY = 20;

    // Start is called before the first frame update
    void Start()
    {
        // Create new grid for Snake game to play off of
        mapGrid = new MapGrid(gridX, gridY);

        // Connects MapGrid and Snake objects
        snake.Setup(mapGrid);
        mapGrid.Setup(snake);
    }
}
