using System;
using System.Collections;
using System.Collections.Generic;
using ServerInteraction.Payload;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing.MiniJSON;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ServerInteraction
{
    public class PasswordResetConfirmManager : MonoBehaviour
    {
        public TMP_InputField resetToken;
        public TMP_InputField newPassword;
        public TMP_InputField confirmPassword;
        public Button submitButton;
        public Button goToSignInButton;
        public TMP_Text feedbackText;
        
        private void Start()
        {
            goToSignInButton.onClick.AddListener(OnGoToSignInButtonClicked);
            submitButton.onClick.AddListener(OnSubmitButtonClicked);
        }
        
        private void OnGoToSignInButtonClicked()
        {
            new WaitForSeconds(2f);
            SceneManager.LoadScene("SignInScene");
        }

        private void OnSubmitButtonClicked()
        {
            string resetToken = this.resetToken.text;
            string newPassword = this.newPassword.text;
            string confirmPassword = this.confirmPassword.text;

            if (!newPassword.Equals(confirmPassword))
            {
                feedbackText.text = "Password not match!";
                feedbackText.color = Color.red;
                return;
            }

            StartCoroutine(PasswordResetConfirm(resetToken, newPassword, confirmPassword));

        }

        IEnumerator PasswordResetConfirm(string resetToken, string newPassword, string confirmNewPassword)
        {
            // check the reset token
            if (string.IsNullOrEmpty(resetToken))
            {
                feedbackText.text = "Invalid reset token";
                yield break;
            }
            // create a request
            string url = "http://localhost:8080/api/auth/password-reset";
            UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
            // custom the request
            // body
            PasswordResetRequest passwordResetRequest = new PasswordResetRequest
            {
                resetToken = resetToken,
                newPassword = newPassword,
                confirmNewPassword = confirmNewPassword
            };
            // change to json string
            string body = JsonUtility.ToJson(passwordResetRequest);
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(body);

            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            //header
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();
            //handle the message
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                feedbackText.text = webRequest.downloadHandler.text;
                feedbackText.color = Color.green;
                LoadSceneWithLoadingScreen("SignInScene");

            }
            else
            {
                feedbackText.text = webRequest.downloadHandler.text;
                feedbackText.color = Color.red;
            }
        }

       
        public void LoadSceneWithLoadingScreen(string sceneToLoad)
        {
            // Set the next scene name in the SceneLoader static class
            SceneLoader.nextSceneName = sceneToLoad;

            // Load the loading scene
            SceneManager.LoadScene("Scenes/FastLoadingScene");
        }
    }
}