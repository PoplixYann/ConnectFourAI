using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    void Update()
    {
        if (Visual3D.Instance.canPlay && !ConnectFour.Instance.GameIsEnd)
        {
            UpdateMousePos();
            MouseLeftClick();
        }
    }

    void UpdateMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layer_mask = LayerMask.GetMask("ColumnCollider");
        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layer_mask))
        {
            if (hit.collider.tag == "ColumnCollider")
            {
                ColumnCollider columnCollider = hit.collider.GetComponent<ColumnCollider>();
                Visual3D.Instance.PreviewCurPiece(columnCollider.columnId);
            }
        }
    }

    void MouseLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layer_mask = LayerMask.GetMask("ColumnCollider");
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, layer_mask))
            {
                if (hit.collider.tag == "ColumnCollider")
                {
                    ColumnCollider columnCollider = hit.collider.GetComponent<ColumnCollider>();
                    Visual3D.Instance.curPieceIsArrived = false;
                    Visual3D.Instance.PreviewCurPiece(columnCollider.columnId);
                    ConnectFour.Instance.AddPiece(columnCollider.columnId);
                }
            }
        }
    }
}
