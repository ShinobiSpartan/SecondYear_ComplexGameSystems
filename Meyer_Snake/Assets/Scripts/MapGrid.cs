using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid
{
    private Vector2Int foodPos;
    private GameObject foodObject;

    private int width;
    private int height;

    private SnakeController snake;

    public MapGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void Setup(SnakeController snake)
    {
        this.snake = snake;

        SpawnFood();
    }

    private void SpawnFood()
    {
        do {
            foodPos = new Vector2Int(Random.Range(-(width / 2), (width / 2)), Random.Range(-(height / 2), (height / 2)));
        } while (snake.GetFullSnakePositionList().IndexOf(foodPos) != -1);

        foodObject = new GameObject("Food", typeof(SpriteRenderer));
        foodObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;
        foodObject.transform.position = new Vector3(foodPos.x, foodPos.y);
    }

    public bool HasSnakeEatenFood(Vector2Int snakePos)
    {
        if(snakePos == foodPos)
        {
            Object.Destroy(foodObject);
            SpawnFood();

            return true;
        }
        else
        {
            return false;
        }
    }


}
