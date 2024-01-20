using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public ChessColor PlayingColor;

    public bool EndTurn;

    public int searchTreeDepth = 3;

    public AIMove bestMove;

    public float penaltyRatio = 1.0f;

    public float killBonus = 300f;

    public float negativeInfinity = -999999;

    public float positiveInfinity = 999999;

    public AIPlayer()
    {
        EndTurn = false;
    }

    public void SetChessColor(ChessColor color)
    {
        PlayingColor = color;
    }

    public void ResetEndTurnVar()
    {
        EndTurn = false;
    }

    public float CheckLose(Board board)
    {
        if (board.BlackRemainingChess == 0)
        {
            return -9999;//black lose
        }
        else if (board.RedRemainingChess == 0)
        {
            return 9999;//red lose
        }
        else
            return 0;
    }

    public float GetPenalty(float penalty)
    {
        if (penalty > 40f)
        {
            return 1;
        }
        else
        {
            return penalty;
        }
    }

    public float MinPlay(Board board, int depth, float alpha, float beta)
    {
        //Debug.Log("Min Depth: " + depth + " Alpha: " + alpha + " beta: " + beta);
        bool pruned = false;
        float returnUtility = 0;
        float utilityVal;
        float checkLoseVal = CheckLose(board);
        if (checkLoseVal == -9999 || checkLoseVal == 9999)
        {
            //Debug.Log("Winner found");
            return checkLoseVal;
        }
        else if (depth >= searchTreeDepth)
        {
            //Debug.Log("Reached button: evl: "+board.GetMinMaxVal());
            return board.GetMinMaxVal();
        }
        else
        {
            float minUtility = positiveInfinity;

            //Check for moving
            List<MoveablePosEVL> sortedMoveable = board.GetSortedMoveablePosArray(false);
            foreach (MoveablePosEVL mp in sortedMoveable)
            {
                if (pruned)
                    break;
                Board clone = board.GetDeepCopy();

                clone.ChessMoveForClone(board.board[mp.chessY, mp.chessX], mp.destinationX, mp.destinationY);

                //Debug.Log("MaxPlay(clone, " + (depth + 1) + ", " + alpha + ", " + beta);
                returnUtility = MaxPlay(clone, depth + 1, alpha, beta) + ((!board.board[mp.destinationY, mp.destinationX].isEmpty) ? (killBonus * -1) : 0f);
                //Debug.Log("Returned From calling max");
                //Debug.Log("Check " + returnUtility + "<" + minUtility);
                if (returnUtility < minUtility)
                {
                    minUtility = returnUtility;
                }
                if (returnUtility < beta)
                {
                    beta = returnUtility;
                }
                if (beta <= alpha)
                {
                    //Debug.Log("Pruned !!: " + beta + "<=" + alpha);
                    pruned = true;
                }
            }

            ChessRevealPos[] possibleReveals = board.GetGoodRevealPosRed();
            //Check for revealing
            //Debug.Log("First "+ possibleReveals[0].ToString());
            foreach (ChessRevealPos p in possibleReveals)
            {
                if (pruned)
                    break;
                //Debug.Log("p!=null : "+p!=null);
                if (p != null)
                {
                    int[] searchedRank = { -1, -1, -1, -1, -1, -1, -1 };
                    for (int i = 0; i < board.unRevealedRedChess.Count; i++)
                    {

                        if (!searchedRank.Contains(board.unRevealedRedChess[i].GetRanking()))
                        {
                            for (int g = 0; g < searchedRank.Length; g++)
                            {
                                if (searchedRank[g] == -1)
                                {
                                    searchedRank[g] = board.unRevealedRedChess[i].GetRanking();

                                    Board clone = board.GetDeepCopy();
                                    //Debug.Log("B4 clone: count: " + board.unRevealedBlackChess.Count + " Remove in: " + i);
                                    float penalty = board.GetPosibilityOfReveal(board.unRevealedRedChess[i].IsBlack(), board.unRevealedRedChess[i].GetRanking());
                                    clone.RevealChessForClone(p.x, p.y, board.unRevealedRedChess[i], i);
                                    //Debug.Log(penalty);
                                    //Debug.Log("MaxPlay(clone, " + (depth + 1) + ", " + alpha + ", " + beta);
                                    returnUtility = MaxPlay(clone, depth + 1, alpha, beta) * ((penalty < 0.4f) ? penalty : 1);
                                    //Debug.Log("Returned From calling max");
                                    //Debug.Log("Check " + returnUtility + "<" + minUtility);
                                    if (returnUtility < minUtility)
                                    {

                                        minUtility = returnUtility;
                                    }
                                    if (returnUtility < beta)
                                    {
                                        beta = returnUtility;
                                    }
                                    if (beta <= alpha)
                                    {
                                        //Debug.Log("Pruned !!: " + beta + "<=" + alpha);
                                        pruned = true;
                                    }
                                    break;
                                }
                            }


                        }
                    }
                }
            }
            // Debug.Log("Did nth !!");
            return minUtility;


        }
    }

    public float MaxPlay(Board board, int depth, float alpha, float beta)//black
    {
        bool pruned = false;
        float returnUtility = 0;
        //Debug.Log("Max Depth: " + depth+" Alpha: "+alpha+" beta: "+beta);
        float checkLoseVal = CheckLose(board);
        if (checkLoseVal == -9999 || checkLoseVal == 9999)
        {
            //Debug.Log("Winner found");
            return checkLoseVal;
        }
        else if (depth >= searchTreeDepth)
        {
            //Debug.Log("Reached button: evl: " + board.GetMinMaxVal());
            return board.GetMinMaxVal();
        }
        else
        {
            float maxUtility = negativeInfinity;
            //Check for moving
            List<MoveablePosEVL> sortedMoveable = board.GetSortedMoveablePosArray(true);
            foreach (MoveablePosEVL mp in sortedMoveable)
            {
                if (pruned)
                    break;
                Board clone = board.GetDeepCopy();
                clone.ChessMoveForClone(board.board[mp.chessY, mp.chessX], mp.destinationX, mp.destinationY);
                //Debug.Log("MinPlay(clone, "+ (depth + 1) + ", "+ alpha + ", " + beta);
                returnUtility = MinPlay(clone, depth + 1, alpha, beta) + ((!board.board[mp.destinationY, mp.destinationX].isEmpty) ? killBonus : 0f);
                //Debug.Log("Check " + returnUtility + ">" + maxUtility);
                if (returnUtility > maxUtility)
                {

                    maxUtility = returnUtility;
                }
                if (returnUtility > alpha)
                {
                    alpha = returnUtility;
                }
                if (beta <= alpha)
                {
                    //Debug.Log("Pruned !!: "+beta+"<="+alpha);
                    pruned = true;
                }
            }

            ChessRevealPos[] possibleReveals = board.GetGoodRevealPosBlack();
            //Debug.Log("First " + possibleReveals[0].ToString());
            //Check for revealing
            foreach (ChessRevealPos p in possibleReveals)
            {
                if (pruned)
                    break;
                if (p != null)
                {
                    int[] searchedRank = { -1, -1, -1, -1, -1, -1, -1 };
                    for (int i = 0; i < board.unRevealedBlackChess.Count; i++)
                    {
                        if (!searchedRank.Contains(board.unRevealedBlackChess[i].GetRanking()))
                        {
                            for (int g = 0; g < searchedRank.Length; g++)
                            {
                                if (searchedRank[g] == -1)
                                {
                                    searchedRank[g] = board.unRevealedBlackChess[i].GetRanking();
                                    float penalty = board.GetPosibilityOfReveal(board.unRevealedBlackChess[i].IsBlack(), board.unRevealedBlackChess[i].GetRanking());
                                    Board clone = board.GetDeepCopy();
                                    //Debug.Log(penalty);
                                    //Debug.Log("B4 clone: count: "+board.unRevealedBlackChess.Count+" Remove in: "+i);
                                    clone.RevealChessForClone(p.x, p.y, board.unRevealedBlackChess[i], i);
                                    //Debug.Log("MinPlay(clone, " + (depth + 1) + ", " + alpha + ", " + beta);
                                    returnUtility = MinPlay(clone, depth + 1, alpha, beta) * ((penalty < 0.4f) ? penalty : 1);
                                    //Debug.Log("Returned From calling min");
                                    //Debug.Log("Check " + returnUtility + ">" + maxUtility+" = "+ (returnUtility > maxUtility));
                                    if (returnUtility > maxUtility)
                                    {
                                        maxUtility = returnUtility;
                                    }
                                    if (returnUtility > alpha)
                                    {
                                        alpha = returnUtility;
                                    }
                                    if (beta <= alpha)
                                    {
                                        //Debug.Log("Pruned !!: " + beta + "<=" + alpha);
                                        pruned = true;
                                    }
                                    break;
                                }
                            }


                        }
                    }
                }
            }

            //Debug.Log("Did nth !!");
            return maxUtility;
        }
    }

    private AIMove GetBestMove(Board board)
    {
        AIMove bestMove = new AIMove();
        float alpha = negativeInfinity;
        float beta = positiveInfinity;
        if (PlayingColor == ChessColor.Black)
        {//black 
            Debug.Log("Playing as Black");
            float maxUtility = negativeInfinity;
            List<MoveablePosEVL> sortedMoveable = board.GetSortedMoveablePosArray(true);
            foreach (MoveablePosEVL mp in sortedMoveable)
            {

                Board clone = board.GetDeepCopy();
                clone.ChessMoveForClone(board.board[mp.chessY, mp.chessX], mp.destinationX, mp.destinationY);
                float returnUtility = MinPlay(clone, 1, alpha, beta) + ((!board.board[mp.destinationY, mp.destinationX].isEmpty) ? killBonus : 0f);
                Debug.Log("Chess(x: " + mp.chessX + ", y: " + mp.chessY + ") moving to x: " + mp.destinationX + ", y: " + mp.destinationY + " Evl: " + returnUtility);
                Debug.Log(clone.boardToString());
                //if (bestMove.move == MoveType.Null) {
                //    bestMove = new AIMove(mp.chessX, mp.chessY, mp.destinationX, mp.destinationY);
                //}
                if (returnUtility > maxUtility)
                {
                    maxUtility = returnUtility;
                    bestMove = new AIMove(mp.chessX, mp.chessY, mp.destinationX, mp.destinationY);
                }
                if (returnUtility > alpha)
                {
                    alpha = returnUtility;
                }
                if (beta <= alpha)
                {
                    return bestMove;
                }
            }

            ChessRevealPos[] possibleReveals = board.GetGoodRevealPosBlack();
            //Check for revealing
            foreach (ChessRevealPos p in possibleReveals)
            {

                if (p != null)
                {
                    Debug.Log("Checking reveal pos: " + p.ToString());
                    int[] searchedRank = { -1, -1, -1, -1, -1, -1, -1 };
                    for (int i = 0; i < board.unRevealedBlackChess.Count; i++)
                    {
                        if (!searchedRank.Contains(board.unRevealedBlackChess[i].GetRanking()))
                        {
                            for (int g = 0; g < searchedRank.Length; g++)
                            {
                                if (searchedRank[g] == -1)
                                {
                                    searchedRank[g] = board.unRevealedBlackChess[i].GetRanking();

                                    float penalty = board.GetPosibilityOfReveal(board.unRevealedBlackChess[i].IsBlack(), board.unRevealedBlackChess[i].GetRanking());
                                    Debug.Log("penalty: " + penalty);
                                    Board clone = board.GetDeepCopy();
                                    clone.RevealChessForClone(p.x, p.y, board.unRevealedBlackChess[i], i);
                                    float returnUtility = MinPlay(clone, 1, alpha, beta) * ((penalty < 0.4f) ? penalty : 1);
                                    Debug.Log("Reveal Chess(x: " + p.x + ", y: " + p.y + ")  Evl: " + returnUtility);
                                    //if (bestMove.move == MoveType.Null)
                                    //{
                                    //    bestMove = new AIMove(p.x, p.y);
                                    //}
                                    if (returnUtility > maxUtility)
                                    {
                                        maxUtility = returnUtility;
                                        bestMove = new AIMove(p.x, p.y);
                                    }
                                    else if (bestMove.move == MoveType.Move && maxUtility < (board.GetMinMaxVal() + 70))
                                    {
                                        Debug.Log("Move replace to reveal");
                                        maxUtility = returnUtility;
                                        bestMove = new AIMove(p.x, p.y);
                                    }
                                    if (returnUtility > alpha)
                                    {
                                        alpha = returnUtility;
                                    }

                                    if (beta <= alpha)
                                    {
                                        return bestMove;
                                    }
                                    break;
                                }

                            }



                        }
                    }

                }
                else
                {
                    //Debug.Log("No chess reveal pos");
                }
            }

        }
        else
        {
            //red
            Debug.Log("Playing as Red");
            float minUtility = positiveInfinity;
            List<MoveablePosEVL> sortedMoveable = board.GetSortedMoveablePosArray(false);
            foreach (MoveablePosEVL mp in sortedMoveable)
            {

                Board clone = board.GetDeepCopy();
                clone.ChessMoveForClone(board.board[mp.chessY, mp.chessX], mp.destinationX, mp.destinationY);
                float returnUtility = MaxPlay(clone, 1, alpha, beta) + ((!board.board[mp.destinationY, mp.destinationX].isEmpty) ? (killBonus * -1) : 0f);
                Debug.Log("Chess(x: " + mp.chessX + ", y: " + mp.chessY + ") moving to x: " + mp.destinationX + ", y: " + mp.destinationY + " Evl: " + returnUtility);
                Debug.Log(board.boardToString());
                Debug.Log(clone.boardToString());
                if (returnUtility < minUtility)
                {
                    minUtility = returnUtility;
                    bestMove = new AIMove(mp.chessX, mp.chessY, mp.destinationX, mp.destinationY);
                }
                if (returnUtility < beta)
                {
                    beta = returnUtility;
                }
                if (beta <= alpha)
                {
                    return bestMove;
                }
            }

            ChessRevealPos[] possibleReveals = board.GetGoodRevealPosRed();
            //Check for revealing
            foreach (ChessRevealPos p in possibleReveals)
            {

                if (p != null)
                {
                    Debug.Log("Checking reveal pos: " + p.ToString());
                    int[] searchedRank = { -1, -1, -1, -1, -1, -1, -1 };
                    for (int i = 0; i < board.unRevealedRedChess.Count; i++)
                    {
                        if (board.unRevealedRedChess[i] != null && !searchedRank.Contains(board.unRevealedRedChess[i].GetRanking()))
                        {
                            for (int g = 0; g < searchedRank.Length; g++)
                            {
                                if (searchedRank[g] == -1)
                                {
                                    searchedRank[g] = board.unRevealedRedChess[i].GetRanking();
                                    float penalty = board.GetPosibilityOfReveal(board.unRevealedRedChess[i].IsBlack(), board.unRevealedRedChess[i].GetRanking());
                                    Debug.Log(penalty);
                                    Board clone = board.GetDeepCopy();
                                    clone.RevealChessForClone(p.x, p.y, board.unRevealedRedChess[i], i);
                                    float returnUtility = MaxPlay(clone, 1, alpha, beta) * ((penalty < 0.4f) ? penalty : 1);
                                    Debug.Log("Reveal Chess(x: " + p.x + ", y: " + p.y + ")  Evl: " + returnUtility);
                                    if (returnUtility < minUtility)
                                    {
                                        minUtility = returnUtility;
                                        bestMove = new AIMove(p.x, p.y);
                                    }
                                    else if (bestMove.move == MoveType.Move && minUtility > (board.GetMinMaxVal() - 70))
                                    {
                                        Debug.Log("Move replace to reveal");
                                        minUtility = returnUtility;
                                        bestMove = new AIMove(p.x, p.y);
                                    }
                                    if (returnUtility < beta)
                                    {
                                        beta = returnUtility;
                                    }
                                    if (beta <= alpha)
                                    {
                                        return bestMove;
                                    }
                                    break;
                                }
                            }



                        }
                    }
                }
                else
                {
                    Debug.Log("No chess reveal pos");
                }
            }
            //Check for moving


        }
        return bestMove;
    }

    public IEnumerator Play(Board curBoard)
    {

        Debug.Log("AI playing");
        //ChessRevealPos[] GoodRevealPos;
        //if (PlayingColor == ChessColor.Black)
        //{
        //    GoodRevealPos = curBoard.GetGoodRevealPosBlack();
        //}
        //else {
        //    GoodRevealPos = curBoard.GetGoodRevealPosRed();
        //}

        //List<MoveableOption> allPosMoves = curBoard.GetAllPossibleMove((PlayingColor==ChessColor.Black) ? true : false);


        //foreach (ChessRevealPos pos in GoodRevealPos) {
        //    if(pos!=null)
        //       Debug.Log(pos.ToString());

        //}
        AIMove bestMove = GetBestMove(curBoard);
        bestMove.printInfo();
        if (bestMove.move == MoveType.Reveal)
            GameManager.Instance.RevealChess(bestMove.x, bestMove.y);
        else if (bestMove.move == MoveType.Null)
        {
            Debug.LogError("bestMove Not assigned");
        }
        else
        {
            Debug.Log("AI moving chess " + curBoard.board[bestMove.y, bestMove.x].ToString() + " to x: " + bestMove.desX + " y: " + bestMove.desY);
            GameManager.Instance.MoveChess(bestMove.desX, bestMove.desY, curBoard.board[bestMove.y, bestMove.x]);

        }


        EndTurn = true;
        yield return new WaitUntil(() => EndTurn == true);
    }

    public IEnumerator PlayFirstTurn(Board curBoard)
    {

        Debug.Log("AI playing");
        GameManager.Instance.RevealChess(Random.Range(0, 7), Random.Range(0, 4));

        EndTurn = true;
        yield return new WaitUntil(() => EndTurn == true);
    }
}

public enum MoveType
{
    Null, Reveal, Move
}

public struct AIMove
{
    public MoveType move;

    public int x, y;

    public int desX, desY;

    public AIMove(int iX, int iY)
    {
        move = MoveType.Reveal;
        x = iX;
        y = iY;
        desX = -1;
        desY = -1;
    }

    public AIMove(int iX, int iY, int iDesX, int iDesY)
    {
        move = MoveType.Move;
        x = iX;
        y = iY;
        desX = iDesX;
        desY = iDesY;
        Debug.Log("Creating new AIMove: (" + x + " , " + y + ") to (" + desX + ", " + desY + ")");
        if (iX == iDesX && iY == iDesY)
        {
            Debug.Log("Error same position");
        }
    }

    public void printInfo()
    {
        Debug.Log("AIMove: type: " + move + " from (" + x + " , " + y + ") to (" + desX + ", " + desY + ")");
    }
}
