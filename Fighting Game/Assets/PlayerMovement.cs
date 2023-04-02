using UnityEngine;
using UnityEngine.InputSystem;
using MichaelWolfGames;
using System.Collections.Generic;

public partial class PlayerEntity : Entity
{

    [Header("Walking")]
    [SerializeField] private float m_moveSpeed; // Left and Right movement speed of player
    [SerializeField] private float m_MovementSmoothing; // Player movement smoothing
    [SerializeField] private float m_AirMoveSmoothing; // Player movement smoothing in air
    [SerializeField] private float m_maxSlopeLimit; // Max slope angle the player can walk on

    [Header("Jumping")]
    [SerializeField] private float m_maxJumpTime; // Max time the player can jump
    [SerializeField] private float m_minJumpTime; // Min time the player can jump
    [SerializeField] private float m_jumpPeakTime; // 
    [SerializeField] private float m_initialJumpVelocity; // The initial jump velocity
    [SerializeField] private float m_jumpGravityScale; // Current jump gravity scale multiplied by a value
    [SerializeField] private float m_fallGravityScale; // Current fall gravity scale multiplied by a value

    [Header("Sliding")]
    [SerializeField] private float m_slideSpeed;
    [SerializeField] private float m_slideMovementSmoothing;
    [SerializeField] private float m_maxSlideTime;
    [SerializeField] private float m_slideCooldown;

    [Header("Wall Jump")]
    [SerializeField] private Vector2 m_wallCheckOffset;
    [SerializeField] private Vector2 m_handSize;

    private RaycastHit2D[] m_groundHitList = new RaycastHit2D[5];
    private Collider2D[] m_wallContactList = new Collider2D[5];

    private PlayerInput m_playerInput;
    private InputAction m_moveAction;
    private InputAction m_jumpAction;
    private InputAction m_slideAction;
    private InputAction m_attackAction;
    private InputAction m_aimLockAction;
    private InputAction m_itemPickupDropAction;

    private Vector2 m_movementInput; // The vertical and horizontal player input.
    private int m_facingDirection = 1; // The direction the player is facing. (-1 left, 1 right)

    private Vector2 m_smoothingVelocity = Vector2.zero; // Velocity for smoothing of player movement
    private float m_jumpTimer = 0; // Timer used for holding jump
    private bool m_remainJumping = false; // Should remain jumping?
    private bool m_isOnGround; // Is the player on the ground?
    private bool m_jump = false; // Flag for jumping.
    private bool m_jumpHeld = false; // Detects if the jump button is held or not
    private bool m_isJumping = false; // If the player is jumping (from jump begin, to touch ground again).
    private bool m_wasJumping = false;
    private bool m_isSliding;
    private float m_slideTimer;
    private bool m_enableHorizontalMovement = true;

    private float m_minJumpOffset;
    private bool m_canBeginSlide = true;

    private bool m_isWallClimbing = false;



    /// <summary>
    /// OnAwake for PlayerEntity
    /// </summary>
    protected override void OnAwake()
    {
        instance = this;

        // Set up item contact filter
        m_itemContactFilter.useLayerMask = true;
        m_itemContactFilter.layerMask = m_itemMask;

        // Set up player input
        m_playerInput = GetComponent<PlayerInput>();

        m_moveAction = m_playerInput.actions.FindAction("Move");
        m_jumpAction = m_playerInput.actions.FindAction("Jump");
        m_slideAction = m_playerInput.actions.FindAction("Slide");
        m_attackAction = m_playerInput.actions.FindAction("Attack");
        m_aimLockAction = m_playerInput.actions.FindAction("AimLock");
        m_itemPickupDropAction = m_playerInput.actions.FindAction("ItemPickupDrop");

        m_jumpAction.started += OnJump;
        m_jumpAction.canceled += OnJump;

        m_slideAction.started += OnSlide;
        m_slideAction.canceled += OnSlide;

        m_attackAction.started += OnAttack;

        m_aimLockAction.started += OnAimLock;
        m_aimLockAction.canceled += OnAimLock;

        m_itemPickupDropAction.started += OnItemPickupDrop;

        m_jumpTimer = m_maxJumpTime;
    }


    /// <summary>
    /// Update is called every frame.
    /// </summary>
    void Update()
    {
        HandlePlayerMovement();
        HandlePlayerShooting();
    }


    /// <summary>
    /// Handle player movement InputAction
    /// </summary>
    private void HandlePlayerMovement()
    {
        m_movementInput = m_moveAction.ReadValue<Vector2>();
    }


    // Handle Jump InputAction
    #region Jump_Input

    /// <summary>
    /// OnJump input callback.
    /// </summary>
    /// <param name="context">Context of the input action.</param>
    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnJumpStarted();
        }
        else if (context.canceled)
        {
            OnJumpCanceled();
        }
    }


    /// <summary>
    /// OnJump input pressed.
    /// </summary>
    private void OnJumpStarted()
    {
        m_jump = true;
        m_jumpHeld = true;
    }


    /// <summary>
    /// OnJump input released.
    /// </summary>
    private void OnJumpCanceled()
    {
        m_jumpHeld = false;
    }
    #endregion

    // Handle Slide InputAction
    #region Slide_Input

    /// <summary>
    /// OnSlide input callback.
    /// </summary>
    /// <param name="context">Context of the input action.</param>
    private void OnSlide(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnSlideStarted();
        }
        else if (context.canceled)
        {
            OnSlideCanceled();
        }
    }


    /// <summary>
    /// OnSlide input pressed.
    /// </summary>
    private void OnSlideStarted()
    {
        if (m_canBeginSlide)
        {
            m_canBeginSlide = false;

            m_isSliding = true;
            m_slideTimer = 0f;

            m_jump = false;
            m_enableHorizontalMovement = false;
        }
    }


    /// <summary>
    /// OnSlide input released.
    /// </summary>
    private void OnSlideCanceled()
    {
        if (m_isSliding)
        {
            // Stop sliding
            m_isSliding = false;
            m_enableHorizontalMovement = true;
            m_slideTimer = 0f;

            // Start slide cooldown
            this.StartTimer(m_slideCooldown, () => m_canBeginSlide = true);
        }
    }
    #endregion

    // Handle Attack InputAction
    #region Attack_Input

    /// <summary>
    /// OnAttack input callback.
    /// </summary>
    /// <param name="context">Context of the input action.</param>
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Attack();
        }
    }
    #endregion

    //Handle Aim Lock InputAction
    #region AimLock_Input

    /// <summary>
    /// OnAimLock input callback.
    /// </summary>
    /// <param name="context">Context of the input action.</param>
    private void OnAimLock(InputAction.CallbackContext context)
    {
        if (!m_isSliding)
        {
            if (context.started)
            {
                m_enableHorizontalMovement = false;
            }
            else if (context.canceled)
            {
                m_enableHorizontalMovement = true;
            }
        }
    }
    #endregion

    // Handle Item Pickup / Drop InputAction
    #region ItemPickupDrop_Input

    /// <summary>
    /// Callback for item pickup and drop input.
    /// </summary>
    /// <param name="context">Context of the input action.</param>
    private void OnItemPickupDrop(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            TryPickupDropItem();
        }
    }
    #endregion


    /// <summary>
    /// Called every fixed framerate frame.
    /// </summary>
    private void FixedUpdate()
    {
        // Get ground check
        m_isOnGround = IsOnGround();

        // Player Direction
        if (!m_isSliding && Mathf.Abs(m_movementInput.x) > 0)
        {
            m_facingDirection = (int)Mathf.Sign(m_movementInput.x);

        }

        Move();
        HandleSlide();
        HandleWallClimb();
        HandleJump();
    }


    /// <summary>
    /// Logic for handling sliding movement.
    /// </summary>
    private void HandleSlide()
    {
        if (m_isSliding)
        {
            if (m_slideTimer < m_maxSlideTime && !m_jump)
            {
                // Normal sliding
                Vector2 targetSlideVelocity = new Vector2((transform.right * m_slideSpeed * m_facingDirection).x, m_rigidbody.velocity.y);
                m_rigidbody.velocity = Vector2.SmoothDamp(m_rigidbody.velocity, targetSlideVelocity, ref m_smoothingVelocity, m_slideMovementSmoothing);
            }
            else
            {
                OnSlideCanceled();

            }

            m_slideTimer += Time.deltaTime;
        }
    }


    /// <summary>
    /// Logic for handling player jumping.
    /// </summary>
    private void HandleJump()
    {
        if (m_jump && m_isOnGround)
        {
            m_jumpTimer = 0;
            m_remainJumping = true;
        }

        // Remain jumping until jump is not held
        if (m_remainJumping)
        {
            if (m_jumpTimer < m_minJumpTime)
            {
                // Minimum jump time
                m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, m_initialJumpVelocity);
                m_jumpTimer += Time.fixedDeltaTime;
            }
            else if (m_jumpHeld && m_jumpTimer < m_maxJumpTime)
            {
                // Maximum jump time
                m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, m_initialJumpVelocity);
                m_jumpTimer += Time.fixedDeltaTime;
            }
            else
            {
                m_remainJumping = false;
            }
        }
        
        /*// Peak jump
        if (m_jumpTimer < m_jumpPeakTime)
        {
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, m_initialJumpVelocity);
        }*/

        if (m_jump)
        {
            bool canWallJump = IsOnWall() && !m_isOnGround;
            if (m_isOnGround || canWallJump)
            {
                m_jumpTimer = 0;
                m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, m_initialJumpVelocity);
                m_remainJumping = true;

                m_wasJumping = true;

                if (canWallJump)
                {
                    // Give velocity for wall jump
                    m_rigidbody.velocity += (Vector2)(-m_facingDirection * m_initialJumpVelocity * transform.right);
                    
                    // Make player face opposite direction after leaving wall.
                    m_facingDirection = -m_facingDirection;
                }
            }
            
        }

        m_jump = false;

        // Handle setting isJumping
        if (m_wasJumping)
        {
            if (!m_isOnGround && !m_isWallClimbing)
            {
                m_isJumping = true;
            }
            else if (m_isJumping)
            {
                m_isJumping = false;
                m_wasJumping = false;
            }
        }

        if (m_remainJumping)
        {
            m_rigidbody.gravityScale = m_jumpGravityScale;
        }
        else
        {
            m_rigidbody.gravityScale = m_fallGravityScale;
        }

    }


    private void HandleWallClimb()
    {
        if (!m_isOnGround && Mathf.Abs(m_movementInput.x) > 0 && m_rigidbody.velocity.y < 0 && IsOnWall())
        {
            m_isWallClimbing = true;
            m_rigidbody.velocity = Vector2.zero;
        }
        else
        {
            m_isWallClimbing = false;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(foot.transform.position, footRadius);
    }

    /// <summary>
    /// Logic for handling left and right movement.
    /// </summary>
    private void Move()
    {
        Vector2 targetVelocity;
        float moveSmoothing;
        Vector2 groundNormal = Vector2.zero;

        if (m_enableHorizontalMovement)
        {
            float rawXInput = (m_movementInput.x != 0 ? Mathf.Sign(m_movementInput.x) : 0);

            // Left and right movement
            if (m_isOnGround)
            {
                groundNormal = m_groundHitList[0].normal;
                Vector2 groundTangent = -Vector2.Perpendicular(m_groundHitList[0].normal);
                float groundAngle = Mathf.Abs(Vector2.SignedAngle(Vector2.right, groundTangent));


                if (groundAngle > 0 && groundAngle <= m_maxSlopeLimit)
                {
                    // Handle slope movement
                    targetVelocity = groundTangent * rawXInput * m_moveSpeed;
                    Debug.DrawRay(transform.position, targetVelocity);

                    if (!m_jump && !m_isJumping && m_movementInput.x == 0)
                    {
                        // TODO - make jump not move player, make weird small jump not happen
                        Vector2 newGravity = (-Physics2D.gravity.normalized + -groundNormal) * Physics2D.gravity.magnitude * m_rigidbody.gravityScale;
                        Debug.DrawRay(transform.position, newGravity, Color.red);
                        m_rigidbody.AddForce(newGravity);
                    }
                }
                else
                {
                    // Default non-slope movement
                    targetVelocity = new Vector2(rawXInput * m_moveSpeed, m_rigidbody.velocity.y);
                }

            }
            else
            {
                targetVelocity = new Vector2(rawXInput * m_moveSpeed, m_rigidbody.velocity.y);
            }
            //targetVelocity = new Vector2(rawXInput * m_moveSpeed, m_rigidbody.velocity.y);

        }
        else
        {
            // No horizontal movement
            targetVelocity = new Vector2(0, m_rigidbody.velocity.y);
        }

        if (m_isOnGround)
        {
            moveSmoothing = m_MovementSmoothing;
        }
        else
        {
            moveSmoothing = m_AirMoveSmoothing;
        }

        m_rigidbody.velocity = Vector2.SmoothDamp(m_rigidbody.velocity, targetVelocity, ref m_smoothingVelocity, moveSmoothing);

        
            
    }


    protected override bool IsOnGround()
    {
        // Check if touching ground
        int results = Physics2D.CircleCast(foot.transform.position, footRadius, -transform.up, groundContactFilter, m_groundHitList, 0);
        return results > 0;
    }


    private bool IsOnWall()
    {
        // Add check for kinematic or not rigidbody?
        int results = Physics2D.OverlapBox(transform.position + (m_facingDirection * m_wallCheckOffset.x * transform.right) + (m_wallCheckOffset.y * transform.up), m_handSize, 0, groundContactFilter, m_wallContactList);
        return results > 0;
    }


    /// <summary>
    /// Get the player movement input.
    /// </summary>
    /// <returns>The current movement input for the player.</returns>
    private Vector2 GetMoveInput()
    {
        return m_movementInput;
    }

}