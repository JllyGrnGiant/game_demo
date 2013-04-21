using Microsoft.Xna.Framework.Graphics;

namespace Innovation
{
    public class TexturedMaterial : Material
    {
        Texture2D texture;

        public virtual Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                SetTextureToEffect(Effect, texture);
            }
        }

        public override Effect Effect
        {
            get { return base.Effect; }
            set
            {
                SetTextureToEffect(value, GetTextureFromEffect(base.Effect));
                base.Effect = value;
            }
        }

        public override Material CreateFromMeshPart(ModelMeshPart MeshPart)
        {
            base.CreateFromMeshPart(MeshPart);
            this.texture = GetTextureFromEffect(MeshPart.Effect);
            return this;
        }

        internal static Texture2D GetTextureFromEffect(Effect Effect)
        {
            if (Effect == null)
                return null;

            if (Effect is BasicEffect)
                return ((BasicEffect)Effect).Texture;
            else if (Effect.Parameters["Texture"] != null)
                return Effect.Parameters["Texture"].GetValueTexture2D();

            return null;
        }

        internal static void SetTextureToEffect(Effect Effect, Texture2D Texture)
        {
            if (Effect == null)
                return;

            if (Effect is BasicEffect)
                ((BasicEffect)Effect).Texture = Texture;
            else if (Effect.Parameters["Texture"] != null)
                Effect.Parameters["Texture"].SetValue(Texture);
        }
    }
}