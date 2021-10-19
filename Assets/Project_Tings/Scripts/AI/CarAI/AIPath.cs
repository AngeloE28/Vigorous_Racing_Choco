using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPath : MonoBehaviour
{
    [SerializeField] private Color lineColor;
    private List<Transform> pathNodes;

    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        // Get all the pathnodes
        Transform[] wayPoints = GetComponentsInChildren<Transform>();

        pathNodes = new List<Transform>();

        // Add the waypoints to the pathNodes
        for(int i = 0; i < wayPoints.Length; i++)
        {
            if (wayPoints[i] != transform)
                pathNodes.Add(wayPoints[i]);            
        }

        // Draws the path for a visual representation
        for(int i = 0; i<pathNodes.Count; i++)
        {
            Vector3 currentNode = pathNodes[i].position;
            Vector3 previousNode = Vector3.zero;

            if (i > 0)
                previousNode = pathNodes[i - 1].position;
            else if (i == 0 && pathNodes.Count > 1)
                previousNode = pathNodes[pathNodes.Count - 1].position;

            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawWireSphere(currentNode, 0.5f);
        }
    }
}
