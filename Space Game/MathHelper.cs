using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simcity3000_2
{
    static class MathHelper
    {
        //Point in polygon algorithm, returns true if a point is in the polygon represented by the matrix
        public static bool PointInPolygon(Vector2f[] polygonVertices, Vector2f point)
        {
            int numVertices = polygonVertices.Length;

            //If we cast a ray from the point and it crosses an odd number of segments then point inside
            //Else point outside
            int passes = 0;
            for (int i = 0; i < numVertices; i++)
            {
                //Cast a ray and see if intersects a segment
                if (RayIntersectsSegment(point,
                new Vector2f(polygonVertices[i].X, polygonVertices[i].Y),
                //Wrap the points so the last point makes a line with the first
                new Vector2f(polygonVertices[(i + 1) % numVertices].X, polygonVertices[(i + 1) % numVertices].Y)))
                {
                    //If it intersects then increment passes
                    passes++;
                }
            }
            //Return true if odd
            return !(passes % 2 == 0);
        }

        public static bool RayIntersectsSegment(Vector2f P, Vector2f e1, Vector2f e2)
        {
            //A is below B
            //Set A to the point with the lower x value
            //Set B to the point with higher x
            Vector2f A, B;
            float m_AB, m_AP;
            if (e1.Y > e2.Y)
            {
                B = e1;
                A = e2;
            }
            else
            {
                B = e2;
                A = e1;
            }

            //If we are on the line then we have to add a small amount
            if (P.Y == A.Y || P.Y == B.Y)
            {
                P.Y += float.Epsilon;
            }
            //If we are above or below the line we cant cross.
            if (P.Y < A.Y || P.Y > B.Y)
            {
                return false;
            }
            //If we are right of the rightmost point, we cant cross.
            else if (P.X > Math.Max(A.X, B.X))
            {
                return false;
            }
            //If we are between the lines and "before"(x) the line then we intersect
            else if (P.X < Math.Min(A.X, B.X))
            {
                return true;
            }
            else
            {
                //We need to get the gradients of AP and AB
                //AB
                if (A.X != B.X)
                {
                    //Calculate gradient
                    m_AB = (B.Y - A.Y) / (B.X - A.X);
                }
                else
                {
                    //horizontal line, gradient infinity
                    m_AB = float.PositiveInfinity;
                }
                //AP
                if (A.X != P.X)
                {
                    //Calculate gradient
                    m_AP = (P.Y - A.Y) / (P.X - A.X);
                }
                else
                {
                    //horizontal line, gradient infinity
                    m_AP = float.PositiveInfinity;
                }

                //If AP is steeper than AB then we intersect the line
                if (m_AP > m_AB)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
    }
}
