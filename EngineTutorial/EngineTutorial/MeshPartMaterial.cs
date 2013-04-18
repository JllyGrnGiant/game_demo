using Microsoft.Xna.Framework.Graphics;

namespace Innovation
{
    public class MeshPartMaterial : TexturedMaterial
    {
        ModelMeshPart meshPart;

        public ModelMeshPart MeshPart
        {
            get { return meshPart; }
            set
            {
                meshPart = value;
                value.Effect = Effect;
            }
        }

        public override Effect Effect
        {
            get { return base.Effect; }
            set
            {
                SetTextureToEffect(value, GetTextureFromEffect(base.Effect));
                base.Effect = value;
                MeshPart.Effect = value;
            }
        }

        public override Material CreateFromMeshPart(ModelMeshPart MeshPart)
        {
            base.CreateFromMeshPart(MeshPart);
            this.meshPart = MeshPart;
            return this;
        }
    }
}