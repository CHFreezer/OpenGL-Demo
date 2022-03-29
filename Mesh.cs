using Assimp;
using System.Collections.Generic;
using Unity.Mathematics;

namespace OpenGL_Demo
{
    public class Mesh
    {
        public struct Vertex
        {
            public float3 position;
            public float2 coord;
        }

        public uint[] Indices;
        public Vertex[] Vertices;

        public static Mesh Load(string path)
        {
            var assimp = new AssimpContext();
            var scene = assimp.ImportFile(path);

            var mesh = scene.Meshes[0];

            var indices = mesh.GetUnsignedIndices();
            var vertices = new Vertex[mesh.VertexCount];

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                var vertex = mesh.Vertices[i];
                vertices[i].position = new float3(vertex.X, vertex.Y, vertex.Z);
                var coord = mesh.TextureCoordinateChannels[0][i];
                vertices[i].coord = new float2(coord.X, coord.Y);
            }

            assimp.Dispose();

            return new Mesh()
            {
                Indices = indices,
                Vertices = vertices
            };
        }
    }
}
