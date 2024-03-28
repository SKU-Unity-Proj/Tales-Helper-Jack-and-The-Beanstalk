using UnityEngine;
using System.Collections.Generic;

public class De : MonoBehaviour
{
    float forceToMass; // 질량을 계산하기 위한 중력에 대한 역수

    public float combinedForce; // 등록된 RigidBody의 합산 힘
    public float calculatedMass; // 계산된 질량

    public int registeredRigidbodies; // 등록된 Rigidbody 수

    Dictionary<Rigidbody, float> impulsePerRigidBody = new Dictionary<Rigidbody, float>(); // 각 Rigidbody에 대한 충격량 저장

    float currentDeltaTime; // 현재 FixedUpdate에서의 deltaTime
    float lastDeltaTime; // 이전 FixedUpdate에서의 deltaTime

    private void Awake()
    {
        forceToMass = 1f / Physics.gravity.magnitude; // 중력의 역수를 사용하여 질량에 대한 계수 계산
    }

    void UpdateWeight()
    {
        registeredRigidbodies = impulsePerRigidBody.Count; // 등록된 Rigidbody의 수 업데이트
        combinedForce = 0; // 합산 힘 초기화

        foreach (var force in impulsePerRigidBody.Values)
        {
            combinedForce += force; // 각 Rigidbody의 힘을 더하여 합산 힘 계산
        }

        calculatedMass = (float)(combinedForce * forceToMass); // 중력과 질량 계수를 곱하여 계산된 질량 구하기
    }

    private void FixedUpdate()
    {
        lastDeltaTime = currentDeltaTime; // 이전 FixedUpdate의 deltaTime 저장
        currentDeltaTime = Time.deltaTime; // 현재 FixedUpdate의 deltaTime 저장
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            if (impulsePerRigidBody.ContainsKey(collision.rigidbody))
                impulsePerRigidBody[collision.rigidbody] = collision.impulse.y / lastDeltaTime; // 충돌로 인한 y축 힘을 deltaTime으로 나누어 저장
            else
                impulsePerRigidBody.Add(collision.rigidbody, collision.impulse.y / lastDeltaTime); // 새로운 Rigidbody의 충돌 힘을 저장

            UpdateWeight(); // 질량 업데이트
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            if (impulsePerRigidBody.ContainsKey(collision.rigidbody))
                impulsePerRigidBody[collision.rigidbody] = collision.impulse.y / lastDeltaTime; // 충돌로 인한 y축 힘을 deltaTime으로 나누어 저장
            else
                impulsePerRigidBody.Add(collision.rigidbody, collision.impulse.y / lastDeltaTime); // 새로운 Rigidbody의 충돌 힘을 저장

            UpdateWeight(); // 질량 업데이트
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            impulsePerRigidBody.Remove(collision.rigidbody); // 충돌 종료된 Rigidbody의 힘 제거
            UpdateWeight(); // 질량 업데이트
        }
    }
}
