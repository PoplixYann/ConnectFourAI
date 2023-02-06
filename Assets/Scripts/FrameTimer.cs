using System.Diagnostics;
using UnityEngine;

public class FrameTimer : MonoBehaviour
{
    static FrameTimer instance;
    public static FrameTimer Instance
    {
        get
        {
            return instance;
        }
    }

    Stopwatch stopwatch;
    long prevFrameDuration = 0;

    public long FrameDuration
    {
        get
        {
            if (stopwatch == null)
            {
                return 0;
            }

            return stopwatch.ElapsedMilliseconds;
        }
    }

    public long PrevFrameDuration
    {
        get
        {
            return prevFrameDuration;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        instance = this;

        stopwatch = new Stopwatch();
    }

    void Update()
    {
        prevFrameDuration = FrameDuration;
        stopwatch.Reset();
        stopwatch.Start();
    }
}
