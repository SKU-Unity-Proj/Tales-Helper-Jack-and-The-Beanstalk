using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class NewBehaviourScript : MonoBehaviour
{
    public Transform player;
    public Vector3 pivotOffset = new Vector3(0.0f, 1.0f, 0.0f);
    public Vector3 camOffset = new Vector3(0.4f, 0.5f, -2.0f);

    public float smooth = 10f;  // ī�޶� �����ӵ�
    public float horizontalAimingSpeed = 6.0f; // ���� ȸ�� �ӵ�.
    public float verticalAimingSpeed = 6.0f; // ���� ȸ�� �ӵ�.
    public float maxVerticalAngle = 30.0f; // ī�޶� ���� �ִ� ����
    public float minverticalAngle = -60.0f; // ī�޶� ���� �ּ� ����
    public float recoilAngleBounce = 5.0f; // ��� �ݵ� ��
    private float angleH = 0.0f; // ���콺 �̵��� ���� ī�޶� �����̵� ��ġ.
    private float angleV = 0.0f; // ���콺 �̵��� ���� ī�޶� �����̵� ��ġ.
    private Transform cameraTransform; // Ʈ������ ĳ��.
    private Camera myCamera;
    private Vector3 relCameraPos; // �÷��̾�κ��� ī�޶������ ����.
    private float relCameraPosMag; // �÷��̾�κ��� ī�޶���� �Ÿ�.
    private Vector3 smoothPivotOffset; // ī�޶� �Ǻ� ������ ����. 
    private Vector3 smoothCamOffset; // ī�޶� ��ġ ������ ����. 
    private Vector3 targetPivotOffset; // ī�޶� �Ǻ� ������ ����. 
    private Vector3 targetCamOffset; // ī�޶� ��ġ ������ ����.
    private float defaultFOV; // �⺻ �þ߰�.
    private float targetFOV; // Ÿ�� �þ߰�.
    private float targetMaxVerticleAngle; // ī�޶� ���� �ִ� ����. -> ��ݽ� �ݵ�
    private float recoilAngle = 0f; // ��� �ݵ� ����

    /// <summary>
    /// private ������Ƽ �߰� get,set
    /// </summary>
    public float GetH
    {
        //get => angleH;
        get
        {
            return angleH;
        }
    }

    private void Awake()
    {
        //ĳ��
        cameraTransform = transform;
        myCamera = cameraTransform.GetComponent<Camera>();

        // ī�޶� �⺻ ������ ����
        cameraTransform.position = player.position + Quaternion.identity * pivotOffset + Quaternion.identity * camOffset;
        cameraTransform.rotation = Quaternion.identity;

        // ī�޶�� �÷��̾�� ��� ����. -> �浹üũ�� �����
        relCameraPos = cameraTransform.position - player.position;
        relCameraPosMag = relCameraPos.magnitude - 0.5f; // �÷��̾ �� �Ÿ�.

        // �⺻����
        smoothPivotOffset = pivotOffset;
        smoothCamOffset = camOffset;
        defaultFOV = myCamera.fieldOfView;
        angleH = player.eulerAngles.y; // �ʱ� y��.

        ResetTargetOffsets();
        ResetFOV();
        ResetMaxVerticalAngle();


    }
    //�ʱ�ȭ �Լ�
    public void ResetTargetOffsets()
    {
        targetPivotOffset = pivotOffset;
        targetCamOffset = camOffset;
    }
    public void ResetFOV()
    {
        this.targetFOV = defaultFOV;
    }
    public void ResetMaxVerticalAngle()
    {
        targetMaxVerticleAngle = maxVerticalAngle;
    }

    // �⺻ ���� ���� �Լ�
    public void BounceVertical(float degree)
    {
        recoilAngle = degree;
    }
    public void SetTargetOffset(Vector3 newPivotOffset, Vector3 newCamOffset)
    {
        targetPivotOffset = newPivotOffset;
        targetCamOffset = newCamOffset;
    }
    public void SetFOV(float customFOV)
    {
        this.targetFOV = customFOV;
    }

    // �浹üũ �Լ�
    bool ViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight) // playerPos�� �߹��̹Ƿ� �÷��̾��� ���̸� üũ�ؼ� �� ������ �浹üũ�� �̾��.
    {
        Vector3 target = player.position + (Vector3.up * deltaPlayerHeight);
        // Raycast : �浹�ϴ� collider�� ���� �Ÿ�, ��ġ�� �ڼ��� ������ RaycastHit�� ��ȯ.
        // ����, LayerMask ���͸��� �ؼ� ���ϴ� layer�� �浹�� �ϸ� true �� ��ȯ.
        // �߰���, OverlapSphere : ������ ���������� ������ ���� ����� �����Ϸ��� �ݰ� �̳��� ���� �ִ� collider�� ��ȯ�ϴ� �Լ�.
        // �Լ��� ��ȯ ���� collider ������Ʈ �迭�� �Ѿ�´�.
        if (Physics.SphereCast(checkPos, 0.2f, target - checkPos, out RaycastHit hit, relCameraPosMag))
        {
            if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger) // isTrigger�� false�� �δ� ������ �̺�Ʈ�� ������ ���� Collider�� ���� ���Ѿ� �Ǳ⶧��.
            {
                return false;
            }
        }
        return true;
    }
    bool ReverseViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight, float maxDistance)
    {
        Vector3 origin = player.position + (Vector3.up * deltaPlayerHeight);
        if (Physics.SphereCast(origin, 0.2f, checkPos - origin, out RaycastHit hit, maxDistance))
        {
            if (hit.transform != player && hit.transform != transform && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// ���� üũ�� ���ؼ� true�� false ��ȯ true -> �浹X false -> �� �߿� �ϳ��� �浹�� �ߴ�.
    /// </summary>
    bool DoubleViewingPosCheck(Vector3 checkPos, float offset)
    {
        float playerFocusHeight = player.GetComponent<CapsuleCollider>().height * 0.75f;
        return ViewingPosCheck(checkPos, playerFocusHeight) && ReverseViewingPosCheck(checkPos, playerFocusHeight, offset);
    }

    private void Update()
    {
        // ���콺 �̵� ��.
        angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 1f) * horizontalAimingSpeed;
        angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1f, 1f) * verticalAimingSpeed;
        // ���� �̵� ����.
        angleV = Mathf.Clamp(angleV, minverticalAngle, targetMaxVerticleAngle);
        // ���� ī�޶� �ٿ.
        angleV = Mathf.LerpAngle(angleV, angleV + recoilAngle, 10f * Time.deltaTime); // ���� ����.

        // ī�޶� ȸ��.
        Quaternion camYRotation = Quaternion.Euler(0.0f, angleH, 0.0f);
        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH, 0.0f);
        cameraTransform.rotation = aimRotation;

        //set FOV
        myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, targetFOV, Time.deltaTime);

        Vector3 baseTempPos = player.position + camYRotation * targetPivotOffset; // �⺻ ������ ��.
        Vector3 noCollisionOffset = targetCamOffset; // ������ �� ī�޶��� ������ ��, ������ ���� ���� ���� �� ���� �ٸ���.

        for (float zOffset = targetCamOffset.z; zOffset <= 0f; zOffset += 0.5f)
        {
            noCollisionOffset.z = zOffset;
            if (DoubleViewingPosCheck(baseTempPos + aimRotation * noCollisionOffset, Mathf.Abs(zOffset)) || zOffset == 0f)
            {
                break;
            }
        }
        // Reposition Camera
        smoothPivotOffset = Vector3.Lerp(smoothPivotOffset, targetPivotOffset, smooth * Time.deltaTime);
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, noCollisionOffset, smooth * Time.deltaTime);

        cameraTransform.position = player.position + camYRotation * smoothPivotOffset + aimRotation * smoothCamOffset;

        // �ݵ� ����� �ֱ�.
        if (recoilAngle > 0.0f)
        {
            recoilAngle -= recoilAngleBounce * Time.deltaTime;
        }
        else if (recoilAngle < 0.0f)
        {
            recoilAngle += recoilAngleBounce * Time.deltaTime;
        }
    }

    /// <summary>
    /// AimBehavior ���� ���� �Լ�.
    /// </summary>
    public float GetCurrentPivotMagnitude(Vector3 finalPivotOffset)
    {
        return Mathf.Abs((finalPivotOffset - smoothPivotOffset).magnitude);
    }
}
