using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject GuidePanel;

    public GameObject GuideSlide;

    private float elapsedTime;

    private float moveDuration = 0.5f;

    private Vector3 startingPos;

    private Vector3 moveToPos;

    private float minX = 1250f;

    private float maxX = -670f;

    private float spacing = 480f;

    private bool sliding = false;

    private void Update()
    {
        if (!startingPos.Equals(moveToPos))
        {
            elapsedTime += Time.deltaTime;
            float percToComplete = elapsedTime / moveDuration;
            GuideSlide.transform.position = Vector3.Lerp(startingPos, moveToPos, Mathf.SmoothStep(0, 1, percToComplete));
        }
        if (GuideSlide.transform.position.Equals(moveToPos))
        {
            sliding = false;
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ShowHowToPlay()
    {
        GuidePanel.GetComponent<FadeAnimation>().FadeIn();
    }

    public void HideHowToPlay()
    {
        GuidePanel.GetComponent<FadeAnimation>().FadeOut();
    }

    public void NextSlide()
    {
        Debug.Log(GuideSlide.transform.position.x);
        if (!sliding && GuideSlide.transform.position.x > -324)
        {
            sliding = true;
            startingPos = GuideSlide.transform.position;
            moveToPos = new Vector3(startingPos.x - spacing, startingPos.y, startingPos.z);
        }
    }

    public void PreviousSlide()
    {
        Debug.Log(GuideSlide.transform.position.x);
        if (!sliding && GuideSlide.transform.position.x < minX)
        {
            startingPos = GuideSlide.transform.position;
            moveToPos = new Vector3(startingPos.x + spacing, startingPos.y, startingPos.z);
        }
    }
}
