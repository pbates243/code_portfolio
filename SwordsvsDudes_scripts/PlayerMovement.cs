using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour {
    private float speed = 6f;
    private float slideForce = 500f;
    private float jumpForce = 15f;
    public bool isGrounded = false;
    public bool isSliding = false;
    public bool touchingHead = false;
    public PlayerCombat combat;
    public PlayerHealth health;
    public float timer = 3;
    Vector2 jump = new Vector2(0, 12);


    float slideRate = 1.5f; // How often the player can slide after having slide(d?)
    float slideTimer;

    [Header("Sound")]
    public float footstepVolume = 0.4f;
    public AudioClip[] footsteps;
    float footstepRate = 0.375f;
    float footstepTimer;

    private Vector3 _downRayOffset = new Vector2(0, -1.15f);
    private Vector3 _leftRayOffset = new Vector2(-0.575f, -0.9f);
    private Vector3 _rightRayOffset = new Vector2(0.575f, -0.9f);
    private float _groundTouchDistance = 0.05f;

    [Header("Axis")]
    public int horizontal = 0;
    public int vertical = 0;
    public bool UpArrow = false, leftShift;

    [Header("Dependencies")]
    public Animator anim;
    public GameObject soundPrefab;

    [Header("Optional")]
    public bool canSlide = true;

    [Header("Set via script")]
    public bool beingPushedBack = false;

    Rigidbody2D rb;

    private void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        //fx = GameObject.FindObjectOfType<FXManager>();
    }

    private void FixedUpdate ()
    {
        if (touchingHead)
        {
            timer -= Time.deltaTime;
        }

        if (timer < 1 && touchingHead)
        {
            health.TakeDamage(1);

            Vector3 vel = rb.velocity;
            vel.y = jump.y;
            rb.velocity = vel;
            timer = 3;
            
        }
        // Make calculations to see if the player is touching the ground
        isGrounded = CheckRayBelow();

        // Caclulate for input
        CalculateInput();

        // Calculate movement sound directly after input
        CalculateSound();

        // Calculate the animator (if one is present)
        if (anim != null)
        {
            CalculateAnimator();
        }

        // Calculate slide timer
        if(!isSliding)
        {
            slideTimer -= Time.deltaTime;
        }

        // The player can't do anything while they're being pushed back
        if (beingPushedBack)
        {
            return;
        }

        // Flip the scale based on input
        if (horizontal == 1)
        {
            transform.localScale = new Vector3(1, 1, 1);
        } else if(horizontal == -1)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Apply calculations for sliding
        if(leftShift && !isSliding && horizontal != 0 && isGrounded && slideTimer <= 0 && canSlide)
        { // If pressed left shift but we haven't started sliding yet AND we're running/moving AND we're on the ground
            rb.AddForce(Vector2.right * horizontal * slideForce);
            slideTimer = 999f;
            isSliding = true;
        }

        // Calculate movement & velocity
        Vector3 cVel = rb.velocity;             // Current velocity
        Vector3 nVel = new Vector3(0, cVel.y);  // New velocity

        // If we aren't sliding then act normally
        if(!isSliding)
        {
            // Apply motion according to input
            nVel.x = horizontal * speed;

            // Secondary calculation for Y input
            if (isGrounded && UpArrow)
            {
                nVel.y = jumpForce;
                //Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 100, Time.deltaTime);
            }

            if (!isGrounded && Input.GetKeyDown(KeyCode.DownArrow))
            {
                nVel.y = -jumpForce;
            }

        } else
        { // If the player is sliding
            // Then inherit the x velocity from the current velocity
            nVel.x = cVel.x;

            // And do some checks in case we need to stop sliding
            if(nVel.x <= 0.3f && nVel.x >= -0.3f)
            {
                isSliding = false;
                slideTimer = slideRate;
            } else if(!isGrounded)
            {
                isSliding = false;
                slideTimer = slideRate;
            }
        }

        // Finally make calculations to stop the player from sticking to walls
        if(horizontal == -1 && HitCheckLeft())
        {
            nVel.x = 0;
        }
        if(horizontal == 1 && HitCheckRight())
        {
            nVel.x = 0;
        }

        // Set the velocity of the player
        rb.velocity = nVel;

        // Apply gravity
        rb.AddForce(Physics2D.gravity);
    }

    private bool HitCheckLeft()
    {
        bool hitAnything = false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + _leftRayOffset, Vector2.left);
        Debug.DrawRay(transform.position + _leftRayOffset, Vector2.left);
        if (hit == false)
        {
            return false;
        }
        float distance = hit.distance;
        GameObject obj = hit.collider.gameObject;

        if (distance <= _groundTouchDistance)
        {
            hitAnything = true;
        }

        return hitAnything;
    }
    private bool HitCheckRight()
    {
        bool hitAnything = false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + _rightRayOffset, Vector2.right);
        Debug.DrawRay(transform.position + _rightRayOffset, Vector2.right);
        if (hit == false)
        {
            return false;
        }
        float distance = hit.distance;
        GameObject obj = hit.collider.gameObject;

        if (distance <= _groundTouchDistance)
        {
            hitAnything = true;
        }



        return hitAnything;
    }

    private void CalculateSound ()
    {
        if(isGrounded)
        {
            // If we're inputing left/right & we're not sliding
            if (horizontal != 0 && !isSliding)
            {
                footstepTimer -= Time.fixedDeltaTime;
                if(footstepTimer <= 0)
                {
                    int footSound = Random.Range(0, footsteps.Length);
                    AudioSource source = Instantiate(soundPrefab).GetComponent<AudioSource>();
                    source.volume = footstepVolume;
                    footstepTimer = footstepRate;
                }
            }
        } else
        {
            footstepTimer = 0;
        }
    }

    private void CalculateAnimator ()
    {
        if(horizontal != 0)
        {
            anim.SetBool("isMoving", true);
        } else
        {
            anim.SetBool("isMoving", false);
        }

        if(UpArrow)
        {
            if(isGrounded)
            {
                anim.SetBool("jumping", true);
            }
        } else
        {
            anim.SetBool("jumping", false);
        }

        if(!isGrounded)
        {
            anim.SetBool("falling", true);
        } else
        {
            anim.SetBool("falling", false);
        }

        anim.SetBool("sliding", isSliding);
    }

    private void CalculateInput ()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            vertical = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            vertical = -1;
        } else
        {
            vertical = 0;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal = -1;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontal = 1;
        } else
        {
            horizontal = 0;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            UpArrow = true;
        } else
        {
            UpArrow = false;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            leftShift = true;
        } else
        {
            leftShift = false;
        }

    }

    private bool CheckRayBelow ()
    {
        bool hitGround = false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + _downRayOffset, Vector2.down);
        Debug.DrawRay(transform.position + _downRayOffset, Vector2.down);
        if (hit == false)
        {
            return hitGround;
        }
        float distance = hit.distance;              // The distance from the thing that was hit
        GameObject obj = hit.collider.gameObject;   // The object that was hit

        // If this object is labeled ground & is within the distance that ground
        // can be for it to count as the player touching it
        if(obj.tag == "Ground" && distance <= _groundTouchDistance)
        {
            hitGround = true;
        }

        return hitGround;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "EnemyHead")
        {
            touchingHead = true;

             
        }


    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyHead")
        {
            touchingHead = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        touchingHead = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            touchingHead = false;
        }
    }

}
