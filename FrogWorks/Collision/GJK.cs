using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public static class GJK
    {
        private const int MaxIterations = 20;

        public static bool Collide(Shape shape, Shape other)
        {
            Vector2 startpoint, endpoint;
            return Distance(shape, other, out startpoint, out endpoint, true) == 0f;
        }

        public static float Distance(Shape shape, Shape other, out Vector2 startpoint, out Vector2 endpoint, bool applyRadius = false)
        {
            var shapeProxy = shape.ToProxy();
            var otherProxy = other.ToProxy();
            var simplex = new Simplex(shapeProxy.Vertices[0], otherProxy.Vertices[0]);

            var savedStartpointIndices = new int[3];
            var savedEndpointIndices = new int[3];
            var collided = false;
            var iteration = 0;

            while (iteration < MaxIterations)
            {
                var savedOutputCount = simplex.OutputCount;
                SaveSupportPoints(ref savedStartpointIndices, ref savedEndpointIndices, simplex, savedOutputCount);

                switch (simplex.OutputCount)
                {
                    case 2: simplex.SolveBarycentricLine(); break;
                    case 3: simplex.SolveBarycentricTriangle(); break;
                    default: break;
                }

                if (simplex.OutputCount == 3)
                {
                    collided = true;
                    break;
                }

                var direction = simplex.GetSearchDirection();
                if (Vector2.Dot(direction, direction) == 0f) break;

                var vertex = simplex.Vertices[simplex.OutputCount];
                var startpointIndex = vertex.StartpointIndex = shapeProxy.GetSupport(-direction);
                var endpointIndex = vertex.EndpointIndex = otherProxy.GetSupport(direction);
                vertex.Startpoint = shapeProxy.Vertices[startpointIndex];
                vertex.Endpoint = otherProxy.Vertices[endpointIndex];
                iteration++;

                if (HasRepeatedSupportPoints(ref savedStartpointIndices, ref savedEndpointIndices, vertex, savedOutputCount))
                    break;

                simplex.OutputCount++;
            }

            simplex.GetWitnessPoints(out startpoint, out endpoint);
            var distance = (startpoint - endpoint).Length();

            if (collided)
            {
                startpoint = endpoint;
                distance = 0f;
            }
            else if (applyRadius)
            {
                if (distance > shapeProxy.Radius + otherProxy.Radius && distance > 0f)
                {
                    var normal = Vector2.Normalize(endpoint - startpoint);
                    startpoint += normal * shapeProxy.Radius;
                    endpoint -= normal * otherProxy.Radius;
                    distance -= shapeProxy.Radius + otherProxy.Radius;
                }
                else
                {
                    startpoint = endpoint = (startpoint + endpoint) / 2f;
                    distance = 0f;
                }
            }

            return distance;
        }

        public static float TimeOfImpact(Shape shape, Shape other, Vector2 shapeOffset, Vector2 otherOffset, out Manifold hit, bool applyRadius = false)
        {
            hit = new Manifold();

            var shapeCopy = shape.Clone();
            var otherCopy = other.Clone();
            var time = 0f;

            Vector2 startpoint, endpoint;
            var distance = StepThrough(shapeCopy, otherCopy, shapeOffset, otherOffset, out startpoint, out endpoint, time, applyRadius);
            var offset = otherOffset - shapeOffset;
            var normal = (endpoint - startpoint).SafeNormalize();

            while (distance > 1e-5f && time < 1f)
            {
                var bound = Math.Abs(Vector2.Dot(Vector2.Normalize(endpoint - startpoint), offset));
                if (bound < float.Epsilon) return 1f;

                time += distance / bound;

                Vector2 nextStartpoint, nextEndpoint;
                distance = StepThrough(shapeCopy, otherCopy, shapeOffset, otherOffset, out nextStartpoint, out nextEndpoint, time, applyRadius);

                if (distance != 0f)
                {
                    startpoint = nextStartpoint;
                    endpoint = nextEndpoint;
                    normal = endpoint - startpoint;
                }
                else
                {
                    break;
                }
            }

            hit.Depth = (endpoint - startpoint).Length();
            hit.Normal = normal.SafeNormalize();
            hit.ContactPoint = (startpoint + endpoint) / 2f;
            return MathHelper.Clamp(time, 0f, 1f);
        }

        private static float StepThrough(Shape shape, Shape other, Vector2 shapeOffset, Vector2 otherOffset, out Vector2 startpoint, out Vector2 endpoint, float time, bool applyRadius)
        {
            shape.Position += shapeOffset * time;
            other.Position += otherOffset * time;

            return Distance(shape, other, out startpoint, out endpoint, applyRadius);
        }

        private static void SaveSupportPoints(ref int[] startpointIndices, ref int[] endpointIndices, Simplex simplex, int outputCount)
        {
            for (int i = 0; i < outputCount; i++)
            {
                startpointIndices[i] = simplex.Vertices[i].StartpointIndex;
                endpointIndices[i] = simplex.Vertices[i].EndpointIndex;
            }
        }

        private static bool HasRepeatedSupportPoints(ref int[] startpointIndices, ref int[] endpointIndices, SimplexVertex vertex, int outputCount)
        {
            for (int i = 0; i < outputCount; i++)
                if (vertex.StartpointIndex == startpointIndices[i] && vertex.EndpointIndex == endpointIndices[i])
                    return true;

            return false;
        }
    }
}
