using System;
using OpenTK;

namespace Template
{
    class Ray
    {
        public Vector2 O, D; //Origin, Direction
        public float t; //Distance
    }

    class Light
    {
        public Vector2 light_pos;
        public int MixColor(int red, int green, int blue) { return (red << 16) + (green << 8) + blue; }
        public int color;
        public float brightness;
        public Light(int a, int b, int c)
        { color = MixColor(a, b, c); }

        public float attentuation(float distance)
        { return brightness / (distance + 1 * distance + 1); }
    }

    abstract class Primitive
    {
        public virtual bool Intersect(Ray ray)
        { return false; }
    }

    class Circle : Primitive
    {
        public Vector2 C; //Center
        public float r; //Radius

        public Light cl = new Light(255, 255, 255);

        public Circle()
        {
            cl.light_pos = new Vector2(0f, 0);
            cl.brightness = 0.8f;
        }

        public override bool Intersect(Ray ray)
        {
            float u = Vector2.Dot(C, ray.D);
            Vector2 crosspoint = C - (ray.O + u * ray.D);
            if (r * r > Vector2.Dot(crosspoint, crosspoint)) return true;
            else return false;
        }
    }

    class Line : Primitive
    {
        public Vector2 B, E; //Begin and End
        public override bool Intersect(Ray ray)
        {
            float X_line = E.X - B.X, Y_line = E.Y - B.Y;
            if (ray.D.Y / ray.D.X != Y_line / X_line)
            {
                float d = (ray.D.X * Y_line) - ray.D.Y * X_line;
                if (d != 0)
                {
                    float r = (((ray.O.Y - B.Y) * X_line) - (ray.O.X - B.X) * Y_line) / d,
                        s = (((ray.O.Y - B.Y) * ray.D.X) - (ray.O.X - B.X) * ray.D.Y) / d;
                    if (r >= 0 && s >= 0 && s <= 1) return true;
                }
            }
            return false;
        }
    }

    class Square : Primitive
    {
        public Line[] line = new Line[3];
        public Square(float size)
        {
            line[0].B = new Vector2(-size, size);
            line[0].E = new Vector2(size, size);
            line[1].B = new Vector2(size, size);
            line[1].E = new Vector2(size, -size);
            line[2].B = new Vector2(size, -size);
            line[2].E = new Vector2(-size, -size);
            line[3].B = new Vector2(-size, -size);
            line[3].E = new Vector2(-size, size);
        }
    }

    static class ExtraMath
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}