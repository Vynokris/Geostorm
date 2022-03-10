using System;
using System.Numerics;
using static System.MathF;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MyMathLib
{
    // ---------- Geometry2D ---------- //

    public static class Geometry2D
    {
        // ---- Vector2 ---- //

        // Returns a null vector.
        public static Vector2 Vector2Zero()                         { return new Vector2(0, 0); }

        // Creates a vector from two values.
        public static Vector2 Vector2Create(in float X, in float Y) { return new Vector2(X, Y); }

        // Creates a vector from one point to another.
        public static Vector2 Vector2FromPoints(in Vector2 p0, in Vector2 p1) { return new Vector2(p1.X - p0.X, p1.Y - p0.Y); }

        // Creates a vector given an angle and a length.
        public static Vector2 Vector2FromAngle(in float rad, in float length) { return new Vector2((float)Cos(rad) * length,
                                                                                                   (float)Sin(rad) * length); }
        // Creates a vector from a segment.
        public static Vector2 Vector2FromSegment(in Segment2 seg)    { return Vector2FromPoints(seg.A, seg.B); }
        
        // Vector dot product.
        public static float Dot(in this Vector2 v0, in Vector2 v1)   { return (v0.X * v1.X) + (v0.Y * v1.Y); }

        // Vector cross product.
        public static float Cross(in this Vector2 v0, in Vector2 v1) { return (v0.X * v1.Y) - (v0.Y * v1.X); }

        // Normalizes the vector so that its length is 1.
        public static void Normalize(ref this Vector2 v)             { v = v.GetNormalized(); }

        // Returns a normalized copy of the vector.
        public static Vector2 GetNormalized(in this Vector2 v)       { return v / v.Length(); }

        // Modifies the length of the given vector to correspond to the given value.
        public static void SetLength(ref this Vector2 v, in float length)           { v = v.GetModifiedLength(length); }
        
        // Modifies the length of the given vector to correspond to the given value.
        public static Vector2 GetModifiedLength(in this Vector2 v, in float length) { return v.GetNormalized() * length; }

        // Copies the signs from the source vector to the destination vector.
        public static void CopySign(ref this Vector2 dest, in Vector2 source)
        {
            dest = dest.GetCopiedSign(source);
        }

        // Returns a vector width the destination's size and the source's signs.
        public static Vector2 GetCopiedSign(in this Vector2 dest, in Vector2 source)
        {
            return new Vector2(MathF.CopySign(dest.X, source.X),
                               MathF.CopySign(dest.Y, source.Y));
        }

        // Returns the normal of the vector.
        public static Vector2 GetNormal(in this Vector2 v) { return new Vector2(-v.Y, v.X); }

        // Interprets the vector as a point and returns the distance to another point.
        public static float GetDistanceFromPoint(in this Vector2 p0, in Vector2 p1) { return (p1 - p0).Length(); }

        // Returns the angle (in radians) of the vector.
        public static float GetAngle(in this Vector2 v)
        {
            return (float)Math.CopySign(Acos(v.GetNormalized().X),
                                        Asin(v.GetNormalized().Y));
        }

        // Returns the angle (in radians) between two vectors.
        public static float GetAngleWithVector(in this Vector2 v0, in Vector2 v1)
        {
            float v0angle = v0.GetAngle();
            float v1angle = v1.GetAngle();
            return (v0angle >= v1angle ? (v0angle - v1angle) : (v1angle - v0angle));
        }

        // Changes the vector's angle while conserving its length.
        public static void SetAngle(this ref Vector2 v, in float angle)
        {
            v = v.GetModifiedAngle(angle);
        }

        // Changes the vector's angle while conserving its length.
        public static Vector2 GetModifiedAngle(in this Vector2 v, in float angle)
        {
            return Vector2FromAngle(angle, v.Length());
        }

        // Rotates the given vector by the given angle.
        public static void Rotate(ref this Vector2 v, in float angle)
        {
            v = v.GetRotated(angle);
        }

        // Rotates the given vector by the given angle.
        public static Vector2 GetRotated(this Vector2 v, in float angle)
        {
            float vLength = v.Length();
            float vAngle = v.GetAngle();
            return new Vector2((float)Cos(vAngle + angle) * vLength, (float)Sin(vAngle + angle) * vLength);
        }

        // Scales the vector as a point around the given origin.
        public static void ScaleAsPoint(ref this Vector2 point, in Vector2 scale, in Vector2 origin)
        {
            point = point.GetScaledAsPoint(scale, origin);
        }

        // Scales the vector as a point around the given origin.
        public static Vector2 GetScaledAsPoint(in this Vector2 point, in Vector2 scale, in Vector2 origin)
        {
            Vector2 origToPoint = Vector2FromPoints(origin, point);
            origToPoint *= scale;
            return origin + origToPoint;
        }

        // Rotates the vector as a point around the given origin.
        public static void RotateAsPoint(ref this Vector2 point, in float angle, in Vector2 origin)
        {
            point = point.GetRotatedAsPoint(angle, origin);
        }

        // Rotates the vector as a point around the given origin.
        public static Vector2 GetRotatedAsPoint(in this Vector2 point, in float angle, in Vector2 origin)
        {
            Vector2 origToPoint = Vector2FromPoints(origin, point);
            if (origToPoint.Length() > 0)
                origToPoint.Rotate(angle);
            return origin + origToPoint;
        }
        
        // Calculates linear interpolation for a value from a start point to an end point.
        public static Vector2 Point2Lerp(in float val, in Vector2 start, in Vector2 end)
        {
            return new Vector2(Arithmetic.Lerp(val, start.X, end.X),
                               Arithmetic.Lerp(val, start.Y, end.Y));
        }


        // ---- Segment2 ---- //
        public class Segment2 : IShape
        {
            public Vector2 A, B;

            // Null Segment.
            public Segment2() { A = Vector2Zero(); B = Vector2Zero(); }

            // Segment from points.
            public Segment2(in Vector2 a, in Vector2 b) { A = a; B = b; }

            // Segment from point and vector.
            public Segment2(in Vector2 origin, in Vector2 vec, in bool vector) { A = origin; B = origin + vec; }

            // Returns the center of mass of the Segment2.
            public Vector2 GetCenterOfMass() { return Vector2Create((A.X + B.X) / 2, (A.Y + B.Y) / 2); }

            // Returns the number of sides of the Segment2.
            public int GetSidesNum() { return 1; }

            // Returns the side of the Segment2 that corresponds to the given index.
            public Segment2 GetSide(in int index)
            {
                Debug.Assert(0 <= index && index < 1);
                return this;
            }

            // Returns the number of vertices of the Segment2.
            public int GetVerticesNum() { return 2; }

            // Returns the vertex of the Segment2 that corresponds to the given index.
            public Vector2 GetVertex(in int index)
            {
                Debug.Assert(0 <= index && index< 2);

                return index switch {
                    0 => A, 
                    1 => B, 
                    _ => Vector2Zero(),
                };
            }
            
            // Returns the radius of the circle in which the segment is inscribed.
            public float InscribedCircleRadius()
            {
               Vector2 O = GetCenterOfMass();
                return Vector2FromPoints(O, A).Length();
            }

            // Moves the Segment2 by the given vector.
            public void Move(in Vector2 vec) { A += vec; B += vec; }
        }


        // ---- Triangle2 ---- //
        public class Triangle2 : IShape
        {
            public Vector2 A, B, C;

            // Null triangle.
            public Triangle2() { A = Vector2Zero(); B = Vector2Zero(); C = Vector2Zero(); }

            // Triangle from points.
            public Triangle2(in Vector2 a, in Vector2 b, in Vector2 c) { A = a; B = b; C = c; }

            // Returns the center of mass of the triangle.
            public Vector2 GetCenterOfMass() { return Vector2Create((A.X + B.X + C.X) / 3,
                                                                    (A.Y + B.Y + C.Y) / 3); }

            // Returns the number of sides of the triangle.
            public int GetSidesNum() { return 3; }

            // Returns the side of the triangle that corresponds to the given index.
            public Segment2 GetSide(in int index)
            {
                Debug.Assert(0<= index && index< 3);

                return index switch
                {
                    0 => new Segment2(A, B),
                    1 => new Segment2(B, C),
                    2 => new Segment2(C, A),
                    _ => new Segment2(Vector2Zero(), Vector2Zero()),
                };
            }

            // Returns the number of vertices of the triangle.
            public int GetVerticesNum() { return 3; }

            // Returns the vertex of the triangle that corresponds to the given index.
            public Vector2 GetVertex(in int index)
            {
                Debug.Assert(0 <= index && index< 3);

                return index switch
                {
                    0 => A,
                    1 => B,
                    2 => C,
                    _ => Vector2Zero(),
                };
            }

            // Returns the radius of the circle in which the triangle is inscribed (NOT TESTED).
            public float InscribedCircleRadius()
            {
                // TODO: This is untested.
                float a = Vector2FromPoints(A, B).Length();
                float b = Vector2FromPoints(B, C).Length();
                float c = Vector2FromPoints(C, A).Length();
                float s = (a + b + c) / 2; // Half of perimeter.
                float area = Sqrt(s * (s - a) * (s - b) * (s - c));
                return (a*b*c) / 4 * area;
            }

            // Moves the triangle by the given vector.
            public void Move(in Vector2 vec) { A += vec; B += vec; C += vec; }
        }


        // ---- Rectangle2 ---- //
        public class Rectangle2 : IShape
        {
            public Vector2 O;
            public float W, H;

            // Null rectangle.
            public Rectangle2() { O = Vector2Zero(); W = 0; H = 0; }

            // Rectangle from posX posY, width and height.
            public Rectangle2(in float x, in float y, in float w, in float h) { O = Vector2Create(x, y); W = w; H = h; }

            // Returns the center of mass of the rectangle.
            public Vector2 GetCenterOfMass() { return Vector2Create(O.X + W / 2, O.Y + H / 2); }

            // Returns the number of sides of the rectangle.
            public int GetSidesNum() { return 4; }

            // Returns the side of the rectangle that corresponds to the given index.
            public Segment2 GetSide(in int index)
            {
                Debug.Assert(0 <= index && index < 4);

                return index switch
                {
                    0 => new Segment2(Vector2Create(O.X + W, O.Y), O),
                    1 => new Segment2(O, Vector2Create(O.X, O.Y + H)),
                    2 => new Segment2(Vector2Create(O.X, O.Y + H), Vector2Create(O.X + W, O.Y + H)),
                    3 => new Segment2(Vector2Create(O.X + W, O.Y + H), Vector2Create(O.X + W, O.Y)),
                    _ => new Segment2(Vector2Zero(), Vector2Zero()),
                };
            }

            // Returns the number of vertices of the rectangle.
            public int GetVerticesNum() { return 4; }

            // Returns the vertex of the rectangle that corresponds to the given index.
            public Vector2 GetVertex(in int index)
            {
                Debug.Assert(0 <= index && index< 4);

                return index switch
                {
                    0 => Vector2Create(O.X + W, O.Y),
                    1 => O,
                    2 => Vector2Create(O.X, O.Y + H),
                    3 => Vector2Create(O.X + W, O.Y + H),
                    _ => Vector2Zero(),
                };
            }

            // Returns the circle's radius.
            public float InscribedCircleRadius()
            {
                return Vector2FromPoints(O, O + Vector2Create(W/2, H/2)).Length();
            }

            // Moves the rectangle by the given vector.
            public void Move(in Vector2 vec) { O += vec; }
        }


        // ---- Polygon2 ---- //
        public class Polygon2 : IShape
        {
            public Vector2 O;
            public float Radius, Rot;
            public int Sides;

            // Null polygon.
            public Polygon2() { O = Vector2Zero(); Radius = 0; Rot = 0; Sides = 3; }

            // Polygon from origin, radius, rotation and number of sides.
            public Polygon2(in float x, in float y, in float radius, in float rotation, in int sides)
            {
                O = Vector2Create(x, y); Radius = radius; Rot = rotation; Sides = sides;
            }

            // Returns the center of mass of the polygon.
            public Vector2 GetCenterOfMass() { return O; }

            // Returns the number of sides of the polygon.
            public int GetSidesNum() { return Sides; }

            // Returns the side of the polygon that corresponds to the given index.
            public Segment2 GetSide(in int index)
            {
                Debug.Assert(0 <= index && index < Sides);

                float corner_angle = Arithmetic.Deg2Rad(360 / Sides);
                float angle_offset = (float)PI / 2 + (index * corner_angle);
                Vector2 poly_point_a = O + Vector2FromAngle(angle_offset + Rot, Radius);
                Vector2 poly_point_b = O + Vector2FromAngle(angle_offset + corner_angle + Rot, Radius);

                return new Segment2(poly_point_a, poly_point_b);
            }

            // Returns the number of vertices of the polygon.
            public int GetVerticesNum() { return Sides; }

            // Returns the vertex of the polygon that corresponds to the given index.
            public Vector2 GetVertex(in int index)
            {
                Debug.Assert(0 <= index && index < Sides);

                float corner_angle = Arithmetic.Deg2Rad(360 / Sides);
                float angle_offset = (float)PI / 2 + (index * corner_angle);
                return O + Vector2FromAngle(angle_offset + Rot, Radius);
            }

            // Returns the polygon's radius.
            public float InscribedCircleRadius()
            {
                return Radius;
            }

            // Moves the polygon by the given vector.
            public void Move(in Vector2 vec) { O += vec; }
        }


        // ---- Circle2 ---- //
        public class Circle2 : IShape
        {
            public Vector2 O;
            public float Radius;

            // Null circle.
            public Circle2() { O = Vector2Zero(); Radius = 0; }

            // Circle from position and radius.
            public Circle2(in float x, in float y, in float radius) { O = Vector2Create(x, y); Radius = radius; }

            // Returns the center of mass of the circle.
            public Vector2 GetCenterOfMass() { return O; }

            // Returns the number of sides of the circle.
            public int GetSidesNum() { return 1; }

            // Does nothing and returns a null Segment2.
            public Segment2 GetSide(in int index) { return new Segment2(); }

            // Returns the number of vertices of the circle.
            public int GetVerticesNum() { return 0; }

            // Does nothing and returns a null vector.
            public Vector2 GetVertex(in int index) { return Vector2Zero(); }

            // Returns the circle's radius.
            public float InscribedCircleRadius()
            {
                return Radius;
            }

            // Moves the circle by the given vector.
            public void Move(in Vector2 vec) { O += vec; }
        }


        // ---- Shapes ---- //

        public interface IShape
        {
            public Vector2  GetCenterOfMass();
            public int      GetSidesNum();
            public Segment2 GetSide(in int index);
            public int      GetVerticesNum();
            public Vector2  GetVertex(in int index);
            public float    InscribedCircleRadius();
            public void     Move(in Vector2 vec);
        }

        // Union that can contain any shape.
        [StructLayout(LayoutKind.Explicit)]
        public struct Shape {
            [FieldOffset(0)] public Vector2    vector;
            [FieldOffset(0)] public Segment2   segment;
            [FieldOffset(0)] public Triangle2  triangle;
            [FieldOffset(0)] public Rectangle2 rectangle;
            [FieldOffset(0)] public Polygon2   polygon;
            [FieldOffset(0)] public Circle2    circle;
        }

        // Shape types enum.
        public enum ShapeTypes {
            Vector,
            Segment,
            Triangle,
            Rectangle,
            Polygon,
            Circle,
        }

        // Structure for shape info, holds shape type and data.
        public class ShapeInfo {
            public ShapeTypes Type;
            public Shape Data;

            public ShapeInfo() { Type = ShapeTypes.Triangle; Data.triangle = new Triangle2(); }
            public ShapeInfo(ShapeTypes type, Shape data) { Type = type; Data = data; }

            // Returns the center of mass of the given shape (returns a vector of coordinates 1,000,000 if the shape type isn't supported).
            public Vector2 GetCenterOfMass()
            {
                return (object)Type switch
                {
                    ShapeTypes.Segment   => Data.segment.GetCenterOfMass(),
                    ShapeTypes.Triangle  => Data.triangle.GetCenterOfMass(),
                    ShapeTypes.Rectangle => Data.rectangle.GetCenterOfMass(),
                    ShapeTypes.Polygon   => Data.polygon.GetCenterOfMass(),
                    ShapeTypes.Circle    => Data.circle.GetCenterOfMass(),
                    _ => Vector2Create(1000000, 1000000),
                };
            }
            
            // Returns the number of sides of a given shape (returns 2 for rectangles).
            public int GetSidesNum()
            {
                return Type switch
                {
                    ShapeTypes.Segment   => 1,
                    ShapeTypes.Triangle  => 3,
                    ShapeTypes.Rectangle => 2,// There are only two axes to check for collision in a rectangle.
                    ShapeTypes.Polygon   => Data.polygon.Sides,
                    ShapeTypes.Circle    => 1,
                    _ => 0,
                };
            }

            // Returns the side of the given shape that corresponds to the given index.
            // Returns a (0, 0) segment if the shape type is not supported (circle and vector).
            public Segment2 GetSide(int index)
            {
                switch (Type)
                {
                case ShapeTypes.Segment:
                    Debug.Assert (index < 1);
                    return Data.segment;
                case ShapeTypes.Triangle:
                    return Data.triangle.GetSide(index);
                case ShapeTypes.Rectangle:
                    return Data.rectangle.GetSide(index);
                case ShapeTypes.Polygon:
                    return Data.polygon.GetSide(index);
                default:
                    return new Segment2(Vector2Zero(), Vector2Zero());
                }
            }

            // Returns the number of vertices of a given shape.
            public int GetVerticesNum()
            {
                return (object)Type switch
                {
                    ShapeTypes.Segment   => 2,
                    ShapeTypes.Triangle  => 3,
                    ShapeTypes.Rectangle => 4,
                    ShapeTypes.Polygon   => Data.polygon.Sides,
                    _ => 0,
                };
            }

            // Returns the vertex of the given shape that corresponds to the given index.
            public Vector2 GetVertex(int index)
            {
                return Type switch
                {
                    ShapeTypes.Segment   => Data.segment.GetVertex(index),
                    ShapeTypes.Triangle  => Data.triangle.GetVertex(index),
                    ShapeTypes.Rectangle => Data.rectangle.GetVertex(index),
                    ShapeTypes.Polygon   => Data.polygon.GetVertex(index),
                    _ => Vector2Create(1000000, 1000000),
                };
            }
        }
    }
}
