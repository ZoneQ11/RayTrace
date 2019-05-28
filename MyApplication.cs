using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        private Ray ray = new Ray();
        private List<Light> light_array = new List<Light>();
        private List<Primitive> primitives = new List<Primitive>();

        private Line line1 = new Line(); private Line line2 = new Line();
        private Line line3 = new Line(); private Line line4 = new Line();

        private Circle circle = new Circle(1.4f, 1.0f, 0.0f, 0.0f);

        private Light light1 = new Light(1.0f, 1.0f, 1.0f); private Light light2 = new Light(1.0f, 0.0f, 0.0f);
        private Light light3 = new Light(0.0f, 1.0f, 0.0f); private Light light4 = new Light(0.0f, 0.0f, 1.0f);

        private int pixelColor, i;
        private int MixColor(int red, int green, int blue) { return (red << 16) + (green << 8) + blue; }
        private float j = 0.5f;
        private float f(float a1, float a2, float b1, float b2, float s)
        { return b1 + (s - a1) * (b2 - b1) / (a2 - a1); }

        // initialize
        public void Init()
        {
            //The range of the coordinates is within -1.0f and 1.0f
            //The Y-coordinate is still inverted

            circle.C = new Vector2(0, 0);
            circle.cl.light_pos = circle.C;
            circle.r = 0.1f;           

            line1.B = new Vector2(0.25f, 0.15f); line1.E = new Vector2(0.25f, -0.15f);
            line2.B = new Vector2(0.15f, 0.25f); line2.E = new Vector2(-0.15f, 0.25f);
            line3.B = new Vector2(-0.25f, 0.15f); line3.E = new Vector2(-0.25f, -0.15f);
            line4.B = new Vector2(0.15f, -0.25f); line4.E = new Vector2(-0.15f, -0.25f);

            light1.brightness = 0.8f;   light2.brightness = 0.8f;
            light3.brightness = 0.8f;   light4.brightness = 0.8f;
            primitives.Add(line1);      primitives.Add(line2);
            primitives.Add(line3);      primitives.Add(line4);

            primitives.Add(circle);
            light_array.Add(circle.cl);

            light_array.Add(light1);    light_array.Add(light2);
            light_array.Add(light3);    light_array.Add(light4);
        }

        // Tick: renders one frame
        public void Tick()
        {
            if (i < 360) i+=5; else i = 0;

            float rx1 = (float)(-1 * Math.Cos(i * Math.PI / 180) - 1 * Math.Sin(i * Math.PI / 180));
            float ry1 = (float)(-1 * Math.Sin(i * Math.PI / 180) + 1 * Math.Cos(i * Math.PI / 180));
            float rx2 = (float)(1 * Math.Cos(i * Math.PI / 180) - 1 * Math.Sin(i * Math.PI / 180));
            float ry2 = (float)(1 * Math.Sin(i * Math.PI / 180) + 1 * Math.Cos(i * Math.PI / 180));
            float rx3 = (float)(1 * Math.Cos(i * Math.PI / 180) + 1 * Math.Sin(i * Math.PI / 180));
            float ry3 = (float)(1 * Math.Sin(i * Math.PI / 180) - 1 * Math.Cos(i * Math.PI / 180));
            float rx4 = (float)(-1 * Math.Cos(i * Math.PI / 180) + 1 * Math.Sin(i * Math.PI / 180));
            float ry4 = (float)(-1 * Math.Sin(i * Math.PI / 180) - 1 * Math.Cos(i * Math.PI / 180));

            light1.light_pos = new Vector2(j * rx1, j * ry1);
            light2.light_pos = new Vector2(j * rx2, j * ry2);
            light3.light_pos = new Vector2(j * rx3, j * ry3);
            light4.light_pos = new Vector2(j * rx4, j * ry4);
        }

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

                        float red_float = l.R * l.attentuation(distanceToLight),
                            green_float = l.G * l.attentuation(distanceToLight),
                            blue_float = l.B * l.attentuation(distanceToLight);
                        int red_int = (int)f(0.0f, 1.0f, 0, 255, red_float),
                            green_int = (int)f(0.0f, 1.0f, 0, 255, green_float),
                            blue_int = (int)f(0.0f, 1.0f, 0, 255, blue_float);

                        bool occluded = false;
                        foreach (Primitive p in primitives)
                            if (p.Intersect(ray)) {occluded = true; break; }
                        if (!occluded)
                            pixelColor += MixColor(red_int, green_int, blue_int);
                    }
                    screen.Plot(x, y, MathHelper.Clamp(pixelColor, 0, MixColor(255, 255, 255)));
                    //Clamp the light so it is always between 0,0,0 (Black) and 255,255,255 (White)
                }
        }
    }
}