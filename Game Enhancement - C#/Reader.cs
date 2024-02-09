using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Enhancement___C_;
using System.Numerics;
using System.Net.Http.Headers;

namespace Game_Enhancement___C_
{
    public class Reader // more memory reading capabilities
    {
        MemManager memoryManage;

        public Reader(MemManager manage)
        {
            memoryManage = manage;
        }

        public List<Vector3> ReadBones(IntPtr boneAddress)
        {
            int maxBones = 27 * 32 + 16;
            byte[] boneBytes = memoryManage.ReadBytes(boneAddress, maxBones); // get max, 27 = id, 32 = step
            List<Vector3> bones = new List<Vector3>();
            foreach(var boneId in Enum.GetValues(typeof(BoneIds))) // loop through enum
            {
                float x = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 0);
                float y = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 4);
                float z = BitConverter.ToSingle(boneBytes, (int)boneId * 32 + 8);
                Vector3 currentBone = new Vector3(x, y, z);
                bones.Add(currentBone);
            }
            return bones;
        }
           
        public List<Vector2> ReadBones2D(List<Vector3> bones, float[] viewMatrix, Vector2 screenSize)
        {
            List<Vector2> bones2D = new List<Vector2>();

            foreach (Vector3 bone in bones)
            {
                Vector2 bone2d = Calculate.WorldToScreen(viewMatrix, bone, screenSize);
                bones2D.Add(bone2d);
            }
            return bones2D;
        }
    }
}
