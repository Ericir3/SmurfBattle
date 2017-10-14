using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
//using SharpDX.Mathematics.Interop;

namespace SmurfBattle
{
    class Matriks
    {
        static float DotProduct(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }
        public static SharpDX.Matrix ToMatrix(float Pitch, float Yaw, float Roll)
        {

            //Origin = default(Vector3); //Set to default Vec3 in C#, which has all elements 0

            //Calculate Radian Values of Angles for Trig Functions
            float radPitch = (float)(Pitch * Math.PI / 180f);
            float radYaw = (float)(Yaw * Math.PI / 180f);
            float radRoll = (float)(Roll * Math.PI / 180f);

            float SP = (float)Math.Sin(radPitch);
            float CP = (float)Math.Cos(radPitch);
            float SY = (float)Math.Sin(radYaw);
            float CY = (float)Math.Cos(radYaw);
            float SR = (float)Math.Sin(radRoll);
            float CR = (float)Math.Cos(radRoll);

            //Create 4x4 Matrix
            SharpDX.Matrix m = new SharpDX.Matrix
            {
                M11 = CP * CY,
                M12 = CP * SY,
                M13 = SP,
                M14 = 0f,

                M21 = SR * SP * CY - CR * SY,
                M22 = SR * SP * SY + CR * CY,
                M23 = -SR * CP,
                M24 = 0f,

                M31 = -(CR * SP * CY + SR * SY),
                M32 = CY * SR - CR * SP * SY,
                M33 = CR * CP,
                M34 = 0f,

                M41 = 0.0f,
                M42 = 0.0f,
                M43 = 0.0f,
                M44 = 1.0f
            };

            //Return matrix
            return m;
        }

        public static void WorldToScreen2(Vector3 WorldLocation, IntPtr CameraManager, out Vector2 Screenlocation)
        {

            //Initialize screen location
            Screenlocation = new Vector2(0, 0);

            POVInfo POV = AMK.Mem.Read<POVInfo>((CameraManager + Offsets.CameraCache) + Offsets.POV);// CameraManager + 0x410 + 0x10; 0x03B8
            SharpDX.Vector3 Rotation = POV.Rotation; //AMK.Mem.Read<Vector3>(POV + 0x00C);
            // AMK.Mem.Read<Vector3>(POV);
            //Console.WriteLine($"POV:{Rotation.Pitch},{Rotation.Yaw},{Rotation.Roll} - Loc: {Location.ToString()}");
            Vector3 vAxisX, vAxisY, vAxisZ;
            SharpDX.Matrix m = ToMatrix(Rotation.X, Rotation.Y, Rotation.Z);

            vAxisX = new Vector3(m.M11, m.M12, m.M13);
            vAxisY = new Vector3(m.M21, m.M22, m.M23);
            vAxisZ = new Vector3(m.M31, m.M32, m.M33);

            Vector3 vDelta = new Vector3(WorldLocation.X - POV.Location.X, WorldLocation.Y - POV.Location.Y, WorldLocation.Z - POV.Location.Z);
            Vector3 vTransformed = new Vector3(Vector3.Dot(vDelta, vAxisY), Vector3.Dot(vDelta, vAxisZ), Vector3.Dot(vDelta, vAxisX));

            if (vTransformed.Z < 1f)
                vTransformed.Z = 1f;

            float FovAngle = POV.FOV;// AMK.Mem.Read<float>(POV + 0x018); ;
            float ScreenCenterX = 1920 / 2;
            float ScreenCenterY = 1080 / 2;

            Screenlocation.X = ScreenCenterX + vTransformed.X * (ScreenCenterX / (float)Math.Tan(FovAngle * (float)Math.PI / 360)) / vTransformed.Z;
            Screenlocation.Y = ScreenCenterY - vTransformed.Y * (ScreenCenterX / (float)Math.Tan(FovAngle * (float)Math.PI / 360)) / vTransformed.Z;
        }
    }
}
