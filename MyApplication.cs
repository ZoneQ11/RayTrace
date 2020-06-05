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

        private Line line1 = new Line(0.25f, 0.15f, 0.25f, -0.15f);
        private Line line2 = new Line(0.15f, 0.25f, -0.15f, 0.25f);
        private Line line3 = new Line(-0.25f, 0.15f, -0.25f, -0.15f);
        private Line line4 = new Line(0.15f, -0.25f, -0.15f, -0.25f);

        private Circle circle = new Circle(1.4f, 1.0f, 0.0f, 0.0f);

        private Light light1 = new Light(0.8f, 1.0f, 1.0f, 1.0f);
        private Light light2 = new Light(0.8f, 1.0f, 0.0f, 0.0f);
        private Light light3 = new Light(0.8f, 0.0f, 1.0f, 0.0f);
        private Light light4 = new Light(0.8f, 0.0f, 0.0f, 1.0f);

        private int pixelColor, i;
        private float j = 0.5f;

        // initialize
        public void Init()
        {
            //The range of the coordinates is within -1.0f and 1.0f
            //The Y-coordinate is still inverted

            circle.C = new Vector2(0, 0);
            circle.cl.light_pos = circle.C;
            circle.r = 0.1f;           

            primitives.Add(line1);
            primitives.Add(line2);
            primitives.Add(line3);
            primitives.Add(line4);

            primitives.Add(circle);
            light_array.Add(circle.cl);

            light_array.Add(light1);
            light_array.Add(light2);
            light_array.Add(light3);
            light_array.Add(light4);
        }

        // Tick: renders one frame
        public void Tick()
        {
            if (i < 360) i+=10; else i = 0;

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
                    float rx = -1 + x * 2 / ((float)screen.width - 1);
                    float ry = -1 + y * 2 / ((float)screen.height - 1);
                    ray.O = new Vector2(rx, ry);

                    pixelColor = 0;
                    foreach (Light l in light_array)
                    {
                        float a = l.light_pos.X - ray.O.X,
                            b = l.light_pos.Y - ray.O.Y,
                            distanceToLight = (float)Math.Sqrt(a * a + b * b);

                        ray.D = Vector2.Normalize(l.light_pos - ray.O);
                        ray.t = distanceToLight;

                        bool occluded = false;
                        foreach (Primitive p in primitives)
                            if (p.Intersect(ray)) {occluded = true; break;}
                        if (!occluded)
                        {
                            int red_int = (int)(l.R * l.attentuation(distanceToLight) * 255),
                                green_int = (int)(l.G * l.attentuation(distanceToLight) * 255),
                                blue_int = (int)(l.B * l.attentuation(distanceToLight) * 255);
                            pixelColor += (red_int << 16) + (green_int << 8) + blue_int;
                        }
                    }
                    screen.Plot(x, y, MathHelper.Clamp(pixelColor, 0, 16777215));
                    //Clamp the light so it is always between 0,0,0 (Black) and 255,255,255 (White)
                }
        }
    }
}