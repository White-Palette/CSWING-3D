using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameScene
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField]
        private GameObject pausePanelBackground = null;
        [SerializeField]
        private GameObject pausePanel = null;
        [SerializeField]
        private GameObject settingPanel = null;

        public bool isActivePausePanel = false;

        private PlayerMove playerMove = null;

        public int score = 0;

        void Start()
        {
            MouseManager.Show(false);
            MouseManager.Lock(true);
            SoundManager.Instance.PlaySound("StreetLove", SoundType.BGM);
            playerMove = FindObjectOfType<PlayerMove>();
            score = 0;
        }

        public void AddScore(int score)
        {
            this.score += score;
            if (PlayerPrefs.GetInt("BestScore") < this.score)
            {
                PlayerPrefs.SetInt("BestScore", this.score);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePausePanel();
                SoundManager.Instance.PlaySound("Stop");
            }
        }

        void TogglePausePanel()
        {
            isActivePausePanel = !isActivePausePanel;
            if (isActivePausePanel)
            {
                MouseManager.Show(true);
                MouseManager.Lock(false);
            }
            else if (!isActivePausePanel)
            {
                MouseManager.Show(false);
                MouseManager.Lock(true);
            }
            pausePanelBackground.SetActive(!pausePanelBackground.activeSelf);
            pausePanel.SetActive(true);
            settingPanel.SetActive(false);
            playerMove.isTimeStop = !playerMove.isTimeStop;
            Time.timeScale = pausePanelBackground.activeSelf ? 0 : 1;
        }

        public void Resume()
        {
            TogglePausePanel();
            MouseManager.Show(false);
            MouseManager.Lock(true);
        }

        public void Setting()
        {
            Debug.Log("Setting");
            pausePanel.SetActive(false);
            settingPanel.SetActive(true);
        }

        public void BackToMainMenu()
        {
            //LoadingSceneManager.LoadScene("TitleScene");
            Time.timeScale = 1;
            SceneManager.LoadScene("TitleScene");
        }

        public void Back()
        {
            pausePanel.SetActive(true);
            settingPanel.SetActive(false);
        }
    }
}