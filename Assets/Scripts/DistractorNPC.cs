using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DistractorNPC : BaseNPC
{
    [Header("Distractor Settings")]
    public float runSpeed = 4f;
    public float pushForce = 10f;
    public float activationDelay = 1f;
    
    private NavMeshAgent agent;
    private bool isActivated = false;
    private Rigidbody playerRb;
    
    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        
        firstInteractionText = "Trying to get something!";
        secondInteractionText = "Fine, you got me!";
        
        if (player != null)
            playerRb = player.GetComponent<Rigidbody>();
    }
    
    public void ActivateDistraction()
    {
        if (!isActivated)
        {
            StartCoroutine(StartDistractionAfterDelay());
        }
    }
    
    IEnumerator StartDistractionAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);
        isActivated = true;
        agent.speed = runSpeed;
        StartCoroutine(RunRandomly());
    }
    
    IEnumerator RunRandomly()
    {
        while (isActivated && !arrested)
        {
            // Pick random point to run to
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * 10f;
            randomPoint.y = transform.position.y;
            
            agent.SetDestination(randomPoint);
            
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isActivated && playerRb != null)
        {
            // Push player
            Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
            pushDirection.y = 0; // Keep push horizontal
            playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
    }
}
