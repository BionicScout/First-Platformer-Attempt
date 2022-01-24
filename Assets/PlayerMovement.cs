using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    Rigidbody2D rb;

//Movement Variables
    public float speed = 5;
    public float jumpSpeed = 5;
    public float wallSlideSpeed = 1;

    private float horizontal;

//Collision Check Variables
    private bool isGrounded = false;

    private bool isOnWall = false;
    private bool hitCeiling = false;
    private float timeOnWall = 0;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform ceilingCheck;
    public Transform leftWallCheck;
    public Transform rightWallCheck;

    public float groundRadius;
    public float wallRadius;

    //Dash 
    public float dashTime = 1;
    private float currentAirTime = 0; 
    public float dashSpeed = 10;
    private bool canDash = true;
    private float dashDir;
    private bool lastFace = false; //False = left      True = right
    private bool isDashing = false;



    void Start() {
        rb = (Rigidbody2D) this.GetComponent(typeof(Rigidbody2D));
    }

    void FixedUpdate() {
    //Movement
        horizontal = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);


    //Collision Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        isOnWall = Physics2D.OverlapCircle(leftWallCheck.position, wallRadius, groundLayer) || Physics2D.OverlapCircle(rightWallCheck.position, wallRadius, groundLayer);
        hitCeiling = Physics2D.OverlapCircle(ceilingCheck.position, wallRadius, groundLayer);

        //Dash
        if (currentAirTime != 0 && currentAirTime <= dashTime)
        {
            currentAirTime += Time.deltaTime;
            if (dashDir < 0)
                rb.velocity = new Vector2(-dashSpeed, 0);
            else
                rb.velocity = new Vector2(dashSpeed, 0);
            Debug.Log("Dashing - " + isDashing);
        }
        else
            isDashing = false;

        if (horizontal > 0)
            lastFace = true;
        else if (horizontal < 0)
            lastFace = false;
    }

    void Update() {
    //Jump/Ground Collision
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            isGrounded = false;

            canDash = true;
            currentAirTime = 0;
        }

    //Wall Jump/Wall Collision
        if (isOnWall && horizontal != 0 && !isDashing) {
            canDash = true;
            currentAirTime = 0;

            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
            timeOnWall += Time.deltaTime;

            if (timeOnWall < 0.05) {
                rb.velocity = new Vector2(rb.velocity.x, 0.5f);
            }
            if (Input.GetButtonDown("Jump")) {
                rb.velocity = new Vector2(-horizontal * speed, jumpSpeed);
            }

            if (Input.GetButtonDown("Fire3")) { //First Dash Check
                canDash = false;
                currentAirTime += Time.deltaTime;

                if (horizontal != 0)
                    dashDir = -horizontal;

                rb.velocity = new Vector2(dashDir * dashSpeed, 0);
                Debug.Log("Start Dash - " + dashDir);
                isDashing = true;
            }

        }
        else {
            timeOnWall = 0;
        }

    //Ceiling Check
        if (hitCeiling) {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, 0));
        }

    //Dash Check
        if (Input.GetButtonDown("Fire3")) { //First Dash Check
            canDash = false;
            currentAirTime += Time.deltaTime;

            if (horizontal != 0)
                dashDir = horizontal;
            else if (lastFace == true) //Facing right
                dashDir = 1;
            else //Facing Left
                dashDir = -1;

            rb.velocity = new Vector2(dashDir * dashSpeed, 0);
            Debug.Log("Start Dash - " + dashDir);

            isDashing = true;
        } else
            Debug.Log("No Dash - " + isDashing);
    }
}
