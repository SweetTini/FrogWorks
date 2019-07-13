using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Shape
    {
        public virtual Vector2 Position { get; set; }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.X; }
            set { Position = new Vector2(Position.X, value); }
        }

        public abstract Rectangle Bounds { get; }

        public abstract void Draw(RendererBatch batch, Color color, bool fill = false);

        public abstract bool Contains(Vector2 point);

        public bool Contains(float x, float y)
        {
            return Contains(new Vector2(x, y));
        }

        public abstract Shape Clone();

        public abstract Proxy ToProxy();
    }

    internal class Simplex
    {
        public SimplexVertex[] Vertices { get; } = new SimplexVertex[3];

        public int OutputCount { get; set; } = 1;

        public float Divider { get; set; } = 1f;

        public Simplex(Vector2 startpoint, Vector2 endpoint)
        {
            Vertices[0].Startpoint = startpoint;
            Vertices[0].Endpoint = endpoint;
        }

        public Vector2 GetSearchDirection()
        {
            switch (OutputCount)
            {
                case 1:
                    return -Vertices[0].Edge;
                case 2:
                    var eAB = Vertices[1].Edge - Vertices[0].Edge;
                    var sign = eAB.Cross(-Vertices[0].Edge);
                    return eAB.Cross(sign > 0f ? -1f : 1f);
                default:
                    return Vector2.Zero;
            }
        }

        public Vector2 GetNearestVertex()
        {
            var inverse = Divider.Inverse();

            switch (OutputCount)
            {
                case 1:
                    return Vertices[0].Edge;
                case 2:
                    return (inverse * Vertices[0].Factor) * Vertices[0].Edge
                         + (inverse * Vertices[1].Factor) * Vertices[1].Edge;
                default:
                    return Vector2.Zero;
            }
        }

        public void GetWitnessPoints(out Vector2 startpoint, out Vector2 endpoint)
        {
            startpoint = endpoint = Vector2.Zero;
            var inverse = Divider.Inverse();

            switch (OutputCount)
            {
                default:           
                case 1:
                    startpoint = Vertices[0].Startpoint;
                    endpoint = Vertices[0].Endpoint;
                    break;
                case 2:
                    startpoint = (inverse * Vertices[0].Factor) * Vertices[0].Startpoint 
                        + (inverse * Vertices[1].Factor) * Vertices[1].Startpoint;
                    endpoint = (inverse * Vertices[0].Factor) * Vertices[0].Endpoint 
                        + (inverse * Vertices[1].Factor) * Vertices[1].Endpoint;
                    break;
                case 3:
                    startpoint = (inverse * Vertices[0].Factor) * Vertices[0].Startpoint
                        + (inverse * Vertices[1].Factor) * Vertices[1].Startpoint
                        + (inverse * Vertices[2].Factor) * Vertices[2].Startpoint;
                    endpoint = startpoint;
                    break;
            }
        }

        public void SolveBarycentricLine()
        {
            var eA = Vertices[0].Edge;
            var eB = Vertices[1].Edge;

            var u = Vector2.Dot(eB, Vector2.Normalize(eB - eA));
            var v = Vector2.Dot(eA, Vector2.Normalize(eA - eB));

            if (v <= 0f) // Region A
            {
                Divider = Vertices[0].Factor = 1f;
                OutputCount = 1;
            }
            else if (u <= 0f) // Region B
            {
                Vertices[0] = Vertices[1];
                Divider = Vertices[0].Factor = 1f;
                OutputCount = 1;
            }
            else // Region AB
            {
                Vertices[0].Factor = u;
                Vertices[1].Factor = v;
                Divider = u + v;
                OutputCount = 2;
            }
        }

        public void SolveBarycentricTriangle()
        {
            var eA = Vertices[0].Edge;
            var eB = Vertices[1].Edge;
            var eC = Vertices[2].Edge;

            var uAB = Vector2.Dot(eB, Vector2.Normalize(eB - eA));
            var vAB = Vector2.Dot(eA, Vector2.Normalize(eA - eB));
            var uBC = Vector2.Dot(eC, Vector2.Normalize(eC - eB));
            var vBC = Vector2.Dot(eB, Vector2.Normalize(eB - eC));
            var uCA = Vector2.Dot(eA, Vector2.Normalize(eA - eC));
            var vCA = Vector2.Dot(eC, Vector2.Normalize(eC - eA));

            var area = Vector2.Normalize(eB - eA).Cross(Vector2.Normalize(eC - eA));
            var uABC = eB.Cross(eC) * area;
            var vABC = eC.Cross(eA) * area;
            var wABC = eA.Cross(eB) * area;

            if (vAB <= 0f && uCA <= 0f) // Region A
            {
                Divider = Vertices[0].Factor = 1f;
                OutputCount = 1;
            }
            else if (uAB <= 0f && vBC <= 0f) // Region B
            {
                Vertices[0] = Vertices[1];
                Divider = Vertices[0].Factor = 1f;
                OutputCount = 1;
            }
            else if (uBC <= 0f && vCA <= 0f) // Region C
            {
                Vertices[0] = Vertices[2];
                Divider = Vertices[0].Factor = 1f;
                OutputCount = 1;
            }
            else if (uAB > 0f && vAB > 0f && wABC <= 0f) // Region AB
            {
                Vertices[0].Factor = uAB;
                Vertices[1].Factor = vAB;
                Divider = uAB + vAB;
                OutputCount = 2;
            }
            else if (uBC > 0f && vBC > 0f && uABC <= 0f) // Region BC
            {
                Vertices[0] = Vertices[1];
                Vertices[1] = Vertices[2];
                Vertices[0].Factor = uBC;
                Vertices[1].Factor = vBC;
                Divider = uBC + vBC;
                OutputCount = 2;
            }
            else if (uCA > 0f && vCA > 0f && vABC <= 0f) // Region CA
            {
                Vertices[1] = Vertices[0];
                Vertices[0] = Vertices[2];
                Vertices[0].Factor = uCA;
                Vertices[1].Factor = vCA;
                Divider = uCA + vCA;
                OutputCount = 2;
            }
            else // Region ABC
            {
                Vertices[0].Factor = uABC;
                Vertices[1].Factor = vABC;
                Vertices[2].Factor = wABC;
                Divider = uABC + vABC + wABC;
                OutputCount = 3;
            }
        }
    }

    internal struct SimplexVertex
    {
        public Vector2 Startpoint { get; set; }

        public Vector2 Endpoint { get; set; }

        public Vector2 Edge => Endpoint - Startpoint;

        public int StartpointIndex { get; set; }

        public int EndpointIndex { get; set; }

        public float Factor { get; set; }
    }

    public struct Proxy
    {
        public Vector2[] Vertices { get; }

        public float Radius { get; }

        public int Count => Vertices.Length;

        public Proxy(Vector2[] vertices, float radius = 0f)
        {
            Vertices = vertices;
            Radius = radius;
        }

        public int GetSupport(Vector2 direction)
        {
            var index = 0;
            var dotProd = Vector2.Dot(Vertices[0], direction);

            for (int i = 1; i < Count; i++)
            {
                var dp = Vector2.Dot(Vertices[i], direction);

                if (dp > dotProd)
                {
                    dotProd = dp;
                    index = i;
                }
            }

            return index;
        }
    }
}
