using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Game_Enhancement___C_
{
    public static class Calculate
    {
        public static Vector2 WorldToScreen(float[] viewMatrix, Vector3 pos, Vector2 windowSize)
        {
            // depth variable
            // calculate screenW
            float screenW = (viewMatrix[12] * pos.X) + (viewMatrix[13] * pos.Y) + (viewMatrix[14] + pos.Z) + viewMatrix[15];

            // if entity is on our screen (in front of us)
            if (screenW > 0.001f)
            {
                // calculate screen X and Y
                // we add the last value as translation
                float screenX = (viewMatrix[0] * pos.X) + (viewMatrix[1] * pos.Y) + (viewMatrix[2] * pos.Z) + viewMatrix[3];
                float screenY = (viewMatrix[4] * pos.X) + (viewMatrix[5] * pos.Y) + (viewMatrix[6] * pos.Z) + viewMatrix[7];

                // perform perspective division:
                // https://learnopengl.com/Getting-started/Coordinate-Systems &
                // https://www.scratchapixel.com/lessons/3d-basic-rendering/perspective-and-orthographic-projection-matrix/projection-matrix-GPU-rendering-pipeline-clipping.html
                float X = (windowSize.X / 2) + (windowSize.X / 2) * screenX / screenW;
                float Y = (windowSize.Y / 2) - (windowSize.Y / 2) * screenY / screenW;

                return new Vector2(X, Y);
            }
            
            // entity was behind us (not seen on screen)
            return new Vector2(-99, -99);
        }
    }
}
