using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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
        public Light(int a, int b, int c)
        { color = MixColor(a, b, c); }

        public float lightAttentuation(float distance)
        {
            if (distance > 1) return 1 / distance;
            else return 1;
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

        public Circle()
        {
            Light l = new Light(255, 255, 255);
            l.light_pos = C;
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

        public int TX(float x)
        { return (int)((x + 2) * screen.width / 4); }

        public int TY(float y)
        { y *= -1; return (int)((y + 2) * screen.height / 4); }

        // member variables
        public Surface screen;
		// initialize
		public void Init()
		{
            ray.O = new Vector2(TX(0),TY(0));
            ray.t = 1;
            ray.D = new Vector2(TX(1), TY(1));
            ray.D = Vector2.Normalize(ray.D);

            circle.C = new Vector2(TX(-1), TY(1));
            circle.r = 1;

            line.B = new Vector2(TX(1), TY(0));
            line.E = new Vector2(TX(0), TY(1));

            light.light_pos = new Vector2(TX(1), TY(1));

            primitives.Add(circle);
            primitives.Add(line);
            light_array.Add(light);

            Debug.WriteLine("Circle: " + circle.Intersect(ray));
            Debug.WriteLine("Line: " + line.Intersect(ray));
        }

        // Tick: renders one frame
        public void Tick()
        { }

        public void RenderGL()
        {
            for (int x = 0; x < screen.width - 1; x++)
                for (int y = 0; y < screen.height - 1; y++)
                {
                    pixelColor = 0;
                    foreach (Light l in light_array)
                    {
                        float a = l.light_pos.X - ray.O.X, b = l.light_pos.Y - ray.O.Y,
                            distanceToLight = (float)Math.Sqrt(a * a + b * b);                        
                        ray.O = new Vector2(x, y * screen.width);
                        ray.D = l.light_pos - ray.O;
                        ray.D = Vector2.Normalize(ray.D);
                        ray.t = distanceToLight;
                        bool occluded = false;
                        foreach (Primitive p in primitives)
                            if (p.Intersect(ray)) occluded = true;
                        if(!occluded)
                            pixelColor += light.MixColor(255, 255, 255) * (int)light.lightAttentuation(distanceToLight);
                    }
                    screen.Plot(x, y, pixelColor);
                }
        }
    }
}