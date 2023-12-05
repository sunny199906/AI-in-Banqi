using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasControl : MonoBehaviour
{
    [SerializeField]
    private GameObject PausePanel;
    public void ShowPausePanel() {
        GameManager.Instance.menuOpened = true;
        PausePanel.GetComponent<FadeAnimation>().FadeIn();
    }
    public void HidePausePanel() {
        GameManager.Instance.menuOpened = false;
        PausePanel.GetComponent<FadeAnimation>().FadeOut();
    }
    public void ExitBackToMenu()
    {
        GameManager.Instance.DestoryAllObject();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
