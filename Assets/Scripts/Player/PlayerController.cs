using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Horizontal Movement Settings")]
        [FormerlySerializedAs("_walkSpeed")] [SerializeField] private float walkSpeed = 1;
        
        [Header("Ground Check Settings")]
        [SerializeField] private float jumpForce = 60;
        [FormerlySerializedAs("GroundCheckPoint")] [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private float groundCheckY = 0.5f;
        [SerializeField] private float groundCheckX = 0.5f;
        [SerializeField] private LayerMask isInGround;
    
        private Rigidbody2D _rig;
        private float _xAsix;
        private Animator _anim;

        public static PlayerController Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }else
            {
                Instance = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _rig = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            GetInputs();
            Move();
            Jump();
            Flip();
        }

        void GetInputs()
        {
            _xAsix = Input.GetAxisRaw("Horizontal");
        }

        void Flip()
        {
            var transform1 = transform;
            if (_xAsix < 0)
            {
                transform1.localScale = new Vector2(-1, transform1.localScale.y);
            }
            else if(_xAsix >0)
            {
                transform1.localScale = new Vector2(1, transform.localScale.y);
            }
        }

        private void Move()
        {
            var velocity = _rig.velocity;
            velocity = new Vector2(walkSpeed * _xAsix, velocity.y);
            _rig.velocity = velocity;
            _anim.SetBool("Walking", (velocity.x != 0) && Grounded());
        }

        public bool Grounded()
        {
            if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, isInGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX,0,0), Vector2.down, groundCheckY, isInGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(+groundCheckX,0,0), Vector2.down, groundCheckY, isInGround)
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void Jump()
        {
            if (Input.GetButtonUp("Jump") && _rig.velocity.y > 0)
            {
                _rig.velocity = new Vector2(_rig.velocity.x, 0);
            }
            if (Input.GetButtonDown("Jump") && Grounded() )
            {
                _rig.velocity = new Vector2(_rig.velocity.x, jumpForce);
            }
            _anim.SetBool("Jumping", !Grounded());
        }
    }
}
