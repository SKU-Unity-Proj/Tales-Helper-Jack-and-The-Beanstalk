using UnityEngine;

namespace FC
{
    public class GiantFootStep : MonoBehaviour
    {
        public SoundList[] stepSounds; // �� ���θ��� �ٸ� ���ڱ� �Ҹ�
        public SoundList swingSound, slamSound, jumpSound; // �� ���θ��� �ٸ� ���ڱ� �Ҹ�

        public Transform clubPos;
        private Animator myAnimator;
        private int index;

        private void Awake()
        {
            myAnimator = GetComponent<Animator>();
        }

        // �ִϸ��̼� �̺�Ʈ���� ȣ���� �Լ�
        public void GiantPlayFootStep()
        {
            int oldIndex = index;
            while (oldIndex == index)
            {
                index = Random.Range(0, stepSounds.Length);
            }

            Debug.Log($"Playing footstep sound index: {index} at position: {transform.position}");
            SoundManager.Instance.PlayOneShotEffect((int)stepSounds[index], transform.position, 3f);
        }
        public void GiantPlaySwing()
        {
            SoundManager.Instance.PlayOneShotEffect((int)swingSound, transform.position, 3f);
        }
        public void GiantPlayJump()
        {
            SoundManager.Instance.PlayOneShotEffect((int)jumpSound, transform.position, 3f);
        }
        public void GiantPlaySlam()
        {
            SoundManager.Instance.PlayOneShotEffect((int)slamSound, clubPos.position, 3f);
        }
    }
}
