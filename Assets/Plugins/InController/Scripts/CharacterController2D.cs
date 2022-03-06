using UnityEngine;

namespace InController.Scripts
{
    /// <summary>
    /// Sets values that will help determine player behavior.
    /// Behaviors themselves are defined as such in their relevant scripts.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public abstract class CharacterController2D : MonoBehaviour
    {
        public float speed;
        public int jumpThrust;
        public float wallSlideSpeed;
        public float slopeSpeed;
        
        [Tooltip("Supports smoother motion transitions.")]
        [Range(0, 0.3f)] public float smoothDamp = 0.05f;
        public GroundCheck groundCheck;
        public CollisionCheck wallCheck;
        
        // normal jump
        public float jumpHeight;

        private float jumpVelocity => Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rigidbody2d.gravityScale));
        private Vector2 velocity = Vector2.zero;
    
        [HideInInspector] public Rigidbody2D rigidbody2d;
        protected Animator animator;
        protected Vector2 motion;
        protected Vector3 currentScale;
        
        public static readonly int Idle = Animator.StringToHash("idle");
        public static readonly int Running = Animator.StringToHash("running");
        public static readonly int Walking = Animator.StringToHash("walking");
        public static readonly int Jumping = Animator.StringToHash("jumping");
        public static readonly int Attacking = Animator.StringToHash("attacking");
        public static readonly int Shooting = Animator.StringToHash("shooting");
        public static readonly int Hurt = Animator.StringToHash("hurt");
        public static readonly int Dead = Animator.StringToHash("dead");
        public static readonly int Tumbling = Animator.StringToHash("tumbling");
        public static readonly int CastingSpell = Animator.StringToHash("castingSpell");
        public static readonly int WallSliding = Animator.StringToHash("wallSliding");
        public static readonly int Climbing = Animator.StringToHash("climbing");
        public static readonly int Grounded = Animator.StringToHash("grounded");

        protected bool walking;
        protected bool hurt;
        protected bool jumping;
        protected bool attacking;
        protected bool shooting;
        protected bool casting;
        protected bool tumbling;
        protected bool dead;
        protected bool facingLeft;
        protected bool climbing;
        protected bool grounded = true;
        protected bool doubleJump;
        protected bool touchingWall;

        public bool IsFacingLeft => facingLeft;
        public bool IsJumping => jumping;
        public bool IsAttacking => attacking;
        public bool IsIdle => motion.x == 0 & motion.y == 0;
        public bool IsRunning => Mathf.Abs(motion.x) > 0 | Mathf.Abs(motion.y) > 0;
        public bool IsWalking => walking;
        public bool IsHurt => hurt;  // character has been hit
        public bool IsTumbling => tumbling; // a rolling or dashing motion
        public bool IsDead => dead;
        public bool IsShooting => shooting;
        public bool IsSpellCasting => casting;
        public bool IsClimbing => climbing;
        public bool IsTouchingWall => touchingWall;
        public bool IsWallSliding => !grounded && rigidbody2d.velocity.y < 0 && touchingWall;
        
        // Movement
        public bool IsReadyToJump => grounded && jumping;
        public bool CanDoubleJump => !grounded && jumping;
        public bool IsGrounded => grounded;
        
        public void Move(Vector2 movement, bool jumpActive, bool doubleJumpActive)
        {
            motion = movement;
            jumping = jumpActive;
            doubleJump = doubleJumpActive;
            float currentSpeed = motion.x * speed;

            // FIXME: Temporary. doesnt account for upward/downward slope speeds.
            if (groundCheck.IsSloped)
            {
                currentSpeed = motion.x * slopeSpeed;
            }
            
            var targetVelocity = new Vector2(currentSpeed, rigidbody2d.velocity.y);
            rigidbody2d.velocity = Vector2.SmoothDamp(rigidbody2d.velocity, targetVelocity, ref velocity, smoothDamp);

            if (doubleJump)
            {
                DoubleJump();
            }
            
            if (IsReadyToJump)
            {
                Jump();
            }
        }
        
        protected void SetAnimationState()
        {
            animator.SetBool(Running, IsRunning);
            animator.SetBool(Walking, IsWalking);
            animator.SetBool(Idle, IsIdle);
            animator.SetBool(Jumping, IsJumping);
            animator.SetBool(Attacking, IsAttacking);
            animator.SetBool(Shooting, IsShooting);
            animator.SetBool(Hurt, IsHurt);
            animator.SetBool(Tumbling, IsTumbling);
            animator.SetBool(Dead, IsDead);
            animator.SetBool(CastingSpell, IsSpellCasting);
            animator.SetBool(WallSliding, IsWallSliding);
            animator.SetBool(Grounded, IsGrounded);
        }
    
        public void ChangeFaceDirection() 
        { 
            if ((!IsFacingLeft && currentScale.x < 0) || (IsFacingLeft && currentScale.x > 0))
            {
                currentScale.x *= -1;
            }
            transform.localScale = currentScale;
        }

        public bool HasDied(float health)
        {
            return health <= 0;
        }

        public void Jump()
        {
            // grounded = false;
            rigidbody2d.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
            doubleJump = true;
        }

        public void DoubleJump()
        {
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, 0);
            rigidbody2d.AddForce(new Vector2(0, jumpThrust), ForceMode2D.Impulse);
            doubleJump = false;
        }

        public void WallSlide()
        {
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, rigidbody2d.velocity.y - wallSlideSpeed);
        }
    }
}
