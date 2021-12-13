using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Playground
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    public class PlayerMovementController : NetworkBehaviour
    {
        private static readonly int s_isWalkingHash = Animator.StringToHash("isWalking");
        private static readonly int s_isJumpingHash = Animator.StringToHash("isJumping");

        [SerializeField] private GroundCheck _foot;

        [SyncVar]
        [SerializeField, Min(0f)] private float _speed = 8f;
        [SyncVar]
        [SerializeField, Min(0f)] private float _jumpPower = 15f;

        [SerializeField, HideInInspector] private SpriteRenderer _playerSprite;
        [SerializeField, HideInInspector] private Rigidbody2D _rigidbody;
        [SerializeField, HideInInspector] private Animator _animator;

        [SyncVar] private bool _flipX;

        private float _horizontalMove;

        private void OnValidate()
        {
            _playerSprite = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        private void Awake()
        {
            Debug.Assert(_foot, gameObject);
        }

        private void Update()
        {
            if (isServer)
            {
                _flipX = _horizontalMove == 0 ? _flipX : _horizontalMove < 0;
            }

            if (isClient)
            {
                _animator.SetBool(s_isWalkingHash, Mathf.Abs(_rigidbody.velocity.x) > 1f);
                _animator.SetBool(s_isJumpingHash, !_foot.IsGround);
                _playerSprite.flipX = _flipX;
            }
        }

        private void FixedUpdate()
        {
            if (!isServer)
            {
                return;
            }

            if (_rigidbody.velocity.x > _speed)
            {
                _rigidbody.velocity = new Vector2(_speed, _rigidbody.velocity.y);
            }
            else if (_rigidbody.velocity.x < -_speed)
            {
                _rigidbody.velocity = new Vector2(-_speed, _rigidbody.velocity.y);
            }
            else
            {
                _rigidbody.AddForce(Vector2.right * _horizontalMove, ForceMode2D.Impulse);
            }
        }

        [Client]
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if (context.performed || context.canceled)
            {
                var direction = context.ReadValue<float>();
                CmdMove(direction);
            }
        }

        [Command]
        private void CmdMove(float direction)
        {
            _horizontalMove = direction;
        }

        [Client]
        public void OnJump(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if (context.phase == InputActionPhase.Performed)
            {
                CmdJump();
            }
        }

        [Command]
        private void CmdJump()
        {
            if (!_foot.IsGround)
            {
                return;
            }
            _rigidbody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
        }
    }
}
