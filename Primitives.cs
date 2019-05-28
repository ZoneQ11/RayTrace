using System;
using System.Diagnostics;
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

        float f(float a1, float a2, float b1, float b2, float s)
        { return b1 + (s - a1) * (b2 - b1) / (a2 - a1); }

        public Light cl = new Light(255, 255, 255);

        public Circle(float brightness)
        {
            cl.light_pos = new Vector2(0f, 0f);
            cl.brightness = brightness;
        }

        public override bool Intersect(Ray ray)
        {
            float u = Vector2.Dot(C - ray.O, ray.D);
            if (ray.t > u)
            {
                Vector2 crosspoint = C - (ray.O + u * ray.D);
                if (r * r > Vector2.Dot(crosspoint, crosspoint)) return true;
            } return false;
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
                float d = (ray.D.X * Y_line) - (ray.D.Y * X_line);
                if (d != 0)
                {
                    float r = (((ray.O.Y - B.Y) * (X_line)) - ((ray.O.X - B.X) * (Y_line))) / d,
                        s = (((ray.O.Y - B.Y) * ray.D.X) - (ray.O.X - B.X) * ray.D.Y) / d;
                    if (r >= 0 && s >= 0 && s <= 1) return true;
                }
            } return false;
        }
    }
}