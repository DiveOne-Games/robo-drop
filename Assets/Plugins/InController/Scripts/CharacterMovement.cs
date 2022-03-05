using InController.Scripts.Utilities;
using Unity.Collections;
using UnityEngine;

namespace InController.Scripts
{
    [RequireComponent(typeof(CharacterController2D))]
    public class CharacterMovement : MonoBehaviour
    {
        public CharacterController2D controller;
        
        // variable jump
        public float airTimeLimit = 0.3f;
        [ReadOnlyField, SerializeField] private float airTime;
        public int doubleJumpLimit;
    
        private Vector2 motion;
        private bool jumping;
        private bool doubleJump;
        private int doubleJumpCount = 0;

        private bool CanDoubleJump => doubleJumpCount < doubleJumpLimit;
        private bool EndJump => airTime > airTimeLimit;
        
        private void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            motion = new Vector2
            {
                x = Input.GetAxis("Horizontal"),
                y = Input.GetAxis("Vertical")
            };
            
            if (controller.IsGrounded)
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
                if (Input.GetButtonDown("Jump") && CanDoubleJump)
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
        }

        public void FixedUpdate()
        {
            controller.Move(motion, jumping, doubleJump);
            // LESSON: Alternative to just setting this value is to change it on the state of a keypress: up, down or long press
            // jumping = false; // prevents continuous jumping
            // doubleJump = false;
        }
    }
}
