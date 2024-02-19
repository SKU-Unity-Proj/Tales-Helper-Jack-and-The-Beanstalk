using UnityEngine;

namespace XEntity.InventoryItemSystem
{
    public class KeyToInventory : MonoBehaviour
    {
        public GameObject instantHarvestObject;

        [SerializeField]
        private LayerMask layerMask;

        void Update()
        {
            PressGetHarvester();
        }

        void PressGetHarvester()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Collider[] colliders = Physics.OverlapSphere(this.transform.position, 3f, layerMask);

                foreach (Collider col in colliders)
                {
                    Debug.Log("P");
                    Interactor interactor = col.GetComponent<Interactor>();
                    if (interactor != null)
                    {
                        Debug.Log("I");
                        instantHarvestObject.GetComponent<InstantHarvest>().AttemptHarvest(interactor);
                        break;
                    }
                }
            }
        }
    }
}

