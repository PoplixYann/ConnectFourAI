using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : MonoBehaviour
{
    void Update()
    {
        if (Visual2D.Instance.canPlay && !ConnectFour.Instance.GameIsEnd)
        {
            UpdateMousePos();
            MouseLeftClick();
        }
    }

    void UpdateMousePos()
    {
        int layer_mask = LayerMask.GetMask("ColumnCollider");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, layer_mask);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "ColumnCollider")
            {
                ColumnCollider columnCollider = hit.collider.GetComponent<ColumnCollider>();
                Visual2D.Instance.PreviewCurPiece(columnCollider.columnId);
            }
        }
    }


    void MouseLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int layer_mask = LayerMask.GetMask("ColumnCollider");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, layer_mask);
            if (hit.collider != null)
            {
                if (hit.collider.tag == "ColumnCollider")
                {
                    ColumnCollider columnCollider = hit.collider.GetComponent<ColumnCollider>();
                    Visual2D.Instance.curPieceIsArrived = false;
                    Visual2D.Instance.PreviewCurPiece(columnCollider.columnId);
                    ConnectFour.Instance.AddPiece(columnCollider.columnId);
                }
            }
        }
    }
}
