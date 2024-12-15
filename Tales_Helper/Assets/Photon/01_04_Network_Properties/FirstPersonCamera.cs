using UnityEngine;

namespace _01_04_Network_Properties
{
    public class FirstPersonCamera : MonoBehaviour
    {
        public Transform Target; // Ÿ���� Transform
        public float MouseSensitivity = 10f; // ���콺 �ΰ���

        private float verticalRotation;
        private float horizontalRotation;

        public float distance = 2.76f;  // ī�޶�� �÷��̾� ������ �Ÿ�
        public float height = 2.0f;  // ī�޶��� ����

        public Vector3 offset = new Vector3(0, 3.07f, 0.24f); // ī�޶��� ��ġ ������ (�Ӹ� ��ġ�� ����)

        //private PlayerMovement2 playerMovement;
        private int originalCullingMask;

        void Start()
        {
            if (Target != null)
            {
               // playerMovement = Target.GetComponent<PlayerMovement2>();
            }
            originalCullingMask = GetComponent<Camera>().cullingMask; // ī�޶��� ���� cullingMask ����
        }

        void LateUpdate()
        {
            if (Target == null)
            {
                return;
            }
            /*
            // playerMovement �ʱ�ȭ (�� ���� ����ǵ���)
            if (playerMovement == null)
            {
                playerMovement = Target.GetComponent<PlayerMovement2>();
                if (playerMovement == null)
                {
                    return;
                }
            }
            */
            Camera cameraComponent = GetComponent<Camera>();

            /*
            if (playerMovement.IsSeated())
            {
                Debug.Log("1��Ī ����");

                cameraComponent.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));

                MouseSensitivity = 3;

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
            else
            {
                // 3��Ī ����
                // ���콺 �Է� �ޱ�
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                // ī�޶� ȸ�� ����
                verticalRotation -= mouseY * MouseSensitivity;
                verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

                horizontalRotation += mouseX * MouseSensitivity;

                // ī�޶��� ȸ�� ����
                Quaternion rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
                transform.rotation = rotation;

                // ī�޶��� ��ġ�� Ÿ�� �������� ����
                Vector3 offset = new Vector3(0, height, -distance);
                transform.position = Target.position + rotation * offset;

                // Player ���̾� Ȱ��ȭ
                cameraComponent.cullingMask = originalCullingMask;
            }
            */
            // 3��Ī ����
            // ���콺 �Է� �ޱ�
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // ī�޶� ȸ�� ����
            verticalRotation -= mouseY * MouseSensitivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

            horizontalRotation += mouseX * MouseSensitivity;

            // ī�޶��� ȸ�� ����
            Quaternion rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
            transform.rotation = rotation;

            // ī�޶��� ��ġ�� Ÿ�� �������� ����
            Vector3 offset = new Vector3(0, height, -distance);
            transform.position = Target.position + rotation * offset;

            // Player ���̾� Ȱ��ȭ
            cameraComponent.cullingMask = originalCullingMask;
        }
    }
}
