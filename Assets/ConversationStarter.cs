using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation myConversation;

    private bool playerInRange;
    private bool conversationStarted;

    private void Update()
    {
        if (!playerInRange || ConversationManager.Instance == null)
            return;

        // Press F to start conversation
        if (Input.GetKeyDown(KeyCode.F) && !conversationStarted)
        {
            if (myConversation != null)
            {
                ConversationManager.Instance.StartConversation(myConversation);
                conversationStarted = true;
            }
        }

        // Press E to exit conversation
        if (Input.GetKeyDown(KeyCode.E) && conversationStarted)
        {
            ConversationManager.Instance.EndConversation();
            conversationStarted = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (conversationStarted && ConversationManager.Instance != null)
            {
                ConversationManager.Instance.EndConversation();
                conversationStarted = false;
            }
        }
    }
}
