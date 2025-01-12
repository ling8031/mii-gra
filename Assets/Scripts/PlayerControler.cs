using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Movement parameters")]
    [Range(0.01f, 20.0f)] [SerializeField] private float moveSpeed = 0.1f;
    [Range(0.01f, 20.0f)] [SerializeField] private float jumpSpeed = 6.0f;

    private Rigidbody2D rigidBody;

    private Animator animator;

    public LayerMask GroundLayer;

    const float rayLength = 1.05f;

    [SerializeField] private bool rayCastVisable = false;

    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip bonusLifeSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip enemyDeathSound;
    [SerializeField] private AudioClip levelFinishSound;

    private bool isFacingRight = true;
    private bool isClimbing = false;
    private bool isLadder = false;

    private float vertical = 0;

    private int keysFound = 0;
    private const int keysNumber = 3;

    private int lives = 3;

    private Vector2 startPosition;

    private AudioSource source;

    void Start()
    {
       // GameManager.instance = GetInstance<GameManager>();
    }

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(lives == 0)
        {
            GameManager.instance.MainMenu_ButtonClicked();
        }

        bool isWalking = false;

        if (GameManager.instance.currentGameState != GameManager.GameState.GAME)
        {
            return;
        }

        vertical = Input.GetAxis("Vertical");

        if (isLadder && System.Math.Abs(vertical) > 0)
        {
            isClimbing = true;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
            isWalking = true;

            if (!isFacingRight)
                Flip();
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
            isWalking = true;

            if (isFacingRight)
                Flip();
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            jump();
        }



        if (rayCastVisable)
            Debug.DrawRay(transform.position, rayLength * Vector3.down, Color.white, 1, false);

        animator.SetBool("isGrounded", isGroundedWithEdges());
        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("verticalVelocity", rigidBody.velocity.y);
        animator.SetBool("isClimbing", isClimbing);
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rigidBody.gravityScale = 0;
            if(!isGrounded())
            {
                rigidBody.isKinematic = true;
            }
            else
            {
                rigidBody.isKinematic= false;
            }
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, vertical * moveSpeed);
        }
        else
        {
            rigidBody.isKinematic = false;
            rigidBody.gravityScale = 1.2f;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "FallLevel")
        {
            lives--;
            transform.position = startPosition;
            Debug.Log("Player fell from The level");
            GameManager.instance.takeLife();
            source.PlayOneShot(damageSound, AudioListener.volume);
        }

        if (col.CompareTag("Bonus"))
        {
            GameManager.instance.addPoints(100);
            col.gameObject.SetActive(false);
            source.PlayOneShot(coinSound, AudioListener.volume);
        }

        if (col.CompareTag("Ladder"))
        {
            isLadder = true;
        }

        if (col.CompareTag("Obstacle"))
        {
            lives--;
            transform.position = startPosition;
            GameManager.instance.takeLife();
            source.PlayOneShot(damageSound, AudioListener.volume);
        }

        if (col.CompareTag("Key"))
        {
            keysFound++;
            col.gameObject.SetActive(false);
            GameManager.instance.addKey();
            Debug.Log("Podniesiono klucz");
            source.PlayOneShot(coinSound, AudioListener.volume);
        }

        if (col.CompareTag("Heart"))
        {
            lives++;
            GameManager.instance.addLife();
            col.gameObject.SetActive(false);
            Debug.Log("Dodano ¿ycie. Liczba ¿yæ: " + lives);
            source.PlayOneShot(bonusLifeSound, AudioListener.volume);
        }

        if (col.CompareTag("Enemy")){
            if (transform.position.y > col.gameObject.transform.position.y)
            {
                Debug.Log("Killed an enemy");
                forcedJump();
                GameManager.instance.addPoints(100);
                GameManager.instance.addEnemyKilled();
                source.PlayOneShot(enemyDeathSound, AudioListener.volume);
            }
            else
            {
                lives--;
                transform.position = startPosition;
                GameManager.instance.takeLife();
                source.PlayOneShot(damageSound, AudioListener.volume);
            }
        }

        if (col.CompareTag("Finish"))
        {
            if (keysFound == keysNumber)
            {
                GameManager.instance.addPoints(100*lives);
                GameManager.instance.LevelCompleted();
                Debug.Log("Koniec gry");
                source.PlayOneShot(levelFinishSound, AudioListener.volume);
            }
            else
            {
                Debug.Log("Nie zebrano wszystkich kluczy");
            }
        }

        if (col.CompareTag("MovingPlatform"))
        {
            transform.SetParent(col.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }

        if (col.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;

        theScale.x *= -1;

        transform.localScale = theScale;
    }

    bool isGrounded()
    {
        return Physics2D.Raycast(this.transform.position, Vector2.down, rayLength, GroundLayer.value);
    }

    bool isGroundedWithEdges()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null) return false;

        Bounds bounds = collider.bounds;

        Vector2 rayOriginCenter = (Vector2)this.transform.position;
        Vector2 rayOriginLeft = rayOriginCenter - new Vector2(bounds.extents.x, 0) + new Vector2(1, 0);
        Vector2 rayOriginRight = rayOriginCenter + new Vector2(bounds.extents.x, 0) - new Vector2(1, 0);

        bool isGroundedCenter = Physics2D.Raycast(rayOriginCenter, Vector2.down, rayLength, GroundLayer.value);
        bool isGroundedLeft = Physics2D.Raycast(rayOriginLeft, Vector2.down, rayLength, GroundLayer.value);
        bool isGroundedRight = Physics2D.Raycast(rayOriginRight, Vector2.down, rayLength, GroundLayer.value);

        if (rayCastVisable)
        {
            Debug.DrawRay(rayOriginCenter, Vector2.down * rayLength, Color.red);
            Debug.DrawRay(rayOriginLeft, Vector2.down * rayLength, Color.green);
            Debug.DrawRay(rayOriginRight, Vector2.down * rayLength, Color.blue);
        }

        return isGroundedCenter || isGroundedLeft || isGroundedRight;
    }




    void jump()
    {
        if (isGrounded())
        {
            rigidBody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            Debug.Log("jumping");
        }
    }

    void forcedJump()
    {
        Vector2 currentVelocity = rigidBody.velocity;
        rigidBody.velocity = new Vector2(currentVelocity.x, jumpSpeed);
        Debug.Log("jumping");
    }
}
