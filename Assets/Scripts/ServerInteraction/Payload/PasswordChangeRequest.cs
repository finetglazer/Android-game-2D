using System;
using UnityEngine;

namespace ServerInteraction.Payload
{
    [Serializable]
    public class PasswordChangeRequest
    {
        public string oldPassword;
        public string newPassword;
        public string confirmPassword;
    }
}