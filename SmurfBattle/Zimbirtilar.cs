using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Memory;
//using System.Numerics;
using System.Collections;

namespace SmurfBattle
{
    public struct POVInfo
    {
        public SharpDX.Vector3 Location;               // 0x0000(0x000C) (CPF_Edit, CPF_BlueprintVisible, CPF_ZeroConstructor, CPF_IsPlainOldData)
        public SharpDX.Vector3 Rotation;              // 0x000C(0x000C) (CPF_Edit, CPF_BlueprintVisible, CPF_ZeroConstructor, CPF_IsPlainOldData)
        public float FOV;                      // 0x0018(0x0004) (CPF_Edit, CPF_BlueprintVisible, CPF_ZeroConstructor, CPF_IsPlainOldData)
    }
    public struct Sekmeme
    {
        public Form1.FTrajectoryWeaponData Trac;
        public Form1.FWeaponGunAnim GunAnim;
        public Form1.FRecoilInfo Recoil;
        //public Form1.FWeaponData WeapC;
    }
    public static class AMK
    {
        public static IntPtr Weapon { get; set; }
        public static IntPtr CameraManager { get; set; }
        public static Dictionary<IntPtr,Sekmeme> weapons = new Dictionary<IntPtr, Sekmeme>();
        public static Dictionary<int, string> tGnames = new Dictionary<int, string>();
        public static IntPtr GNames { get; set; }
        public static bool Hile { get; set; }
        public static bool Reco { get; set; }
        public static bool Bone { get; set; }
        public static IntPtr[] hMods { get; set; }
        public static IntPtr AMKProcessi { get; set; }
        public static NativeMemory Mem { get; set; }

        public static IntPtr LocalPlayer {get; set;}
        public static IntPtr LocalPlayerControl { get; set; }

        public static IntPtr ULevel { get; set; }
        public static IntPtr UWorld { get; set; }
        public static IntPtr GameInstance { get; set; }

    }
}

