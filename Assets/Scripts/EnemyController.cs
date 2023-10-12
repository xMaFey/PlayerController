using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5F;
    [SerializeField] private float groundCheckDistance = 0.6F;
    [SerializeField] private LayerMask whatIsGround;
    private bool movingRight = true;

    [Header("Combat Settings")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;
    [SerializeField] private int damage = 1;
    private Rigidbody2D enemyRigidBody;
    //private EnemySpawner spawner;

    // Start is called before the first frame update
    void Awake()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Move()
    {
        Vector2 groundCheckPosition = movingRight ?
            new Vector2(transform.position.x + 0.5F, enemyRigidBody.velocity.y):
            new Vector2(transform.position.x - 0.5F, enemyRigidBody.velocity.y);

        bool isGrounded = Physics2D.Raycast(groundCheckPosition, Vector2.down, groundCheckDistance);

        if(!isGrounded)
        {
            movingRight = !movingRight;
        }

        enemyRigidBody.velocity = movingRight ?
            new Vector2(moveSpeed, enemyRigidBody.velocity.y):
            new Vector2(-moveSpeed, enemyRigidBody.velocity.y);
    }


}
