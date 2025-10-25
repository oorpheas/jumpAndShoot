using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
        public static GameOverManager Instance;

        [SerializeField] private GameObject _gOUI;

        private void Start() {
                if (Instance == null)
                        Instance = this;
                else
                        Destroy(gameObject);

                _gOUI.SetActive(false);

        }

        public void TriggerGO() {
                //Time.timeScale = 0f;
                _gOUI.SetActive(true);

        }

        public void Restart() {
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
        }

        public void Return() {
                //Time.timeScale = 1f;
                SceneManager.LoadScene("TelaInicial");
        }

        private void OnEnable() {
                GameManager.OnGameOver += TriggerGO;
        }

        private void OnDisable() {
                GameManager.OnGameOver -= TriggerGO;
        }

}
