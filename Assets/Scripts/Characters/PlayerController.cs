using InController.Scripts;
using InController.Scripts.Utilities;
using UnityEngine;

namespace Characters
{
    public class PlayerController : CharacterController2D
    {
        // variable jump
        public float airTimeLimit = 0.3f;
        [ReadOnlyField, SerializeField] private float airTime;
        public int doubleJumpLimit;
        private int doubleJumpCount = 0;
        
        private bool EndJump => airTime > airTimeLimit;
        private bool DoubleJumpReady => doubleJumpCount < doubleJumpLimit;
        
        private void Awake()
        {
            rigidbody2d = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            currentScale = rigidbody2d.transform.localScale;
        }
        
        private void Update()
        {
            if (motion.x > 0) facingLeft = false;
            if (motion.x < 0) facingLeft = true;
            
#if ENABLE_LEGACY_INPUT_MANAGER
            motion = new Vector2
            {
                x = Input.GetAxis("Horizontal"),
                y = Input.GetAxis("Vertical")
            };
            
            if (IsGrounded)
            {
                doubleJumpCount = 0;
                if (Input.GetButtonDown("Jump"))
                {
                    jumping = true;
                    airTime = 0;
                }
            }
            else
            {
                if (Input.GetButtonDown("Jump") && DoubleJumpReady)
                {
                    doubleJump = true;
                    doubleJumpCount += 1;
                }
            }

            if (jumping | doubleJump)
            {
                airTime += Time.deltaTime;
            }

            // Variable jump height
            if (Input.GetButtonUp("Jump") | EndJump)
            {
                jumping = false;
                doubleJump = false;
            }
#endif
            SetAnimationState();
        }

        private void FixedUpdate()
        {
            grounded = groundCheck.IsGrounded();
            touchingWall = wallCheck.IsTouching();

            if (IsWallSliding)
            {
                WallSlide();
            }
            
            Move(motion, jumping, doubleJump);
            ChangeFaceDirection();
        }
    }
}