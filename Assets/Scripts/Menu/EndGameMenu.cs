using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour
{
    [SerializeField]
    GameObject EndGameMenuGO;
    [SerializeField]
    Text winnerText;
    [SerializeField]
    GameObject GameUIGO;

    void Start()
    {
        ConnectFour.Instance.GameEndEvent += OnGameEnd;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackToMainMenu();
        }
    }

    private void OnDestroy()
    {
        ConnectFour.Instance.GameEndEvent -= OnGameEnd;
    }

    void OnGameEnd(bool _gameIsEnd)
    {
        if (!_gameIsEnd)
            return;

        StartCoroutine(WaitVisualFinishCoroutine());
    }

    IEnumerator WaitVisualFinishCoroutine()
    {
        bool isFinish = false;

        GameUIGO.SetActive(false);

        while (!isFinish)
        {
            if (Visual3D.Instance != null && Visual3D.Instance.gameObject.activeSelf)
            {
                if (Visual3D.Instance.canPlay)
                {
                    isFinish = true;
                }
            }
            else if (Visual2D.Instance != null && Visual2D.Instance.gameObject.activeSelf)
            {
                if (Visual2D.Instance.canPlay)
                {
                    isFinish = true;
                }
            }

            yield return null;
        }

        ActiveEndGameMenu();
    }

    void ActiveEndGameMenu()
    {
        EndGameMenuGO.SetActive(true);

        if (ConnectFour.Instance.winner == Team.NONE)
        {
            winnerText.text = "Draw !";
        }
        else if (ConnectFour.Instance.curMode == Mode.PVE)
        {
            if (ConnectFour.Instance.winner == ConnectFour.Instance.playerOne)
            {
                winnerText.text = "You Win !";
            }
            else
            {
                winnerText.text = "AI Win !";
            }
        }
        else
        {
            if (ConnectFour.Instance.winner == Team.RED)
            {
                winnerText.text = "Red Win !";
            }
            else
            {
                winnerText.text = "Yellow Win !";
            }
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
