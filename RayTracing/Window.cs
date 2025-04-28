using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace RayTracing
{
    public class Window : GameWindow
    {
        int BasicProgramID;
        int BasicVertexShader;
        int BasicFragmentShader;

        int uTimeLocation;

        float time;

        float width, height;

        public List<Vector3> vertices = new List<Vector3>()
        {
            new Vector3(-1f,1f,-1f),
            new Vector3(1f,1f,-1f),
            new Vector3(1f,-1f,-1f),
            new Vector3(-1f,-1f,-1f),
        };

        public uint[] indices =
        {
            0, 1, 2,
            2, 3, 0
        };

        public int VAO;
        public int VBO;
        public int EBO;

        public Window(float width, float height) : base(GameWindowSettings.Default, NativeWindowSettings.Default) {
            CenterWindow(new Vector2i((int)width, (int)height));
            this.width = width;
            this.height = height;
        }
        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
        }

        void LoadShader(string filename, ShaderType type, int program, out int address) {
            address = GL.CreateShader(type);

            string shaderSource = "";
            try
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    shaderSource = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load shader source file: " + e.Message);
            }

            GL.ShaderSource(address, shaderSource);
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        void InitShaders() {
            BasicProgramID = GL.CreateProgram();
            LoadShader("../../../Shaders/shader.vert", ShaderType.VertexShader, BasicProgramID,
                        out BasicVertexShader);
            LoadShader("../../../Shaders/shader.frag", ShaderType.FragmentShader, BasicProgramID,
                        out BasicFragmentShader);
            GL.LinkProgram(BasicProgramID);

            int status = 0;
            GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);
            Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));
        }

        protected override void OnLoad() {
            base.OnLoad();

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            InitShaders();

            uTimeLocation = GL.GetUniformLocation(BasicProgramID, "uTime");
            time = 0.0f;
        }

        protected override void OnRenderFrame(FrameEventArgs args) {
            GL.ClearColor(0.1f, 0.3f, 0.8f, 0.5f);
            GL.Clear(ClearBufferMask.ColorBufferBit);


            GL.UseProgram(BasicProgramID);
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

            GL.Uniform1(uTimeLocation, time);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            Context.SwapBuffers();
            base.OnRenderFrame(args);

            //Title = $"Ray Tracing (FPS: {1.0 / args.Time:0.00})";
        }

        protected override void OnUpdateFrame(FrameEventArgs args) {
            base.OnUpdateFrame(args);
            time += (float)args.Time;
        }
    }
}
