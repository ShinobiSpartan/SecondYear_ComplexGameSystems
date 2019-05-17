using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private SnakeController snake;
    private MapGrid mapGrid;

    // Start is called before the first frame update
    void Start()
    {
        mapGrid = new MapGrid(20, 20);

        snake.Setup(mapGrid);
        mapGrid.Setup(snake);
    }
}
