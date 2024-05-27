using UnityEngine;
using DiasGames.Components;
using System.Collections;
using UnityEngine.SceneManagement;

namespace DiasGames.Controller
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private GameObject player = null;
        [SerializeField] private float delayToRestartLevel = 3f;

        // player components
        private Health _playerHealth;
        private RoomManager _roomManager;

        // controller vars
        private bool _isRestartingLevel;

        private void Awake()
        {
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");

            _playerHealth = player.GetComponent<Health>();
            _roomManager = FindObjectOfType<RoomManager>();
        }

        private void OnEnable()
        {
            _playerHealth.OnDead += RestartLevel;
        }

        private void OnDisable()
        {
            _playerHealth.OnDead -= RestartLevel;
        }

        // Resets the player's position to the current room's start position
        private void RestartLevel()
        {
            if (!_isRestartingLevel)
                StartCoroutine(OnRestart());
        }

        public void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        private IEnumerator OnRestart()
        {
            _isRestartingLevel = true;

            yield return new WaitForSeconds(delayToRestartLevel);

            if (_roomManager != null)
            {
                //_roomManager.ResetPlayerPosition(); // 플레이어 위치 초기화
            }

            _isRestartingLevel = false;
        }
    }
}
