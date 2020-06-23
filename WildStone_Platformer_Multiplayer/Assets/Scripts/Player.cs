using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Config
    [SerializeField] float speed = 10.0f;
    [SerializeField] float jumpForce = 5.0f;
    [SerializeField] float climbSpeed = 3.0f;
    [SerializeField] float deathDelay = 1.0f;
    [SerializeField] Vector2 deathKick = new Vector2(25f, 25f);

    // State
    bool isAlive = true;

    // Cached component references
    Rigidbody2D myRigidbody;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeet;
    Animator myAnimator;
    SoundEffects soundEffects;
    float gravityScaleAtStart;

    
    // Message then methods
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeet = GetComponent<BoxCollider2D>();
        myAnimator = GetComponent<Animator>();
        soundEffects = FindObjectOfType<SoundEffects>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) {return;}
        Run();
        ClimbLadder();
        Jump();
        FlipSprite();
        Die();
    }

    private void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards")))
        {
            myAnimator.SetTrigger("Die");
            myRigidbody.velocity = deathKick;
            isAlive = false;
            soundEffects.PlayHurtSound();
            StartCoroutine(DieRoutine());
        }
    }

    IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(deathDelay);
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }

    private void Run()
    {
        float controlThrow = Input.GetAxis("Horizontal"); // -1 to +1
        Vector2 playerVelocity = new Vector2(controlThrow * speed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        myAnimator.SetBool("Running", IsRunning());
    }

    private void ClimbLadder()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing"))) 
        {
            myAnimator.SetBool("Climbing", false);
            myRigidbody.gravityScale = gravityScaleAtStart;
            return; 
        }

        float controlThrow = Input.GetAxis("Vertical"); // -1 to +1
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, controlThrow * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;

        myAnimator.SetBool("Climbing", PlayerHasVerticalSpeed());
    }


    private void Jump()
    {
        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))) {return;}
        
        if (Input.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpForce);
            myRigidbody.velocity += jumpVelocityToAdd;
            soundEffects.PlayJumpSound();
        }
    }


    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = IsRunning();
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    private bool IsRunning()
    {
        return Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
    }

    private bool PlayerHasVerticalSpeed()
    {
        return Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
