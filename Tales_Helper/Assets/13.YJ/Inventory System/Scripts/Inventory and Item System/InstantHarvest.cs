using UnityEngine;

namespace XEntity.InventoryItemSystem
{
    //This script is attached to any item that is picked up by the interactor on a single click such as small rocks and sticks.
    //NOTE: The item is only added if the interactor is within the interaction range.
    public class InstantHarvest : MonoBehaviour, IInteractable
    {
        private bool isHarvested = false;

        //Ŭ�� �� ��Ȯ�Ǵ� �׸��Դϴ�.
        public Item harvestItem;

        [SerializeField]
        private LayerMask layerMask;

        void Update()
        {
            //PressGetHarvester();
        }

        //The item is instantly added to the inventory of the interactor on interact.
        public void OnClickInteract(Interactor interactor)
        {
            //���� ��Ȯ���� ���� ��� ��Ȯ �õ�
            AttemptHarvest(interactor);
        }

        public void AttemptHarvest(Interactor harvestor) 
        {
            if (!isHarvested)
            {
                if (harvestor.AddToInventory(harvestItem, gameObject))
                {
                    isHarvested = true;
                }
            }
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Player")) { }
            Interactor interactor = col.GetComponent<Interactor>();
            if (interactor != null)
            {
                AttemptHarvest(interactor);
            }
        }

        /*
        void PressGetHarvester()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Collider[] colliders = Physics.OverlapSphere(this.transform.position, 3f, layerMask);

                foreach (Collider col in colliders)
                {
                    Interactor interactor = col.GetComponent<Interactor>();
                    if (interactor != null)
                    {
                        AttemptHarvest(interactor);
                        break;
                    }
                }
            }
        }
        */
    }
}
