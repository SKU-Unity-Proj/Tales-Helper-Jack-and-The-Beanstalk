using UnityEngine;
using static IFKeyInteractable;

public class BedroomToggleLight : MonoBehaviour, IFInteractable
{
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

    public void Interact()
    {
        if (!directionalLight)
        {
            directionalLight.SetActive(true);
            zombieStopLight.SetActive(false);

            foreach (var p in zombiePuppet)
            {
                if (p != null)
                {
                    PuppetController puppetController = p.GetComponent<PuppetController>();
                    if (puppetController != null)
                    {
                        puppetController.isTrace = false;
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
                    if (puppetController != null)
                    {
                        puppetController.isTrace = true;
                    }
                }
            }
        }
    }
}
