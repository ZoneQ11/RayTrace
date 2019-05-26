using System;
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

    class Circle
    {
        public Vector2 C; //Center
        public float r; //Radius
        public bool Intersect(Ray ray)
        {
            float u = Vector2.Dot(C, ray.D);
            Vector2 crosspoint = C - (ray.O + u * ray.D);
            if (r * r > Vector2.Dot(crosspoint, crosspoint)) return true;
            else return false;
        }
    }

    class Line
    {
        public Vector2 B, E; //Begin and End
        public bool Intersect(Ray ray)
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

    class Light
    {
        public Vector2 light_pos;
        public int MixColor(int red, int green, int blue) { return (red << 16) + (green << 8) + blue; }
        public int color, radius;
        public Light(int a, int b, int c) { color = MixColor(a, b, c); }
        public float lightAttentuation(int distance)
        { return 1/distance; }
    }

    class MyApplication
	{
        static int i;
        int pixelColor;
        Ray ray = new Ray();
        Circle circle = new Circle();
        Line line = new Line();
        Light light = new Light(255, i, i);

        // member variables
        public Surface screen;
		// initialize
		public void Init()
		{
            ray.O = new Vector2(0,0);
            ray.t = 1;
            ray.D = new Vector2(1,1);
            ray.D = Vector2.Normalize(ray.D);

            circle.C = new Vector2(-1f,1f);
            circle.r = 1;

            line.B = new Vector2(0, 10);
            line.E = new Vector2(10, 0);

            light.light_pos = new Vector2(0,0);

            Debug.WriteLine("Circle: " + circle.Intersect(ray));
            Debug.WriteLine("Line: " + line.Intersect(ray));
        }
        // tick: renders one frame
        public void Tick()
		{
            if (i < 255) i++; else i = 0;

            screen.Clear( 0 );
			screen.Line( 2, 20, 160, 20, 0xff0000 );

            for (int x = 0; x < (screen.width - 1); x++)
                for (int y = 0; y < screen.height - 1; y++)
                {
                    pixelColor = 0;
                    pixelColor += light.MixColor(184, 115, 51);
                    screen.Plot(x, y, pixelColor);
                }
        }
    }
}