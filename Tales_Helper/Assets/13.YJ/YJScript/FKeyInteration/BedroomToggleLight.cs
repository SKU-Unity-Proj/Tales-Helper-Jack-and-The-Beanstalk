using UnityEngine;
using static IFKeyInteractable;

public class BedroomToggleLight : MonoBehaviour, IFInteractable
{
    public float canDistance = 3f; //상호 작용 가능한 거리

    public GameObject directionalLight;
    public GameObject zombieStopLight;
    public GameObject[] zombiePuppet;

    void Start()
    {
        if (directionalLight == null)
            directionalLight = GameObject.Find("Directional Light");

        if (zombieStopLight == null)
            zombieStopLight = GameObject.Find("ZombieStopLight");
    }

    public void Interact(float distance)
    {
        if(distance < canDistance)
        {
            if (!directionalLight.activeSelf)
            {
                directionalLight.SetActive(true);
                zombieStopLight.SetActive(false);

                foreach (var p in zombiePuppet)
                {
                    if (p != null)
                    {
                        PuppetController puppetController = p.GetComponent<PuppetController>();
                        CapsuleCollider capsuleCollider = p.GetComponent<CapsuleCollider>();
                        if (puppetController != null)
                        {
                            puppetController.StopTarce();
                            capsuleCollider.enabled = false;
                        }
                    }
                }
            }
            else
            {
                directionalLight.SetActive(false);
                zombieStopLight.SetActive(true);

                foreach (var p in zombiePuppet)
                {
                    if (p != null)
                    {
                        PuppetController puppetController = p.GetComponent<PuppetController>();
                        CapsuleCollider capsuleCollider = p.GetComponent<CapsuleCollider>();
                        if (puppetController != null)
                        {
                            puppetController.isTrace = true;
                            capsuleCollider.enabled = true;
                        }
                    }
                }
            }
        }
    }
}
