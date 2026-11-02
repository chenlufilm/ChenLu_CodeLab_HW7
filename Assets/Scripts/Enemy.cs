using UnityEngine;

public class Enemy : MonoBehaviour
{
    //give enemy a rigidbody
    public Rigidbody2D myBody;
    //call for sprite renderer for enemies to flip their sprites when patrolling
    public SpriteRenderer mySprite;
    
    //define enemy patrolling borders
    public Transform leftPoint;
    public Transform rightPoint;
    
    //enemy patrolling speed
    public float speed;
    
    //bool to define enemy's moving direction
    private bool movingRight = true;

    //enemy movement
    void FixedUpdate()
    {
        //enemy patrol state switch
        if (transform.position.x >= rightPoint.position.x)
        {
            movingRight = false;
        }else if (transform.position.x <= leftPoint.position.x)
        {
            movingRight = true;
        }

        //if enemy's moving right, move with set speed and keep sprite's position
        if (movingRight)
        {
            myBody.linearVelocityX = speed;
            mySprite.flipX = false;
        }
        else //if enemy's moving left, move with set speed and flip sprite's position
        {
            myBody.linearVelocityX = -speed;
            mySprite.flipX = true;
        }
    }
}
