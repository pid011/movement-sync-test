using UnityEngine;

namespace Playground
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] private bool _isGround;

        public bool IsGround => _isGround;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Boundary"))
            {
                return;
            }
            _isGround = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Boundary"))
            {
                return;
            }
            _isGround = false;
        }
    }
}
