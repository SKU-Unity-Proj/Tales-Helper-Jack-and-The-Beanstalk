using UnityEngine;

public class ZombieMoveTrigger : MonoBehaviour
{
    public PuppetController puppetController;

    private void Start()
    {
        if(puppetController == null)
            puppetController = FindObjectOfType<PuppetController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (puppetController != null&&other.CompareTag("Player"))
        {
            puppetController.TracePlayer();

            this.gameObject.SetActive(false);
            Debug.Log("Trace");
        }
    }
}
