using UnityEngine;

public class Chess : MonoBehaviour
{
    public static float defeatedBlackX = 8f;

    public static float defeatedBlackZ = 14f;

    public static float defeatedRedX = -9f;

    public static float defeatedRedZ = 14f;

    private bool isBlack;

    private bool hasRevealed;

    private int ranking;

    public bool isEmpty;

    private int x, y;

    private bool selected;

    private float elapsedTime;

    private float moveDuration = 3f;

    private bool EmbbedToOject = false;

    private bool isMoving = false;

    private Vector3 startingPos;

    private Vector3 moveToPos;

    private float rotateToX;

    private void Update()
    {
        if (transform.eulerAngles.x != rotateToX)
            this.transform.Rotate(new Vector3(1, 0, 0));
        if (EmbbedToOject)
        {
            if (isMoving)
            {

                elapsedTime += Time.deltaTime;
                float percToComplete = elapsedTime / moveDuration;
                this.transform.position = Vector3.Lerp(startingPos, moveToPos, Mathf.SmoothStep(0, 1, percToComplete));
                Debug.Log("Object Moving");
            }
            if (this.transform.position.Equals(moveToPos))
            {
                isMoving = false;
            }
        }
    }

    public Chess(bool notEmpty)
    {
        if (!notEmpty)
            isEmpty = true;
        else
            Debug.Log("Invaild chess creation ar");
    }

    public Chess(bool colorBlack, int inputRanking)
    {
        isBlack = colorBlack;
        ranking = inputRanking;
        hasRevealed = false;
        isEmpty = false;
    }

    public Chess(bool colorBlack, int inputRanking, int inputX, int inputY, bool inputReveal, bool inputEmpty)
    {
        isBlack = colorBlack;
        ranking = inputRanking;
        x = inputX;
        y = inputY;
        hasRevealed = inputReveal;
        isEmpty = inputEmpty;
    }

    public void InitVar(bool colorBlack, int inputRanking, int inputX, int inputY)
    {
        isBlack = colorBlack;
        ranking = inputRanking;
        x = inputX;
        y = inputY;
        hasRevealed = false;
        isEmpty = false;
        startingPos = this.transform.position;
        rotateToX = transform.eulerAngles.x;
        moveToPos = this.transform.position;
        EmbbedToOject = true;
    }

    public void SetX(int newX)
    {
        x = newX;
    }

    public void SetY(int newY)
    {
        y = newY;
    }

    public void Reveal()
    {
        hasRevealed = true;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public void SetXY(int iX, int iY)
    {
        this.x = iX;
        this.y = iY;
    }

    public bool IsBlack()
    {
        return isBlack;
    }

    public bool CheckHasRevealed()
    {
        return hasRevealed;
    }

    public int GetRanking()
    {
        return ranking;
    }

    public override string ToString()
    {
        if (!isEmpty)
        {
            string color = "";
            if (isBlack)
                color = "b";
            else
                color = "r";
            return color + ranking + "r:" + hasRevealed + "(" + x + ", " + y + ")";
        }
        else
        {
            return "Empty" + "(" + x + ", " + y + ")";
        }
    }

    public void FilpOverChess()
    {
        rotateToX = transform.eulerAngles.x + 180;
    }

    public void FilpOverChess(bool b)
    {
        rotateToX = transform.eulerAngles.x + 180;
        gameObject.GetComponent<MeshCollider>().enabled = false;
    }

    public void KillChess()
    {

        startingPos = this.transform.position;
        if (isBlack)
        {
            moveToPos = new Vector3(defeatedBlackX, 0.09f, defeatedBlackZ);
            if (defeatedBlackZ <= -9.4)
            {
                defeatedBlackZ = 14f;
                defeatedBlackX += 2.6f;
            }
            else
                defeatedBlackZ -= 2.6f;
        }
        else
        {
            moveToPos = new Vector3(defeatedRedX, 0.09f, defeatedRedZ);
            if (defeatedRedZ <= -9.4)
            {
                defeatedRedZ = 14f;
                defeatedRedX -= 2.6f;
            }
            else
                defeatedRedZ -= 2.6f;
        }


        isMoving = true;
    }

    public void MoveChess(int iX, int iY)
    {
        startingPos = this.transform.position;
        moveToPos = new Vector3(3.3f - (2.6f * (float)iY), 0.09f, 12.5f - (2.65f * (float)iX));
        isMoving = true;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.Instance.IsSelectable())
        {
            if (!this.hasRevealed)
            {

                GameManager.Instance.RevealChess(this.x, this.y);
                GameManager.Instance.playerChessPressed = true;
            }
            else
            {
                if (!selected)
                {
                    Vector3 pos = transform.position;
                    GameManager.Instance.HighlightChess.transform.position = pos;
                    GameManager.Instance.HighlightChess.SetActive(true);
                    //Debug.Log("X: " + x + " Y: " + y + " Pos: " + pos);
                    selected = true;
                    GameManager.Instance.PlaceGreenClickable(pos.x, pos.z, isBlack, x, y);
                }
                else
                {
                    GameManager.Instance.HighlightChess.SetActive(false);
                    selected = false;
                    GameManager.Instance.HideHighlight();
                }
            }
        }
    }
}
