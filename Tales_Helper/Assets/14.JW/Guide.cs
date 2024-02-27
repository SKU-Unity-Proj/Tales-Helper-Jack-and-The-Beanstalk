using UnityEngine;

public class Guide : MonoBehaviour
{
    [SerializeField] private GameObject NoticeUI;
    private bool isUIopen = true;
    public LayerMask layerMask;

    void Update()
    {
        CheckUI();
    }

    void CheckUI()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (NoticeUI.activeSelf)
            {
                NoticeUI.SetActive(false);
                isUIopen = false;
            }

            Collider[] colliders = Physics.OverlapSphere(this.transform.position, 3f, layerMask);

            foreach (Collider col in colliders)
            {
                if (!NoticeUI.activeSelf && isUIopen)
                {
                    NoticeUI.SetActive(true);
                    break;
                }
                isUIopen = true;
            }
            
        }
    }
}
