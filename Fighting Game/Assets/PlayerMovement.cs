using UnityEngine;
using UnityEngine.InputSystem;
using MichaelWolfGames;

public partial class PlayerEntity : Entity
{

    [Header("Walking")]
    [SerializeField] private float m_moveSpeed; // Left and Right movement speed of player
    [SerializeField] private float m_MovementSmoothing; // Player movement smoothing
    [SerializeField] private float m_AirMoveSmoothing; // Player movement smoothing in air

    [Header("Jumping")]
    [SerializeField] private float m_maxJumpTime; // Max time the player can jump
    [SerializeField] private float m_minJumpTime; // Min time the player can jump
    [SerializeField] private float m_initialJumpVelocity; // The initial jump velocity
    [SerializeField] private GameObject m_foot; // The position where the ground check will take place
    [SerializeField] private float m_footRadius; // Radius of the ground check
    [SerializeField] private LayerMask m_groundMask;
    [SerializeField] private float m_fallGravityScale; // Current fall velocity multiplied by a value

    [Header("Sliding")]
    [SerializeField] private float m_slideSpeed;
    [SerializeField] private float m_slideMovementSmoothing;
    [SerializeField] private float m_maxSlideTime;
    [SerializeField] private float m_slideCooldown;

    [Header("Wall Jump")]
    [SerializeField] private float m_wallCheckOffset;
    [SerializeField] private Vector2 m_handSize;

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
    private bool m_isSliding;
    private float m_slideTimer;
    private bool m_enableHorizontalMovement = true;

    private float m_minJumpOffset;
    private bool m_canBeginSlide = true;


    /// <summary>
    /// OnAwake for PlayerEntity
    /// </summary>
    protected override void OnAwake()
    {
        instance = this;
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
    }


    /// <summary>
    /// Update is called every frame.
    /// </summary>
    void Update()
    {
        HandlePlayerMovement();
        HandlePlayerShooting();
        OnAnimate();
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

        m_minJumpOffset = 0f;
    }


    /// <summary>
    /// OnJump input released.
    /// </summary>
    private void OnJumpCanceled()
    {
        m_jumpHeld = false;

        if (m_remainJumping)
        {
            if (m_rigidbody.velocity.y > 0)
            {
                m_minJumpOffset = m_minJumpTime;
            }
        }

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
            Consume();
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
        // Ground Check
        Collider2D groundCollider = Physics2D.OverlapCircle(m_foot.transform.position, m_footRadius, m_groundMask.value);
        if (groundCollider)
        {
            m_isOnGround = true;
        }
        else
        {
            m_isOnGround = false;
        }

        // Handle gravity scale for player.
        if (m_rigidbody.velocity.y < 0)
        {
            m_rigidbody.gravityScale = m_fallGravityScale;
        }
        else
        {
            m_rigidbody.gravityScale = 1f;
        }

        // Player Direction
        if (!m_isSliding && Mathf.Abs(m_movementInput.x) > 0)
        {
            m_facingDirection = (int)Mathf.Sign(m_movementInput.x);
        }

        Move();
        HandleSlide();
        HandleJump();
    }


    /// <summary>
    /// Logic for handling sliding movement.
    /// </summary>
    private void HandleSlide()
    {
        if (m_isSliding)
        {
            if (m_slideTimer < m_maxSlideTime)
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
        // Initial Jump
        if (m_jump && m_isOnGround)
        {
            m_jumpTimer = 0;
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, m_initialJumpVelocity);
            m_remainJumping = true;
        }

        // Remain Jumping
        if (m_remainJumping)
        {
            if (m_jumpTimer <= m_maxJumpTime && m_rigidbody.velocity.y >= 0)
            {
                m_jumpTimer += Time.fixedDeltaTime;

                if (m_maxJumpTime > 0)
                {
                    float jumpVelFactor = 1 - Mathf.Clamp01((m_jumpTimer / m_maxJumpTime) + m_minJumpOffset);

                    m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, m_rigidbody.velocity.y * jumpVelFactor);
                }

            }
            else
            {
                m_remainJumping = false;
                m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, 0);
            }
        }
        
        m_jump = false;
    }


    /// <summary>
    /// Logic for handling left and right movement.
    /// </summary>
    private void Move()
    {
        Vector2 targetVelocity;
        float moveSmoothing;

        if (m_enableHorizontalMovement)
        {
            // Left and right movement
            targetVelocity = new Vector2(m_movementInput.x * m_moveSpeed, m_rigidbody.velocity.y);
        }
        else
        {
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


    /// <summary>
    /// Get the player movement input.
    /// </summary>
    /// <returns>The current movement input for the player.</returns>
    private Vector2 GetMoveInput()
    {
        return m_movementInput;
    }

}