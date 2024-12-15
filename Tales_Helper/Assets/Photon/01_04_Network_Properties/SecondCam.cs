using UnityEngine;

namespace _01_04_Network_Properties
{
    public class SecondCam : MonoBehaviour
    {
        public Transform Target; // Ÿ���� Transform
        public float MouseSensitivity = 10f; // ���콺 �ΰ���

        private float verticalRotation;
        private float horizontalRotation;

        public Vector3 offset = new Vector3(0, 1.6f, 0); // ī�޶��� ��ġ ������ (�Ӹ� ��ġ�� ����)

        void Start()
        {
            //Cursor.lockState = CursorLockMode.Locked; // Ŀ�� ���
        }

        void Update()
        {
            if (Target == null)
            {
                return;
            }

            // ���콺 �Է� �ޱ�
            float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

            verticalRotation -= mouseY * MouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

            horizontalRotation += mouseX * MouseSensitivity;

            // ī�޶��� ȸ�� ����
            Quaternion rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
            transform.rotation = rotation;

            // ī�޶��� ��ġ�� Ÿ�� �������� ����
            transform.position = Target.position + offset;
        }
    }
}
