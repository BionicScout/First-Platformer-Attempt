using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    Rigidbody2D rb;

//Movement Speed Variables
    public float speed = 5;
    public float jumpSpeed = 5;
    public float wallSlideSpeed = 1;
    public float dashSpeed = 12;

    //Player Input
    private float horizontal;

//Collision Check Variables
    private bool isGrounded = false;
    private bool isOnWall = false;
    private bool hitCeiling = false;

    private float timeOnWall = 0;

//Collision Points
    public LayerMask groundLayer;

    public Transform groundCheck;
    public Transform ceilingCheck;
    public Transform leftWallCheck;
    public Transform rightWallCheck;

    public float groundRadius;
    public float wallRadius;

//Dash 
    public float dashTime = 1;
    private float currentAirTime;
    //private bool canDash = true;
    private float dashDir;
    private bool lastFace = false; //False = left      True = right
   //private bool isDashing = false;

    void Start() {
        rb = (Rigidbody2D) this.GetComponent(typeof(Rigidbody2D));
        currentAirTime = dashTime + 1;
    }

    void FixedUpdate() {
        // print(currentAirTime);

        if (dashDir > 0)
            print("Right - On wall: " + isOnWall);
        else
            print("Left - On wall: " + isOnWall);
    //Movement
        horizontal = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

    //Collision Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        isOnWall = Physics2D.OverlapCircle(leftWallCheck.position, wallRadius, groundLayer) || Physics2D.OverlapCircle(rightWallCheck.position, wallRadius, groundLayer);
        hitCeiling = Physics2D.OverlapCircle(ceilingCheck.position, wallRadius, groundLayer);

    //Dash Rest
        if(currentAirTime > dashTime && (isGrounded || isOnWall))
        {
            currentAirTime = 0;
        }

    //Dash
        if (currentAirTime != 0 && currentAirTime <= dashTime) {
            dash();
        }
        //else
        //    isDashing = false;
    //Last direction moved in or the direction facing
        if (horizontal > 0)
            lastFace = true;
        else if (horizontal < 0)
            lastFace = false;
    }

    void Update() {
    //Jump/Ground Collision
        if (isGrounded && Input.GetButtonDown("Jump")) {
            jump(1); //Ground State
        }

    //Wall Slide and Wall from Dash Collision
        if (isOnWall && currentAirTime >.1) {
            currentAirTime += dashTime + 1;
        }

        if (isOnWall && horizontal != 0) {
            againstWall();
        }
        else {
            timeOnWall = 0;
        }

    //Ceiling Check
        if (hitCeiling) {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, 0));
        }

    //Dash Check
        if (Input.GetButtonDown("Fire3") && (currentAirTime == 0 || currentAirTime > dashTime)) { //First Dash Check
            if (lastFace == true) //Facing right
                dashDir = 1;
            else //Facing Left
                dashDir = -1;

            if (isOnWall) //Dash opposite way of wall
                dashDir = -dashDir;

            dash();
        } 
    }

    private void jump(short jumpState) {
        if (jumpState == 1) { //Ground Jump
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        else if (jumpState == 2) {  //Wall Jump
            rb.velocity = new Vector2(-rb.velocity.x, jumpSpeed);
        }

        isGrounded = false;

        //canDash = true;
        //currentAirTime = 0;
    }

    private void againstWall() {
    //Defualt slide speed
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        timeOnWall += Time.deltaTime;

        if (timeOnWall < 0.05) { //Initial Slide speed
            rb.velocity = new Vector2(rb.velocity.x, 0.5f);
        }

    //Wall Jump
        if (Input.GetButtonDown("Jump")) {
            jump(2); //Wall Jump
        }
    }

    private void dash() {
        currentAirTime += Time.deltaTime;
        if (dashDir < 0)
            rb.velocity = new Vector2(-dashSpeed, 0);
        else
            rb.velocity = new Vector2(dashSpeed, 0);
    }
}
