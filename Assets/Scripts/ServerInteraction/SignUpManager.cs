using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.Serialization;

namespace ServerInteraction
{
    public class SignUpManager : MonoBehaviour
{
    public TMP_InputField fullNameInputField;
    public TMP_InputField usernameInputField;
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_InputField confirmPasswordInputField;
    public Button signUpButton;
    public Button goToSignInButton;
    public TMP_Text feedbackText; // For displaying messages

    private void Start()
    {
        signUpButton.onClick.AddListener(OnSignUpButtonClicked);
        goToSignInButton.onClick.AddListener(OnGoToSignInButtonClicked);
    }

    private void OnSignUpButtonClicked()
    {
        string fullName = fullNameInputField.text;
        string username = usernameInputField.text;
        string emailAddress = emailInputField.text;
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;

        // Basic validation
        if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(emailAddress) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Please fill in all the fields.";
            return;
        }

        if (password != confirmPassword)
        {
            feedbackText.text = "Passwords do not match.";
            return;
        }

        // Start the sign-up coroutine
        StartCoroutine(SignUp(fullName, username, emailAddress, password));
    }

    private void OnGoToSignInButtonClicked()
    {
        SceneManager.LoadScene("SignInScene");
    }

    IEnumerator SignUp(string fullName, string username, string emailAddress, string password)
    {
        // Prepare the request
        string url = "http://localhost:8080/api/auth/signup"; // Update with your server's URL
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // Create the JSON body
        string jsonBody = "{\"fullName\":\"" + fullName + "\",\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"emailAddress\":\"" + emailAddress + "\"}";
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Display the server's response message
            feedbackText.text = request.downloadHandler.text;
            feedbackText.color = Color.green;
            Debug.Log(request.downloadHandler.text);

            // Optionally, delay before transitioning to the SignInScene
            StartCoroutine(LoadSignInSceneAfterDelay(2f)); // 2-second delay
        }
        else
        {
            // Handle error + display feedback
            feedbackText.text = "Sign-up failed: " + request.downloadHandler.text; ;
            Debug.LogError(request.error);
        }
    }
    
    IEnumerator LoadSignInSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("SignInScene");
    }
}

}

