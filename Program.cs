using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.IO;
using System.Numerics;
using Unity.Mathematics;

namespace OpenGL_Demo
{
    class Program
    {
        static IWindow window;

        public static GL GL;

        private static readonly float[] Vertices =
{
            //X    Y      Z     U   V
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 1.0f,

             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 0.0f
        };

        static BufferObject<uint> EBO;
        //static BufferObject<float> VBO;
        static BufferObject<Mesh.Vertex> VBO;
        static VertexArrayObject VAO;

        static Shader Shader;
        static int uTexture0;
        static int uModel;
        static int uView;
        static int uProjection;

        static Texture Texture;

        static Mesh Mesh;

        static void Main(string[] args)
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "Hello World";

            window = Window.Create(options);

            window.Load += Window_Load;
            window.Render += Window_Render;

            window.Run();
        }

        private static void Window_Load()
        {
            var input = window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                var keyboard = input.Keyboards[i];
                keyboard.KeyDown += Keyboard_KeyDown;
            }

            GL = GL.GetApi(window);

            Mesh = Mesh.Load(Path.Combine("Assets", "kokoro.obj"));

            VAO = new VertexArrayObject();
            VAO.Bind();

            EBO = new BufferObject<uint>(BufferTargetARB.ElementArrayBuffer);
            EBO.Bind();
            EBO.BufferData(Mesh.Indices);

            VBO = new BufferObject<Mesh.Vertex>(BufferTargetARB.ArrayBuffer);
            VBO.Bind();
            VBO.BufferData(Mesh.Vertices);

            VAO.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, 20, 0);
            VAO.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, 20, 12);

            var shaderVert = File.ReadAllText(Path.Combine("Shaders", "vert.glsl"));
            var shaderFrag = File.ReadAllText(Path.Combine("Shaders", "frag.glsl"));

            Shader = new ShaderSource(shaderVert, shaderFrag);
            uTexture0 = Shader.GetUniformLocation("uTexture0");
            uModel = Shader.GetUniformLocation("uModel");
            uView = Shader.GetUniformLocation("uView");
            uProjection = Shader.GetUniformLocation("uProjection");

            //Texture = new TextureImage("silk.png");
            Texture = new TextureImage(Path.Combine("Assets", "kokoro.png"));

            GL.Viewport(0, 0, (uint)window.Size.X, (uint)window.Size.Y);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        private static void Window_Render(double dt)
        {
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            VAO.Bind();
            Texture.Bind();
            Shader.Use();
            Shader.SetInt(uTexture0, 0);

            var rot = (float)(window.Time * 100);

            //var modelMat = float4x4.Euler(math.radians(rot), math.radians(rot), 0);
            var modelMat = float4x4.Euler(0, math.radians(rot), 0);
            modelMat.c0 *= -1f;

            //var viewMat = float4x4.LookAt(new float3(0, 0, 2f), float3.zero, new float3(0, 1f, 0));
            var viewMat = float4x4.LookAt(new float3(0, 0.8f, 3.5f), new float3(0, 0.8f, 0), new float3(0, 1f, 0));
            viewMat.c2 *= -1f;
            viewMat = math.inverse(viewMat);

            var ProjMat = float4x4.PerspectiveFov(math.radians(30f), (float)window.Size.X / window.Size.Y, 0.1f, 100f);

            Shader.SetMatrix(uModel, modelMat);
            Shader.SetMatrix(uView, viewMat);
            Shader.SetMatrix(uProjection, ProjMat);

            //GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            unsafe
            {
                GL.DrawElements(GLEnum.Triangles, (uint)Mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
            }
        }

        private static void Keyboard_KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            if(arg2 == Key.Escape)
            {
                window.Close();
            }
        }
    }
}
