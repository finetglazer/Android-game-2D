using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ServerInteraction
{
    public class SignOutManager : MonoBehaviour
    {
        public Button signOutButton;
        public Button cancelButton;
        
        void Start()
        {
            signOutButton.onClick.AddListener(OnSignOutButtonClicked);
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
        
        private void OnSignOutButtonClicked()
        {
            // Sign out the user
            // Redirect to the sign-in scene
            StartCoroutine(SignOut());
           
        }
        
        private void OnCancelButtonClicked()
        {
            // Redirect to the main menu scene
            // Back to the last scene
            
            SceneManager.LoadScene("SignInScene");
            
            
        }

        IEnumerator SignOut()
        {
            string sessionToken = PlayerPrefs.GetString("SessionToken", null);
            if (string.IsNullOrEmpty(sessionToken))
            {
                Debug.Log("No session token found, you need to sign in first");
                StartCoroutine(Delayed());
                SceneManager.LoadScene("SignInScene");
                yield break;
            }
            
            // create a webrequest
            string url = "http://localhost:8080/api/auth/signout";
            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
            
            // custom the request
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Session-Token", sessionToken);
            // send the request

            yield return webRequest.SendWebRequest();
            // handle the response
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Display the server's response message
                Debug.Log("Sign out successful");
                PlayerPrefs.DeleteKey("SessionToken");
                StartCoroutine(Delayed());
                SceneManager.LoadScene("SignInScene");
            }
            else
            {
                Debug.Log("Sign out failed" + webRequest.downloadHandler.text);
                PlayerPrefs.DeleteKey("SessionToken");
                StartCoroutine(Delayed());
                SceneManager.LoadScene("SignInScene");
            }
        }

        IEnumerator Delayed()
        {
            yield return new WaitForSeconds(2f);
        }
    }
    
    
}