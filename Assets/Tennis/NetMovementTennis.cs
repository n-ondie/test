using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code generated using ChatGPT 3.5 (12 Jun 2024)
// Prompts: In a Unity game, make a cube elastic when a ball hits it, if the cube is a net and therefore very thin,
// if I want the net to deform less only if the collision is near the top, left or right edges;
// the weights should change smoothly over the surface of the net.

[RequireComponent(typeof(MeshFilter))]
public class NetMovementTennis : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Vector3[] originalVertices;
    public float deformationAmount = 0.15f;
    public float restorationSpeed = 2.0f;

    private float bottomEdgeY;
    private float topEdgeY;
    private float leftEdgeX;
    private float rightEdgeX;
    private float netWidth;
    private float netHeight;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        originalVertices = meshFilter.mesh.vertices;

        // Calculate the bounds of the net
        bottomEdgeY = float.MaxValue;
        topEdgeY = float.MinValue;
        leftEdgeX = float.MaxValue;
        rightEdgeX = float.MinValue;
        foreach (var vertex in originalVertices)
        {
            if (vertex.y < bottomEdgeY)
            {
                bottomEdgeY = vertex.y;
            }
            if (vertex.y > topEdgeY)
            {
                topEdgeY = vertex.y;
            }
            if (vertex.x < leftEdgeX)
            {
                leftEdgeX = vertex.x;
            }
            if (vertex.x > rightEdgeX)
            {
                rightEdgeX = vertex.x;
            }
        }
        netHeight = topEdgeY - bottomEdgeY;
        netWidth = rightEdgeX - leftEdgeX;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            DeformNet(collision.contacts[0].point, deformationAmount);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            StartCoroutine(RestoreNet());
        }
    }

    void DeformNet(Vector3 collisionPoint, float amount)
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        Vector3 localCollisionPoint = transform.InverseTransformPoint(collisionPoint);

        // Calculate normalized distances from edges
        float distanceFromTop = (topEdgeY - localCollisionPoint.y) / netHeight;
        float distanceFromLeft = (localCollisionPoint.x - leftEdgeX) / netWidth;
        float distanceFromRight = (rightEdgeX - localCollisionPoint.x) / netWidth;

        // Weight based on proximity to top, left, or right edges (less deformation near these edges)
        float edgeProximityWeight = Mathf.Min(distanceFromTop, distanceFromLeft, distanceFromRight);
        edgeProximityWeight = Mathf.Clamp01(edgeProximityWeight);

        // Apply less deformation if the collision is near the top, left, or right edge
        float adjustedAmount = amount * edgeProximityWeight;

        for (int i = 0; i < vertices.Length; i++)
        {
            // Calculate distance from collision point to vertex
            float distanceToCollision = Vector3.Distance(vertices[i], localCollisionPoint);

            // Apply deformation based on distance to collision point and adjusted amount
            vertices[i] += (localCollisionPoint - vertices[i]).normalized * adjustedAmount / (distanceToCollision + 1);
        }

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
        meshFilter.mesh.RecalculateBounds();
    }

    System.Collections.IEnumerator RestoreNet()
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        while (!VerticesMatch(vertices, originalVertices))
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector3.Lerp(vertices[i], originalVertices[i], restorationSpeed * Time.deltaTime);
            }
            meshFilter.mesh.vertices = vertices;
            meshFilter.mesh.RecalculateNormals();
            yield return null;
        }
    }

    bool VerticesMatch(Vector3[] a, Vector3[] b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }
}
