using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ServerInteraction
{
    public class PasswordResetManager : MonoBehaviour
    {
        public TMP_InputField emailInputField;
        public Button submitButton;
        public Button goToSignInButton;
        
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
            
        }
    }
}