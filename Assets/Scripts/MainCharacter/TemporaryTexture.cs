using UnityEngine;

public class TemporaryTexture : MonoBehaviour
{
    [HideInInspector]public bool playerIsOnTexture;
    private float timeOnTexture = 0f;
    public float duration = 3f; // Time required for the texture to disappear.

    private Vector3 initialPosition;
    private bool textureActive = true;
    
    private void Start()
    {
        // Store the initial position of the texture
        initialPosition = transform.position;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collides with the texture's collider.
        if (collision.collider.CompareTag("Player") && textureActive)
        {
            playerIsOnTexture = true;
        }
    }

    // private void OnCollisionExit2D(Collision2D collision)
    // {
    //     // Reset when the player leaves the texture's collider.
    //     if (collision.collider.CompareTag("Player") && textureActive)
    //     {
    //         playerIsOnTexture = false;
    //         // timeOnTexture = 0f; // Reset the timer
    //     }
    // }

    private void Update()
    {
        if (playerIsOnTexture && textureActive)
        {
            timeOnTexture += Time.deltaTime; // Increment the time spent on the texture.
            Debug.Log(timeOnTexture);
            if (timeOnTexture >= duration)
            {
                // Deactivate the texture instead of destroying it
                gameObject.SetActive(false);
                textureActive = false;
            }
        }
    }
    
    public void ResetTexture()
    {   
        Debug.Log("Resetting texture");
        // Reset the texture's position and activate it again.
        transform.position = initialPosition;
        gameObject.SetActive(true);
        timeOnTexture = 0f;
        textureActive = true;
        playerIsOnTexture = false;

    }
}
/*

*/