using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlashLightAngle : MonoBehaviour
{
    [SerializeField] private float viewAngle = 20;
    [SerializeField] private float viewDistance = 20;
    public LayerMask layerMask;

    private void Start()
    {
        StartCoroutine(View());
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance  );
    }

    #region �þ߰�
    private Vector3 BoundaryAngle(float _angle)
    {
        // ���� ��ü�� y�� ȸ������ ������ ���մϴ�.
        _angle += transform.eulerAngles.y;
        // �־��� ������ ���� ���� ���͸� ��ȯ�մϴ�.
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }
    
    IEnumerator View()
    {
        // �þ� ������ ���͸� ����մϴ�.
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        Debug.DrawRay(transform.position, _leftBoundary*10f, Color.red);
        Debug.DrawRay(transform.position, _rightBoundary*10f, Color.red);

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance * 0.7f, layerMask);

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;

            // Ÿ�ٱ����� ���� ���͸� ����մϴ�.
            Vector3 _direction = (_targetTf.position - transform.position).normalized;

            // �þ� ���� ���� �ִ��� Ȯ���մϴ�.
            float _angle = Vector3.Angle(_direction, transform.forward);

            if (_angle < viewAngle * 0.5f)
            {
                RaycastHit _hit;
                // ����ĳ��Ʈ�� ��ֹ��� �ִ��� Ȯ���մϴ�.
                if (Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance, layerMask))
                {
                    //Debug.Log("Hit object: " + _hit.transform.name);

                    _hit.collider.GetComponent<PuppetController>().isTrace = false;
                    _hit.transform.GetComponent<NavMeshAgent>().ResetPath();
                }
            }
            else
            {
                _targetTf.GetComponent<PuppetController>().isTrace = true;
            }
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(View());
    }
    #endregion
}
