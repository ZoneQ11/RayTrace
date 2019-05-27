using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
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

        /*public Circle()
        {
            Light l = new Light(255, 255, 255);
            l.light_pos = C;
        }*/

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
            } return false;
        }
    }

    class MyApplication
	{
        int pixelColor;
        Ray ray = new Ray();
        Circle circle = new Circle();
        Line line = new Line();
        Light light = new Light(255, 255, 255);
        List<Light> light_array = new List<Light>();
        List<Primitive> primitives = new List<Primitive>();

        float f(float a1, float a2, float b1, float b2, float s)
        { return b1 + (s - a1) * (b2 - b1) / (a2 - a1); }

        public int TX(float x) { return (int)((x + 2) * screen.width / 4); }
        public int TY(float y) { y *= -1; return (int)((y + 2) * screen.height / 4); }

        // member variables
        public Surface screen;
		// initialize
		public void Init()
		{
            circle.C = new Vector2(-1,1);
            circle.r = 1;

            line.B = new Vector2(1,0);
            line.E = new Vector2(0,1);

            light.light_pos = new Vector2(1,1);
            light.brightness = 1.0f;

            primitives.Add(circle);
            primitives.Add(line);
            light_array.Add(light);
        }

        // Tick: renders one frame
        public void Tick()
        { }

        public void RenderGL()
        {
            for (int x = 0; x < screen.width - 1; x++)
                for (int y = 0; y < screen.height - 1; y++)
                {
                    float rx = f(0, screen.width - 1, -1, 1, x);
                    float ry = f(0, screen.height - 1, -1, 1, y);

                    pixelColor = 0;
                    foreach (Light l in light_array)
                    {
                        ray.O = new Vector2(rx, ry);
                        float distanceToLight = l.light_pos.Length - ray.O.Length;
                        ray.D = l.light_pos - ray.O;
                        ray.D = Vector2.Normalize(ray.D);
                        ray.t = distanceToLight;
                        //bool occluded = false;
                        //foreach (Primitive p in primitives)
                        //if (p.Intersect(ray)) occluded = true;
                        //if (!occluded)
                        pixelColor += light.MixColor(255, 255, 255) * (int)light.attentuation(distanceToLight);
                        //Debug.WriteLine("l.light_pos.X: " + l.light_pos.X);
                        //Debug.WriteLine("l.light_pos.Y: " + l.light_pos.Y);
                        //Debug.WriteLine("ray.O.X: " + ray.O.X);
                        //Debug.WriteLine("ray.O.Y: " + ray.O.Y);
                        //Debug.WriteLine("distanceToLight: " + distanceToLight);
                    }
                    screen.Plot(x, y, pixelColor);
                }
        }
    }
}