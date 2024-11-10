using System;
using UnityEngine;

namespace ServerInteraction.Payload
{
    [Serializable]
    public class PasswordResetRequest
    {
        public string resetToken;
        public string newPassword;
        public string confirmNewPassword;
    }
}