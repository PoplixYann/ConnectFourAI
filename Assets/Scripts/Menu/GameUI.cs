using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    GameObject Visual3DGO;
    [SerializeField]
    GameObject Visual2DGO;
    [SerializeField]
    Text visualModeText;

    [SerializeField]
    Text switchVisualText;

    Coroutine curCoroutine = null;

    public void SwitchVisual()
    {
        if (curCoroutine != null)
            return;

        curCoroutine = StartCoroutine(SwitchVisualCoroutine());
    }

    IEnumerator SwitchVisualCoroutine()
    {
        if (Visual3D.Instance != null && Visual3D.Instance.gameObject.activeSelf)
        {
            Visual3D.Instance.DisableVisual();
            yield return StartCoroutine(Visual2D.Instance.EnableVisualCoroutine());
            switchVisualText.text = "2D";
            Visual3DGO.SetActive(false);
            Visual2DGO.SetActive(true);
            visualModeText.color = Color.black;
        }
        else if (Visual2D.Instance != null && Visual2D.Instance.gameObject.activeSelf)
        {
            Visual2D.Instance.DisableVisual();
            yield return StartCoroutine(Visual3D.Instance.EnableVisualCoroutine());
            switchVisualText.text = "3D";
            Visual3DGO.SetActive(true);
            Visual2DGO.SetActive(false);
            visualModeText.color = Color.white;
        }

        curCoroutine = null;
    }
}
