using UnityEngine.Serialization;

namespace ServerInteraction.Responses
{
    [System.Serializable]
    public class SignInResponse
    {
        public string sessionToken;
    }
}