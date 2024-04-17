using UnityEngine;
using DiasGames.Components;

namespace DiasGames.Abilities
{
    public class JumpInBed : MonoBehaviour
    {
        private bool onBed;
        private bool onBedPrevState; // 이전 프레임의 onBed 상태를 저장할 변수
        private bool canJumpCombo = false;
        private int jumpCombo = 0;

        private float GroundedOffset = -0.2f;
        private float GroundedRadius = 0.28f;
        public LayerMask BedLayers;

        public AirControlAbility airControlAbility;

        private float timer = 0f; // 조건이 만족된 후 경과한 시간

        private void Update()
        {
            CheckBed();
            CheckBedCondition();
            JumpComboLogic();
        }

        private void CheckBed()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            onBed = Physics.CheckSphere(spherePosition, GroundedRadius, BedLayers, QueryTriggerInteraction.Ignore);
        }

        private void CheckBedCondition()
        {
            // 이전 프레임과 현재 프레임의 onBed 상태가 다르고, 현재 프레임에서 onBed가 true일 때
            if (onBed != onBedPrevState && onBed)
            {
                // 변경된 직후 0.5초 동안 실행
                if (timer < 0.5f)
                {
                    canJumpCombo = true;

                    timer += Time.deltaTime; // 시간 측정
                }
                else
                {
                    timer = 0f; // 시간 초기화
                    canJumpCombo = false;
                }
            }

            onBedPrevState = onBed; // 현재 프레임의 onBed 상태를 이전 프레임으로 저장
        }

        private void JumpComboLogic()
        {
            if (canJumpCombo)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    jumpCombo++;

                    //jumpCombo에 따른 점프 높이 향상
                    switch (jumpCombo)
                    {
                        case 1:
                            Debug.Log("첫 번째 점프! 기본 높이로 점프합니다.");
                            airControlAbility.jumpHeight = 1.5f;
                            break;
                        case 2:
                            Debug.Log("두 번째 점프! 높이가 증가된 점프를 합니다.");
                            airControlAbility.jumpHeight = 3f;
                            break;
                        case 3:
                            Debug.Log("세 번째 점프! 더 높이 점프합니다.");
                            airControlAbility.jumpHeight = 4f;
                            break;
                        default:
                            Debug.Log("이제는 최대 점프 높이에 도달했습니다!");

                            break;
                    }
                }
            }
        }
    }
}
