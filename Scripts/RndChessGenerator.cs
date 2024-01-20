using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RndChessGenerator
{
    private int[] RedChessPieceNum;
    private int[] BlackChessPieceNum;
    private List<int> RemainingRanksOfRed;//0=pawn, 1=bishop, 2=guard, 3=cannon, 4=knight, 5=chariot, 6=king
    private List<int> RemainingRanksOfBlack;

    public RndChessGenerator() {
        RedChessPieceNum = new int[7];
        BlackChessPieceNum = new int[7];
        RemainingRanksOfRed = new List<int>();
        RemainingRanksOfBlack = new List<int>();
        InitPieceNumArray();
    }

    public Chess GetRandomChess() {
        int ColorRnd;
        //Debug.Log("Red count: "+ RemainingRanksOfRed.Count);
        //Debug.Log("Black count: " + RemainingRanksOfBlack.Count);
        if (RemainingRanksOfBlack.Count > 0 && RemainingRanksOfRed.Count > 0)
        {
            ColorRnd = Random.Range(0, 2);
            //Debug.Log("ColorRnd: " + ColorRnd);
        }
        else if (RemainingRanksOfBlack.Count == 0)
        {
            ColorRnd = 1;
        }
        else {
            ColorRnd = 0;
        }
        

        if (ColorRnd == 0)
        {//black
            int rndIndex = Random.Range(0, RemainingRanksOfBlack.Count);

            //Debug.Log("RndRankArrayIndex: " + rndIndex);

            //string outprint = "RemainingRanksOfBlack: ";
            //foreach (int i in RemainingRanksOfBlack)
            //{
            //    outprint += i + ", ";
            //}
            //Debug.Log(outprint);

            //outprint = "BlackChessPieceNum: ";
            //foreach (int i in BlackChessPieceNum)
            //{
            //    outprint += i + ", ";
            //}
            //Debug.Log(outprint);

            int RankRnd = RemainingRanksOfBlack[rndIndex];
            //Debug.Log("RankRnd: " + RankRnd);
            BlackChessPieceNum[RankRnd] -= 1;
            if (BlackChessPieceNum[RankRnd] == 0) {
                RemainingRanksOfBlack.Remove(RankRnd);
            }
            return new Chess(true, RankRnd);//0=pawn, 1=bishop, 2=guard, 3=cannon, 4=knight, 5=chariot, 6=king
        }
        else {//red
            int rndIndex = Random.Range(0, RemainingRanksOfRed.Count);
            //Debug.Log("RndRankArrayIndex: " + rndIndex);

            //string outprint = "RemainingRanksOfRed: ";
            //foreach (int i in RemainingRanksOfRed) {
            //    outprint += i+", ";
            //}
            //Debug.Log(outprint);

            //outprint = "RedChessPieceNum: ";
            //foreach (int i in RedChessPieceNum)
            //{
            //    outprint += i + ", ";
            //}
            //Debug.Log(outprint);

            int RankRnd = RemainingRanksOfRed[rndIndex];
            //Debug.Log("RankRnd: " + RankRnd);
            RedChessPieceNum[RankRnd] -= 1;
            if (RedChessPieceNum[RankRnd] == 0)
            {
                RemainingRanksOfRed.Remove(RankRnd);
            }
            return new Chess(false, RankRnd);//0=pawn, 1=bishop, 2=guard, 3=cannon, 4=knight, 5=chariot, 6=king
        }
            
        
        
    }

    private void InitPieceNumArray() {
        for (int i = 0; i < 7; i++) {
            RemainingRanksOfRed.Add(i);
            RemainingRanksOfBlack.Add(i);
            if (i == 0)
            {
                RedChessPieceNum[i] = 5;
                BlackChessPieceNum[i] = 5;
                //Debug.Log("Insert 1");
            }
            else if (i < 6)
            {
                RedChessPieceNum[i] = 2;
                BlackChessPieceNum[i] = 2;
                //Debug.Log("Insert 2");
            }
            else {
                RedChessPieceNum[i] = 1;
                BlackChessPieceNum[i] = 1;
                //Debug.Log("Insert 5");
            }
        }
        //string outprint = "RemainingRanksOfRed: ";
        //foreach (int i in RemainingRanksOfRed) {
        //    outprint += i+", ";
        //}
        //Debug.Log(outprint);
        //outprint = "RemainingRanksOfBlack: ";
        //foreach (int i in RemainingRanksOfBlack)
        //{
        //    outprint += i + ", ";
        //}
        //Debug.Log(outprint);
        //outprint = "RedChessPieceNum: ";
        //foreach (int i in RedChessPieceNum)
        //{
        //    outprint += i + ", ";
        //}
        //Debug.Log(outprint);
        //outprint = "BlackChessPieceNum: ";
        //foreach (int i in BlackChessPieceNum)
        //{
        //    outprint += i + ", ";
        //}
        //Debug.Log(outprint);

    }
}
