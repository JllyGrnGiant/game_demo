using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.ComponentModel;

namespace Innovation
{
    public class Terrain2 : Component, I3DComponent
    {
        float[,] heights;
        float heightMod = 1;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        string textureMap;
        Texture2D texMap;

        float uvScaling = 30.0f;

        string redTexture;
        Texture2D rTex;
        string greenTexture;
        Texture2D gTex;
        string blueTexture;
        Texture2D bTex;
        string alphaTexture;
        Texture2D aTex;

        HeightMapObject physicsHeightmap;

        // Setting [Browsable(false)] will make the property not show up in a properties box
        // (like the one in the editor)
        [Browsable(false)]
        public Vector3 Position { get; set; }

        [Browsable(false)]
        public Vector3 EulerRotation { get; set; }

        [Browsable(false)]
        public Matrix Rotation { get; set; }

        [Browsable(false)]
        public Vector3 Scale { get; set; }

        [Browsable(false)]
        public BoundingBox BoundingBox { get; set; }

        public string TextureMap
        {
            get { return textureMap; }
            set { textureMap = value; texMap = Engine.Content.Load<Texture2D>(value); }
        }

        public float UVScaling
        {
            get { return uvScaling; }
            set { uvScaling = value; }
        }

        public Vector3 LightDirection { get; set; }
        public Vector3 AmbientColor { get; set; }
        public Vector3 LightColor { get; set; }
        public float FogDensity { get; set; }
        public float MaxFog { get; set; }
        public float MinFog { get; set; }
        public float FogStart { get; set; }
        public float FogEnd { get; set; }
        public Vector3 FogColor { get; set; }

        public string RedTexture
        {
            get { return redTexture; }
            set { redTexture = value; if (string.IsNullOrEmpty(value)) rTex = null; else rTex = Engine.Content.Load<Texture2D>(value); }
        }

        public string GreenTexture
        {
            get { return greenTexture; }
            set { greenTexture = value; if (string.IsNullOrEmpty(value)) gTex = null; else gTex = Engine.Content.Load<Texture2D>(value); }
        }

        public string BlueTexture
        {
            get { return blueTexture; }
            set { blueTexture = value; if (string.IsNullOrEmpty(value)) bTex = null; else bTex = Engine.Content.Load<Texture2D>(value); }
        }

        public string AlphaTexture
        {
            get { return alphaTexture; }
            set { alphaTexture = value; if (string.IsNullOrEmpty(value)) aTex = null; else aTex = Engine.Content.Load<Texture2D>(value); }
        }

        void PopulateBuffers(float[,] heights, float heightMod)
        {
            this.heights = heights;
            if (heights == null)
                return;

            // Offset the heights entered by the height scaling value
            float[,] heightsModded = new float[heights.GetLength(0), heights.GetLength(1)];

            for (int z = 0; z < heights.GetLength(1); z++)
                for (int x = 0; x < heights.GetLength(0); x++)
                    heightsModded[x, z] = heights[x, z] * heightMod;

            // Generate the vertices and indices
            VertexPositionNormalTexture[] vertices = BuildVertices(heightsModded);
            int[] indices = BuildIndices(heightsModded);

            // Generate normals for the index buffer
            vertices = GenerateNormalsForTriangleStrip(vertices, indices);

            // Create the vertex declaration
            vertexBuffer = new VertexBuffer(Engine.GraphicsDevice, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);

            // Create the index buffer with the generated indices
            indexBuffer = new IndexBuffer(Engine.GraphicsDevice, typeof(int), indices.Length,
                BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);

            float highest = 0;

            // Find highest vertex
            for (int x = 0; x < heights.GetLength(0); x++)
                for (int z = 0; z < heights.GetLength(1); z++)
                    if (heightsModded[x, z] > highest)
                        highest = heightsModded[x, z];

            // Rebuild the BoundingBox centered around (0, 0, 0), with ends at 1/2 the width
            // and depth of the terrain, and as high as the highest (scaled) vertex
            BoundingBox = new BoundingBox(new Vector3(-heights.GetLength(0) / 2, 0,
                heights.GetLength(1) / 2), new Vector3(heights.GetLength(0) / 2, highest,
                -heights.GetLength(1) / 2));

            // Rebuilt the physics heightmap simulation object
            CreatePhysicsHeightmap(heightsModded);
        }

        VertexPositionNormalTexture[] BuildVertices(float[,] heights)
        {
            // Find the width and depth of the terrain by getting the lengths of the
            // heights array
            int width = heights.GetLength(0);
            int depth = heights.GetLength(1);

            // Create list of vertices
            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[width * depth];

            int i = 0;

            // Move from front to back, row by row from left to right, creating vertices and UV values
            for (int z = 0; z < depth; z++)
                for (int x = 0; x < width; x++)
                {
                    Vector3 position = new Vector3(x - width / 2, heights[x, z], -z + depth / 2);

                    // Temporary normal will be recalculated later
                    Vector3 normal = new Vector3(0, 0, 0);

                    // Since we want a square texture to completely fill the terrain
                    Vector2 uv = new Vector2((float)x / (float)width, (float)z / (float)depth);

                    vertices[i++] = new VertexPositionNormalTexture(position, normal, uv);
                }

            return vertices;
        }

        // Generates the indices for the heights given
        int[] BuildIndices(float[,] heights)
        {
            int width = heights.GetLength(0);
            int depth = heights.GetLength(1);

            int[] indices = new int[width * 2 * (depth - 1)];

            int i = 0;
            int z = 0;

            while (z < depth - 1)
            {
                for (int x = 0; x < width; x++)
                {
                    indices[i++] = x + z * width;
                    indices[i++] = x + (z + 1) * width;
                }

                z++;

                if (z < depth - 1)
                {
                    for (int x = width - 1; x >= 0; x--)
                    {
                        indices[i++] = x + (z + 1) * width;
                        indices[i++] = x + z * width;
                    }
                }

                z++;
            }

            return indices;
        }

        // Calculates normals for given list of vertices and indices
        VertexPositionNormalTexture[] GenerateNormalsForTriangleStrip(VertexPositionNormalTexture[] vertices, int[] indices)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            bool swappedWinding = false;

            for (int i = 2; i < indices.Length; i++)
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

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();

            return vertices;
        }

        // Rebuilds the HeightMapObject from the given heights
        void CreatePhysicsHeightmap(float[,] heights)
        {
            // If the physics object has been created already with previous settings...
            if (physicsHeightmap != null)
            {
                // And there is a physics simulator available
                Physics p = Engine.Services.GetService<Physics>();

                if (p != null)
                {
                    // Disable old height map object and remove it from engine
                    physicsHeightmap.Body.DisableBody();
                    p.PhysicsSystem.RemoveBody(physicsHeightmap.Body);
                    physicsHeightmap.DisableComponent();
                }
            }

            // Create new HeightMapObject
            physicsHeightmap = new HeightMapObject(new HeightMapInfo(heights, 1),
                new Vector2(-1.0f, -1.0f));
        }

        // Accepts a texture and extracts the height values from it to create an array of heights
        void BuildTerrainFromTexture(Texture2D Texture, float heightMod)
        {
            Texture2D t = Texture;

            // Extract the pixel values into an array of Colors
            Color[] colors = new Color[t.Width * t.Height];
            t.GetData(colors);

            // Create 2D array of heights
            float[,] heights = new float[t.Width, t.Height];

            // Move from left to right
            for (int x = 0; x < t.Width; x++)
                for (int z = 0; z < t.Height; z++)
                    heights[x, z] = colors[x + z * t.Width].R / 5f;

            PopulateBuffers(heights, heightMod);
        }

        public float HeightMod
        {
            get { return heightMod; }
            set { heightMod = value; if (heights != null) PopulateBuffers(heights, value); }
        }

        Effect effect;

        public Terrain2(Texture2D HeightMap, GameScreen Parent)
            : base(Parent)
        {
            Setup(HeightMap);
        }

        public Terrain2(Texture2D HeightMap)
            : base()
        {
            Setup(HeightMap);
        }

        void Setup(Texture2D HeightMap)
        {
            effect = Engine.Content.Load<Effect>("Content/Effect1");
            //effect = new BasicEffect(Engine.GraphicsDevice);
            BuildTerrainFromTexture(HeightMap, heightMod);
            LightDirection = new Vector3(1, 1, 1);
            AmbientColor = new Vector3(0.2f, 0.2f, 0.2f);
            LightColor = new Vector3(1, 1, 1);

            FogStart = 20;
            FogEnd = 150;
            FogDensity = 1;
            FogColor = new Vector3(Color.CornflowerBlue.R / 255f, Color.CornflowerBlue.G / 255f, Color.CornflowerBlue.B / 255f);
            MaxFog = 1;
            MinFog = 0;
        }

        public override void Draw()
        {
            Camera c = Engine.Services.GetService<Camera>();

            if (c == null || heights == null || vertexBuffer == null || indexBuffer == null)
                return;

            int width = heights.GetLength(0);
            int depth = heights.GetLength(1);

            // We need to flip the terrain across the z axis to make it fit the physics heightmap
            effect.Parameters["World"].SetValue(Matrix.CreateScale(1, 1, -1));
            effect.Parameters["View"].SetValue(c.View);
            effect.Parameters["Projection"].SetValue(c.Projection);
            effect.Parameters["LightDirection"].SetValue(LightDirection);
            effect.Parameters["Ambient"].SetValue(AmbientColor);
            effect.Parameters["LightColor"].SetValue(LightColor);
            effect.Parameters["FogStart"].SetValue(FogStart);
            effect.Parameters["FogEnd"].SetValue(FogEnd);
            effect.Parameters["FogDensity"].SetValue(FogDensity);
            effect.Parameters["FogColor"].SetValue(FogColor);
            effect.Parameters["MaxFog"].SetValue(MaxFog);
            effect.Parameters["MinFog"].SetValue(MinFog);

            if (texMap != null)
                effect.Parameters["TextureMap"].SetValue(texMap);

            effect.Parameters["UVScaling"].SetValue(uvScaling);

            if (rTex != null)
                effect.Parameters["RedTexture"].SetValue(rTex);
            if (gTex != null)
                effect.Parameters["GreenTexture"].SetValue(gTex);
            if (bTex != null)
                effect.Parameters["BlueTexture"].SetValue(bTex);
            if (aTex != null)
                effect.Parameters["AlphaTexture"].SetValue(aTex);

            //effect.EnableDefaultLighting();

            // The normals will be inverted by the z-flip so we need to switch the cullmode around to draw properly
            Engine.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                // Set the vertex source, index source, and VertexDeclaration
                Engine.GraphicsDevice.SetVertexBuffer(vertexBuffer);
                Engine.GraphicsDevice.Indices = indexBuffer;

                // Draw TriangleStrip using vertices and indices
                Engine.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, width * depth, 0, width * 2 * (depth - 1) - 2);
            }

            Engine.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }
    }
}
