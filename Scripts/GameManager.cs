using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float StartingCorrPosX = 3.3f;

    public float StartingCorrPosY = 12.5f;

    public List<GameObject> ObjectToDelete = new List<GameObject>();

    public GameState State;

    public static event Action<GameState> OnGameStateChange;

    public Board boardInfo;

    private bool firstTurn;

    private RndChessGenerator ChessGenerator;

    private AIPlayer aiPlayer;

    public ChessColor PLayerColor;

    public GameObject[] ChessModels;

    public GameObject[] Highlights;

    public int whosfirst;

    public bool menuOpened = false;

    public bool playerChessPressed = false;

    public GameObject HighlightChess;

    public GameObject[] HighlightGreenPlates;

    [SerializeField]
    private GameObject WhosFirstBanner;

    [SerializeField]
    private GameObject PlayersTurnBanner;

    [SerializeField]
    private GameObject AIsturnBanner;

    [SerializeField]
    private GameObject RedScore;

    [SerializeField]
    private GameObject BlackScore;

    [SerializeField]
    private GameObject WinningPopup;

    [SerializeField]
    private GameObject LosingPopup;

    [SerializeField]
    private GameObject PausePanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

        State = GameState.StartGame;
        System.Random rnd = new System.Random();
        whosfirst = rnd.Next(1, 3);//1=player first, 2=ai first
        //int whosfirst = 1;
        Debug.Log(whosfirst);
        ShowWhosFirstBanner();
        Invoke("GameStart", 3);
        SetupVar();
        boardInfo = new Board(1);
        SetupChessBoard();
        InitGameObject();

        boardInfo.printboard();
    }

    private void ShowWhosFirstBanner()
    {
        WhosFirstBanner.SetActive(true);
        TextMeshProUGUI textMeshPro = WhosFirstBanner.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(textMeshPro);
        if (whosfirst == 1)
        {
            textMeshPro.text = "You go first";
        }
        else
        {
            textMeshPro.text = "AI goes first";
        }
        WhosFirstBanner.GetComponent<FadeAnimation>().FadeIn();
        Invoke("HideWhosFirstBanner", 2);
    }

    private void DeductScore(bool isBlack)
    {
        TextMeshProUGUI textMeshPro;
        if (isBlack)
        {
            textMeshPro = BlackScore.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.text = boardInfo.BlackRemainingChess.ToString();
        }
        else
        {
            textMeshPro = RedScore.GetComponentInChildren<TextMeshProUGUI>();
            textMeshPro.text = boardInfo.RedRemainingChess.ToString();
        }
    }

    private void HidePlayersTurnBanner()
    {
        PlayersTurnBanner.GetComponent<FadeAnimation>().FadeOut();
    }

    private void HideAIsTurnBanner()
    {
        AIsturnBanner.GetComponent<FadeAnimation>().FadeOut();
    }

    private void HideWhosFirstBanner()
    {
        WhosFirstBanner.GetComponent<FadeAnimation>().FadeOut();
    }

    private void GameStart()
    {

        if (whosfirst == 1)
            UpdateGameState(GameState.PlayerTurn);
        else
            UpdateGameState(GameState.AITurn);

        if (State == GameState.AITurn)
        {
            Debug.Log("Starting AI turn");

        }
    }

    private void NextTurn()
    {
        boardInfo.printboard();
        if (boardInfo.BlackRemainingChess > 0 && boardInfo.RedRemainingChess > 0)
        {
            if (State == GameState.PlayerTurn)
            {
                AIsturnBanner.SetActive(true);
                AIsturnBanner.GetComponent<FadeAnimation>().FadeIn();
                Invoke("HideAIsTurnBanner", 2f);
                Invoke("UpdateAIGameState", 2.5f);
            }
            else if (State == GameState.AITurn)
            {
                PlayersTurnBanner.SetActive(true);
                PlayersTurnBanner.GetComponent<FadeAnimation>().FadeIn();
                Invoke("HidePlayersTurnBanner", 2f);
                Invoke("UpdatePLayerGameState", 2.5f);
            }
        }
        else
        {
            if (PLayerColor == ChessColor.Black && boardInfo.BlackRemainingChess <= 0 ||
                PLayerColor == ChessColor.Red && boardInfo.RedRemainingChess <= 0)
            {
                menuOpened = true;
                LosingPopup.GetComponent<FadeAnimation>().FadeIn();
            }
            else
            {
                menuOpened = true;
                WinningPopup.GetComponent<FadeAnimation>().FadeIn();
            }

        }
    }

    private void UpdatePLayerGameState()
    {
        UpdateGameState(GameState.PlayerTurn);
    }

    private void UpdateAIGameState()
    {
        UpdateGameState(GameState.AITurn);
    }

    private void SetupVar()
    {
        ChessGenerator = new RndChessGenerator();
        firstTurn = true;
        aiPlayer = new AIPlayer();
    }

    private void InitGameObject()
    {
        HighlightChess = Instantiate(Highlights[0], new Vector3(100, 0.09f, 100), Quaternion.Euler(new Vector3(-90, 0, -90)));
        HighlightChess.SetActive(false);
        ObjectToDelete.Add(HighlightChess);
        HighlightGreenPlates = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            HighlightGreenPlates[i] = Instantiate(Highlights[i + 1], new Vector3(100, 1000f, 100), Quaternion.Euler(new Vector3(-90, 0, -90)));
            HighlightGreenPlates[i].SetActive(false);
            ObjectToDelete.Add(HighlightGreenPlates[i]);
        }
    }

    public void UpdateGameState(GameState newState)
    {
        //Debug.Log("Setting state");
        State = newState;
        switch (newState)
        {
            case GameState.PlayerTurn:
                playerChessPressed = false;
                break;
            case GameState.AITurn:
                aiPlayer.ResetEndTurnVar();
                Invoke("StartAIPlay", 1.0f);
                break;
        }

        OnGameStateChange?.Invoke(newState);
    }

    public void SetupChessBoard()
    {
        //string outprint = "";
        float posX = StartingCorrPosX;
        float posY = StartingCorrPosY;

        for (int y = 0; y < 4; y++)
        {

            for (int x = 0; x < 8; x++)
            {
                Chess newChess = ChessGenerator.GetRandomChess();
                int ChessModelIndex;
                bool isBlack = newChess.IsBlack();
                int rank = newChess.GetRanking();
                if (isBlack)
                {
                    ChessModelIndex = 0;//black rank range from 0-7
                }
                else
                {
                    ChessModelIndex = 7;//red rank range from 7-13
                }
                ChessModelIndex += newChess.GetRanking();
                GameObject chess = Instantiate(ChessModels[ChessModelIndex], new Vector3(posX, 0.09f, posY), Quaternion.Euler(new Vector3(90, 0, 90)));
                Chess gameObjChess = chess.GetComponent<Chess>();
                gameObjChess.InitVar(isBlack, rank, x, y);
                boardInfo.AddChess(gameObjChess);
                ObjectToDelete.Add(chess);
                posY -= 2.65f;

                //Debug.Log(chess.GetComponent<Chess>().ToString());
                //outprint += newChess.ToString() + " | ";
            }
            posY = 12.5f;
            posX -= 2.6f;
            //outprint += "\n";
        }
    }

    public void StartAIPlay()
    {
        if (firstTurn)
        {
            StartCoroutine(aiPlayer.PlayFirstTurn(boardInfo));
        }
        else
        {
            StartCoroutine(aiPlayer.Play(boardInfo));
        }
    }

    public bool IsSelectable()
    {
        //Debug.Log(State);
        return State == GameState.PlayerTurn && !menuOpened && !playerChessPressed;
    }

    public void RevealChess(int chessX, int chessY)
    {
        //Setting chess color for players in the 1st turn
        Debug.Log("Reveal: " + boardInfo.GetChess(chessX, chessY).ToString());
        Chess ChessToReveal = boardInfo.board[chessY, chessX];
        if (firstTurn)
        {
            if (ChessToReveal.IsBlack() && State == GameState.AITurn || !ChessToReveal.IsBlack() && State == GameState.PlayerTurn)
            {
                aiPlayer.SetChessColor(ChessColor.Black);
                PLayerColor = ChessColor.Red;
                Debug.Log("AI: Black , PLayer: Red");
            }
            else
            {
                aiPlayer.SetChessColor(ChessColor.Red);
                PLayerColor = ChessColor.Black;
                Debug.Log("AI: Red , PLayer: Black");
            }
            firstTurn = false;
        }
        if (boardInfo.GetChess(chessX, chessY).IsBlack() && PLayerColor == ChessColor.Red ||
            !boardInfo.GetChess(chessX, chessY).IsBlack() && PLayerColor == ChessColor.Black)
        {
            Debug.Log("Different color!!: " + boardInfo.GetChess(chessX, chessY).ToString());
            boardInfo.GetChess(chessX, chessY).FilpOverChess(false);
        }
        else
        {
            boardInfo.GetChess(chessX, chessY).FilpOverChess();
        }

        boardInfo.RevealChess(chessX, chessY);
        Invoke("NextTurn", 2.0f);
    }

    public void MoveChess(int desX, int desY, Chess targetChess)
    {
        HideHighlight();
        Chess chessToKill = boardInfo.board[desY, desX];
        Debug.Log("Move chess " + targetChess.ToString() + " to: X:" + desX + " Y: " + desY);
        if (!boardInfo.board[desY, desX].isEmpty)
        {
            boardInfo.board[desY, desX].KillChess();
            Debug.Log("Kill: " + boardInfo.board[desY, desX].ToString());
        }
        //Debug.Log("Move: " + targetChess.ToString());
        boardInfo.ChessMove(targetChess, desX, desY);
        targetChess.MoveChess(desX, desY);
        if (!chessToKill.isEmpty)
            DeductScore(chessToKill.IsBlack());
        //Invoke("NextTurn", 3.0f);
        Invoke("NextTurn", 2.0f);
    }

    public void PlaceGreenClickable(float x, float y, bool isBlack, int indexX, int indexY)
    {
        MoveablePos moveables = boardInfo.GetMoveablePos(indexX, indexY, isBlack);
        if (moveables.down)
        {
            HighlightGreenPlates[1].transform.position = new Vector3(x - 2.6f, 0.09f, y);
            GreenSelectable g = HighlightGreenPlates[1].GetComponent<GreenSelectable>();
            g.SetVar(indexX, indexY + 1, boardInfo.board[indexY, indexX]);
            HighlightGreenPlates[1].SetActive(true);

        }

        if (moveables.up)
        {
            HighlightGreenPlates[0].transform.position = new Vector3(x + 2.6f, 0.09f, y);
            GreenSelectable g = HighlightGreenPlates[0].GetComponent<GreenSelectable>();
            g.SetVar(indexX, indexY - 1, boardInfo.board[indexY, indexX]);
            HighlightGreenPlates[0].SetActive(true);
        }
        if (moveables.left)
        {
            HighlightGreenPlates[2].transform.position = new Vector3(x, 0.09f, y + 2.65f);
            GreenSelectable g = HighlightGreenPlates[2].GetComponent<GreenSelectable>();
            g.SetVar(indexX - 1, indexY, boardInfo.board[indexY, indexX]);
            HighlightGreenPlates[2].SetActive(true);
        }

        if (moveables.right)
        {
            HighlightGreenPlates[3].transform.position = new Vector3(x, 0.09f, y - 2.65f);
            GreenSelectable g = HighlightGreenPlates[3].GetComponent<GreenSelectable>();
            g.SetVar(indexX + 1, indexY, boardInfo.board[indexY, indexX]);
            HighlightGreenPlates[3].SetActive(true);
        }


        Debug.Log(moveables.ToString());
    }

    public void HideHighlight()
    {
        HighlightChess.SetActive(false);
        //Debug.Log("remove green");
        for (int i = 0; i < 4; i++)
        {
            HighlightGreenPlates[i].SetActive(false);
        }
    }

    public void DestoryAllObject()
    {
        for (int i = 0; i < ObjectToDelete.Count; i++)
        {
            Destroy(ObjectToDelete[i]);
        }
    }
}

public enum GameState
{
    StartGame, PlayerTurn, AITurn, PlayerVictory, PlayerLose
}

public enum ChessColor
{
    Black, Red
}
