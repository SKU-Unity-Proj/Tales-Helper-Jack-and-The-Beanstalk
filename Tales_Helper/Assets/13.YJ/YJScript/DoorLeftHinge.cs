using UnityEngine;

public class DoorLeftHinge : MonoBehaviour
{
    public GameObject door;

    public float maxRight;

    public float rotationSpeed = 10.0f;

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (door.transform.eulerAngles.y > maxRight)
            {
                Vector3 currentRotation = door.transform.rotation.eulerAngles;

                currentRotation.y -= rotationSpeed * Time.deltaTime;

                door.transform.rotation = Quaternion.Euler(currentRotation);
            }
        }
    }
}
