using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerMovement : MonoBehaviour
{
    NodeGrid nodeGrid;

    public GameObject seeker;
    private Vector2 seekerPos;

    private float moveTimer;
    private float moveTimerMax;

    private void Awake()
    {
        nodeGrid = GetComponent<NodeGrid>();

        seekerPos = new Vector2(seeker.transform.position.x, seeker.transform.position.y);

        moveTimerMax = 0.15f;
        moveTimer = moveTimerMax;

    }

    private void Update()
    {
        if(nodeGrid.path.Count > 0)
        {
            moveTimer += Time.deltaTime;
            if(moveTimer >= moveTimerMax)
            {
                if (seekerPos != nodeGrid.path[0].worldPosition)
                {
                    seekerPos = nodeGrid.path[0].worldPosition;
                    seeker.transform.position = seekerPos;
                }

                moveTimer -= moveTimerMax;
            }


        }
    }
}
