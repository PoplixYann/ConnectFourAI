using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visual3D : MonoBehaviour
{
    static Visual3D instance;
    public static Visual3D Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField]
    GameObject pieceParent;
    [SerializeField]
    GameObject pieceRedPrefab;
    [SerializeField]
    GameObject pieceYellowPrefab;

    GameObject curPieceGO;

    GameObject[,] pieceArray = new GameObject[6, 7];
    Team winner = Team.NONE;
    Color baseColor;
    Color curColor;
    float colorProgress = 0;
    bool colorGoDown = true;

    int middleColumn = 3;
    public int curColumn = 3;
    public bool curPieceIsArrived = false;

    [SerializeField]
    float curPieceSpeed = 5.0f;

    public bool canPlay = true;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        ConnectFour.Instance.AddPieceEvent += AddPiece;
        ConnectFour.Instance.GameEndEvent += OnGameEnd;

        if (IsUserPlayed(ConnectFour.Instance.playerTurn))
        {
            SpawnNewPiece(ConnectFour.Instance.playerTurn, middleColumn);
        }
    }

    private void OnDisable()
    {
        ConnectFour.Instance.AddPieceEvent -= AddPiece;
        ConnectFour.Instance.GameEndEvent -= OnGameEnd;
    }

    private void Update()
    {
        MoveCurPiece();
        UpdateWinnerPiece();
    }

    public void DisableVisual()
    {
        canPlay = false;
    }

    public IEnumerator EnableVisualCoroutine()
    {
        foreach (Transform child in pieceParent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int line = 0; line < 6; line++)
        {
            for (int column = 0; column < 7; column++)
            {
                Team curCell = ConnectFour.Instance.board[line, column];
                if (curCell == Team.RED)
                {
                    GameObject pieceGO = Instantiate(pieceRedPrefab, pieceParent.transform);
                    pieceGO.transform.position = GetCellPos(line, column);
                    pieceArray[line, column] = pieceGO;
                }
                else if (curCell == Team.YELLOW)
                {
                    GameObject pieceGO = Instantiate(pieceYellowPrefab, pieceParent.transform);
                    pieceGO.transform.position = GetCellPos(line, column);
                    pieceArray[line, column] = pieceGO;
                }
            }
            yield return null;
        }

        canPlay = true;
    }

    void MoveCurPiece()
    {
        if (curPieceGO != null)
        {
            curPieceGO.transform.position = Vector3.Lerp(curPieceGO.transform.position, GetTopColumnPos(curColumn), Time.deltaTime * curPieceSpeed);
            if (Vector3.Distance(curPieceGO.transform.position, GetTopColumnPos(curColumn)) <= 0.025f)
            {
                curPieceGO.transform.position = GetTopColumnPos(curColumn);
                curPieceIsArrived = true;
            }
            else
            {
                curPieceIsArrived = false;
            }
        }
    }

    void UpdateWinnerPiece()
    {
        if (ConnectFour.Instance.GameIsEnd && canPlay && winner != Team.NONE)
        {
            colorProgress += Time.deltaTime;
            if (colorGoDown)
            {
                curColor = Color.Lerp(baseColor, Color.black, colorProgress);
            }
            else
            {
                curColor = Color.Lerp(Color.black, baseColor, colorProgress);
            }
            if (colorProgress >= 1.0f)
            {
                colorGoDown = !colorGoDown;
                colorProgress = 0.0f;
            }

            for (int i = 0; i < 4; i++)
            {
                pieceArray[ConnectFour.Instance.winnerPieces[i, 0], ConnectFour.Instance.winnerPieces[i, 1]].GetComponentInChildren<Renderer>().material.color = curColor;
            }
        }
    }

    void OnGameEnd(bool _gameIsEnd)
    {
        if (!_gameIsEnd)
            return;

        winner = ConnectFour.Instance.winner;
        if (winner == Team.RED)
        {
            baseColor = Color.red;
        }
        else if (winner == Team.YELLOW)
        {
            baseColor = Color.yellow;
        }
        curColor = baseColor;
    }

    Vector3 GetTopColumnPos(int column)
    {
        Vector3 pos = new Vector3(transform.position.x - (6 / 2) + column,
            transform.position.y + 7f,
            0);

        return pos;
    }


    Vector3 GetCellPos(int line, int column)
    {
        Vector3 pos = new Vector3(transform.position.x - (6 / 2) + column,
            transform.position.y + 5.5f - line,
            0);

        return pos;
    }

    void AddPiece(int line, int column, Team team)
    {
        //Debug.Log("Team : " + team + " - Line : " + line + " - Column : " + column);

        if (IsUserPlayed(team)) //Is user turn
        {
            StartCoroutine(AddPieceUserCoroutine(line, column));
        }
        else //Is AI turn
        {
            StartCoroutine(AddPieceAICoroutine(team, line, column));
        }
    }

    IEnumerator AddPieceUserCoroutine(int _line, int _column)
    {
        //Impossible to play during user turn
        canPlay = false;

        //Wait piece arrive to choose column
        while (!curPieceIsArrived)
        {
            yield return null;
        }

        //Let user piece fall
        curPieceGO.GetComponent<Rigidbody>().isKinematic = false;
        pieceArray[_line, _column] = curPieceGO;
        curPieceGO = null;

        //Wait user piece fall
        yield return StartCoroutine(NextTurnCoroutine());

        //Possible to play after user turn is finished
        canPlay = true;

        if (ConnectFour.Instance.curMode == Mode.PVP && !ConnectFour.Instance.GameIsEnd)
        {
            SpawnNewPiece(ConnectFour.Instance.playerTurn, middleColumn);
        }
    }

    IEnumerator AddPieceAICoroutine(Team _team, int _line, int _column)
    {
        //Wait previous play is finished
        while (!canPlay)
        {
            yield return null;
        }

        //Impossible to play during AI turn
        canPlay = false;

        //Spawn AI piece and let it fall
        SpawnNewPiece(_team, _column);
        curPieceGO.transform.position = GetTopColumnPos(_column);
        curPieceGO.GetComponent<Rigidbody>().isKinematic = false;
        pieceArray[_line, _column] = curPieceGO;
        curPieceGO = null;

        //Wait AI piece fall
        yield return StartCoroutine(NextTurnCoroutine());

        //Possible to play after AI turn is finished
        canPlay = true;

        //If next turn is user to play spawn new piece
        if (ConnectFour.Instance.curMode == Mode.PVE && !ConnectFour.Instance.GameIsEnd)
        {
            SpawnNewPiece(ConnectFour.Instance.playerTurn, middleColumn);
        }
    }

    IEnumerator NextTurnCoroutine()
    {
        float timer = 0.5f;
        while (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
    }

    void SpawnNewPiece(Team _team, int _column)
    {
        if (_team == Team.RED)
        {
            curPieceGO = Instantiate(pieceRedPrefab, pieceParent.transform);
        }
        else if (_team == Team.YELLOW)
        {
            curPieceGO = Instantiate(pieceYellowPrefab, pieceParent.transform);
        }

        curPieceGO.GetComponent<Rigidbody>().isKinematic = true;
        curPieceGO.transform.position = GetTopColumnPos(_column);
        curColumn = middleColumn;
    }

    public void PreviewCurPiece(int column)
    {
        if (column != -1)
        {
            curColumn = column;
        }
    }

    bool IsUserPlayed(Team _team)
    {
        return (ConnectFour.Instance.curMode == Mode.PVE && _team == ConnectFour.Instance.playerOne)
            || ConnectFour.Instance.curMode == Mode.PVP;
    }
}
