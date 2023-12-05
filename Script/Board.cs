using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Board
{

    public int BoardHeight;
    public int BoardWidth;
    public Chess[,] board;
    public List<Chess> unRevealedBlackChess;
    public List<Chess> unRevealedRedChess;
    public List<Chess> RevealedBlackChess;
    public List<Chess> RevealedRedChess;
    public IDictionary<int, int> BlackGraveyard;
    public IDictionary<int, int> RedGraveyard;
    public float thresholdForPawnPos;
    public float thresholdForHigherRankPos;
    public int RedRemainingChess;
    public int BlackRemainingChess;
    public int NumberOfPosToCompute;

    private void Start() { }
    public Board(int n) {
        BoardHeight = 4;
        BoardWidth = 8;
        thresholdForPawnPos = 45f;
        thresholdForHigherRankPos = 40f;
        RedRemainingChess = 16;
        BlackRemainingChess = 16;
        NumberOfPosToCompute = 4;
        board = new Chess[BoardHeight, BoardWidth];
        unRevealedBlackChess = new List<Chess>();
        unRevealedRedChess = new List<Chess>();
        RevealedBlackChess = new List<Chess>();
        RevealedRedChess = new List<Chess>();
        BlackGraveyard = new Dictionary<int, int>();
        RedGraveyard = new Dictionary<int, int>();
        for (int i = 0; i < 7; i++)
        {
            BlackGraveyard.Add(i, 0);
            RedGraveyard.Add(i, 0);
        }
    }

    public void SetVar(Chess[,] newBoard, List<Chess> newUnRevealBlackChess, List<Chess> newUnRevealRedChess, 
        List<Chess> newRevealBlackChess, List<Chess> newRevealRedChess, 
        IDictionary<int, int> newBlackGraveyard, IDictionary<int, int> newRedGraveyard, int newRedRemainingChess, int newBlackRemainingChess) {
        
        board = newBoard;
        unRevealedBlackChess = newUnRevealBlackChess;
        unRevealedRedChess = newUnRevealRedChess;
        RevealedBlackChess = newRevealBlackChess;
        RevealedRedChess = newRevealRedChess;
        BlackGraveyard = newBlackGraveyard;
        RedGraveyard = newRedGraveyard;
        RedRemainingChess = newRedRemainingChess;
        BlackRemainingChess = newBlackRemainingChess;
    }

    public Board GetDeepCopy() {
        Board newBoard = new Board(5);
        Chess[,] cloneBoard = new Chess[BoardHeight, BoardWidth];
        List<Chess> cloneUnRevealBlackChess = new List<Chess>();
        List<Chess> cloneUnRevealRedChess = new List<Chess>();
        List<Chess> cloneRevealBlackChess = new List<Chess>();
        List<Chess> cloneRevealRedChess = new List<Chess>();
        IDictionary<int, int> cloneBlackGraveyard = new Dictionary<int, int>();
        IDictionary<int, int> cloneRedGraveyard = new Dictionary<int, int>();
        int cloneRedRemainingChess = RedRemainingChess;
        int cloneBlackRemainingChess = BlackRemainingChess;
        for (int col = 0; col < BoardHeight; col++)
        {
            for (int row = 0; row < BoardWidth; row++)
            {
                Chess chessToCopy = board[col, row];
                bool IsBlack = chessToCopy.IsBlack();
                bool hasRevealed = chessToCopy.CheckHasRevealed();
                bool empty = chessToCopy.isEmpty;
                cloneBoard[col, row] = new Chess(IsBlack, chessToCopy.GetRanking(),
                     row, col, hasRevealed, empty);

                if (!chessToCopy.isEmpty) {
                    if (IsBlack)
                    {
                        if (!hasRevealed)
                            cloneUnRevealBlackChess.Add(cloneBoard[col, row]);
                        else
                            cloneRevealBlackChess.Add(cloneBoard[col, row]);
                    }
                    else
                    {
                        if (!hasRevealed)
                            cloneUnRevealRedChess.Add(cloneBoard[col, row]);
                        else
                            cloneRevealRedChess.Add(cloneBoard[col, row]);
                    }
                }

            }
        }

        for (int i = 0; i < 7; i++)
        {
            cloneBlackGraveyard.Add(i, BlackGraveyard[i]);
            cloneRedGraveyard.Add(i, RedGraveyard[i]);
        }
        //Debug.Log("Clone: number of UnRevealRedChess: "+ cloneUnRevealRedChess.Count + " ori: "+ unRevealedRedChess.Count+"\nClone: number of UnRevealBlackChess: " + cloneUnRevealBlackChess.Count + " ori: " + unRevealedBlackChess.Count);
        newBoard.SetVar(cloneBoard, cloneUnRevealBlackChess, cloneUnRevealRedChess, cloneRevealBlackChess,
            cloneRevealRedChess, cloneBlackGraveyard, cloneRedGraveyard, cloneRedRemainingChess, cloneBlackRemainingChess);
        return newBoard;
    }

    public void AddChess(Chess newChess) {
        //Debug.Log("Add chess: X: "+ newChess.GetX() + ", Y: "+ newChess.GetY());
        board[newChess.GetY(), newChess.GetX()] = newChess;
        if (newChess.IsBlack())
            unRevealedBlackChess.Add(newChess);
        else
            unRevealedRedChess.Add(newChess);
    }

    public Chess GetChess(int x, int y) {
        return board[y, x];
    }

    public bool CanMove(Chess attacker, Chess defender) {
        if (defender.isEmpty)
        {
            return true;
        }
        else if (!defender.CheckHasRevealed())
        {
            return false;
        }
        else 
        {
            if (attacker.IsBlack() != defender.IsBlack())
            {
                if (attacker.GetRanking() == 0 && defender.GetRanking() == 6)
                    return true;
                else if (attacker.GetRanking() == 6 && defender.GetRanking() == 0)
                    return false;
                else if (attacker.GetRanking() >= defender.GetRanking())
                    return true;
            }
        }
        return false;
    }

    public void RevealChessForClone(int x, int y, Chess chessToReveal, int indexOfChessToRemove)
    {
        
        board[y, x] = new Chess(chessToReveal.IsBlack(), chessToReveal.GetRanking(),
            x, y, true, false);

        RemoveChessFromUnrevealed(chessToReveal.GetRanking(), chessToReveal.IsBlack(), x, y);
        if (chessToReveal.IsBlack())
        {
            //Debug.Log("[Black] Rank: "+ chessToReveal.GetRanking() + "Remove at: " + indexOfChessToRemove + " unRevealedArray length: " + unRevealedBlackChess.Count);
            RevealedBlackChess.Add(board[y, x]);
        }
        else
        {
            //Debug.Log("[Red] Rank: " + chessToReveal.GetRanking() + "Remove at: " + indexOfChessToRemove + " unRevealedArray length: " + unRevealedRedChess.Count);
            RevealedRedChess.Add(board[y, x]);
        }

    }
    public void RevealChess(int x, int y) {
        Chess chessToReveal = board[y, x];
        if (chessToReveal.IsBlack())
        {
            unRevealedBlackChess.Remove(chessToReveal);
            RevealedBlackChess.Add(chessToReveal);
        }
        else
        {
            unRevealedRedChess.Remove(chessToReveal);
            RevealedRedChess.Add(chessToReveal);
        }
        chessToReveal.Reveal();
    }

    public void ChessMoveForClone(Chess targetChess, int desX, int desY)
    {
        if (!board[desY, desX].isEmpty)
        {
            if (board[desY, desX].IsBlack())
            {
                RevealedBlackChess.Remove(board[desY, desX]);
                BlackGraveyard[board[desY, desX].GetRanking()] += 1;
                BlackRemainingChess--;
                if (BlackGraveyard[board[desY, desX].GetRanking()] < 0)
                {
                    Debug.Log("Warning: Black GraveYard smaller than 0");
                }
            }
            else
            {
                RevealedRedChess.Remove(board[desY, desX]);
                RedGraveyard[board[desY, desX].GetRanking()] += 1;
                RedRemainingChess--;
                if (RedGraveyard[board[desY, desX].GetRanking()] < 0)
                {
                    Debug.Log("Warning: Red GraveYard smaller than 0");
                }
            }

        }
        //Debug.Log("Moving "+ targetChess.GetX() + ", " + targetChess.GetY());
        board[desY, desX] = new Chess(targetChess.IsBlack(), targetChess.GetRanking(),
                     desX, desY, targetChess.CheckHasRevealed(), false);
        board[targetChess.GetY(), targetChess.GetX()] = new Chess(false);
        //printboard();
    }

    public void ChessMove(Chess targetChess, int desX, int desY)
    {
        if (!board[desY, desX].isEmpty) {
            if (board[desY, desX].IsBlack())
            {
                RevealedBlackChess.Remove(board[desY, desX]);
                BlackGraveyard[board[desY, desX].GetRanking()] += 1;
                BlackRemainingChess--;
                if (BlackGraveyard[board[desY, desX].GetRanking()] < 0)
                {
                    Debug.Log("Warning: Black GraveYard smaller than 0");
                }
            }
            else {
                RevealedRedChess.Remove(board[desY, desX]);
                RedGraveyard[board[desY, desX].GetRanking()] += 1;
                RedRemainingChess--;
                if (RedGraveyard[board[desY, desX].GetRanking()] < 0)
                {
                    Debug.Log("Warning: Red GraveYard smaller than 0");
                }
            }
            
        }
        //Debug.Log("Moving "+ targetChess.GetX() + ", " + targetChess.GetY());
        board[desY, desX] = targetChess;
        board[targetChess.GetY(), targetChess.GetX()] = new Chess(false);
        targetChess.SetXY(desX, desY);
        //printboard();
    }

    public MoveablePos GetMoveablePos(int x, int y, bool isBlack) {
        MoveablePos moveablePos = new MoveablePos(false, false, false, false);
        //Debug.Log("Moveable check: X: "+x+" Y: "+y);
        if (y>0) {
            moveablePos.up = CanMove(board[y, x], board[y-1, x]);
        }
        if (y < BoardHeight -1)
        {
            moveablePos.down = CanMove(board[y, x], board[y + 1, x]);
        }
        if (x > 0)
        {
            moveablePos.left = CanMove(board[y, x], board[y, x - 1]);
        }
        if (x < BoardWidth -1)
        {
            moveablePos.right = CanMove(board[y, x], board[y, x + 1]);
        }
        return moveablePos;
    }

    public List<MoveablePosEVL> GetSortedMoveablePosArray(bool isBlack) {
        List<MoveablePosEVL> moveables = new List<MoveablePosEVL>();
        List<MoveablePosEVL> sortedMoveables;
        List<Chess> listToChesk;
        if (isBlack) {
            listToChesk = RevealedBlackChess;
        }
        else {
            listToChesk = RevealedRedChess;
        }
        foreach(Chess chessChecking in listToChesk)
        {
            int x = chessChecking.GetX();
            int y = chessChecking.GetY();
            if (y > 0 && CanMove(board[y, x], board[y - 1, x]))
            {
                moveables.Add(new MoveablePosEVL(x, y, x, y-1, (!board[y - 1, x].isEmpty) ? board[y - 1, x].GetRanking() + 1 : 0));
            }
            if (y < BoardHeight - 1 && CanMove(board[y, x], board[y + 1, x]))
            {
                moveables.Add(new MoveablePosEVL(x, y, x, y + 1, (!board[y + 1, x].isEmpty) ? board[y + 1, x].GetRanking() + 1 : 0));
            }
            if (x > 0 && CanMove(board[y, x], board[y, x - 1]))
            {
                moveables.Add(new MoveablePosEVL(x, y, x - 1, y, (!board[y, x - 1].isEmpty) ? board[y, x - 1].GetRanking() + 1 : 0));
            }
            if (x < BoardWidth - 1 && CanMove(board[y, x], board[y, x + 1]))
            {
                moveables.Add(new MoveablePosEVL(x, y, x + 1, y, (!board[y, x + 1].isEmpty) ? board[y, x + 1].GetRanking() + 1 : 0));
            }
        }
        
        sortedMoveables = moveables.OrderByDescending(o => o.score).ToList();
        return sortedMoveables;

    }

    public ChessRevealPos[] GetGoodRevealPosBlack() {
        ChessRevealPos[] GoodRevealPos = new ChessRevealPos[NumberOfPosToCompute];

        List<ChessRevealPos> IsolatedRevealPos = new List<ChessRevealPos>();
        List<ChessRevealPos> AdvantageRevealPos = new List<ChessRevealPos>();
        ChessRevealPos[] HighestRevealPos = new ChessRevealPos[NumberOfPosToCompute];
        int HighestRevealPosInserted = 0;
        float LastHighestScore = 0f;

        int testI = 0;
        for (int col = 0; col < BoardHeight; col++) {
            for (int row = 0; row < BoardWidth; row++) {
                if (board[col, row].CheckHasRevealed() || board[col, row].isEmpty) {
                    testI++;
                }
                if (!board[col, row].CheckHasRevealed()&&!board[col, row].isEmpty) {
                    bool KingNearby = false;
                    bool RevealNearby = false;
                    int highestRankNearby = 0;
                    if (col > 0)
                    {
                        Chess c = board[col - 1, row];
                        if (!c.IsBlack())
                        {
                            if (c.GetRanking() == 6)
                                KingNearby = true;
                            if (c.CheckHasRevealed())
                            {
                                RevealNearby = true;
                                if (highestRankNearby < c.GetRanking())
                                {
                                    highestRankNearby = c.GetRanking();
                                }
                            }
                        }

                    }
                    if (col < BoardHeight - 1)
                    {
                        Chess c = board[col + 1, row];
                        if (!c.IsBlack())
                        {
                            if (c.GetRanking() == 6)
                                KingNearby = true;
                            if (c.CheckHasRevealed())
                            {
                                RevealNearby = true;
                                if (highestRankNearby < c.GetRanking())
                                {
                                    highestRankNearby = c.GetRanking();
                                }
                            }
                        }
                    }
                    if (row > 0)
                    {
                        Chess c = board[col, row - 1];
                        if (!c.IsBlack())
                        {
                            if (c.GetRanking() == 6)
                                KingNearby = true;
                            if (c.CheckHasRevealed())
                            {
                                RevealNearby = true;
                                if (highestRankNearby < c.GetRanking())
                                {
                                    highestRankNearby = c.GetRanking();
                                }
                            }
                        }
                    }
                    if (row < BoardWidth - 1)
                    {
                        Chess c = board[col, row + 1];
                        if (!c.IsBlack())
                        {
                            if (c.GetRanking() == 6)
                                KingNearby = true;
                            if (c.CheckHasRevealed())
                            {
                                RevealNearby = true;
                                if (highestRankNearby < c.GetRanking())
                                {
                                    highestRankNearby = c.GetRanking();
                                }
                            }
                        }
                    }

                    if (!RevealNearby)// No revealed chess
                        IsolatedRevealPos.Add(new ChessRevealPos(row, col, 50f));
                    else if (KingNearby)
                    {
                        //check pawn
                        float PossibOfGettingPawn = ((5 - BlackGraveyard[0]) / (unRevealedBlackChess.Count + unRevealedRedChess.Count)) * 100;
                        if (PossibOfGettingPawn > thresholdForPawnPos)
                        {
                            AdvantageRevealPos.Add(new ChessRevealPos(row, col, PossibOfGettingPawn));
                        }

                        if (HighestRevealPosInserted > NumberOfPosToCompute - 1)
                        {
                            if (PossibOfGettingPawn > LastHighestScore)
                            {
                                for (int i = NumberOfPosToCompute - 1; i > -1; i--)
                                {
                                    if (PossibOfGettingPawn > HighestRevealPos[i].Score)
                                    {
                                        ChessRevealPos temp = HighestRevealPos[i];
                                        HighestRevealPos[i] = new ChessRevealPos(row, col, PossibOfGettingPawn);
                                        for (int q = i; q < NumberOfPosToCompute - 1; q++)
                                        {
                                            ChessRevealPos temp2 = HighestRevealPos[q + 1];
                                            HighestRevealPos[q + 1] = temp;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else {
                            HighestRevealPos[HighestRevealPosInserted] = new ChessRevealPos(row, col, PossibOfGettingPawn);
                            HighestRevealPosInserted++;
                            LastHighestScore = PossibOfGettingPawn;
                        }
                    }
                    else if (RevealNearby)
                    {
                        //check chess with higher rank than highestRankNearby
                        float PossibOfGettingHigher = (GetNumOfChessHigherRank(highestRankNearby, true) / (unRevealedBlackChess.Count + unRevealedRedChess.Count)) * 100;
                        if (PossibOfGettingHigher > thresholdForHigherRankPos)
                        {
                            AdvantageRevealPos.Add(new ChessRevealPos(row, col, PossibOfGettingHigher));
                        }

                        if (HighestRevealPosInserted > NumberOfPosToCompute-1)
                        {
                            if (PossibOfGettingHigher > LastHighestScore)
                            {
                                for (int i = NumberOfPosToCompute - 1; i > -1; i--)
                                {
                                    if (PossibOfGettingHigher > HighestRevealPos[i].Score)
                                    {
                                        ChessRevealPos temp = HighestRevealPos[i];
                                        HighestRevealPos[i] = new ChessRevealPos(row, col, PossibOfGettingHigher);
                                        for (int q = i; q < NumberOfPosToCompute - 1; q++)
                                        {
                                            ChessRevealPos temp2 = HighestRevealPos[q + 1];
                                            HighestRevealPos[q + 1] = temp;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            HighestRevealPos[HighestRevealPosInserted] = new ChessRevealPos(row, col, PossibOfGettingHigher);
                            HighestRevealPosInserted++;
                            LastHighestScore = PossibOfGettingHigher;
                        }
                    }
                }
                
            }
        }
        //Debug.Log("Empty number: " + testI);
        int nextHighestRevealIndex = 0;
        for (int i = 0; i < GoodRevealPos.Length; i++)
        {
            if (IsolatedRevealPos.Count != 0)
            {
                int random = UnityEngine.Random.Range(0, IsolatedRevealPos.Count);
                GoodRevealPos[i] = IsolatedRevealPos[random];
                IsolatedRevealPos.RemoveAt(random);
            }
            else if (AdvantageRevealPos.Count != 0)
            {
                int random = UnityEngine.Random.Range(0, AdvantageRevealPos.Count);
                GoodRevealPos[i] = AdvantageRevealPos[random];
                AdvantageRevealPos.RemoveAt(random);
            }
            if (GoodRevealPos[i] == null)
            {
                GoodRevealPos[i] = HighestRevealPos[nextHighestRevealIndex];
                nextHighestRevealIndex++;
            }
        }
        

        return GoodRevealPos;
    }

    public ChessRevealPos[] GetGoodRevealPosRed()
    {
        ChessRevealPos[] GoodRevealPos = new ChessRevealPos[NumberOfPosToCompute];

        List<ChessRevealPos> IsolatedRevealPos = new List<ChessRevealPos>();
        List<ChessRevealPos> AdvantageRevealPos = new List<ChessRevealPos>();
        ChessRevealPos[] HighestRevealPos = new ChessRevealPos[NumberOfPosToCompute];
        int HighestRevealPosInserted = 0;
        float LastHighestScore = 0f;

        int testI = 0;
        for (int col = 0; col < BoardHeight; col++)
        {
            for (int row = 0; row < BoardWidth; row++)
            {
                if (board[col, row].CheckHasRevealed() || board[col, row].isEmpty)
                {
                    testI++;
                }
                if (!board[col, row].CheckHasRevealed() && !board[col, row].isEmpty) {
                    bool KingNearby = false;
                    bool RevealNearby = false;
                    int highestRankNearby = 0;
                    if (col > 0)
                    {
                        Chess c = board[col - 1, row];
                        if (c.IsBlack())
                        {
                            if (c.GetRanking() == 6)
                                KingNearby = true;
                            if (c.CheckHasRevealed())
                            {
                                RevealNearby = true;
                                if (highestRankNearby < c.GetRanking())
                                {
                                    highestRankNearby = c.GetRanking();
                                }
                            }
                        }

                    }
                    if (col < BoardHeight - 1)
                    {
                        Chess c = board[col + 1, row];
                        if (!c.IsBlack())
                        {
                            if (c.GetRanking() == 6)
                                KingNearby = true;
                            if (c.CheckHasRevealed())
                            {
                                RevealNearby = true;
                                if (highestRankNearby < c.GetRanking())
                                {
                                    highestRankNearby = c.GetRanking();
                                }
                            }
                        }
                    }
                    if (row > 0)
                    {
                        Chess c = board[col, row - 1];
                        if (!c.IsBlack())
                        {
                            if (c.GetRanking() == 6)
                                KingNearby = true;
                            if (c.CheckHasRevealed())
                            {
                                RevealNearby = true;
                                if (highestRankNearby < c.GetRanking())
                                {
                                    highestRankNearby = c.GetRanking();
                                }
                            }
                        }
                    }
                    if (row < BoardWidth - 1)
                    {
                        Chess c = board[col, row + 1];
                        if (!c.IsBlack())
                        {
                            if (c.GetRanking() == 6)
                                KingNearby = true;
                            if (c.CheckHasRevealed())
                            {
                                RevealNearby = true;
                                if (highestRankNearby < c.GetRanking())
                                {
                                    highestRankNearby = c.GetRanking();
                                }
                            }
                        }
                    }

                    if (!RevealNearby)// No revealed chess
                        IsolatedRevealPos.Add(new ChessRevealPos(row, col, 50));
                    else if (KingNearby)
                    {
                        float PossibOfGettingPawn = (GetNumOfHiddenPawn(false) / (unRevealedBlackChess.Count + unRevealedRedChess.Count)) * 100;
                        if (PossibOfGettingPawn > thresholdForPawnPos)
                        {
                            AdvantageRevealPos.Add(new ChessRevealPos(row, col, PossibOfGettingPawn));
                        }

                        if (HighestRevealPosInserted > NumberOfPosToCompute - 1)
                        {
                            if (PossibOfGettingPawn > LastHighestScore)
                            {
                                for (int i = NumberOfPosToCompute - 1; i > -1; i--)
                                {
                                    if (PossibOfGettingPawn > HighestRevealPos[i].Score)
                                    {
                                        ChessRevealPos temp = HighestRevealPos[i];
                                        HighestRevealPos[i] = new ChessRevealPos(row, col, PossibOfGettingPawn);
                                        for (int q = i; q < NumberOfPosToCompute - 1; q++)
                                        {
                                            ChessRevealPos temp2 = HighestRevealPos[q + 1];
                                            HighestRevealPos[q + 1] = temp;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            HighestRevealPos[HighestRevealPosInserted] = new ChessRevealPos(row, col, PossibOfGettingPawn);
                            HighestRevealPosInserted++;
                            LastHighestScore = PossibOfGettingPawn;
                        }
                    }
                    else if (RevealNearby)
                    {
                        float PossibOfGettingHigher = (GetNumOfChessHigherRank(highestRankNearby, false) / (unRevealedBlackChess.Count + unRevealedRedChess.Count)) * 100;
                        if (PossibOfGettingHigher > thresholdForHigherRankPos)
                        {
                            AdvantageRevealPos.Add(new ChessRevealPos(row, col, PossibOfGettingHigher));
                        }

                        if (HighestRevealPosInserted > NumberOfPosToCompute - 1)
                        {
                            if (PossibOfGettingHigher > LastHighestScore)
                            {
                                for (int i = NumberOfPosToCompute - 1; i > -1; i--)
                                {
                                    if (PossibOfGettingHigher > HighestRevealPos[i].Score)
                                    {
                                        ChessRevealPos temp = HighestRevealPos[i];
                                        HighestRevealPos[i] = new ChessRevealPos(row, col, PossibOfGettingHigher);
                                        for (int q = i; q < NumberOfPosToCompute - 1; q++)
                                        {
                                            ChessRevealPos temp2 = HighestRevealPos[q + 1];
                                            HighestRevealPos[q + 1] = temp;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            HighestRevealPos[HighestRevealPosInserted] = new ChessRevealPos(row, col, PossibOfGettingHigher);
                            HighestRevealPosInserted++;
                            LastHighestScore = PossibOfGettingHigher;
                        }
                    }
                }
            }
        }
        //Debug.Log("Empty number: "+ testI);
        int nextHighestRevealIndex = 0;
        for (int i=0;i< GoodRevealPos.Length;i++) {
            if (IsolatedRevealPos.Count != 0)
            {
                int random = UnityEngine.Random.Range(0, IsolatedRevealPos.Count);
                GoodRevealPos[i] = IsolatedRevealPos[random];
                IsolatedRevealPos.RemoveAt(random);
            }
            else if (AdvantageRevealPos.Count != 0)
            {
                int random = UnityEngine.Random.Range(0, AdvantageRevealPos.Count);
                GoodRevealPos[i] = AdvantageRevealPos[random];
                AdvantageRevealPos.RemoveAt(random);
            }


            if (GoodRevealPos[i] == null)
            {
                GoodRevealPos[i] = HighestRevealPos[nextHighestRevealIndex];
                nextHighestRevealIndex++;
            }
        }

        
        return GoodRevealPos;
    }

    public void RemoveChessFromUnrevealed(int rank, bool isBlack, int revealx, int revealy) {
        List<Chess> listToChange;
        //bool noChessRemoved = true;
        int x = -1;
        int y = -1;
        if (isBlack)
        {
            listToChange = unRevealedBlackChess;
        }
        else 
        {
            listToChange = unRevealedRedChess;
        }
        for (int i = 0;i< listToChange.Count;i++) {
            if (listToChange[i].GetRanking() == rank) {
                //noChessRemoved = false;
                x = listToChange[i].GetX();
                y = listToChange[i].GetY();
                listToChange.RemoveAt(i);
                break;
            }
        }
        //if(noChessRemoved)
        //    Debug.Log("No chess to be removed: "+noChessRemoved);
        if (x==-1) {
            Debug.Log("No chess to be removed!!! ");
        }
        foreach(Chess c in unRevealedBlackChess)
        {
            if (c.GetX() == revealx && c.GetY()== revealy)
            {
                board[y, x] = c;
                c.SetXY(x,y);
                break;
            }
        }
        foreach (Chess c in unRevealedRedChess)
        {
            if (c.GetX() == revealx && c.GetY() == revealy)
            {
                board[y, x] = c;
                c.SetXY(x, y);
                break;
            }
        }
    }
    public float GetNumOfChessOnBoard(int rank, bool isBlack)
    {
        float numOfChessDefeated;
        if (isBlack)
            numOfChessDefeated = BlackGraveyard[rank];
        else
            numOfChessDefeated = RedGraveyard[rank];

        if (rank == 0)
        {
            return 5f - numOfChessDefeated;
        }
        else if (rank < 6)
        {
            return 2f - numOfChessDefeated;
        }
        else
        {
            return 1f - numOfChessDefeated;
        }
    }

    public float GetPosibilityOfReveal(bool isBlack, int rank) {
        float totalUnrevealChessNumber = unRevealedBlackChess.Count + unRevealedRedChess.Count;
        float numberOfChessUnrevealed = 0;
        List<Chess> listToLoop;
        if (isBlack)
        {
            listToLoop = unRevealedBlackChess;
        }
        else {
            listToLoop = unRevealedRedChess;
        }
        foreach (Chess c in listToLoop) {
            if (!c.isEmpty&&c.GetRanking()==rank) {
                numberOfChessUnrevealed++;
            }
        }
        return numberOfChessUnrevealed / totalUnrevealChessNumber;
    }

    public float GetNumOfHiddenPawn(bool isBlack) {
        float numOfHiddenPawn = 0;
        if (isBlack)
        {
            foreach (Chess c in unRevealedBlackChess)
            {
                if (c.GetRanking()==0)
                    numOfHiddenPawn++;
            }
        }
        else
        {
            foreach (Chess c in unRevealedRedChess)
            {
                if (c.GetRanking()==0)
                    numOfHiddenPawn++;
            }
        }
        return numOfHiddenPawn;
    }

    public float GetNumOfChessHigherRank(int rank, bool isBlack) {
        float numOfHiddenChess = 0;

        if (isBlack)
        {
            foreach (Chess c in unRevealedBlackChess)
            {
                if (c.GetRanking() > rank)
                    numOfHiddenChess++;
            }
        }
        else
        {
            foreach (Chess c in unRevealedRedChess)
            {
                if (c.GetRanking() > rank)
                    numOfHiddenChess++;
            }
        }
        return numOfHiddenChess;

    }

    public float GetMinMaxVal() {// black is max, red is min
        float blackTotal= 0f;
        float redTotal = 0f;

        float multipVal = 10f;
        for(int i=0; i<7; i++){

            blackTotal += GetNumOfChessOnBoard(i, true) * multipVal;
            redTotal += GetNumOfChessOnBoard(i, false) * multipVal;

            multipVal *= 2;//higher the rank, more valuable the chess
        }
        return blackTotal - redTotal;// black > 0 , red < 0
    }

    //public List<MoveableOption> GetAllPossibleMove(bool isBlack) {
    //    List<Chess> revealedChess;
    //    if (isBlack)
    //    {
    //        revealedChess = RevealedBlackChess;
    //    }
    //    else 
    //    {
    //        revealedChess = RevealedRedChess;
    //    }
    //    List<MoveableOption> allPosMoves = new List<MoveableOption>();
    //    foreach (Chess c in revealedChess) {
    //        MoveablePos cMoveables = GetMoveablePos(c.GetX(), c.GetY(), c.IsBlack());
    //        if (cMoveables.up|| cMoveables.down || cMoveables.left|| cMoveables.right) { //check if it can move
    //            allPosMoves.Add(new MoveableOption(c, cMoveables));
    //        }
    //    }
    //    return allPosMoves;
    //}

    public void printboard() {
        string prtmsg = "";
        for (int i =0; i< BoardHeight; i++) {
            for (int q=0;q<BoardWidth;q++) {
                prtmsg += board[i, q].ToString() + "|";

            }
            prtmsg += "\n";
        }
        Debug.Log(prtmsg);
    }

    public string boardToString()
    {
        string prtmsg = "";
        for (int i = 0; i < BoardHeight; i++)
        {
            for (int q = 0; q < BoardWidth; q++)
            {
                prtmsg += board[i, q].ToString() + "|";

            }
            prtmsg += "\n";
        }
        prtmsg+= "\n unRevealedBlackChess: " + unRevealedBlackChess.Count + "\n unRevealedRedkChess: " + unRevealedRedChess.Count
                + "\n RevealedBlackkChess: " + RevealedBlackChess.Count + "\n RevealedRedkChess: " + RevealedRedChess.Count
                + " \n RedRemainingChess: " + RedRemainingChess + " \n BlackRemainingChess: " + BlackRemainingChess;

        return prtmsg;
    }
}
public class ChessRevealPos
{
    public int x;
    public int y;
    public float Score;

    public ChessRevealPos(int xPos, int yPos, float inScore)
    {
        this.x = xPos;
        this.y = yPos;
        this.Score = inScore;
    }

    public string ToString() {
        return "x: " + x + " y: " + y + " Score: " + Score;
    }
}

public struct MoveablePos {
    public bool up, down, left, right;

    public MoveablePos(bool inUp, bool inDown, bool inLeft, bool inRight) {
        this.up = inUp;
        this.down = inDown;
        this.left = inLeft;
        this.right = inRight;
    }
    public string ToString() {
        return "up: " + up + " down: " + down + " left: " + left + " right: " + right;
    }
}

public class MoveablePosEVL{
    public int chessX, chessY, destinationX, destinationY, score;

    public MoveablePosEVL(int inChessX, int inChessY, int inDestinationX, int inDestinationY, int inScore) {
        chessX = inChessX;
        chessY = inChessY;
        destinationX = inDestinationX;
        destinationY = inDestinationY;
        score = inScore;
    }
}
