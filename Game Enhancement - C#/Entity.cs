using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Game_Enhancement___C_
{
    public class Entity
    {
        public Vector3 position;
        public Vector3 viewOffset;
        public Vector2 position2D;
        public Vector2 viewPosition2D;
        public Vector3 origin;
        public int team;
        public float distance;

        public List<Vector3> bones;
        public List<Vector2> bones2d;

    }

    public enum BoneIds // they need to be in numerical value order, otherwise iterating will mess up
                        // these are from the perspective of the entity, handright will be their right hand
    {
        Waist = 0, // 1
        Neck = 5, // 2
        Head = 6, // 3
        ShoulderLeft = 8, // 4
        ForeLeft = 9, // 5
        HandLeft = 11, // 6
        ShoulderRight = 13, // 7
        ForeRight = 14, // 8
        HandRight = 16, // 9
        KneeLeft = 23, // 10
        FeetLeft = 24, // 11
        KneeRight = 26, // 12
        Feetright = 27 // 13
    }
}
