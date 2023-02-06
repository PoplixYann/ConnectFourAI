using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    RED,
    YELLOW,
    NONE
}

public enum Mode
{
    PVE,
    PVP,
    EVE
}

public class ConnectFour : MonoBehaviour
{
    public event System.Action<int, int, Team> AddPieceEvent;
    public event System.Action<bool> GameEndEvent;

    static ConnectFour instance;
    public static ConnectFour Instance
    {
        get
        {
            return instance;
        }
    }

    public Team[,] board = new Team[6, 7];
    public Team playerTurn = Team.RED;
    bool gameIsEnd = false;
    public bool GameIsEnd
    {
        set
        {
            gameIsEnd = value;
            GameEndEvent?.Invoke(gameIsEnd);
        }
        get
        {
            return gameIsEnd;
        }
    }
    public Team winner = Team.NONE;
    public int[,] winnerPieces = new int[4, 2];

    public Team playerOne = Team.RED;
    public Team playerTwo = Team.YELLOW;

    public Mode curMode = Mode.PVE;
    public int AIOneDepth = 2;
    public int AITwoDepth = 2;
    public bool AIStart = false;

    ConnectFourAI ai = null;
    ConnectFourAI ai2 = null;

    void Awake()
    {
        instance = this;
        curMode = ConnectFourSettings.Instance.CurMode;
        ClearBoard();

        GameIsEnd = false;

        ai = new GameObject("AI").AddComponent<ConnectFourAI>();
        ai2 = new GameObject("AI2").AddComponent<ConnectFourAI>();

        AIOneDepth = ConnectFourSettings.Instance.AIOneDepth;
        AITwoDepth = ConnectFourSettings.Instance.AITwoDepth;
        AIStart = ConnectFourSettings.Instance.AIStart;
        ai.depth = AIOneDepth;
        ai2.depth = AITwoDepth;

        if (curMode == Mode.PVE)
        {
            playerOne = ConnectFourSettings.Instance.playerOneTeam;
            playerTwo = Team.YELLOW;
            if (playerOne == Team.YELLOW)
            {
                playerTwo = Team.RED;
            }

            ai.team = playerTwo;

            if (AIStart)
            {
                playerTurn = playerTwo;
            }
            else
            {
                playerTurn = playerOne;
            }
        }
        else if (curMode == Mode.PVP)
        {
            playerOne = ConnectFourSettings.Instance.playerOneTeam;
            playerTwo = Team.YELLOW;
            if (playerOne == Team.YELLOW)
            {
                playerTwo = Team.RED;
            }

            playerTurn = playerOne;
        }
        else if (curMode == Mode.EVE)
        {
            playerOne = Team.RED;
            playerTwo = Team.YELLOW;
            ai.team = playerOne;
            ai2.team = playerTwo;
        }
    }

    private void Start()
    {
        if (curMode == Mode.PVE)
        {
            if (playerTurn == playerTwo)
            {
                ai.PlayAI();
            }
        }
        else if (curMode == Mode.EVE)
        {
            ai.PlayAI();
        }
    }

    void ClearBoard()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                board[i, j] = Team.NONE;
            }
        }
    }

    public void AddPiece(int column)
    {
        if (GameIsEnd)
            return;

        //Find line
        int line = 5;
        while (board[line, column] != Team.NONE)
        {
            line--;
            if (line < 0)
            {
                return;
            }
        }

        //Put piece
        board[line, column] = playerTurn;

        //Invoke event
        AddPieceEvent?.Invoke(line, column, playerTurn);

        //Check winner or draw
        winner = CheckWin(board);
        if (winner != Team.NONE)
        {
            GameIsEnd = true;
        }
        else if (CheckDraw(board))
        {
            GameIsEnd = true;
        }
        else //No winner and no draw
        {
            //Swap turn
            if (playerTurn == Team.RED)
                playerTurn = Team.YELLOW;
            else if (playerTurn == Team.YELLOW)
                playerTurn = Team.RED;

            //Do AI play
            if (curMode == Mode.PVE)
            {
                if (playerTurn == playerTwo)
                {
                    ai.PlayAI();
                }
            }
            else if (curMode == Mode.EVE)
            {
                if (playerTurn == playerOne)
                {
                    ai.PlayAI();
                }
                else
                {
                    ai2.PlayAI();
                }
            }
        }
    }

    void ResetWinnerPieces()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                winnerPieces[i, j] = 0;
            }
        }
    }

    public Team CheckWin(Team[,] _board)
    {
        Team winner = Team.NONE;
        ResetWinnerPieces();

        //Line
        for (int line = 0; line < 6; line++)
        {
            for (int column = 0; column < 4; column++)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_board[line, column + i] != _board[line, column + i + 1])
                    {
                        winner = Team.NONE;
                        break;
                    }
                    winnerPieces[i, 0] = line;
                    winnerPieces[i, 1] = column + i;
                    winnerPieces[i + 1, 0] = line;
                    winnerPieces[i + 1, 1] = column + i + 1;
                    winner = _board[line, column];
                }
                if (winner != Team.NONE)
                {
                    return winner;
                }
            }
        }

        //Column
        for (int line = 0; line < 3; line++)
        {
            for (int column = 0; column < 7; column++)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_board[line + i, column] != _board[line + i + 1, column])
                    {
                        winner = Team.NONE;
                        break;
                    }
                    winnerPieces[i, 0] = line + i;
                    winnerPieces[i, 1] = column;
                    winnerPieces[i + 1, 0] = line + i + 1;
                    winnerPieces[i + 1, 1] = column;
                    winner = _board[line, column];
                }
                if (winner != Team.NONE)
                {
                    return winner;
                }
            }
        }

        //Diago Left-Right Top-Down
        for (int line = 0; line < 3; line++)
        {
            for (int column = 0; column < 4; column++)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_board[line + i, column + i] != _board[line + i + 1, column + i + 1])
                    {
                        winner = Team.NONE;
                        break;
                    }
                    winnerPieces[i, 0] = line + i;
                    winnerPieces[i, 1] = column + i;
                    winnerPieces[i + 1, 0] = line + i + 1;
                    winnerPieces[i + 1, 1] = column + i + 1;
                    winner = _board[line, column];
                }
                if (winner != Team.NONE)
                {
                    return winner;
                }
            }
        }

        //Diago Left-Right Down-Top
        for (int line = 3; line < 6; line++)
        {
            for (int column = 0; column < 4; column++)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_board[line - i, column + i] != _board[line - i - 1, column + i + 1])
                    {
                        winner = Team.NONE;
                        break;
                    }
                    winnerPieces[i, 0] = line - i;
                    winnerPieces[i, 1] = column + i;
                    winnerPieces[i + 1, 0] = line - i - 1;
                    winnerPieces[i + 1, 1] = column + i + 1;
                    winner = _board[line, column];
                }
                if (winner != Team.NONE)
                {
                    return winner;
                }
            }
        }

        return Team.NONE;
    }

    public bool CheckDraw(Team[,] _board)
    {
        for (int column = 0; column < 7; column++)
        {
            if (_board[0, column] == Team.NONE)
            {
                return false;
            }
        }

        return true;
    }

    public bool CellIsValid(Team[,] _board, int _line, int _column)
    {
        return (_line >= 0 && _line <= 5 && _column >= 0 && _column <= 6);
    }

    public string ConsoleBoard(Team[,] _board)
    {
        string result = "";

        for (int i = 0; i < 6; i++)
        {
            result += "|";
            for (int j = 0; j < 7; j++)
            {
                switch (_board[i, j])
                {
                    case Team.RED:
                        result += "X|";
                        break;
                    case Team.YELLOW:
                        result += "O|";
                        break;
                    case Team.NONE:
                        result += "_|";
                        break;
                    default:
                        break;
                }
            }
            result += "\n";
        }

        return result;
    }
}
