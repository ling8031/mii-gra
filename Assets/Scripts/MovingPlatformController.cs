using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    [Range(0.01f, 20.0f)] [SerializeField] private float moveSpeed = 0.1f;
    [Range(1f, 20.0f)] [SerializeField] private float moveRange = 1f;

    private bool isFacingRight = false;

    private Animator animator;

    private float startPositionX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isFacingRight)
        {
            if (this.transform.position.x < startPositionX + moveRange)
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
                isFacingRight = false;
            }
        }
        else {
            if (this.transform.position.x > startPositionX - moveRange)
            {
                MoveLeft();
            }
            else
            {
                MoveRight();
                isFacingRight = true;
            }
        }
    }

    void Awake()
    {
        startPositionX = this.transform.position.x;
        animator = GetComponent<Animator>();
    }

    private void MoveRight()
    {
        transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }

    private void MoveLeft()
    {
        transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
    }

}
