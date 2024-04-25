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

    #region 시야각
    private Vector3 BoundaryAngle(float _angle)
    {
        // 현재 객체의 y축 회전값을 각도에 더합니다.
        _angle += transform.eulerAngles.y;
        // 주어진 각도에 대한 방향 벡터를 반환합니다.
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    IEnumerator View()
    {
        // 시야 각도의 좌측 경계 벡터를 계산합니다.
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        // 시야 각도의 우측 경계 벡터를 계산합니다.
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        // 시각적 디버깅을 위해 좌우 경계를 빨간색으로 그립니다.
        Debug.DrawRay(transform.position, _leftBoundary*10f, Color.red);
        Debug.DrawRay(transform.position, _rightBoundary*10f, Color.red);

        // 시야 내의 모든 오브젝트를 검색합니다.
        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, layerMask);

        // 각 타겟에 대해 검사합니다.
        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            // 타겟이 플레이어인 경우
            // 타겟까지의 방향 벡터를 계산합니다.
            //Vector3 _direction = (_targetTf.position - transform.position).normalized;
            Vector3 _direction = _targetTf.position - transform.position;
            Debug.DrawRay(transform.position, _direction * 10f, Color.red);
            // 시야 각도 내에 있는지 확인합니다.
            float _angle = Vector3.Angle(_direction, transform.forward);

            if (_angle < viewAngle)
            {
                RaycastHit _hit;
                // 레이캐스트로 장애물이 있는지 확인합니다.
                if (Physics.Raycast(transform.position, _direction, out _hit, viewDistance, layerMask))
                {
                    Debug.Log("Hit object: " + _hit.transform.name);
                    if (_hit.collider != null)
                    {
                        Debug.Log("Stop");

                        _hit.collider.GetComponent<PuppetController>().isTrace = false;
                        _hit.transform.GetComponent<NavMeshAgent>().ResetPath();
                    }
                }
            }
            else
            {
                Debug.Log("ReTrace");

                _targetTf.GetComponent<PuppetController>().isTrace = true;
            }
        }
        // 일정 시간 뒤에 View 함수를 다시 호출합니다.
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(View());
    }
    #endregion
}
