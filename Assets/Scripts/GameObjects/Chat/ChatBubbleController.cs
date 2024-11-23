using UnityEngine;

namespace GameObjects.Chat
{
    public class ChatBubbleController: MonoBehaviour
    {
        void LateUpdate()
        {
            // Ensure the chat bubble's local scale X is always positive
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}