using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Utils;

namespace Innovation
{
    public class HeightMapObject : PhysicsObject
    {
        public HeightMapInfo info;

        public HeightMapObject(HeightMapInfo heightMapInfo, Vector2 shift) : base()
        {
            Setup(heightMapInfo, shift);
        }

        public HeightMapObject(HeightMapInfo heightMapInfo, Vector2 shift, GameScreen parent) : base(parent)
        {
            Setup(heightMapInfo, shift);
        }

        void Setup(HeightMapInfo heightMapInfo, Vector2 shift)
        {
            // A dummy. The physics object uses its position to get draw pos
            Body = new Body();

            CollisionSkin = new CollisionSkin(null);

            info = heightMapInfo;
            Array2D field = new Array2D(heightMapInfo.Heights.GetUpperBound(0), heightMapInfo.Heights.GetUpperBound(1));

            for (int x = 0; x < heightMapInfo.Heights.GetUpperBound(0); ++x)
            {
                for (int z = 0; z < heightMapInfo.Heights.GetUpperBound(1); ++z)
                {
                    field.SetAt(x, z, heightMapInfo.Heights[x, z]);
                }
            }

            // Move dummy body. The body isn't connected to the collision skin.
            // But the base class should know where to draw the model.
            Body.MoveTo(new Vector3(shift.X, 0, shift.Y), Matrix.Identity);

            CollisionSkin.AddPrimitive(new Heightmap(field, shift.X, shift.Y, 1, 1), new MaterialProperties(0.7f, 0.7f, 0.6f));

            PhysicsSystem.CurrentPhysicsSystem.CollisionSystem.AddCollisionSkin(CollisionSkin);
        }
    }


}