using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectFourAI : MonoBehaviour
{
    int AIWinScore = 2000000;
    int oppWinScore = -1000000;

    public Team team;
    public int depth;
    int chooseColumn;

    bool minmaxIsFinished = false;
    int minmaxValue = 0;
    bool minmaxIsWaiting = false;

    int MinMax(Team[,] _board, int _depth, int _alpha, int _beta, bool _maximizingPlayer)
    {
        if (_depth == depth || ConnectFour.Instance.CheckWin(_board) != Team.NONE || ConnectFour.Instance.CheckDraw(_board))
        {
            int value = Evaluate(_board, team);
            return value;
        }

        int[] possibleColumns = GetPossibleColumn(_board);
        Team oppTeam = Team.YELLOW;
        if (team == Team.YELLOW)
        {
            oppTeam = Team.RED;
        }

        if (_maximizingPlayer)
        {
            int max = int.MinValue;
            foreach (int column in possibleColumns)
            {
                Team[,] boardCopy = GetNextTurnBoard(_board, column, team);
                int value = MinMax(boardCopy, _depth + 1, _alpha, _beta, false);
                if (_depth == 0 && value > max)
                {
                    chooseColumn = column;
                }
                max = Mathf.Max(max, value);
                _alpha = Mathf.Max(_alpha, value);
                if (_beta <= _alpha)
                {
                    break;
                }
            }
            return max;
        }
        else
        {
            int min = int.MaxValue;
            foreach (int column in possibleColumns)
            {
                Team[,] boardCopy = GetNextTurnBoard(_board, column, oppTeam);
                int value = MinMax(boardCopy, _depth + 1, _alpha, _beta, true);
                min = Mathf.Min(min, value);
                _beta = Mathf.Min(_beta, value);
                if (_beta <= _alpha)
                {
                    break;
                }
            }
            return min;
        }
    }

    IEnumerator MinMaxCoroutine(Team[,] _board, int _depth, int _alpha, int _beta, bool _maximizingPlayer)
    {
        if (FrameTimer.Instance.FrameDuration >= (int)((1.0f/60.0f) * 1000.0f))
        {
            minmaxIsWaiting = true;
            yield return null;
            minmaxIsWaiting = false;
        }
        if (_depth == depth || ConnectFour.Instance.CheckWin(_board) != Team.NONE || ConnectFour.Instance.CheckDraw(_board))
        {
            minmaxValue = Evaluate(_board, team);
        }
        else
        {
            int[] possibleColumns = GetPossibleColumn(_board);
            Team oppTeam = Team.YELLOW;
            if (team == Team.YELLOW)
            {
                oppTeam = Team.RED;
            }

            if (_maximizingPlayer)
            {
                int max = int.MinValue;
                foreach (int column in possibleColumns)
                {
                    Team[,] boardCopy = GetNextTurnBoard(_board, column, team);
                    StartCoroutine(MinMaxCoroutine(boardCopy, _depth + 1, _alpha, _beta, false));
                    while (minmaxIsWaiting)
                    {
                        yield return null;
                    }
                    int value = minmaxValue;
                    if (_depth == 0 && value > max)
                    {
                        chooseColumn = column;
                    }
                    max = Mathf.Max(max, value);
                    _alpha = Mathf.Max(_alpha, value);
                    if (_beta <= _alpha)
                    {
                        break;
                    }
                }
                minmaxValue = max;
            }
            else
            {
                int min = int.MaxValue;
                foreach (int column in possibleColumns)
                {
                    Team[,] boardCopy = GetNextTurnBoard(_board, column, oppTeam);
                    StartCoroutine(MinMaxCoroutine(boardCopy, _depth + 1, _alpha, _beta, true));
                    while (minmaxIsWaiting)
                    {
                        yield return null;
                    }
                    int value = minmaxValue;
                    min = Mathf.Min(min, value);
                    _beta = Mathf.Min(_beta, value);
                    if (_beta <= _alpha)
                    {
                        break;
                    }
                }
                minmaxValue = min;
            }
        }

        if (_depth == 0)
        {
            minmaxIsFinished = true;
        }
    }

    IEnumerator PlayAICoroutine()
    {
        minmaxIsFinished = false;
        minmaxValue = 0;
        chooseColumn = 0;
        minmaxIsWaiting = false;
        StartCoroutine(MinMaxCoroutine(ConnectFour.Instance.board, 0, int.MinValue, int.MaxValue, true));

        float timer = 0.5f;
        while (!minmaxIsFinished || timer > 0.0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        ConnectFour.Instance.AddPiece(chooseColumn);

        yield return null;
    }

    public void PlayAI()
    {
        //MinMax(ConnectFour.Instance.board, 0, int.MinValue, int.MaxValue, true);
        //ConnectFour.Instance.AddPiece(chooseColumn);
        StopAllCoroutines();
        StartCoroutine(PlayAICoroutine());
    }

    int[] GetPossibleColumn(Team[,] _board)
    {
        List<int> possibleColumns = new List<int>();
        for (int column = 0; column < 7; column++)
        {
            if (_board[0, column] == Team.NONE)
                possibleColumns.Add(column);
        }
        return possibleColumns.ToArray();
    }

    int GetOpenLine(Team[,] _board, int _column)
    {
        int line = 5;
        while (_board[line, _column] != Team.NONE)
        {
            line--;
            if (line < 0)
            {
                break;
            }
        }

        return line;
    }

    Team[,] GetNextTurnBoard(Team[,] _board, int _column, Team _piece)
    {
        int line = GetOpenLine(_board, _column);
        Team[,] boardCopy = new Team[6, 7];
        System.Array.Copy(_board, boardCopy, _board.Length);
        boardCopy[line, _column] = _piece;
        return boardCopy;
    }

    int Evaluate(Team[,] _board, Team _AITeam)
    {
        //This array will store 4 align piece
        Team[] window = new Team[4];

        //Reset value to 0
        int tempValue;
        int value = 0;

        //Line
        for (int line = 0; line < 6; line++)
        {
            for (int column = 0; column < 4; column++)
            {
                for (int i = 0; i < 4; i++)
                {
                    window[i] = _board[line, column + i];
                }
                tempValue = EvaluateWindow(window, _AITeam);
                if (tempValue == AIWinScore) return tempValue;
                if (tempValue == oppWinScore) return tempValue;
                value += tempValue;
            }
        }

        //Column
        for (int line = 0; line < 3; line++)
        {
            for (int column = 0; column < 7; column++)
            {
                for (int i = 0; i < 4; i++)
                {
                    window[i] = _board[line + i, column];
                }
                tempValue = EvaluateWindow(window, _AITeam);
                if (tempValue == AIWinScore) return tempValue;
                if (tempValue == oppWinScore) return tempValue;
                value += tempValue;
            }
        }

        //Diago Left-Right Top-Down
        for (int line = 0; line < 3; line++)
        {
            for (int column = 0; column < 4; column++)
            {
                for (int i = 0; i < 4; i++)
                {
                    window[i] = _board[line + i, column + i];
                }
                tempValue = EvaluateWindow(window, _AITeam);
                if (tempValue == AIWinScore) return tempValue;
                if (tempValue == oppWinScore) return tempValue;
                value += tempValue;
            }
        }

        //Diago Left-Right Down-Top
        for (int line = 3; line < 6; line++)
        {
            for (int column = 0; column < 4; column++)
            {
                for (int i = 0; i < 4; i++)
                {
                    window[i] = _board[line - i, column + i];
                }
                tempValue = EvaluateWindow(window, _AITeam);
                if (tempValue == AIWinScore) return tempValue;
                if (tempValue == oppWinScore) return tempValue;
                value += tempValue;
            }
        }

        //Debug.Log("Value : " + value);
        //Debug.Log(ConnectFour.Instance.ConsoleBoard(_board));

        return value;
    }

    //Evaluate 4 align piece to get a value
    int EvaluateWindow(Team[] _window, Team _AITeam)
    {
        int nbAIPiece = 0;
        int nbOppPiece = 0;

        Team oppTeam = Team.RED;
        if (_AITeam == Team.RED)
        {
            oppTeam = Team.YELLOW;
        }


        /*Check piece in window to know which team the piece is*/
        foreach (Team piece in _window)
        {
            if (piece == _AITeam)
            {
                nbAIPiece++;
            }
            else if (piece == oppTeam)
            {
                nbOppPiece++;
            }
        }

        if (nbAIPiece == 4)
        {
            return AIWinScore;
        }
        else if (nbOppPiece == 4)
        {
            return oppWinScore;
        }

        return nbAIPiece;
    }

}
