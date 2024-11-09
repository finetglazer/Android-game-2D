using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using ServerInteraction.Responses;
using UnityEditor.PackageManager.Requests;

namespace ServerInteraction
{
    public class SignInManager : MonoBehaviour
    {
        public TMP_InputField usernameInputField;
        public TMP_InputField passwordInputField;
        public Button signInButton;
        public Button goToSignUpButton;
        public TMP_Text feedbackText; // For displaying messages
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
            string url = "http://localhost:8080/api/auth/signin"; // Update with your server's URL
            UnityWebRequest request = new UnityWebRequest(url, "POST");

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
                feedbackText.text = "Sign-in successful!";
                feedbackText.color = Color.green;
                print(request.downloadHandler.text);

                // You can parse the response and store the session token if needed
                var signInResponse = JsonUtility.FromJson<SignInResponse>(request.downloadHandler.text);
                yield return StartCoroutine(GetUserIdBySessionToken(signInResponse.sessionToken));
                // For now, we'll load the next scene
                SceneManager.LoadScene("TestScene - Hiep/DashboardScene"); // Replace with your gameplay scene
                StartCoroutine(LoadingSceneDelay());
            }
            else
            {
                // Handle error
                feedbackText.text = "Sign-in failed: " + request.downloadHandler.text;
                print(request.error);
            }
        }
        
        IEnumerator LoadingSceneDelay()
        {
            yield return new WaitForSeconds(2f); // 2-second delay
        }
        
        private static IEnumerator GetUserIdBySessionToken(string sessionToken)
        {
            const string getUserIdUrl = "http://localhost:8080/api/auth/user-id";
            var getUserIdBySessionTokenReq = new UnityWebRequest(getUserIdUrl, "POST");
            var jsonBody = "{\"sessionToken\":\"" + sessionToken + "\"}";
            var jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
            getUserIdBySessionTokenReq.uploadHandler = new UploadHandlerRaw(jsonToSend);
            getUserIdBySessionTokenReq.downloadHandler = new DownloadHandlerBuffer();
            getUserIdBySessionTokenReq.SetRequestHeader("Content-Type", "application/json");
            yield return getUserIdBySessionTokenReq.SendWebRequest();
            UserId = getUserIdBySessionTokenReq.result == UnityWebRequest.Result.Success
                ? JsonUtility.FromJson<GetUserIdBySessionTokenResponse>(getUserIdBySessionTokenReq.downloadHandler.text).userId
                : "";
            print(getUserIdBySessionTokenReq.downloadHandler.text);
        }

    }
}