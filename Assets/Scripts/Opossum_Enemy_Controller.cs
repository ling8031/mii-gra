using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opossum_Enemy_Controller : MonoBehaviour
{
    [Range(0.01f, 20.0f)][SerializeField] private float moveSpeed = 0.1f;

    private bool isFacingRight = false;

    private Animator animator;

    [SerializeField] private Transform groundCheck; // A point to check for ground
    [SerializeField] private LayerMask groundLayer; // Layer for ground detection
    [SerializeField] private float groundCheckDistance = 0.1f; // Distance to check for ground
    [SerializeField] private Transform wallCheck; // A point to check for walls
    [SerializeField] private float wallCheckDistance = 0.1f; // Distance to check for walls

    private bool isDead = false;

    void Start()
    {

    }

    void Update()
    {
        if (GameManager.instance.currentGameState != GameManager.GameState.GAME)
        {
            return;
        }

        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        bool isWallAhead = Physics2D.Raycast(wallCheck.position, isFacingRight ? Vector2.right : Vector2.left, wallCheckDistance, groundLayer);

        if (!isGroundAhead || isWallAhead)
        {
            Flip();
        }

        Move();
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Move()
    {
        float direction = isFacingRight ? 1 : -1;
        transform.Translate(direction * moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
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
        if (col.CompareTag("Player") && transform.position.y < col.gameObject.transform.position.y)
        {
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
