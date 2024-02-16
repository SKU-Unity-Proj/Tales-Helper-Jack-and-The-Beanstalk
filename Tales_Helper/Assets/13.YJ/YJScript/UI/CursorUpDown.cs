using UnityEngine;

public class CursorUpDown : MonoBehaviour
{
    public float speed = 100;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * speed);
    }
}
