using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Profiling;

namespace FlatPixel.XR.Mock.Example
{
    [ExecuteInEditMode]
    public class MockBoundaryPlane : MockTrackable
    {
        Mesh m_mesh;
        MeshFilter m_meshFilter = null;

        [Header("Plane parameters")]
        [SerializeField]
        float m_Radius = 1f;

        [SerializeField]
        float m_Speed = 5f;

        [SerializeField]
        float m_HullChangeProbability = .2f;

        [SerializeField]
        float m_TrackingLostProbability = 0.01f;

        [SerializeField]
        int m_NumPoints = 10;

        Vector2[] points;
        Vector2[] velocities;

        bool IsLeftOfLine(Vector2 point, Vector2 a, Vector2 b)
        {
            var u = b - a;
            var v = point - a;
            return (u.x * v.y - u.y * v.x) > 0;
        }

        Vector2[] GenerateConvexHull(Vector2[] points)
        {
            Profiler.BeginSample("GenerateConvexHull");

            // Find leftmost point which is guaranteed to be part of the CH(S)
            Vector2 pointOnHull = Vector2.one * Mathf.Infinity;
            foreach (var point in points)
            {
                if (point.x < pointOnHull.x)
                    pointOnHull = point;
            }

            List<Vector2> convexHull = new List<Vector2>();
            Vector2 endPoint;
            do
            {
                convexHull.Add(pointOnHull);

                // initial endpoint for a candidate edge on the hull
                endPoint = points[0];

                // Check all the other points, finding the leftmost of the line
                for (int i = 1; i < points.Length; ++i)
                {
                    if (endPoint == pointOnHull || (IsLeftOfLine(points[i], pointOnHull, endPoint)))
                    {
                        // found greater left turn, update endpoint
                        endPoint = points[i];
                    }
                }

                pointOnHull = endPoint;
            } while (endPoint != convexHull[0]); // wrapped around to first hull point

            Profiler.EndSample();

            return convexHull.ToArray();
        }

        void Awake()
        {
            m_mesh = new Mesh();
        }

        private void OnEnable()
        {
            m_meshFilter = GetComponent<MeshFilter>();

            points = new Vector2[m_NumPoints];
            velocities = new Vector2[m_NumPoints];

            for (int i = 0; i < points.Length; ++i)
            {
                points[i] = Random.insideUnitCircle * m_Radius;
                velocities[i] = Random.insideUnitCircle * m_Speed;
            }

            trackableId = PlaneApi.Add(pose, GenerateConvexHull(points));

            if (m_meshFilter != null)
                if (ARPlaneMeshGenerators.GenerateMesh(m_mesh, new Pose(transform.localPosition, transform.localRotation), NativeApi.planes[trackableId].boundaryPoints))
                    m_meshFilter.sharedMesh = m_mesh;

            tracking = TrackingState.Tracking;
            PlaneApi.SetTrackingState(trackableId, tracking);
        }

        private void OnDisable()
        {
            tracking = TrackingState.None;
            PlaneApi.SetTrackingState(trackableId, tracking);

            PlaneApi.Remove(trackableId);
        }

        private void Update()
        {
            PlaneApi.SetTrackingState(trackableId, tracking);
            if (tracking != TrackingState.Tracking) return;

            var hullChanged = false;
            if (Random.value < m_HullChangeProbability)
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    if (points[i].magnitude > m_Radius && Vector3.Dot(points[i], velocities[i]) > 0)
                        velocities[i] = -velocities[i];

                    points[i] += velocities[i] * Time.deltaTime;
                }

                hullChanged = true;
            }

            if (hullChanged || transform.hasChanged)
                PlaneApi.Update(trackableId, pose, GenerateConvexHull(points));

            transform.hasChanged = false;

            if (Random.value < m_TrackingLostProbability)
                PlaneApi.SetTrackingState(trackableId, TrackingState.None);
            else
                PlaneApi.SetTrackingState(trackableId, TrackingState.Tracking);
        }
    }
}
