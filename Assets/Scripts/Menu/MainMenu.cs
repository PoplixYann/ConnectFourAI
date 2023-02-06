using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    GameObject PVEPanel;
    [SerializeField]
    GameObject PVPPanel;
    [SerializeField]
    GameObject EVEPanel;

    [SerializeField]
    Dropdown ModeDropdown;
    [SerializeField]
    Dropdown PVEAiDepthDropdown;
    [SerializeField]
    Dropdown PVEPlayerChooseColorDropdown;
    [SerializeField]
    Toggle PVEAIStartToggle;
    [SerializeField]
    Dropdown PVPPlayerChooseColorDropdown;
    [SerializeField]
    Dropdown EVEAiOneDepthDropdown;
    [SerializeField]
    Dropdown EVEAiTwoDepthDropdown;

    private void Start()
    {
        ConnectFourSettings.Instance.OnModeChangeEvent += OnModeChanged;

        OnModeChanged(ConnectFourSettings.Instance.CurMode);
    }

    private void OnDestroy()
    {
        ConnectFourSettings.Instance.OnModeChangeEvent -= OnModeChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnModeChanged(Mode _mode)
    {
        ModeDropdown.value = (int)_mode;

        PVEPanel.SetActive(false);
        PVPPanel.SetActive(false);
        EVEPanel.SetActive(false);

        switch (_mode)
        {
            case Mode.PVE:
                PVEPanel.SetActive(true);
                break;
            case Mode.PVP:
                PVPPanel.SetActive(true);
                break;
            case Mode.EVE:
                EVEPanel.SetActive(true);
                break;
        }

        ResetSettings();
    }

    void ResetSettings()
    {
        PVEAiDepthDropdown.value = (ConnectFourSettings.Instance.AIOneDepth - 2) / 2;
        PVEPlayerChooseColorDropdown.value = (int)ConnectFourSettings.Instance.playerOneTeam;
        PVEAIStartToggle.isOn = ConnectFourSettings.Instance.AIStart;
        PVPPlayerChooseColorDropdown.value = (int)ConnectFourSettings.Instance.playerOneTeam;
        EVEAiOneDepthDropdown.value = (ConnectFourSettings.Instance.AIOneDepth - 2) / 2;
        EVEAiTwoDepthDropdown.value = (ConnectFourSettings.Instance.AITwoDepth - 2) / 2;
    }

    public void ChangeModeWithDropdown(Dropdown _dropdown)
    {
        ConnectFourSettings.Instance.SetMode(_dropdown.value);
    }

    public void ChangeAIOneDepthWithDropdown(Dropdown _dropdown)
    {
        ConnectFourSettings.Instance.SetAIOneDepth((_dropdown.value * 2) + 2);
    }

    public void ChangeAITwoDepthWithDropdown(Dropdown _dropdown)
    {
        ConnectFourSettings.Instance.SetAITwoDepth((_dropdown.value * 2) + 2);
    }

    public void ChangePlayerOneTeamWithDropdown(Dropdown _dropdown)
    {
        ConnectFourSettings.Instance.SetPlayerOneTeam(_dropdown.value);
    }

    public void ChangeAIStartWithToggle(Toggle _toggle)
    {
        ConnectFourSettings.Instance.SetAIStart(_toggle.isOn);
    }
}
