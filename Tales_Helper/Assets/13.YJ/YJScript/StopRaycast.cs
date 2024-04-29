using UnityEngine;

public class StopRaycast : MonoBehaviour
{
    public BeanStalkRaycast beanStalkRaycast;

    private void Start()
    {
        if(beanStalkRaycast == null)
        {
            beanStalkRaycast = FindObjectOfType<BeanStalkRaycast>();
        }
    }

    public void StoppingRaycast()
    {
        beanStalkRaycast.enabled = false;

        Invoke("StartRaycast", 1.5f);
    }

    private void StartRaycast()
    {
        beanStalkRaycast.enabled = true;
    }
}
