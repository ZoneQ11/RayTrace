using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OpenTK;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        private Ray ray = new Ray();
        private Line line = new Line();
        private Circle circle = new Circle(0.8f);
        private Light light = new Light();
        private List<Light> light_array = new List<Light>();
        private List<Primitive> primitives = new List<Primitive>();

        private int pixelColor;
        private int MixColor(int red, int green, int blue) { return (red << 16) + (green << 8) + blue; }
        private float f(float a1, float a2, float b1, float b2, float s)
        { return b1 + (s - a1) * (b2 - b1) / (a2 - a1); }

        // initialize
        public void Init()
        {
            circle.C = new Vector2(0, 0);
            circle.cl.light_pos = circle.C;
            circle.r = 0.1f;           

            line.B = new Vector2(0.5f, 0f);
            line.E = new Vector2(0f, -0.5f);

            light.light_pos = new Vector2(1.0f, 0f);
            light.brightness = 1.41f;

            primitives.Add(line);
            primitives.Add(circle);
            light_array.Add(light);
            light_array.Add(circle.cl);
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
                        float a = l.light_pos.X - ray.O.X,
                            b = l.light_pos.Y - ray.O.Y,
                            distanceToLight = (float)Math.Sqrt(a * a + b * b);

                        ray.O = new Vector2(rx, ry);
                        ray.D = Vector2.Normalize(l.light_pos - ray.O);
                        ray.t = distanceToLight;

                        float red_float = 1.0f * l.attentuation(distanceToLight),
                            green_float = 1.0f * l.attentuation(distanceToLight),
                            blue_float = 1.0f * l.attentuation(distanceToLight);
                        int red_int = (int)f(0.0f, 1.0f, 0, 255, red_float),
                            green_int = (int)f(0.0f, 1.0f, 0, 255, green_float),
                            blue_int = (int)f(0.0f, 1.0f, 0, 255, blue_float);

                        bool occluded = false;
                        foreach (Primitive p in primitives)
                            if (p.Intersect(ray)) occluded = true;
                        if (!occluded)
                            pixelColor += MixColor(red_int, green_int, blue_int);
                    }
                    screen.Plot(x, y, MathHelper.Clamp(pixelColor, 0, MixColor(255, 255, 255)));
                    //Clamp the light so it is always between 0,0,0 (Black) and 255,255,255 (White)
                }
        }
    }
}