using UnityEngine;

namespace GameObjects.Chat
{
    [CreateAssetMenu(fileName = "ChatData", menuName = "Chat/ChatData", order = 1)]
    public class ChatData : ScriptableObject
    {
        [System.Serializable]
        public class ChatSequence
        {
            public string playerMessage;
            public string authorityMessage;
        }

        public ChatSequence[] chatSequences;
    }
}