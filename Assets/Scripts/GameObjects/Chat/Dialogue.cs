using UnityEngine;

namespace GameObjects.Chat
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Chat/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        public DialogueLine[] lines;
    }
}