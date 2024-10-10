using UnityEngine;

namespace MainCharacter
{
    public class PlayerSlide : MonoBehaviour
    {
        public LayerMask slope;  // Add the slope layer here
        public float slideSpeed;  // Control how fast the player slides
        private Rigidbody2D _rb;
        // private bool isSliding = false;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            CheckForSlope();
        }

        private void CheckForSlope()
        {
            
            var hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, slope);

            if (!hit.collider) return;
            // If we detect a slope, start sliding
            if (Mathf.Abs(hit.normal.x) > 0.1f)  // Detect a slope by checking the normal
            {
                
                // isSliding = true;
                // Debug.DrawRay(transform.position, Vector2.down * 1f, Color.red);
                _rb.velocity = new Vector2(hit.normal.x * slideSpeed, _rb.velocity.y);
                Debug.Log("Checking for slope" + Mathf.Abs(hit.normal.x));
                // Debug.Log(_rb.velocity);
                
            }
        }
    }
}