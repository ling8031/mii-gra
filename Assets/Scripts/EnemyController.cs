using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f;
    [Range(1f, 20.0f)][SerializeField] private float moveRange = 1f;

    private bool isFacingRight = false;

    private Animator animator;

    private float startPositionX;

    private bool isDead = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState != GameManager.GameState.GAME)
        {
            return;
        }
        if (isFacingRight)
        {
            if (this.transform.position.x < startPositionX + moveRange)
            {
                MoveRight();
            }
            else
            {
                Flip();
                MoveLeft();
            }
        }
        else
        {
            if (this.transform.position.x > startPositionX - moveRange)
            {
                MoveLeft();
            }
            else
            {
                Flip();
                MoveRight();
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

        if (!isFacingRight)
            Flip();
    }

    private void MoveLeft()
    {
        transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);

        if (isFacingRight)
            Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;

        theScale.x *= -1;

        transform.localScale = theScale;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && transform.position.y < col.gameObject.transform.position.y) {
            GetComponent<Collider2D>().enabled = false;
            animator.SetBool("isDead", true);
            StartCoroutine(KillOnAnimationEnd());
        }
    }

    IEnumerator KillOnAnimationEnd()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);

        float animationLength = (clipInfo[0].clip.length + 0.2f);
        yield return new WaitForSeconds(animationLength);

        this.gameObject.SetActive(false);
    }
}
