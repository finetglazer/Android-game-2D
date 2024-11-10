using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ServerInteraction
{
    public class PasswordResetManager : MonoBehaviour
    {
        public TMP_InputField emailInputField;
        public Button submitButton;
        public Button goToSignInButton;
        public TMP_Text feedbackText;
        
        private void Start()
        {
            submitButton.onClick.AddListener(handleResetPassword);
            goToSignInButton.onClick.AddListener(OnGoToSignInButtonClicked);
        }
        
        private void OnGoToSignInButtonClicked()
        {
            SceneManager.LoadScene("SignInScene");
        }

        private void handleResetPassword()
        {
            string email = emailInputField.text;
            StartCoroutine(ResetPassword(email));
        }

        IEnumerator ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                Debug.Log("Invalid email address");
                yield break;
            }
            
            // custom request
            string url = "http://localhost:8080/api/auth/password-reset-request";
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            string body = "{\"email\": \"" + email + "\"}";
            Debug.Log(body);
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(body);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-type", "application/json");
            
            // Send the request
            feedbackText.text = "Wait a minute!";
            feedbackText.color = Color.white;
            yield return request.SendWebRequest();
            
            // handle the response
            if (request.result == UnityWebRequest.Result.Success)
            {
                feedbackText.text = request.downloadHandler.text;
                feedbackText.color = Color.green;

                StartCoroutine(DelayLoadScene());
            }
            else
            {
                feedbackText.text = request.downloadHandler.text;
                feedbackText.color = Color.red;
                
            }
            


        }

        IEnumerator DelayLoadScene()
        {
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene("PasswordResetConfirmScene");
        }
    }
}