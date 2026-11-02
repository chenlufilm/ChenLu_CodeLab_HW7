using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    //bring in sprite renderer for later player sprite flipping
    public SpriteRenderer mySprite;

    //bring in TMP to update player lives
    public TMP_Text titleText;
    public TMP_Text instructionText;
    public TMP_Text playerLives;
    public TMP_Text winText;

    //bring in Unity's Player's input system
    private InputAction moveInput;

    //make Rigidbody component public
    public Rigidbody2D myBody;

    //set player movement variables
    private Vector2 inputForce;
    public float speed;
    public float jumpForce;
    public float bounceForce;

    //player's ground checker
    public Transform groundChecker;
    public float checkSize = 0.1f;
    public LayerMask groundLayer;
    private bool onGround;

    //enemy checker
    public LayerMask enemyLayer;

    //player lives
    private int lives;

    //player cannot receive damage immediately but has to wait for 3 seconds to receive new damage
    private float damageInterval = 3f;
    private float allowDamageTime;

    //different game stage bool set
    private bool started;
    private bool won;
    private bool restart;
    
    //sound effects
    public AudioSource bgmSound;
    public AudioSource winSound;
    public AudioSource gameoverSound;
    public AudioSource fireSound;
    public AudioSource stompSound;
    public AudioSource bumpSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //player movement input
        moveInput = InputSystem.actions["Move"];
        moveInput.Enable();
        //set player lives
        lives = 3;
        playerLives.text = "Lives: " + lives;
        //set wait time for player's next damage interval
        damageInterval = 3f;
        //the damage timer to 0 at first
        allowDamageTime = 0f;

        //set all game state to false
        started = false;
        won = false;
        restart = false;
        onGround = false;

        //clear inputForce
        inputForce = Vector2.zero;

        //screen text
        titleText.gameObject.SetActive(true);
        instructionText.gameObject.SetActive(true);
        playerLives.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);

        //pause game's time
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //allow game to restart after GAME OVER sound stops playing
        if (restart == true)
        {
            if (gameoverSound.isPlaying == false)
            {
                RestartLevel();
            }
            //
            return;
        }
        
        if (restart == false)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
                //if player press R key, restart the game
            {
                RestartLevel();
            }
            else if (Keyboard.current.spaceKey.wasPressedThisFrame)
                //if player press space key, start the game
            {
                if (won == true)
                {
                    RestartLevel();
                }
                else if (started == false)
                {
                    started = true;
                    //set game UI to in game UI
                    titleText.gameObject.SetActive(false);
                    instructionText.gameObject.SetActive(false);
                    playerLives.gameObject.SetActive(true);
                    
                    //restore game's time
                    Time.timeScale = 1f;
                }
            }

            //if game has started
            if (started == true && won == false && restart == false)
            {
                //input will determine the input force
                inputForce = moveInput.ReadValue<Vector2>();
                //player move to left and sprite will flip vertically
                if (inputForce.x > 0f)
                {
                    mySprite.flipX = false;
                }
                else if (inputForce.x < 0f)
                {
                    mySprite.flipX = true;
                }
            }
        }
    }

    //physics movement update
    void FixedUpdate()
    {
        //if game is started
        if (started == true && won == false && restart == false)
        {
            //move player avatar left and right with Rigidbody component
            myBody.linearVelocityX = inputForce.x * speed;
            //if player avatar is touching the ground, player can jump
            if (onGround && inputForce.y > 0f)
            {
                myBody.linearVelocityY = inputForce.y * jumpForce;
                onGround = false;
            }
            else
                //if onGround is false & player input in y (w) is smaller than 0, update ground checker's stats
            {
                onGround = Physics2D.OverlapCircle(groundChecker.position, checkSize, groundLayer);
            }
        }
    }

    //if player jumps onto an enemy, enemy gets destroyed
    void OnCollisionEnter2D(Collision2D collision)
    {
        //make sure is in game
        if (started == true && won == false && restart == false)
        {
            //if player collide with enemy and enemy is still active in hierarchy
            if ((collision.gameObject.CompareTag("Enemy")) && collision.gameObject.activeInHierarchy)
            {
                //set new collider when player's ground checker is overlapped with enemy layer's enemy
                Collider2D jumpedOnEnemy = Physics2D.OverlapCircle(groundChecker.position, checkSize, enemyLayer);
                //if player jumped on enemy and is the current collision
                if (jumpedOnEnemy == collision.collider)
                {
                    //play stomp sound
                    stompSound.Play();
                    //de-active current enemy to avoid multiple collision detection
                    collision.gameObject.SetActive(false);
                    //destroy current enemy
                    Destroy(collision.gameObject);
                    //bounce the player up by the bounceForce
                    myBody.linearVelocityY = bounceForce;
                    //set onGround checker bool to false
                    onGround = false;
                }else
                {
                    //if player has not jumped on enemy's head but is touching enemy, player just take 1 damage
                    TakeDamage(bumpSound);
                }
            }else if (collision.gameObject.CompareTag("Hazard"))
            {
                TakeDamage(fireSound);
            }
        }
    }

    //if player keeps colliding with the enemies or the hazards
    void OnCollisionStay2D(Collision2D collision)
    {
        //if enemy or hazard is still active in hierarchy
        if (collision.gameObject.activeInHierarchy)
        {
            //if player keeps colliding with enemies or hazard, take damage
            if (collision.gameObject.CompareTag("Enemy"))
            {
                TakeDamage(bumpSound);
            }

            if (collision.gameObject.CompareTag("Hazard"))
            {
                TakeDamage(fireSound);
            }
        }
    }

    //if player enters different special zones
    void OnTriggerEnter2D(Collider2D other)
    {
        //make sure is in game
        if (started == true && won == false && restart == false)
        {
            //if entered environmental hazard zone like fire, take damage
            if (other.CompareTag("Hazard"))
            {
                TakeDamage(fireSound);
            }
            else if (other.CompareTag("FallZone"))
                //if entered fall zone, restart the game
            {
                restart = true;
                bgmSound.Stop();
                gameoverSound.Play();
                Time.timeScale = 0f;
            }
            else if (other.CompareTag("Finish"))
                //if entered finish zone, end the game
            {
                //if player reaches the Win Zone, stop playing BGM and play the winning BGM
                won = true;
                bgmSound.Stop();
                winSound.Play();
                //reset all the inputs and rigidbody
                inputForce = Vector2.zero;
                myBody.linearVelocity = Vector2.zero;
                
                //pause game's time
                Time.timeScale = 0f;
                
                //set UI text
                playerLives.gameObject.SetActive(false);
                winText.gameObject.SetActive(true);
            }
        }
    }

    //if player keeps staying in hazard zone
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Hazard"))
        {
            TakeDamage(fireSound);
        }
    }

    //damage calculations
    void TakeDamage(AudioSource damageSound)
    {
        //make sure in game and last damage is more than allowDamageTime (3s)
        if (started == true && won == false && restart == false && Time.time >= allowDamageTime)
        {
            //player lives minus one every time TakeDamage is triggered
            lives -= 1;
            //print new player lives to UI
            playerLives.text = "Lives: " + lives;
            //play take damage sound
            damageSound.Play();
            //give player some allowDamageTime for the next damage
            allowDamageTime = Time.time + damageInterval;
            
            //if player life is fewer than or equal to 0, play GAME OVER sound and restart the game
            if (lives <= 0)
            {
                restart = true;
                bgmSound.Stop();
                gameoverSound.Play();
                Time.timeScale = 0f;
            }
        }
    }

    //player falls and restart the game
    void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
