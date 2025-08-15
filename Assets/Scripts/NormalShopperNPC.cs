using UnityEngine;
using UnityEngine.AI;

public class NormalShopperNPC : BaseNPC
{
    [Header("Normal Shopper Settings")]
    public Transform[] shelfPoints;
    public Transform checkoutPoint;
    public float browseTime = 5f;
    private NavMeshAgent agent;
    private bool isAtCheckout = false;
    private int currentShelfIndex = 0;
    
    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        
        firstInteractionText = "Just shopping, thanks.";
        secondInteractionText = "Why are you arresting me?!";
        
        StartShopping();
    }
    
    void StartShopping()
    {
        if (shelfPoints.Length > 0)
        {
            MoveToShelf();
        }
        else
        {
            GoToCheckout();
        }
    }
    
    void MoveToShelf()
    {
        if (currentShelfIndex < shelfPoints.Length)
        {
            agent.SetDestination(shelfPoints[currentShelfIndex].position);
            StartCoroutine(BrowseAtShelf());
        }
        else
        {
            GoToCheckout();
        }
    }
    
    System.Collections.IEnumerator BrowseAtShelf()
    {
        // Wait to reach shelf
        while (agent.pathPending || agent.remainingDistance > 0.5f)
            yield return null;
        
        // Browse for a while
        yield return new WaitForSeconds(browseTime);
        
        // Move to next shelf or checkout
        currentShelfIndex++;
        if (currentShelfIndex < shelfPoints.Length)
        {
            MoveToShelf();
        }
        else
        {
            GoToCheckout();
        }
    }
    
    void GoToCheckout()
    {
        if (checkoutPoint != null && !isAtCheckout)
        {
            isAtCheckout = true;
            agent.SetDestination(checkoutPoint.position);
        }
    }
}