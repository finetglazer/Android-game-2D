using UnityEngine;
using System.Collections;
using MainCharacter;
using TMPro;


namespace GameObjects.Chat
{
   public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;

    public GameObject playerChatBubblePrefab;
    public GameObject narratorChatBubblePrefab;

    private GameObject playerChatBubbleInstance;
    private GameObject narratorChatBubbleInstance;

    private Coroutine dialogueCoroutine;


    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persists between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }
        dialogueCoroutine = StartCoroutine(PlayDialogue(dialogue));
    }

    private IEnumerator PlayDialogue(Dialogue dialogue)
    {
        foreach (DialogueLine line in dialogue.lines)
        {
            ShowLine(line);
            yield return new WaitForSeconds(line.duration);
        }
        HideAllBubbles();
    }

    private void ShowLine(DialogueLine line)
    {
        HideAllBubbles(); // Hide any existing bubbles

        if (line.speaker == DialogueLine.Speaker.Player)
        {
            if (playerChatBubbleInstance == null)
            {
                // Instantiate the player chat bubble
                playerChatBubbleInstance = Instantiate(playerChatBubblePrefab);
            }

            playerChatBubbleInstance.GetComponentInChildren<TMP_Text>().text = line.text;
            playerChatBubbleInstance.SetActive(true);

            // Ensure the chat bubble follows the player
            StartFollowingPlayer();
        }
        else if (line.speaker == DialogueLine.Speaker.Narrator)
        {
            if (narratorChatBubbleInstance == null)
            {
                // Instantiate the narrator chat bubble
                narratorChatBubbleInstance = Instantiate(narratorChatBubblePrefab);
                // Since it's Screen Space - Overlay, no need to set parent
            }
            narratorChatBubbleInstance.GetComponentInChildren<TMP_Text>().text = line.text;
            narratorChatBubbleInstance.SetActive(true);
        }
    }
    
    private bool isFollowingPlayer = false;
    private GameObject player;

    private void StartFollowingPlayer()
    {
        if (!isFollowingPlayer)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                isFollowingPlayer = true;
            }
            else
            {
                Debug.LogError("Player not found. Ensure your player has the 'Player' tag.");
            }
        }
    }

    private void Update()
    {
        if (isFollowingPlayer && playerChatBubbleInstance != null)
        {
            // Position the chat bubble above the player
            Vector3 offset = new Vector3(1.5f, 1.5f, 0); // Adjust as needed
            playerChatBubbleInstance.transform.position = player.transform.position + offset;

            // Ensure the chat bubble has no rotation
            playerChatBubbleInstance.transform.rotation = Quaternion.identity;
        }
    }

    private void HideAllBubbles()
    {
        if (playerChatBubbleInstance != null)
        {
            playerChatBubbleInstance.SetActive(false);
            isFollowingPlayer = false; // Stop following the player
        }
        if (narratorChatBubbleInstance != null)
        {
            narratorChatBubbleInstance.SetActive(false);
        }
    }
}
}