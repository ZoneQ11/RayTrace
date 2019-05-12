using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace Template
{
    class MyApplication
    {
        int MixColor(int red, int green, int blue) { return (red << 16) + (green << 8) + blue; }
        int i = 0, VBO, programID, vsID, fsID, attribute_vpos, attribute_vcol, attribute_norm, uniform_mview, vbo_pos, vbo_color, vbo_norm;

        public int TX(float x)
        { return (int)((x + 2) * 160); }

        public int TY(float y)
        { y *= -1; return (int)((y + 2) * 100); }

        Surface map;
        float[,] h;
        float f (float a1, float a2, float b1, float b2, float s)
        { return b1 + (s - a1) * (b2 - b1) / (a2 - a1); }
        float[] vertexData, colorData, normData;

        // member variables
        public Surface screen;
        // initialize
        public void Init()
        {
            map = new Surface("../../assets/coin.png");
            h = new float[256, 256];
            for (int y = 0; y < 256; y++)
                for (int x = 0; x < 256; x++)
                    h[x, y] = (((float)(map.pixels[x + y * 256] & 255)) / 256);

            vertexData = new float[255 * 255 * 2 * 3 * 3];
            colorData = new float[255 * 255 * 2 * 3 * 3];
            int count = 0, colorcount = 0, index = 0;
            float coin_red = 0.7f, coin_green = 0.4f, coin_blue = 0.0f;

            for (int y = 0; y < 255; y++)
                for (int x = 0; x < 255; x++)
                {
                    float rx = f(0, 255, -128, 127, x);
                    float ry = f(0, 255, -128, 127, y);
                    vertexData[count] = rx; //top-left
                    vertexData[count + 1] = ry;
                    vertexData[count + 2] = h[x, y] * -8f;
                    vertexData[count + 3] = rx + 1; //top-right
                    vertexData[count + 4] = ry;
                    vertexData[count + 5] = h[x + 1, y] * -8f;
                    vertexData[count + 6] = rx + 1; //bottom-right
                    vertexData[count + 7] = ry + 1;
                    vertexData[count + 8] = h[x + 1, y + 1] * -8f;
                    vertexData[count + 9] = rx + 1; //bottom-right
                    vertexData[count + 10] = ry + 1;
                    vertexData[count + 11] = h[x + 1, y + 1] * -8f;
                    vertexData[count + 12] = rx; //bottom-left
                    vertexData[count + 13] = ry + 1;
                    vertexData[count + 14] = h[x, y + 1] * -8f;
                    vertexData[count + 15] = rx; //top-left
                    vertexData[count + 16] = ry;
                    vertexData[count + 17] = h[x, y] * -8f;

                    for (int cc = 0; cc < 18; cc += 3)
                    {
                        colorData[colorcount + cc] = coin_red * h[x, y];
                        colorData[colorcount + cc + 1] = coin_green * h[x, y];
                        colorData[colorcount + cc + 2] = coin_blue * h[x, y];
                    }

                    count += 18;
                    colorcount += 18;
                }

            normData = new float[255 * 255 * 2 * 3 * 3];
            int normcount = 0;
            for (int y = 0; y < 255; y++)
                for (int x = 0; x < 255; x++)
                {
                    Vector3 v1a = new Vector3(vertexData[index] - vertexData[index + 3],
                        vertexData[index + 1] - vertexData[index + 4],
                        vertexData[index + 2] - vertexData[index + 5]);
                    Vector3 v2a = new Vector3(vertexData[index + 6] - vertexData[index + 3],
                        vertexData[index + 7] - vertexData[index + 4],
                        vertexData[index + 8] - vertexData[index + 5]);
                    index += 18;

                    Vector3 v3a = Vector3.Normalize(v1a);
                    Vector3 v3b = Vector3.Normalize(v2a);

                    Vector3 v4a = Vector3.Cross(v3b, v3a);

                    float norm_X1 = v4a.X, norm_Y1 = v4a.Y, norm_Z1 = v4a.Z;

                    for (int nc = 0; nc < 18; nc += 3)
                    {
                        normData[normcount + nc] = norm_X1;
                        normData[normcount + nc + 1] = norm_Y1;
                        normData[normcount + nc + 2] = norm_Z1;
                    }
                    normcount += 18;
                }

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData<float>(BufferTarget.ArrayBuffer,
                vertexData.Length * 4,
                vertexData, BufferUsageHint.StaticDraw);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 12, 0);

            programID = GL.CreateProgram();
            LoadShader("../../shaders/vs.glsl",
             ShaderType.VertexShader, programID, out vsID);
            LoadShader("../../shaders/fs.glsl",
             ShaderType.FragmentShader, programID, out fsID);
            GL.LinkProgram(programID);

            attribute_vpos = GL.GetAttribLocation(programID, "vPosition");
            attribute_vcol = GL.GetAttribLocation(programID, "vColor");
            attribute_norm = GL.GetAttribLocation(programID, "Norm");
            uniform_mview = GL.GetUniformLocation(programID, "M");

            vbo_pos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_pos);
            GL.BufferData<float>(BufferTarget.ArrayBuffer,
                vertexData.Length * 4,
                vertexData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vpos, 3,
                VertexAttribPointerType.Float,
                false, 0, 0);

            vbo_color = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_color);
            GL.BufferData<float>(BufferTarget.ArrayBuffer,
                colorData.Length * 4,
                colorData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vcol, 3,
                VertexAttribPointerType.Float,
                false, 0, 0);

            vbo_norm = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_norm);
            GL.BufferData<float>(BufferTarget.ArrayBuffer,
                normData.Length * 4,
                normData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_norm, 3,
                VertexAttribPointerType.Float,
                false, 0, 0);
        }
        // tick: renders one frame
        public void Tick()
        {
            if (i < 360)
                i++;            else i = 0;

            screen.Clear(0);
            screen.Print("hello world", 2, 2, 0xffffff);

            /*for (int x = 0; x < (screen.width - 1); x++)
            {
                for (int y = 100; y < 200; y++)
                {
                    int location = x + y * screen.width, m = 5 * Math.Abs(y - 150);
                    screen.pixels[location] = MixColor(255 - m, 255 - m, 0);
                }
            }

            float rx1 = (float)(-1 * Math.Cos(i * Math.PI / 180) - 1 * Math.Sin(i * Math.PI / 180));
            float ry1 = (float)(-1 * Math.Sin(i * Math.PI / 180) + 1 * Math.Cos(i * Math.PI / 180));
            float rx2 = (float)(1 * Math.Cos(i * Math.PI / 180) - 1 * Math.Sin(i * Math.PI / 180));
            float ry2 = (float)(1 * Math.Sin(i * Math.PI / 180) + 1 * Math.Cos(i * Math.PI / 180));
            float rx3 = (float)(1 * Math.Cos(i * Math.PI / 180) + 1 * Math.Sin(i * Math.PI / 180));
            float ry3 = (float)(1 * Math.Sin(i * Math.PI / 180) - 1 * Math.Cos(i * Math.PI / 180));
            float rx4 = (float)(-1 * Math.Cos(i * Math.PI / 180) + 1 * Math.Sin(i * Math.PI / 180));
            float ry4 = (float)(-1 * Math.Sin(i * Math.PI / 180) - 1 * Math.Cos(i * Math.PI / 180));

            screen.Line(TX(rx1), TY(ry1), TX(rx2), TY(ry2), 0x00ffcd00);
            screen.Line(TX(rx2), TY(ry2), TX(rx3), TY(ry3), 0x00ffcd00);
            screen.Line(TX(rx3), TY(ry3), TX(rx4), TY(ry4), 0x00ffcd00);
            screen.Line(TX(rx4), TY(ry4), TX(rx1), TY(ry1), 0x00ffcd00);*/        }

        public void RenderGL()
        {
            /*GL.Color3(1.0f, 0.0f, 0.0f);
            var M = Matrix4.CreatePerspectiveFieldOfView(1.6f, 1.3f, .1f, 1000);
            GL.LoadMatrix(ref M);
            GL.Translate(0, 0, -150);
            GL.Rotate(180, 1, 0, 0);
            GL.Rotate(i, 0, 0, 1);

            GL.Begin(PrimitiveType.Triangles);*/

            /*for (int y = 0; y < map.height - 1; y++)
                for (int x = 0; x < map.width - 1; x++)
                {
                    float rx = f(0, 255, -128, 127, x);
                    float ry = f(0, 255, -128, 127, y);
                    GL.Color3(h[x, y], h[x, y], h[x, y]);

                    GL.Vertex3(rx, ry, h[x, y] * -0.2f);
                    GL.Vertex3(rx + 1, ry, h[x + 1, y] * -0.2f);
                    GL.Vertex3(rx + 1, ry + 1, h[x + 1, y + 1] * -0.2f);

                    GL.Vertex3(rx + 1, ry + 1, h[x + 1, y + 1] * -0.2f);
                    GL.Vertex3(rx, ry + 1, h[x, y + 1] * -0.2f);
                    GL.Vertex3(rx, ry, h[x, y] * -0.2f);
                }
            GL.End();*/

            Matrix4 M = Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), i / (180 / 3.14159f));
            M *= Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), +2.7f);
            M *= Matrix4.CreateTranslation(0, 0, -100);
            M *= Matrix4.CreatePerspectiveFieldOfView(1.6f, 1.3f, .1f, 1000);            GL.UseProgram(programID);
            GL.UniformMatrix4(uniform_mview, false, ref M);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.EnableVertexAttribArray(attribute_vpos);
            GL.EnableVertexAttribArray(attribute_vcol);
            GL.EnableVertexAttribArray(attribute_norm);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 255 * 255 * 2 * 3);

            GL.UseProgram(0);
        }

        void LoadShader(String name, ShaderType type, int program, out int ID)
        {
            ID = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(name))
                GL.ShaderSource(ID, sr.ReadToEnd());
            GL.CompileShader(ID);
            GL.AttachShader(program, ID);
            Console.WriteLine(GL.GetShaderInfoLog(ID));
        }
    }
}