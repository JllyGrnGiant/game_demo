﻿using Microsoft.Xna.Framework;
using System;

namespace Innovation
{
    public class GodCamera : Camera
    {
        Vector3 rotation;
        Vector3 translation;

        public GodCamera(GameScreen Parent) : base(Parent) { }
        public GodCamera() : base() { }

        // Adds to rotation and translation to change camera view
        public void RotateTranslate(Vector3 Rotation, Vector3 Translation)
        {
            translation += Translation;
            rotation += Rotation;
        }

        public override void Update()
        {
            Rotation = MathUtil.Vector3ToMatrix(rotation);

            translation = Vector3.Transform(translation, Rotation);
            Position += translation;

            translation = Vector3.Zero;

            Target = Vector3.Add(Position, Rotation.Forward);
            
            base.Update();
        }
    }
}