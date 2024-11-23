using System.Collections.Generic;
using UnityEngine;

namespace GameObjects.Chat
{
    [System.Serializable]
    public class DialogueLine
    {
        public enum Speaker
        {
            Player,
            Narrator
        }

        public Speaker speaker;

        [TextArea(2, 5)]
        public string text;

        public float duration = 2f; // Duration to display this line

       

       
    }
}