using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Innovation
{
    // Keeps track of an effect and handles basic properties
    public class Material
    {
        Effect effect;

        public virtual Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        // Sets up material from values embedded in a MeshPart
        public virtual Material CreateFromMeshPart(ModelMeshPart MeshPart)
        {
            this.effect = MeshPart.Effect;
            return this;
        }

        public virtual void Prepare3DDraw(Matrix World)
        {
            if (effect is BasicEffect)
            {
                ((BasicEffect)effect).World = World;
                ((BasicEffect)effect).View = Engine.Services.GetService<Camera>().View;
                ((BasicEffect)effect).Projection = Engine.Services.GetService<Camera>().Projection;
            }

            //effect.Parameters["World"].SetValue(World);
            //effect.Parameters["View"].SetValue(
            //    Engine.Services.GetService<Camera>().View);
            //effect.Parameters["Projection"].SetValue(
            //    Engine.Services.GetService<Camera>().Projection);
        }
    }
}