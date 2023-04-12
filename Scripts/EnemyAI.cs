using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 5f;
    public float detectionRange = 10f;
    public Transform player;

    private Rigidbody2D rb;
    private Rigidbody2D playerRb;

    // Add a public variable for the enemy's damage amount
    public float enemyDamage = 10f;

     // Add a variable for collision cooldown time
    public float collisionCooldown = 1f;
    private float timeSinceLastCollision = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerRb = playerObject.GetComponent<Rigidbody2D>();
        }
        else
        {
            Debug.LogError("Player GameObject with the 'Player' tag not found.");
        }

        Debug.Log("Initial enemy position: " + transform.position);
        Debug.Log("Initial player position: " + playerRb.position);
    }

    private void Update()
    {
        // Add a null check for playerRb
        if (playerRb == null)
        {
            StopMovement();
            return;
        }

        // Add this line to increment the time since the last collision
        timeSinceLastCollision += Time.deltaTime;

        float distance = Vector2.Distance(transform.position, playerRb.position);

        if (distance <= detectionRange)
        {
            //Debug.Log("Player detected, moving towards player.");
            MoveTowardsPlayer();
        }
        else
        {
            //Debug.Log("Player not detected, stopping movement.");
            StopMovement();
        }

        //Debug.Log("Enemy position: " + transform.position);
        //Debug.Log("Player position: " + playerRb.position);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && timeSinceLastCollision >= collisionCooldown)
        {
            Debug.Log("Enemy collided with player.");

            // Reset the time since the last collision
            timeSinceLastCollision = 0f;

            // Cause damage to the player
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerController != null)
            {
                playerController.TakeDamage(enemyDamage);
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (playerRb.position - (Vector2)transform.position);

        // Check if the direction vector is valid using a small tolerance value
        float tolerance = 0.001f;

        if (Mathf.Abs(direction.x) < tolerance && Mathf.Abs(direction.y) < tolerance)
        {
            //Debug.Log("Invalid direction vector: " + direction);
            return;
        }

        float distance = Vector2.Distance(transform.position, playerRb.position);
        //Debug.Log("Distance between player and enemy: " + distance);

        Vector2 normalizedDirection = direction.normalized;
        //Debug.Log("Enemy position before moving: " + transform.position);
        //Debug.Log("Direction to player: " + normalizedDirection);
        transform.position = Vector2.MoveTowards(transform.position, playerRb.position, speed * Time.deltaTime);
        //Debug.Log("Enemy position after moving: " + transform.position);
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }
}
