using UnityEngine;
using System.Collections.Generic;

public class De : MonoBehaviour
{
    float forceToMass; // ������ ����ϱ� ���� �߷¿� ���� ����

    public float combinedForce; // ��ϵ� RigidBody�� �ջ� ��
    public float calculatedMass; // ���� ����

    public int registeredRigidbodies; // ��ϵ� Rigidbody ��

    Dictionary<Rigidbody, float> impulsePerRigidBody = new Dictionary<Rigidbody, float>(); // �� Rigidbody�� ���� ��ݷ� ����

    float currentDeltaTime; // ���� FixedUpdate������ deltaTime
    float lastDeltaTime; // ���� FixedUpdate������ deltaTime

    private void Awake()
    {
        forceToMass = 1f / Physics.gravity.magnitude; // �߷��� ������ ����Ͽ� ������ ���� ��� ���
    }

    void UpdateWeight()
    {
        registeredRigidbodies = impulsePerRigidBody.Count; // ��ϵ� Rigidbody�� �� ������Ʈ
        combinedForce = 0; // �ջ� �� �ʱ�ȭ

        foreach (var force in impulsePerRigidBody.Values)
        {
            combinedForce += force; // �� Rigidbody�� ���� ���Ͽ� �ջ� �� ���
        }

        calculatedMass = (float)(combinedForce * forceToMass); // �߷°� ���� ����� ���Ͽ� ���� ���� ���ϱ�
    }

    private void FixedUpdate()
    {
        lastDeltaTime = currentDeltaTime; // ���� FixedUpdate�� deltaTime ����
        currentDeltaTime = Time.deltaTime; // ���� FixedUpdate�� deltaTime ����
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            if (impulsePerRigidBody.ContainsKey(collision.rigidbody))
                impulsePerRigidBody[collision.rigidbody] = collision.impulse.y / lastDeltaTime; // �浹�� ���� y�� ���� deltaTime���� ������ ����
            else
                impulsePerRigidBody.Add(collision.rigidbody, collision.impulse.y / lastDeltaTime); // ���ο� Rigidbody�� �浹 ���� ����

            UpdateWeight(); // ���� ������Ʈ
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            if (impulsePerRigidBody.ContainsKey(collision.rigidbody))
                impulsePerRigidBody[collision.rigidbody] = collision.impulse.y / lastDeltaTime; // �浹�� ���� y�� ���� deltaTime���� ������ ����
            else
                impulsePerRigidBody.Add(collision.rigidbody, collision.impulse.y / lastDeltaTime); // ���ο� Rigidbody�� �浹 ���� ����

            UpdateWeight(); // ���� ������Ʈ
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            impulsePerRigidBody.Remove(collision.rigidbody); // �浹 ����� Rigidbody�� �� ����
            UpdateWeight(); // ���� ������Ʈ
        }
    }
}
