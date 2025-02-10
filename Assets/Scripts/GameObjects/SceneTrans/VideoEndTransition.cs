using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace GameObjects.SceneTrans
{
    public class VideoEndTransition : MonoBehaviour
    {
        // Reference to the VideoPlayer component.
        // You can assign this in the Inspector or let the script grab it from the same GameObject.
        public VideoPlayer videoPlayer;

        // The name of the scene to load after the video finishes.
        public string nextSceneName = "NextScene";

        private void Start()
        {
            // If no VideoPlayer is assigned, try to get one from the same GameObject.
            if (videoPlayer == null)
            {
                videoPlayer = GetComponent<VideoPlayer>();
            }

            // Ensure the videoPlayer is available.
            if (videoPlayer != null)
            {
                // Subscribe to the loopPointReached event, which is triggered when the video finishes playing.
                videoPlayer.loopPointReached += OnVideoEnd;
            }
            else
            {
                Debug.LogError("No VideoPlayer found. Please assign a VideoPlayer component.");
            }
        }

        // Unsubscribe from the event when this object is destroyed.
        private void OnDestroy()
        {
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached -= OnVideoEnd;
            }
        }

        // This method is called when the video finishes playing.
        private void OnVideoEnd(VideoPlayer vp)
        {
            Debug.Log("Video has ended. Transitioning to scene: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}