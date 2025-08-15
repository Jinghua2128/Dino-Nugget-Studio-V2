using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ShoplifterNPC : BaseNPC
{
    [Header("Shoplifter Settings")]
    public Transform[] itemShelves;
    public GameObject stolenItemPrefab; // Visual representation of stolen item
    public Transform handTransform; // Where to show stolen item
    public float stealTime = 2f;
    
    private NavMeshAgent agent;
    private bool hasStolen = false;
    private GameObject currentStolenItem;
    
    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        
        firstInteractionText = "I'm good bro. What u doing?";
        secondInteractionText = "Alright, you got me!";
        
        StartStealing();
    }
    
    void StartStealing()
    {
        if (itemShelves.Length > 0 && !hasStolen)
        {
            Transform targetShelf = itemShelves[Random.Range(0, itemShelves.Length)];
            agent.SetDestination(targetShelf.position);
            StartCoroutine(StealFromShelf(targetShelf));
        }
    }
    
    IEnumerator StealFromShelf(Transform shelf)
    {
        // Wait to reach shelf
        while (agent.pathPending || agent.remainingDistance > 0.5f)
            yield return null;
        
        // Steal animation time
        yield return new WaitForSeconds(stealTime);
        
        // Check if shelf has stealable items
        StealableItem[] items = shelf.GetComponentsInChildren<StealableItem>();
        if (items.Length > 0)
        {
            StealableItem itemToSteal = items[Random.Range(0, items.Length)];
            StealItem(itemToSteal);
        }
        
        hasStolen = true;
    }
    
    void StealItem(StealableItem item)
    {
        // Notify game manager
        if (GameManager.instance != null)
            GameManager.instance.ItemStolen(item.itemName);
        
        // Show stolen item in hand
        if (stolenItemPrefab != null && handTransform != null)
        {
            currentStolenItem = Instantiate(stolenItemPrefab, handTransform.position, handTransform.rotation, handTransform);
        }
        
        // Hide original item
        item.gameObject.SetActive(false);
        
        // Activate distractors
        ActivateDistractors();
    }
    
    void ActivateDistractors()
    {
        DistractorNPC[] distractors = Object.FindObjectsOfType<DistractorNPC>();
        foreach (var distractor in distractors)
        {
            distractor.ActivateDistraction();
        }
    }
}