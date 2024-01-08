using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.Components
{
    // Ragdoll ������Ʈ: ĳ���Ͱ� ��� �� ragdoll ���� ȿ���� ����
    public class Ragdoll : MonoBehaviour
    {
        private Health _health;          // Health ������Ʈ ����. ĳ���� ��� ���� Ȯ�ο�
        private Animator _animator;      // Animator ������Ʈ ����. Ragdoll�� �۵��Ϸ��� ��Ȱ��ȭ�Ǿ�� ��

        // Ragdoll�� ���Ǵ� Rigidbody��
        private List<Rigidbody> _ragdollRigidbodies = new List<Rigidbody>();
        // Ragdoll�� ���Ǵ� Collider��
        private List<Collider> _ragdollColliders = new List<Collider>();

        private void Awake()
        {
            // Health �� Animator ������Ʈ ã��
            _health = GetComponent<Health>();
            _animator = GetComponent<Animator>();

            // Ragdoll�� �ʿ��� Rigidbody�� Collider ���� ���
            GetRagdollReferences();
        }

        private void OnEnable()
        {
            // Health ������Ʈ�� ���� ���, ĳ���� ��� �� Ragdoll Ȱ��ȭ �̺�Ʈ ����
            if (_health)
                _health.OnDead += ActivateRagdoll;
        }

        private void OnDisable()
        {
            // Health ������Ʈ�� ���� ���, �̺�Ʈ ���� ����
            if (_health)
                _health.OnDead -= ActivateRagdoll;
        }

        private void GetRagdollReferences()
        {
            // Animator ������Ʈ�� ������ ����
            if (_animator == null) return;

            // Animator�� ������ ��� ����(Bones)�� ��ȸ�ϸ鼭 Rigidbody�� Collider ã��
            for (int i = 0; i < 18; i++)
            {
                var bone = _animator.GetBoneTransform((HumanBodyBones)i);

                // Rigidbody ������Ʈ�� ������ ��Ȱ��ȭ�ϰ� ����Ʈ�� �߰�
                if (bone.TryGetComponent(out Rigidbody rb))
                {
                    rb.isKinematic = true; // ���� ȿ�� ��Ȱ��ȭ
                    _ragdollRigidbodies.Add(rb);
                }

                // Collider ������Ʈ�� ������ ��Ȱ��ȭ�ϰ� ����Ʈ�� �߰�
                if (bone.TryGetComponent(out Collider coll))
                {
                    coll.enabled = false;
                    _ragdollColliders.Add(coll);
                }
            }
        }

        private void ActivateRagdoll()
        {
            // Animator ������Ʈ�� ������ ����
            if (_animator == null) return;

            // Animator ��Ȱ��ȭ
            _animator.enabled = false;

            // ��� Rigidbody�� Ȱ��ȭ�Ͽ� ���� ȿ�� ����
            _ragdollRigidbodies.ForEach(r => {
                r.isKinematic = false;
                r.useGravity = true;
            });

            // ��� Collider�� Ȱ��ȭ
            _ragdollColliders.ForEach(c => c.enabled = true);
        }
    }
}
