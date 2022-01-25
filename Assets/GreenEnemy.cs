using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenEnemy : MonoBehaviour
{
    public Vector2 pos1, pos2;
    public float velocity;

    public GameObject platform;

    bool direction; //False = pos1    True = pos2

    private void Start()
    {
        transform.position = pos1;
        direction = true;
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
    }

    void Update() {
       
        if (Input.GetKeyDown(KeyCode.R))
        {
            alive();
        }
    }

    public void dead() {
        platform.GetComponent<BoxCollider2D>().enabled = true;
        platform.GetComponent<SpriteRenderer>().enabled = true;

        transform.GetComponent<CircleCollider2D>().enabled = false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }
    public void alive() {
        platform.GetComponent<BoxCollider2D>().enabled = !true;
        platform.GetComponent<SpriteRenderer>().enabled = !true;

        transform.GetComponent<CircleCollider2D>().enabled = !false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = !false;
    }
}
