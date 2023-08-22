using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    protected bool isMoving;
    protected Vector3 targetPosition;
    protected float moveSpeed = 5f;
    public string ipEndPoint = "";

    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoving = true;
    }

    protected void MoveUpdate()
    {
        if (isMoving == false) return;

        transform.LookAt(targetPosition);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= 0.01f)
        {
            isMoving = false;
        }
    }
}
