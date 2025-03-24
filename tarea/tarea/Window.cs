using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenTK_U
{
    public class Window : GameWindow
    {
        // Vértices con grosor para la base
        private readonly float[] vertices = {
    // Front face
    -0.6f,  0.6f,  0.1f,  // 0 - Top-left
    -0.6f, -0.6f,  0.1f,  // 1 - Bottom-left
    -0.4f, -0.6f,  0.1f,  // 2 - Bottom-right
    -0.4f,  0.6f,  0.1f,  // 3 - Top-right

     0.4f,  0.6f,  0.1f,  // 4
     0.4f, -0.6f,  0.1f,  // 5
     0.6f, -0.6f,  0.1f,  // 6
     0.6f,  0.6f,  0.1f,  // 7

    // Back face
    -0.6f,  0.6f, -0.1f,  // 8
    -0.6f, -0.6f, -0.1f,  // 9
    -0.4f, -0.6f, -0.1f,  // 10
    -0.4f,  0.6f, -0.1f,  // 11

     0.4f,  0.6f, -0.1f,  // 12
     0.4f, -0.6f, -0.1f,  // 13
     0.6f, -0.6f, -0.1f,  // 14
     0.6f,  0.6f, -0.1f,  // 15

    // Base de la U (con grosor)
    -0.4f, -0.398f,  0.1f,  // 16 - Frente izquierda
     0.4f, -0.398f,  0.1f,  // 17 - Frente derecha
     0.4f, -0.398f, -0.1f,  // 18 - Atrás derecha
    -0.4f, -0.398f, -0.1f,  // 19 - Atrás izquierda

    -0.4f, -0.6f,  0.1f,  // 20 - Frente izquierda baja
     0.4f, -0.6f,  0.1f,  // 21 - Frente derecha baja
     0.4f, -0.6f, -0.1f,  // 22 - Atrás derecha baja
    -0.4f, -0.6f, -0.1f   // 23 - Atrás izquierda baja
};

        private readonly uint[] indices = {
    // Front face
    0, 1, 2, 0, 2, 3,
    4, 5, 6, 4, 6, 7,

    // Back face
    8, 9, 10, 8, 10, 11,
    12, 13, 14, 12, 14, 15,

    // Sides
    0, 1, 9, 0, 9, 8,
    1, 2, 10, 1, 10, 9,
    2, 3, 11, 2, 11, 10,
    3, 0, 8, 3, 8, 11,
    4, 5, 13, 4, 13, 12,
    5, 6, 14, 5, 14, 13,
    6, 7, 15, 6, 15, 14,
    7, 4, 12, 7, 12, 15,

    // Bottom face (Base gruesa de la U)
    16, 17, 21, 16, 21, 20,  // Cara frontal de la base
    18, 19, 23, 18, 23, 22,  // Cara trasera de la base
    16, 17, 18, 16, 18, 19,  // Parte superior de la base
    20, 21, 22, 20, 22, 23   // Parte inferior de la base
};

        private int _vao, _vbo, _ebo, _shader;
        private Matrix4 _mvp;
        private float _rotation = 0.0f;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.DarkSeaGreen);
            GL.Enable(EnableCap.DepthTest);

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            _shader = Shader.LoadShader("shader.vert", "shader.frag");
            GL.UseProgram(_shader);

            int vertexLocation = GL.GetAttribLocation(_shader, "aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            GL.BindVertexArray(0);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Incrementamos el ángulo de rotación
            _rotation += 0.0002f;

            // Definimos el eje de rotación, vector hacia Z negativo
            Vector3 axisOfRotation = new Vector3(0f, 0f, -1f);

            //  matriz de traslación para llevar el eje de rotación al origen
            Matrix4 translateToOrigin = Matrix4.CreateTranslation(axisOfRotation);

            // rotación alrededor del eje Y
            Matrix4 rotateAroundAxis = Matrix4.CreateRotationY(_rotation);

            // de vuelta al lugar original
            Matrix4 translateBack = Matrix4.CreateTranslation(-axisOfRotation);

            //   rotación alrededor del eje deseado
            Matrix4 modelMatrix = translateToOrigin * rotateAroundAxis * translateBack;

            //  vista y proyección
            Matrix4 viewMatrix = Matrix4.LookAt(new Vector3(0, 0, 2), Vector3.Zero, Vector3.UnitY);
            Matrix4 projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), 800f / 600f, 0.1f, 100f);

            // MVP * 
            _mvp = modelMatrix * viewMatrix * projectionMatrix;

            // Enviar la matriz al shader
            int mvpLocation = GL.GetUniformLocation(_shader, "mvp");
            GL.UseProgram(_shader);
            GL.UniformMatrix4(mvpLocation, false, ref _mvp);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);
            GL.DeleteProgram(_shader);
        }
    }
}