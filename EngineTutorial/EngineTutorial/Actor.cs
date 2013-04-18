using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Innovation
{
    public class Actor : Component, I3DComponent
    {
        // The model to draw
        Model model;

        public Dictionary<ModelMeshPart, Material> Materials = new Dictionary<ModelMeshPart, Material>();

        // I3DComponent values
        public virtual Vector3 Position { get; set; }
        public Vector3 EulerRotation
        {
            get { return MathUtil.MatrixToVector3(Rotation); }
            set { Rotation = MathUtil.Vector3ToMatrix(value); }
        }
        public virtual Matrix Rotation { get; set; }
        public virtual Vector3 Scale { get; set; }
        public virtual BoundingBox BoundingBox
        {
            get
            {
                return new BoundingBox(
                    Position - (Scale / 2),
                    Position + (Scale / 2));
            }
        }

        // Constructors take a model to draw and position
        public Actor(Model Model, Vector3 Position)
            : base()
        {
            Setup(Model, Position);
        }

        public Actor(Model Model, Vector3 Position, GameScreen Parent)
            : base(Parent)
        {
            Setup(Model, Position);
        }

        // Provide method to setup actor so we don't need to rewrite in each constructor
        void Setup(Model Model, Vector3 Position)
        {
            foreach (ModelMesh mesh in Model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                    Materials.Add(part, new MeshPartMaterial().CreateFromMeshPart(part));

            this.model = Model;
            this.Position = Position;
            Scale = Vector3.One;
            EulerRotation = Vector3.Zero;
        }

        public override void Draw()
        {
            // Look for a camera in the service container
            Camera camera = (Camera)Engine.Services.GetService(typeof(Camera));

            // Throw an exception if one isn't present
            if (camera == null)
            {
                throw new Exception("Camera not found in engine's "
                    + "service container, cannot draw");
            }

            // Generate the world matrix (describes object's movement in 3D
            Matrix world = MathUtil.CreateWorldMatrix(Position, Rotation, Scale);

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Set some renderstates so the model will draw properly
            Engine.GraphicsDevice.BlendState = BlendState.Opaque;
            Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Loop through meshes and effects and set them up to draw
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Materials[part].Prepare3DDraw(world);

                    // Enable default lighting if possible
                    Effect effect = Materials[part].Effect;
                    if (effect is BasicEffect)
                        ((BasicEffect)effect).EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }
    }
}