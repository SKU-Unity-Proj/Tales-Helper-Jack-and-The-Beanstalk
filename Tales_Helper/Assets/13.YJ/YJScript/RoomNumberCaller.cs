using UnityEngine;

public class RoomNumberCaller : MonoBehaviour
{
    public MapController mapController;
    public int RoomNum = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mapController.CurrentRoom(RoomNum);
        }
    }
}
