using System;
using System.Numerics;
using static System.Math;
using System.Diagnostics;
using static MyMathLib.Geometry2D;
using static MyMathLib.Arithmetic;

// TODO.

namespace MyMathLib
{
    public static class Collisions2D
    {
        // Returns the smallest rectangle that contanins the given shape.
        public static Rectangle2 GetBoundingBox(ShapeInfo shape)
        {
            // If the shape is a circle.
            if (shape.Type == ShapeTypes.Circle)
            {
                return new Rectangle2(shape.Data.circle.O.X - shape.Data.circle.Radius, 
                                      shape.Data.circle.O.Y - shape.Data.circle.Radius, 
                                      shape.Data.circle.Radius * 2, 
                                      shape.Data.circle.Radius * 2);
            }

            // Get the shape's vertices information.
            int vertices_num = shape.GetVerticesNum();

            // Create the min and max values for x and y.
            Vector2 vertex = shape.GetVertex(0);
            float Xmin = vertex.X;
            float Xmax = vertex.X;
            float Ymin = vertex.Y;
            float Ymax = vertex.Y;

            // Loop though the vertices and find the min and max values for x and y.
            for (int i = 1; i < vertices_num; i++)
            {
                vertex = shape.GetVertex(i);
                if (vertex.X <= Xmin)
                    Xmin = vertex.X;
                if (vertex.X >= Xmax)
                    Xmax = vertex.X;
                if (vertex.Y <= Ymin)
                    Ymin = vertex.Y;
                if (vertex.Y >= Ymax)
                    Ymax = vertex.Y;
            }

            // Create the shape's bounding box.
            Rectangle2 bounding_box = new Rectangle2(Xmin, Ymin, Xmax - Xmin, Ymax - Ymin);

            return bounding_box;
        }

        // Returns an axis that passes through the center of the given circle and the center of the given shape.
        public static Segment2 CircleGetAxis(Circle2 circle, ShapeInfo shape)
        {
            // Make a segment that starts at the center of the circle, goes in the direction of the center of the shape and is of length 1.
            return new Segment2(circle.O,
                                Vector2FromPoints(circle.O, shape.GetCenterOfMass()).GetNormalized(), true);
        }

        // Returns the axis of the given shapes that corresponds to the given index.
        public static Segment2 ShapesGetAxis(ShapeInfo shape1, ShapeInfo shape2, int index)
        {
            Debug.Assert(index < shape1.GetSidesNum() + shape2.GetSidesNum());

            Segment2 side;
            Segment2 axis;

            // If the given index refers to an axis of the first shape...
            if (index < shape1.GetSidesNum())
            {
                // If the first shape is not a circle, get the side pointed to by the index and calculate its normal.
                if (shape1.Type != ShapeTypes.Circle) {
                    side = shape1.GetSide(index);
                    axis = new Segment2((side.A + side.A) / 2,
                                         Vector2FromSegment(side).GetNormalized().GetNormal(), true);
                }
                // If the first shape is a circle, get its axis.
                else { 
                    axis = CircleGetAxis(shape1.Data.circle, shape2);
                }
            }
            // If the given index refers to an axis of the second shape...
            else
            {
                // If the second shape is not a circle, get the side pointed to by the index and calculate its normal.
                if (shape2.Type != ShapeTypes.Circle) {
                    side = shape2.GetSide(index - shape1.GetSidesNum());
                    axis = new Segment2((side.A + side.B) / 2,
                                         Vector2FromSegment(side).GetNormalized().GetNormal(), true);
                }
                // If the second shape is a circle, get its axis.
                else { 
                    axis = CircleGetAxis(shape2.Data.circle, shape1);
                }
            }

            return axis;
        }

        // Returns true if the given point is colliding with the given circle.
        public static bool CollisionCirclePoint(Circle2 c, Vector2 p)
        {
            return Vector2FromPoints(c.O, p).Length() <= c.Radius;
        }

        // Returns true if the given circles are in collision.
        public static bool CollisionCircles(Circle2 c1, Circle2 c2)
        {
            return Vector2FromPoints(c1.O, c2.O).Length() <= c1.Radius + c2.Radius;
        }

        // Checks for collision between two rectangles.
        public static bool CollisionAABB(Rectangle2 rec1, Rectangle2 rec2)
        {
            if (rec1.O.X + rec1.W >= rec2.O.X &&
                rec1.O.X <= rec2.O.X + rec2.W &&
                rec1.O.Y + rec1.H >= rec2.O.Y &&
                rec1.O.Y <= rec2.O.Y + rec2.H) 
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        // Project a shape onto a given axis.
        public static Segment2 ProjectShapeOnAxis(Segment2 axis, ShapeInfo shape)
        {
            // Get the axis' vector.
            Vector2 axis_vec = Vector2FromSegment(axis);

            // Handle circles.
            if (shape.Type == ShapeTypes.Circle)
            {
                // Project the circle's origin onto the axis.
                Vector2 origin_projection = axis.A + axis_vec * Vector2FromPoints(axis.A, shape.Data.circle.O).Dot(axis_vec);

                // Create a segment of the circle's projection.
                Segment2 circle_projection = new Segment2(origin_projection - axis_vec * shape.Data.circle.Radius,
                                                          origin_projection + axis_vec * shape.Data.circle.Radius);
        
                return circle_projection;
            }

            // https://fr.wikipedia.org/wiki/Projection_orthogonale#Projet%C3%A9_orthogonal_sur_une_droite,_distance

            // Get all the vertices of the shape.
            int vertices_num = shape.GetVerticesNum();
            Vector2 vertex;
            Vector2[] projected_points = new Vector2[vertices_num];

            // Loop over the vertices of the shape and get their projections onto the axis.
            for (int i = 0; i < vertices_num; i++)
            {
                vertex = shape.GetVertex(i);
                projected_points[i] = axis.A + axis_vec * Vector2FromPoints(axis.A, vertex).Dot(axis_vec);
            }

            // Find the closest and farthest points from the axis origin.
            Vector2 min_point = projected_points[0];
            Vector2 max_point = min_point;

            for (int i = 0; i < vertices_num; i++)
            {
                if (projected_points[i].GetCopiedSign(axis_vec).X > max_point.GetCopiedSign(axis_vec).X ||
                    projected_points[i].GetCopiedSign(axis_vec).Y > max_point.GetCopiedSign(axis_vec).Y)
                {
                    max_point = projected_points[i];
                }

                if (projected_points[i].GetCopiedSign(axis_vec).X < min_point.GetCopiedSign(axis_vec).X ||
                    projected_points[i].GetCopiedSign(axis_vec).Y < min_point.GetCopiedSign(axis_vec).Y)
                {
                    min_point = projected_points[i];
                }
            }

            Vector2 axis_orig_to_min_point = Vector2FromPoints(axis.A, min_point);
            Segment2 projection = new Segment2(axis.A + axis_orig_to_min_point, 
                                               Vector2FromPoints(min_point, max_point), true);

            return projection;
        }

        // Returns true if the given point is colliding with the given segment.
        public static bool CollisionSegmentPoint(Segment2 segment, Vector2 point)
        {
            if (RoundInt(Vector2FromSegment(segment).Cross(Vector2FromPoints(segment.A, point))) == 0)
            {
                if ((point.X >= segment.A.X && point.X <= segment.B.X) || (point.Y >= segment.A.Y && point.Y <= segment.B.Y) ||
                    (point.X <= segment.A.X && point.X >= segment.B.X) || (point.Y <= segment.A.Y && point.Y >= segment.B.Y))
                {
                    return true;
                }
            }
            return false;
        }

        // Returns true if the given projections are colliding each others
        public static bool CollisionProjections(Segment2 projection1, Segment2 projection2)
        {
            if (CollisionSegmentPoint(projection1, projection2.A) ||
                CollisionSegmentPoint(projection1, projection2.B) ||
                CollisionSegmentPoint(projection2, projection1.A) ||
                CollisionSegmentPoint(projection2, projection1.B))
            {
                return true;
            }
            return false;
        }

        // Checks for collision between two given shapes.
        public static bool CollisionSAT(ShapeInfo shape1, ShapeInfo shape2)
        {
            // If both shapes are circles, don't use SAT.
            if (shape1.Type == ShapeTypes.Circle && shape2.Type == ShapeTypes.Circle)
                return CollisionCircles(shape1.Data.circle, shape2.Data.circle);

            // If both shapes are rectangles, don't use SAT.
            else if (shape1.Type == ShapeTypes.Rectangle && shape2.Type == ShapeTypes.Rectangle)
                return CollisionAABB(shape1.Data.rectangle, shape2.Data.rectangle);

            // Check for collisions on the shapes' bounding boxes to not have to check if they are not in collision.
            else if (CollisionAABB(GetBoundingBox(shape1), GetBoundingBox(shape2)))
            {
                // Get the number of sides of both shapes.
                int sides = shape1.GetSidesNum() + shape2.GetSidesNum();

                // Loop over all of the axes.
                for (int i = 0; i < sides; i++)
                {
                    // Project both shapes onto the axis.
                    Segment2 projection1 = ProjectShapeOnAxis(ShapesGetAxis(shape1, shape2, i), shape1);
                    Segment2 projection2 = ProjectShapeOnAxis(ShapesGetAxis(shape1, shape2, i), shape2);

                    // If the projections don't overlap, the shapes are not in collision.
                    if (!CollisionProjections(projection1, projection2))
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }
    }
}
