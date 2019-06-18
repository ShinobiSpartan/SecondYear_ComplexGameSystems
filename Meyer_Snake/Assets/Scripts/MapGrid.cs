using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid
{
    // Variable for Food Object
    private GameObject foodObject;
    private Vector2 foodPos;

    // Grid size variables
    private int width;
    private int height;

    private SnakeController snake;

    // Constructor for Game Grid
    public MapGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    // Setup function for the Food and Snake
    public void Setup(SnakeController snake)
    {
        this.snake = snake;

        SpawnFood();
    }

    // Spawns the food for the Snake to collect around the grid/map
    private void SpawnFood()
    {
        // While the food position is not the same as a body location, generate a position for the next food
        do {
            foodPos = new Vector2(Random.Range(-(width / 2), (width / 2)), Random.Range(-(height / 2), (height / 2)));
        } while (snake.GetFullSnakePositionList().IndexOf(foodPos) != -1);

        // Create the new food object
        foodObject = new GameObject("Food", typeof(SpriteRenderer), typeof(BoxCollider2D));
        foodObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;
        foodObject.tag = "Food";
        foodObject.transform.position = new Vector3(foodPos.x, foodPos.y);
    }

    // Function to check if the food has been eaten
    public bool HasSnakeEatenFood(Vector2 snakePos)
    {
        // If the snakes posistion is equal to the foods' position
        if(snakePos == foodPos)
        {
            // Destroy the current object and then spawn the next food object
            Object.Destroy(foodObject);
            SpawnFood();

            return true;
        }
        else
            return false;
        
    }


}
