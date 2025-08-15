using TMPro;
using UnityEngine;
using System.Collections;

public enum NPCType
{
    NormalShopper,
    Shoplifter,
    Distractor
}

public abstract class BaseNPC : MonoBehaviour
{
    [Header("NPC Base Settings")]
    public NPCType npcType;
    public float moveSpeed = 2f;
    public string npcName = "NPC";
    
    [Header("Interaction")]
    public TextMeshProUGUI interactionPrompt;
    public string firstInteractionText = "Hello there.";
    public string secondInteractionText = "Okay, you got me.";
    
    protected bool playerInRange = false;
    protected bool firstInteraction = false;
    protected bool arrested = false;
    protected Transform player;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);
    }
    
    protected virtual void Update()
    {
        if (arrested) return;
        
        // Check for interaction
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            HandleInteraction();
        }
    }
    
    protected virtual void HandleInteraction()
    {
        if (!firstInteraction)
        {
            firstInteraction = true;
            ShowInteractionText(firstInteractionText);
        }
        else
        {
            ArrestNPC();
        }
    }
    
    protected virtual void ArrestNPC()
    {
        arrested = true;
        
        if (GameManager.instance != null)
            GameManager.instance.ArrestNPC(npcType, npcName);

        StartCoroutine(ArrestSequence());
    }
    
    protected virtual IEnumerator ArrestSequence()
    {
        // Show arrest text
        ShowInteractionText(secondInteractionText);
        
        // Spawn police car and play cutscene
        yield return StartCoroutine(PoliceCutscene());
        
        // Cleanup
        if (GameManager.instance != null)
            GameManager.instance.OnNPCDestroyed();

        Destroy(gameObject);
    }
    
    protected virtual IEnumerator PoliceCutscene()
    {
        // Simple cutscene - could be enhanced with actual car model
        Debug.Log("Police car arrives for arrest");
        yield return new WaitForSeconds(2f);
    }
    
    protected virtual void ShowInteractionText(string text)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.text = text;
            interactionPrompt.gameObject.SetActive(true);
        }
    }
    
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionPrompt != null && !arrested)
            {
                interactionPrompt.text = "Press E to interact";
                interactionPrompt.gameObject.SetActive(true);
            }
        }
    }
    
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionPrompt != null)
                interactionPrompt.gameObject.SetActive(false);
        }
    }
}
