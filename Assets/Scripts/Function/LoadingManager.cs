using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
        
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Load the next scene
        SceneManager.LoadScene(SceneLoader.nextSceneName);
    }
}