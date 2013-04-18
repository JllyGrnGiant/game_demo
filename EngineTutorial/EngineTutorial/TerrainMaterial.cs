using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Innovation
{
    public class TerrainMaterial : TexturedMaterial
    {
        public TerrainMaterial(Texture2D Texture)
            : base()
        {
            this.Effect = new BasicEffect(Engine.GraphicsDevice);
            this.Texture = Texture;

            SetupEffect();
        }

        private void SetupEffect()
        {
            BasicEffect basicEffect = (BasicEffect)this.Effect;

            basicEffect.Texture = Texture;
            basicEffect.TextureEnabled = true;
            basicEffect.EnableDefaultLighting();
            basicEffect.AmbientLightColor = new Vector3(0.3f, 0.3f, 0.3f);
            basicEffect.SpecularColor = new Vector3(0, 0, 0);
            basicEffect.DirectionalLight0.Direction = new Vector3(1, -1, 1);
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight1.Enabled = false;
            basicEffect.DirectionalLight2.Enabled = false;
        }

        public override void Prepare3DDraw(Matrix World)
        {
            base.Prepare3DDraw(World);

            Engine.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}