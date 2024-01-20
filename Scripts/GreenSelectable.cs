using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSelectable : MonoBehaviour
{
    public int x, y;
    public Chess movingChess;

    public void SetVar(int inputX, int inputY, Chess inputMovingChess) {
        this.x = inputX;
        this.y = inputY;
        this.movingChess = inputMovingChess;
    }

    public void ResetVar() {
        x = 0;
        y = 0;
        movingChess = null;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameManager.Instance.MoveChess(x, y, this.movingChess);
            ResetVar();
        }
    }
}
