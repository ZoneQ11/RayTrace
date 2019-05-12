using System;
using System.Diagnostics;
using System.Threading;

namespace Template
{
    class MyApplication
    {
        int MixColor(int red, int green, int blue) { return (red << 16) + (green << 8) + blue; }
        int i = 0;

        // member variables
        public Surface screen;
        // initialize
        public void Init()
        {
        }
        // tick: renders one frame
        public void Tick()
        {
            screen.Clear(0x333333);
            screen.Print("hello world", 2, 2, 0xffffff);

            /*for (int x = 0; x < (screen.width - 1); x++)
            {
                for (int y = 100; y < 200; y++)
                {
                    int location = x + y * screen.width, m = 5 * Math.Abs(y - 150);
                    screen.pixels[location] = MixColor(255 - m, 255 - m, 0);
                }
            }*/

            float rx1 = (float)(-1 * Math.Cos(i * Math.PI / 180) - 1 * Math.Sin(i * Math.PI / 180));
            float ry1 = (float)(-1 * Math.Sin(i * Math.PI / 180) + 1 * Math.Cos(i * Math.PI / 180));
            float rx2 = (float)(1 * Math.Cos(i * Math.PI / 180) - 1 * Math.Sin(i * Math.PI / 180));
            float ry2 = (float)(1 * Math.Sin(i * Math.PI / 180) + 1 * Math.Cos(i * Math.PI / 180));
            float rx3 = (float)(1 * Math.Cos(i * Math.PI / 180) + 1 * Math.Sin(i * Math.PI / 180));
            float ry3 = (float)(1 * Math.Sin(i * Math.PI / 180) - 1 * Math.Cos(i * Math.PI / 180));
            float rx4 = (float)(-1 * Math.Cos(i * Math.PI / 180) + 1 * Math.Sin(i * Math.PI / 180));
            float ry4 = (float)(-1 * Math.Sin(i * Math.PI / 180) - 1 * Math.Cos(i * Math.PI / 180));

            screen.Line(TX(rx1), TY(ry1), TX(rx2), TY(ry2), 0x00ff0000);
            screen.Line(TX(rx2), TY(ry2), TX(rx3), TY(ry3), 0x00ff0000);
            screen.Line(TX(rx3), TY(ry3), TX(rx4), TY(ry4), 0x00ff0000);
            screen.Line(TX(rx4), TY(ry4), TX(rx1), TY(ry1), 0x00ff0000);
            if (i < 360)
                i++;            else i = 0;        }

        public int TX(float x)
        {
            return (int) ((x+2) * 160);
        }

        public int TY(float y)
        {
            y *= -1;
            return (int) ((y+2) * 100);
        }
    }
}