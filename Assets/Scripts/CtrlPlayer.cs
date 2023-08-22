using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlPlayer : BasePlayer
{
    private void Update()
    {
        MoveUpdate();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider.CompareTag("Ground"))
            {
                MoveTo(hit.point);

                // 发送移动协议
                string sendStr = "Move|";
                sendStr += NetManager.GetIPEndPoint() + ",";
                sendStr += hit.point.x + ",";
                sendStr += hit.point.y + ",";
                sendStr += hit.point.z + ",";
                NetManager.Send(sendStr);
            }
        }
    }
}
