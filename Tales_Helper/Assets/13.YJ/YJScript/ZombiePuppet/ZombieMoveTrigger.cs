using UnityEngine;

public class ZombieMoveTrigger : MonoBehaviour
{
    public GameObject[] zombiePuppet;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject puppets in zombiePuppet)
            {
                PuppetController puppetController = puppets.GetComponent<PuppetController>();

                if (puppetController != null)
                {
                    puppetController.TracePlayer();
                }
            }
            this.gameObject.SetActive(false);
            Debug.Log("Trace");
        }
    }
}
