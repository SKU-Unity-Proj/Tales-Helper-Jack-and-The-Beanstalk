using FC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiasGames.Components;

/// <summary>
/// 발자국 소리를 출력.
/// </summary>
public class PlayerFootStep : MonoBehaviour
{
    public SoundList[] stepSounds;
    private Animator myAnimator;
    private int index;
    private float dist;

    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;

    private IMover _mover = null;
    private bool grounded;

    public enum Foot
    {
        LEFT,
        RIGHT,
    }
    private Foot step = Foot.LEFT;
    private float oldDist, maxDist = 0; // 이동거리 체크.
    private float lastStepTime = 0f; // 마지막 발자국 소리 시간
    private float stepInterval = 0.1f; // 최소 발자국 소리 간격

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        _mover = GetComponent<IMover>(); // 이동자 컴포넌트 가져오기
    }

    private void PlayFootStep()
    {
        if (oldDist < maxDist || Time.time - lastStepTime < stepInterval)
        {
            return;
        }
        oldDist = maxDist = 0;
        lastStepTime = Time.time; // 마지막 발자국 소리 시간 업데이트
        int oldIndex = index;
        while (oldIndex == index)
        {
            index = Random.Range(0, stepSounds.Length);
        }
        SoundManager.Instance.PlayOneShotEffect((int)stepSounds[index], transform.position, 0.2f);
    }

    private void Update()
    {
        if (!grounded && _mover.IsGrounded())
        {
            PlayFootStep();
        }
        grounded = _mover.IsGrounded();
        float factor = 0.15f;

        if (grounded && myAnimator.velocity.magnitude > 1.6f)
        {
            oldDist = maxDist;
            switch (step)
            {
                case Foot.LEFT:
                    dist = leftFoot.position.y - transform.position.y;
                    maxDist = dist > maxDist ? dist : maxDist;
                    if (dist <= factor)
                    {
                        PlayFootStep();
                        step = Foot.RIGHT;
                    }
                    break;
                case Foot.RIGHT:
                    dist = rightFoot.position.y - transform.position.y;
                    maxDist = dist > maxDist ? dist : maxDist;
                    if (dist <= factor)
                    {
                        PlayFootStep();
                        step = Foot.LEFT;
                    }
                    break;
            }
        }
    }
}
