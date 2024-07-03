using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code generated using ChatGPT 3.5 (12 Jun 2024)
// Prompts: In a Unity game, make a cube elastic when a ball hits it, if the cube is a net and therefore very thin,
// if I want the net to deform less only if the collision is near an edge.

public class NetMovement : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Vector3[] originalVertices;
    public float deformationAmount = 0.1f;
    public float restorationSpeed = 2.0f;

    private Vector3 center;
    private float maxDistanceFromCenter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        originalVertices = meshFilter.mesh.vertices;

        // Calculate the center of the net
        center = Vector3.zero;
        foreach (var vertex in originalVertices)
        {
            center += vertex;
        }
        center /= originalVertices.Length;

        // Calculate the maximum distance from the center to any vertex
        maxDistanceFromCenter = 0;
        foreach (var vertex in originalVertices)
        {
            float distance = Vector3.Distance(center, vertex);
            if (distance > maxDistanceFromCenter)
            {
                maxDistanceFromCenter = distance;
            }
        }
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

        // Calculate the distance from the collision point to the center of the net
        float distanceToCenter = Vector3.Distance(localCollisionPoint, center);

        // Calculate a weighting factor based on the distance to the center
        float weight = 1 - (distanceToCenter / maxDistanceFromCenter);
        weight = Mathf.Clamp01(weight); // Ensure weight is between 0 and 1

        // Apply less deformation if the collision is near the edge
        float adjustedAmount = amount * weight;

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