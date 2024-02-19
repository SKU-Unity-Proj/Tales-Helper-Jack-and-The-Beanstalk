using UnityEngine;
using XEntity.InventoryItemSystem;

namespace XEntity.InventoryItemSystem
{
    public class TriggerToInventory : MonoBehaviour
    {
        public GameObject instantHarvestObject;

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Player"))
            {
                Interactor interactor = col.GetComponent<Interactor>();

                if (interactor != null)
                {
                    instantHarvestObject.GetComponent<InstantHarvest>().AttemptHarvest(interactor);
                }
            }
        }
    }
}

