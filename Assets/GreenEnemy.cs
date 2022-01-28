using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenEnemy : MonoBehaviour
{
    public Vector2 pos1, pos2;
    public float velocity;

    public GameObject platform;
    public float deadTime;

    private float currentDeadTime;

    bool direction; //False = pos1    True = pos2

    private void Start()
    {
        transform.position = pos1;
        direction = true;
        alive();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(direction == true) {
            transform.position = Vector2.MoveTowards(transform.position, pos2, velocity);

            float distance = Vector2.Distance(transform.position, pos2);
            if (distance < 0.05) {
                direction = false;
            }
        }
        else {
            transform.position = Vector2.MoveTowards(transform.position, pos1, velocity);

            float distance = Vector2.Distance(transform.position, pos1);
            if (distance < 0.05) {
                direction = true;
            }
        }

    //Respawn Time
        currentDeadTime += Time.deltaTime;

        if (deadTime*2 > currentDeadTime && currentDeadTime >= deadTime)
        {
            alive();
        }
    }

    void Update() {
       
        if (Input.GetKeyDown(KeyCode.R))
        {
            alive();
        }
    }

    public void dead() {
        currentDeadTime = 0;

        platform.GetComponent<BoxCollider2D>().enabled = true;
        platform.GetComponent<SpriteRenderer>().enabled = true;

        transform.GetComponent<CircleCollider2D>().enabled = false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }
    public void alive() {
        currentDeadTime = deadTime * 1;

        platform.GetComponent<BoxCollider2D>().enabled = !true;
        platform.GetComponent<SpriteRenderer>().enabled = !true;

        transform.GetComponent<CircleCollider2D>().enabled = !false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = !false;
    }
}
