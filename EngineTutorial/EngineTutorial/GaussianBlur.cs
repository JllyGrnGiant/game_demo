using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Innovation
{
    public enum GaussianBlurDirection { Horizontal, Vertical };

    // Post processor that applies Gaussian Blur to an image or frame buffer
    public class GaussianBlur : PostProcessor
    {
        // Offsets and weights for horizontal and vertical blurs
        Vector2[] sampleOffsetsH = new Vector2[15];
        float[] sampleWeightsH = new float[15];

        Vector2[] sampleOffsetsV = new Vector2[15];
        float[] sampleWeightsV = new float[15];

        public GaussianBlur(int Width, int Height)
            : base(Engine.Content.Load<Effect>("Content/GaussianBlur"), Width, Height)
        {
            Setup(Width, Height);
        }

        public GaussianBlur(int Width, int Height, GameScreen Parent)
            : base(Engine.Content.Load<Effect>("Content/GaussianBlur"), Width, Height, Parent)
        {
            Setup(Width, Height);
        }

        // Calculate weights and offsets
        void Setup(int Width, int Height)
        {
            Vector2 texelSize = new Vector2(1f / Width, 1f / Height);

            SetBlurParameters(texelSize.X, 0, ref sampleOffsetsH, ref sampleWeightsH);
            SetBlurParameters(0, texelSize.Y, ref sampleOffsetsV, ref sampleWeightsV);
        }

        // Borrowed from "Shadow Mapping" by Andrew Joli
        // http://www.ziggyware.com/readarticle.php?article_id=161
        void SetBlurParameters(float dx, float dy, ref Vector2[] vSampleOffsets, ref float[] fSampleWeights)
        {
            // First sample always has zero offset
            fSampleWeights[0] = ComputeGaussian(0);
            vSampleOffsets[0] = new Vector2(0);

            // Maintain sum of weighting values
            float totalWeights = fSampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from center
            for (int i = 0; i < 15 / 2; ++i)
            {
                // Store weights for positive and negative taps
                float weight = ComputeGaussian(i + 1);

                fSampleWeights[i * 2 + 1] = weight;
                fSampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // The 1.5 offset kicks things off by
                // positioning us nicely in between two texels
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coord offsets for positive and negative taps
                vSampleOffsets[i * 2 + 1] = delta;
                vSampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize list of sample weightings so they'll always sum to one
            for (int i = 0; i < fSampleWeights.Length; ++i)
                fSampleWeights[i] /= totalWeights;
        }

        private float ComputeGaussian(float n)
        {
            float theta = 2.0f + float.Epsilon;
            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) * Math.Exp(-(n * n) / (2 * theta * theta)));
        }

        // Applies post processor to texture in specified direction
        public void Draw(GaussianBlurDirection Direction, Texture2D Input)
        {
            this.Input = Input;
            SetParameters(Direction);
            base.Draw();
        }

        public override void Draw()
        {
            GetInputFromFrameBuffer();
            Engine.GraphicsDevice.Clear(Color.Black);
            SetParameters(GaussianBlurDirection.Horizontal);
            base.Draw();

            GetInputFromFrameBuffer();
            Engine.GraphicsDevice.Clear(Color.Black);
            SetParameters(GaussianBlurDirection.Vertical);
            base.Draw();
        }

        void SetParameters(GaussianBlurDirection Direction)
        {
            if (Direction == GaussianBlurDirection.Horizontal)
            {
                Effect.Parameters["sampleWeights"].SetValue(sampleWeightsH);
                Effect.Parameters["sampleOffsets"].SetValue(sampleOffsetsH);
            }
            else if (Direction == GaussianBlurDirection.Vertical)
            {
                Effect.Parameters["sampleWeights"].SetValue(sampleWeightsV);
                Effect.Parameters["sampleOffsets"].SetValue(sampleOffsetsV);
            }
        }
    }
}