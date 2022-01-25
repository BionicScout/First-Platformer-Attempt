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
    public Transform leftWallCheck;
    public Transform rightWallCheck;

    public float groundRadius;
    public float wallRadius;

//Dash 
    public float dashTime = 1;
    private float currentAirTime;
    private float dashDir;
    private bool lastFace = false; //False = left      True = right

//Restart
    Vector2 startPos;

//Sword
    public LayerMask swordLayer;
    public Transform swordCheck;
    public float swordCheckRadius = 0.5f;
    private bool isHit;

    public GameObject sword;
    private Vector2 direction;
    private short swordState;
    

    void Start() {
        rb = (Rigidbody2D) this.GetComponent(typeof(Rigidbody2D));
        currentAirTime = dashTime + 1;
        startPos = transform.position;
    }

    void FixedUpdate() {
    //Movement
        horizontal = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

    //Collision Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        isOnWall = Physics2D.OverlapCircle(leftWallCheck.position, wallRadius, groundLayer) || Physics2D.OverlapCircle(rightWallCheck.position, wallRadius, groundLayer);
        isHit = Physics2D.OverlapCircle(swordCheck.position, swordCheckRadius, swordLayer);

    //Dash Rest
        if(currentAirTime > dashTime && (isGrounded || isOnWall))
        {
            currentAirTime = 0;
        }

    //Dash
        if (currentAirTime != 0 && currentAirTime <= dashTime) {
            dash();
        }

    //Last direction moved in or the direction facing
        if (horizontal > 0)
            lastFace = true;
        else if (horizontal < 0)
            lastFace = false;

    //Attack
        attack();

        if (isHit && swordState != 0) {
            Collider2D other = Physics2D.OverlapCircle(swordCheck.position, swordCheckRadius, swordLayer);
       
            if (other.gameObject.tag == "Enemy") {
                other.gameObject.GetComponent<GreenEnemy>().dead();
            }

            jump(1);
        }
        
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

    //Attack
        if (Input.GetKeyDown(KeyCode.J) && Input.GetKey(KeyCode.S)) { //Down Attack
            direction = new Vector3(0, -0.75f, 2);
            sword.transform.rotation = Quaternion.Euler(sword.transform.rotation.x, sword.transform.rotation.y, -90);
            
            swordState = 1;
        }
        else if (Input.GetKeyDown(KeyCode.J) && Input.GetKey(KeyCode.W)) { //Up Attack
            direction = new Vector3(0, 0.75f, 2);
            sword.transform.rotation = Quaternion.Euler(sword.transform.rotation.x, sword.transform.rotation.y, 90);

            swordState = 1;
        }
        else if (Input.GetKeyDown(KeyCode.J)) { //Right/Left Attack
            if (lastFace == true) { // Right Face
                sword.transform.rotation = Quaternion.Euler(sword.transform.rotation.x, sword.transform.rotation.y, 0);
                direction = new Vector3(0.75f, 0, 2);
            } else if(lastFace == false) { //Left Face
                sword.transform.rotation = Quaternion.Euler(sword.transform.rotation.x, sword.transform.rotation.y, 180);
                direction = new Vector3(-0.75f, 0, 2);
            }
            
            swordState = 1;
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

    private void attack() {
        if (swordState == 1) {//Sword Out
            sword.transform.localPosition = Vector3.MoveTowards(sword.transform.localPosition, direction, .5f);

            float distance = Vector2.Distance(sword.transform.localPosition, direction);
            if (distance < 0.05) { 
                swordState = 2; //Sword in
            }
        }
        else if(swordState == 2) { //Sword In
            sword.transform.localPosition = Vector3.MoveTowards(sword.transform.localPosition, new Vector3(0, 0, 2), .5f);

            float distance = Vector2.Distance(sword.transform.localPosition, new Vector3(0, 0, 2));
            if (distance < 0.05) {
                swordState = 0; //sword stay
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Harmful")
        {
            restart();
        }

        if (collision.gameObject.tag == "Exit") {
            print("Exit");
        }

    }

    private void restart() {
        transform.position = startPos;
    }
}
