using MainCharacter;
using UnityEngine;

namespace GameObjects.Chat
{
    using UnityEngine;
    using System.Collections.Generic;

    public class ChatTrigger : MonoBehaviour
    {
        public Dialogue dialogue;

        private bool hasTriggered = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!hasTriggered && collision.CompareTag("Player"))
            {
                hasTriggered = true;
                // Start the scenario with dialogue and waypoints
                // start only chat
                ChatManager.Instance.StartDialogue(dialogue);
            }
        }

        // Optional: Reset trigger when player exits
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                hasTriggered = false;
            }
        }
    }

}