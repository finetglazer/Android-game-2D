using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

namespace ServerInteraction
{
    public class SignInManager : MonoBehaviour
    {
        public TMP_InputField usernameInputField;
        public TMP_InputField passwordInputField;
        public Button signInButton;
        public Button goToSignUpButton;
        public Text feedbackText; // For displaying messages
        internal static string UserId;

        private void Start()
        {
            signInButton.onClick.AddListener(OnSignInButtonClicked);
            goToSignUpButton.onClick.AddListener(OnGoToSignUpButtonClicked);
        }

        private void OnSignInButtonClicked()
        {
            string usernameOrEmail = usernameInputField.text;
            string password = passwordInputField.text;

            // Basic validation
            if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
            {
                feedbackText.text = "Please enter your username/email and password.";
                return;
            }

            // Start the sign-in coroutine
            StartCoroutine(SignIn(usernameOrEmail, password));
        }

        private void OnGoToSignUpButtonClicked()
        {
            SceneManager.LoadScene("SignUpScene");
        }

        IEnumerator SignIn(string usernameOrEmail, string password)
        {
            // Prepare the request
            string signinUrl = "http://localhost:8080/api/auth/signin"; // Update with your server's URL
            UnityWebRequest request = new UnityWebRequest(signinUrl, "POST");

            // Create the JSON body
            string jsonBody = "{\"usernameOrEmail\":\"" + usernameOrEmail + "\",\"password\":\"" + password + "\"}";
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Handle success
                // feedbackText.text = "Sign-in successful!";
                Debug.Log(request.downloadHandler.text);
                var sessionToken = request.downloadHandler.text;
                // You can parse the response and store the session token if needed
                StartCoroutine(GetUserIdBySessionToken(sessionToken));
                // For now, we'll load the next scene
                SceneManager.LoadScene("1stscene"); // Replace with your gameplay scene
            }
            else
            {
                // Handle error
                feedbackText.text = "Sign-in failed: " + request.downloadHandler.text;
                Debug.LogError(request.error);
            }
        }

        private static IEnumerator GetUserIdBySessionToken(string sessionToken)
        {
            const string getUserIdUrl = "http://localhost:8080/api/auth/user-id";
            var getUserIdBySessionTokenReq = new UnityWebRequest(getUserIdUrl, "GET");
            var jsonBody = "{\"sessionToken\":\"" + sessionToken + "\"}";
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            getUserIdBySessionTokenReq.uploadHandler = new UploadHandlerRaw(jsonToSend);
            getUserIdBySessionTokenReq.downloadHandler = new DownloadHandlerBuffer();
            getUserIdBySessionTokenReq.SetRequestHeader("Content-Type", "application/json");
            yield return getUserIdBySessionTokenReq.SendWebRequest();
            UserId = getUserIdBySessionTokenReq.result == UnityWebRequest.Result.Success
                ? getUserIdBySessionTokenReq.downloadHandler.text
                : "";
        }

    }
}