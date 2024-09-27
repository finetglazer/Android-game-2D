using UnityEngine;

public class PlayerSlide : MonoBehaviour
{
    public LayerMask slope;  // Add the slope layer here
    public float slideSpeed;  // Control how fast the player slides
    private Rigidbody2D rb;
    // private bool isSliding = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckForSlope();
    }

    void CheckForSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, slope);

        if (hit.collider != null)
        {
            // If we detect a slope, start sliding
            if (Mathf.Abs(hit.normal.x) > 0.1f)  // Detect a slope by checking the normal
            {
                // isSliding = true;
                rb.velocity = new Vector2(hit.normal.x * slideSpeed, rb.velocity.y);
            }
            // else
            // {
            //     isSliding = false;
            // }
        }
        // else
        // {
        //     isSliding = false;
        // }
    }
}