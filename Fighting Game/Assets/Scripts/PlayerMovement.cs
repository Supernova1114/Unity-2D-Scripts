using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerMovement : MonoBehaviour
{
    private static PlayerMovement instance;

    [Header("Walking")]
    [SerializeField] private float m_moveSpeed; // Left and Right movement speed of player
    [SerializeField] private float m_MovementSmoothing; // Player movement smoothing
    [SerializeField] private float m_AirMoveSmoothing; // Player movement smoothing in air

    [Header("Jumping")]
    [SerializeField] private float m_maxJumpTime; // Max time the player can jump
    [SerializeField] private float m_initialJumpVelocity; // The initial jump velocity
    [SerializeField] private GameObject m_foot; // The position where the ground check will take place
    [SerializeField] private float m_footRadius; // Radius of the ground check
    [SerializeField] private LayerMask m_groundMask;
    [SerializeField] private float m_fallGravityScale; // Current fall velocity multiplied by a value

    [Header("Sliding")]
    [SerializeField] private float m_slideSpeed;
    [SerializeField] private float m_slideMovementSmoothing;
    [SerializeField] private float m_maxSlideTime;
    [SerializeField] private float m_maxSlideCooldown;

    [Header("Wall Jump")]
    [SerializeField] private float m_wallCheckOffset;
    [SerializeField] private Vector2 m_handSize;

    private PlayerInput m_playerInput;
    private InputAction m_moveAction;
    private InputAction m_jumpAction;

    private Rigidbody2D m_rigidbody; // Rigidbody of the player
    private Vector2 m_movementInput; // The vertical and horizontal player input.
    private int m_facingDirection = 1; // The direction the player is facing. (-1 left, 1 right)
    private Vector2 m_velocity = Vector2.zero;

    private Vector2 m_smoothingVelocity = Vector2.zero; // Velocity for smoothing of player movement
    private float m_jumpTimer = 0; // Timer used for holding jump
    private bool m_remainJumping = false; // Should remain jumping?
    private bool m_isOnGround; // Is the player on the ground?
    private bool m_jump = false; // Flag for jumping.
    private bool m_jumpHeld = false; // Detects if the jump button is held or not
    private bool m_isSliding;
    private float m_slideTimer;
    private bool m_enableHorizontalMovement = true;

    ///-///////////////////////////////////////////////////////////
    ///
    /// Start Method
    /// 
    void Start()
	{
        instance = this;
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_playerInput = GetComponent<PlayerInput>();

        m_moveAction = m_playerInput.actions.FindAction("Move");
        m_jumpAction = m_playerInput.actions.FindAction("Jump");

        m_jumpAction.performed += OnJump;
        m_jumpAction.started += OnJump;
        m_jumpAction.canceled += OnJump;

        m_slideTimer = m_maxSlideCooldown;
    }

    
	///-///////////////////////////////////////////////////////////
    ///
    /// Update Method
    /// 
    void Update()
    {
        HandlePlayerMovement();
    }

    private void HandlePlayerMovement()
    {
        m_movementInput = m_moveAction.ReadValue<Vector2>();
    }


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

    private void OnJumpStarted()
    {
        m_jump = true;
        m_jumpHeld = true;
        m_enableHorizontalMovement = true;
    }

    private void OnJumpCanceled()
    {
        m_jumpHeld = false;

        if (m_remainJumping)
        {

            if (m_rigidbody.velocity.y > 0)
            {
                m_remainJumping = false;
                m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, 0);
            }
        }
        
    }

    
    ///-///////////////////////////////////////////////////////////
    ///
    /// Fixed-Update Method
    /// 
    private void FixedUpdate()
    {
        m_velocity = m_rigidbody.velocity;

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


    ///-///////////////////////////////////////////////////////////
    ///
    /// Logic for handling sliding movement.
    /// 
    private void HandleSlide()
    {
        if (!m_isSliding)
        {
            if (m_slideTimer < m_maxSlideCooldown)
            {
                m_slideTimer += Time.deltaTime;
            }
            else if (m_jump && m_movementInput.y < 0)
            {
                m_isSliding = true;
                m_slideTimer = 0f;

                m_enableHorizontalMovement = false;
            }
        }
        else
        {
            if (m_slideTimer < m_maxSlideTime)
            {
                // Normal sliding
                Vector2 targetSlideVelocity = new Vector2((transform.right * m_slideSpeed * m_facingDirection).x, m_rigidbody.velocity.y);
                m_rigidbody.velocity = Vector2.SmoothDamp(m_rigidbody.velocity, targetSlideVelocity, ref m_smoothingVelocity, m_slideMovementSmoothing);
            }
            else
            {
                // Stop sliding
                m_enableHorizontalMovement = true;
                m_isSliding = false;
                m_slideTimer = 0f;
            }
            m_slideTimer += Time.deltaTime;
        }
    }


    ///-///////////////////////////////////////////////////////////
    ///
    /// Logic for handling player jumping
    /// 
    private void HandleJump()
    {
        // Initial Jump
        if (m_jump && m_isOnGround && m_movementInput.y >= 0)
        {
            m_jumpTimer = 0;
            m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, m_initialJumpVelocity);
            m_remainJumping = true;
        }

        // Remain Jumping
        if (m_remainJumping)
        {
            if (m_jumpTimer <= m_maxJumpTime)
            {
                m_jumpTimer += Time.fixedDeltaTime;
            }
            else
            {
                m_remainJumping = false;
                m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, 0);
            }
        }
        
        m_jump = false;
    }


    ///-///////////////////////////////////////////////////////////
    ///
    /// Logic for handling left and right movement.
    /// 
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


    private Vector2 GetPlayerInput()
    {
        return m_movementInput;
    }

    public Vector2 GetVelocity()
    {
        return m_velocity;
    }

    public static PlayerMovement GetInstance()
    {
        return instance;
    }

}