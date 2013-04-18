using System;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Innovation
{
    public class Terrain : Component, I3DComponent
    {
        // Height representation
        public float[,] heightData;

        // Physics height representation
        HeightMapInfo heightMapInfo;

        // Physics object
        HeightMapObject heightMapObject;

        public TerrainMaterial Material;

        // Vertex and index buffers
        VertexDeclaration myVertexDeclaration;
        VertexBuffer terrainVertexBuffer;
        IndexBuffer terrainIndexBuffer;

        // I3DComponent values
        Vector3 position = Vector3.Zero;
        Matrix rotation = Matrix.Identity;
        Vector3 scale = new Vector3(1, 1, -1);
        BoundingBox boundingBox = new BoundingBox(new Vector3(-1), new Vector3(1));

        public Vector3 Position
        { 
            get { return position; }
            set { position = value; }
        }
        public Vector3 EulerRotation
        {
            get { return MathUtil.MatrixToVector3(Rotation); }
            set { this.rotation = MathUtil.Vector3ToMatrix(value); }
        }
        public Matrix Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public BoundingBox BoundingBox { get { return boundingBox; } }

        public Terrain(Texture2D HeightMap, Texture2D Texture)
            : base()
        {
            Setup(HeightMap, Texture);
        }

        public Terrain(Texture2D HeightMap, Texture2D Texture, GameScreen Parent)
            : base(Parent)
        {
            Setup(HeightMap, Texture);
        }

        void Setup(Texture2D HeightMap, Texture2D Texture)
        {
            heightData = CreateTerrain(HeightMap);

            // Create vertex and index buffers
            myVertexDeclaration = new VertexDeclaration(VertexPositionNormalTexture.VertexDeclaration.GetVertexElements());
            VertexPositionNormalTexture[] terrainVertices = CreateVertices();
            int[] terrainIndices = CreateIndices();
            terrainVertices = GenerateNormalsForTriangleStrip(terrainVertices, terrainIndices);
            CreateBuffers(terrainVertices, terrainIndices);

            Material = new TerrainMaterial(Texture);
        }

        // Sets up terrain, texture, ...
        private float[,] CreateTerrain(Texture2D heightMap)
        {
            float minimumHeight = 0;
            float maximumHeight = 255;

            int width = heightMap.Width;
            int height = heightMap.Height;

            boundingBox = new BoundingBox(
                new Vector3(-width / 2, maximumHeight - minimumHeight, -height / 2),
                new Vector3(width / 2, maximumHeight - minimumHeight, height / 2));

            Color[] heightMapColors = new Color[width * height];
            heightMap.GetData<Color>(heightMapColors);

            // Setup height data from heightmap
            float[,] heightData = new float[width, height];
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    heightData[x, y] = heightMapColors[x + y * width].R;
                    if (heightData[x, y] < minimumHeight)
                        minimumHeight = heightData[x, y];
                    if (heightData[x, y] > maximumHeight)
                        maximumHeight = heightData[x, y];
                }
            }

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    heightData[x, y] = (heightData[x, y] - minimumHeight) / (maximumHeight - minimumHeight) * 30;

            // Setup Physics
            heightMapInfo = new HeightMapInfo(heightData, 1);

            if (heightMapObject != null)
            {
                heightMapObject.DisableComponent();
                heightMapObject = null;
            }

            heightMapObject = new HeightMapObject(
                heightMapInfo,
                new Vector2(heightMapInfo.Width / 2, -heightMapInfo.Height / 2 + heightMapInfo.Height));

            return heightData;
        }

        private VertexPositionNormalTexture[] CreateVertices()
        {
            int width = heightData.GetLength(0);
            int height = heightData.GetLength(1);
            VertexPositionNormalTexture[] terrainVertices = new VertexPositionNormalTexture[width * height];

            // Calculate position, normal, and texcoords for vertices
            int i = 0;
            for (int z = 0; z < height; ++z)
            {
                for (int x = 0; x < width; ++x)
                {
                    Vector3 position = new Vector3(x, heightData[x, z], -z);
                    Vector3 normal = new Vector3(0, 0, 1);
                    Vector2 texCoord = new Vector2((float)x / 30.0f, (float)z / 30.0f);

                    terrainVertices[i++] = new VertexPositionNormalTexture(position, normal, texCoord);
                }
            }

            return terrainVertices;
        }

        private int[] CreateIndices()
        {
            int width = heightData.GetLength(0);
            int height = heightData.GetLength(1);
            int[] terrainIndices = new int[(width) * 2 * (height - 1)];

            int i = 0;
            int z = 0;
            while (z < height - 1)
            {
                for (int x = 0; x < width; ++x)
                {
                    terrainIndices[i++] = x + z * width;
                    terrainIndices[i++] = x + (z + 1) * width;
                }
                ++z;

                if (z < height - 1)
                {
                    for (int x = width - 1; x >= 0; --x)
                    {
                        terrainIndices[i++] = x + (z + 1) * width;
                        terrainIndices[i++] = x + z * width;
                    }
                }
                ++z;
            }

            return terrainIndices;
        }

        private VertexPositionNormalTexture[] GenerateNormalsForTriangleStrip(VertexPositionNormalTexture[] vertices, int[] indices)
        {
            for (int i = 0; i < vertices.Length; ++i)
                vertices[i].Normal = new Vector3(0, 0, 0);

            bool swappedWinding = false;
            for (int i = 2; i < indices.Length; ++i)
            {
                Vector3 firstVec = vertices[indices[i - 1]].Position - vertices[indices[i]].Position;
                Vector3 secondVec = vertices[indices[i - 2]].Position - vertices[indices[i]].Position;
                Vector3 normal = Vector3.Cross(firstVec, secondVec);
                normal.Normalize();

                if (swappedWinding)
                    normal *= -1;

                if (!float.IsNaN(normal.X))
                {
                    vertices[indices[i]].Normal += normal;
                    vertices[indices[i - 1]].Normal += normal;
                    vertices[indices[i - 2]].Normal += normal;
                }

                swappedWinding = !swappedWinding;
            }

            for (int i = 0; i < vertices.Length; ++i)
                vertices[i].Normal.Normalize();

            return vertices;
        }

        private void CreateBuffers(VertexPositionNormalTexture[] vertices, int[] indices)
        {
            terrainVertexBuffer = new VertexBuffer(
                Engine.GraphicsDevice,
                myVertexDeclaration,
                vertices.Length,
                BufferUsage.WriteOnly);
            terrainVertexBuffer.SetData(vertices);

            terrainIndexBuffer = new IndexBuffer(
                Engine.GraphicsDevice,
                typeof(int),
                indices.Length,
                BufferUsage.WriteOnly);
            terrainIndexBuffer.SetData(indices);
        }

        public override void Draw()
        {
            // Requires camera
            Camera camera = Engine.Services.GetService<Camera>();
            if (camera == null)
                throw new Exception("The engine services does not contain a camera required by the terrain to draw");

            // Set effect values
            Material.Prepare3DDraw(MathUtil.CreateWorldMatrix(Position, Rotation, Scale));

            int width = heightData.GetLength(0);
            int height = heightData.GetLength(1);

            // Terrain uses different vertex winding than normal models, so set the new one
            Engine.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            foreach (EffectPass pass in Material.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                // Draw terrain vertices and indices
                Engine.GraphicsDevice.SetVertexBuffer(terrainVertexBuffer);
                Engine.GraphicsDevice.Indices = terrainIndexBuffer;
                Engine.GraphicsDevice.DrawIndexedPrimitives(
                    Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip,
                    0, 0, width * height, 0, width * 2 * (height - 1) - 2);
            }

            Engine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }
    }
}