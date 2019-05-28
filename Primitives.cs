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
        public float brightness, R, G, B;
        public Light(float R, float G, float B)
        { this.R = R; this.G = G; this.B = B; }

        public float attentuation(float distance)
        {
            if (brightness > 0.9) brightness = 0.9f;
            return brightness * brightness / (distance + 1 * distance + 1);
        }
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

        public Light cl = new Light(1.0f, 1.0f, 1.0f);
        public Circle(float brightness, float R, float G, float B)
        {
            cl.brightness = brightness;
            cl.R = R; cl.G = G; cl.B = B;
        }

        public override bool Intersect(Ray ray)
        {
            Vector2 i = ray.O - C;
            float u = Vector2.Dot(i, ray.D),
                v = u * u + r * r - Vector2.Dot(i, i);
            if (v >= 0) //Check for intersection
            {
                v = (float)Math.Sqrt(v);
                float a = - v - u, b = v - u;
                if (a >= 0 && b > 0 && a < ray.t) return true; // Ray Outside Circle
                if (a < 0 && b > 0 && b < ray.t) return true; // Ray Inside Circle                
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
                float s = (ray.D.X * (B.Y - ray.O.Y) + ray.D.Y * (ray.O.X - B.X)) / (X_line * ray.D.Y - Y_line * ray.D.X),
                    r = (B.X - ray.O.X + X_line * s) / ray.D.X;
                if (r >= 0 && r < ray.t && s >= 0 && s <= 1) return true;
            } return false;
        }
    }
}