using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class AdvancedPlayerMovement : MonoBehaviour
{

[Header("Player Settings")]
public float speed = 10F;
public float jumpHeight = 7F;
public float dashSpeed = 20F;
public float crouchHeight = 0.5F;

[Header("Ground Check")]
public LayerMask whatIsGround;
public Transform groundCheckPoint;
public float groundCheckRadius = 0.2F;

[Header("Attack")]
[SerializeField] private int attackDamage = 1;
[SerializeField] private float attackRange = 1F;

public LayerMask enemyLayers;

private Rigidbody2D body;
private Animator anim;
private bool grounded;
private bool canDoubleJump = false;
private bool isDashing = false;
private bool isCrouching = false;
private bool facingRight = true;

    // Start is called before the first frame update
    private void Awake()
    {
        InitializeComponents();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);
        HandleMovement();
    }

    void Update()
    {
        HandleInputs();
    }

    private void HandleInputs()
    {
        HandleJump();
        HandleDash();
        HandleCrouch();
        HandleAttack();
    }

    private void InitializeComponents()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        facingRight = !facingRight;
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpHeight);
        anim.SetTrigger("Jump");
        grounded = false;
        AudioManager.instance.PlayJumpSound();
    }

    private void HandleAttack()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            attack();
        }
    }

    private void HandleCrouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl) && grounded)
        {
            if(!isCrouching){
                transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
                isCrouching = true;
            }
            else if(isCrouching)
            {
                transform.localScale = new Vector3(transform.localScale.x, 1F, transform.localScale.z);
                isCrouching = false;
            }
        }
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        anim.SetBool("Walk", horizontalInput != 0);

        if(horizontalInput != 0 && grounded)
        {
            AudioManager.instance.PlayFootstepSound();
        }

        if((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
        {
            Flip();
        }
    }

    private void HandleDash()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private void HandleJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            Jump();
            canDoubleJump = true;
        }
        else if(Input.GetKeyDown(KeyCode.Space) && canDoubleJump)
        {
            Jump();
            canDoubleJump = false;
        }
    }

    IEnumerator Dash()
    {
        AudioManager.instance.PlayDashSound();
        float originalSpeed = speed;
        speed = dashSpeed;
        isDashing = true;
        yield return new WaitForSeconds(0.2F);
        speed = originalSpeed;
        isDashing = false;
    }

    void attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if(enemyController != null)
            {
                enemyController.TakeDamage(attackDamage);
                Debug.Log("Enemy Damaged!");
            }
        }
    }

    void onDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
