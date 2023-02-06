using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectFourSettings : MonoBehaviour
{
    static ConnectFourSettings instance;
    public static ConnectFourSettings Instance
    {
        get
        {
            return instance;
        }
    }

    public event System.Action<Mode> OnModeChangeEvent;

    Mode curMode = Mode.PVE;
    public Mode CurMode
    {
        set
        {
            curMode = value;
            OnModeChangeEvent?.Invoke(curMode);
        }
        get
        {
            return curMode;
        }
    }

    public int AIOneDepth = 2;
    public int AITwoDepth = 2;

    public Team playerOneTeam = Team.RED;

    public bool AIStart = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public void SetMode(int _mode)
    {
        CurMode = (Mode)_mode;
    }

    public void SetAIOneDepth(int _depth)
    {
        AIOneDepth = _depth;
    }

    public void SetAITwoDepth(int _depth)
    {
        AITwoDepth = _depth;
    }

    public void SetPlayerOneTeam(int _team)
    {
        playerOneTeam = (Team)_team;
    }

    public void SetAIStart(bool _start)
    {
        AIStart = _start;
    }
}
