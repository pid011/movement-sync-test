using Mirror;
using UnityEngine;

namespace Playground
{
    public class PositionSync : NetworkBehaviour
    {
        [SerializeField, HideInInspector] private Rigidbody2D _rigidbody;

        [SyncVar] private Vector2 _networkPosition;
        [SyncVar] private Vector2 _networkVelocity;

        private Vector2 _lastPostion;
        private float _lastChangedTime;
        private float _delay;

        private void OnValidate()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (isServer)
            {
                _networkPosition = _rigidbody.position;
                _networkVelocity = _rigidbody.velocity;
            }

            if (isClient)
            {
                if (Vector2.SqrMagnitude(_lastPostion - _networkPosition) > 0.0001f)
                {
                    _delay = Time.time - _lastChangedTime;
                    _lastChangedTime = Time.time;

                    _lastPostion = _networkPosition + (_networkVelocity * _delay);
                    _networkPosition = _lastPostion;
                    _rigidbody.velocity = _networkVelocity;
                }

                _rigidbody.position = Vector2.MoveTowards(_rigidbody.position, _lastPostion, Time.deltaTime);
            }
        }
    }
}
