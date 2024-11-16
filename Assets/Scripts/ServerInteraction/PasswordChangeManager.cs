using System;
using System.Collections;
using ServerInteraction.Payload;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ServerInteraction
{
    public class PasswordChangeManager : MonoBehaviour
    {
        public TMP_InputField oldPass;
        public TMP_InputField newPass;
        public TMP_InputField confirmPass;
        public Button submitButton;
        public Button backButton;
        public TMP_Text feedBackText;
        
        private void Start()
        {
            submitButton.onClick.AddListener(OnChangePasswordButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        private void OnChangePasswordButtonClicked()
        {
            string oldPassword = oldPass.text;
            string newPassword = newPass.text;
            string confirmPassword = confirmPass.text;
            
            // Basic validation
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                feedBackText.text = "Please fill all fields.";
                feedBackText.color = Color.red;
                return;
            }
            
            if (newPassword != confirmPassword)
            {
                feedBackText.text = "Passwords do not match.";
                feedBackText.color = Color.red;
                return;
            }
            
            // Start the change password coroutine
            StartCoroutine(ChangePassword(oldPassword, newPassword));
        }
        
        private void OnBackButtonClicked()
        {
            // Redirect to the main menu scene
            // continue playing
            SceneManager.LoadScene("DashboardScene");
        }
        
        IEnumerator ChangePassword(string oldPassword, string newPassword)
        {
           // step 1: Check session token
           String sessionToken = PlayerPrefs.GetString("SessionToken", null);
           Debug.Log(sessionToken);
           
              if (string.IsNullOrEmpty(sessionToken))
              {
                Debug.Log("Session token not found. Please login again.");
                SceneManager.LoadScene("SignInScene");
                yield break;
              }
              // Prepare the request
              string url = "http://localhost:8080/api/auth/change-password";
              UnityWebRequest request = new UnityWebRequest(url, "POST");
              // create the body with Json
              PasswordChangeRequest passwordChangeRequest = new PasswordChangeRequest
              {
                  oldPassword = oldPassword,
                  newPassword = newPassword,
                  confirmPassword = newPassword
              };

              string body = JsonUtility.ToJson(passwordChangeRequest);
              byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(body);
              request.uploadHandler = new UploadHandlerRaw(bodyRaw);
              request.downloadHandler = new DownloadHandlerBuffer();
              request.SetRequestHeader("Content-Type", "application/json");
              request.SetRequestHeader("Session-Token", sessionToken);
              
              // Send the request
              yield return request.SendWebRequest();
              
              // Handler response
              if (request.result == UnityWebRequest.Result.Success)
              {
                  Debug.Log("Password change successful: " + request.downloadHandler.text);
                  feedBackText.text = "Password change successful!";
                  feedBackText.color = Color.green;
                  LoadSceneWithLoadingScreen("DashboardScene");
                  // SceneManager.LoadScene("Scenes/DashboardScene");
              }
              else
              {
                  Debug.Log("Error: " + request.error);
                  feedBackText.text = "Password change failed: " + request.downloadHandler.text;
                  feedBackText.color = Color.red;
              }
              
              
        }
        
        // delay for 2 seconds
        public void LoadSceneWithLoadingScreen(string sceneToLoad)
        {
            // Set the next scene name in the SceneLoader static class
            SceneLoader.nextSceneName = sceneToLoad;

            // Load the loading scene
            SceneManager.LoadScene("Scenes/FastLoadingScene");
        }
    }
}