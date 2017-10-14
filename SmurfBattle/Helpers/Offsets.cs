using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;

namespace SmurfBattle
{
    public static class Offsets
    {
        public static int UWorld = 0x37D67A8;
        public static int GNames = 0x36D9590;
        public static int GameInst = 0x140;
        public static int ULocalP = 0x38;
        public static int ViewPort = 0x58;
        public static int pUWorld = 0x80;
        public static int ULevel = 0x30;//PersistentLevel
        public static int LPlayerControl = 0x30;
        public static int LocalPlayer = 0x3A8;//ATSLCharacter : Pawn
        //silah
        public static int WeaponProcess = 0x9E8;
        public static int CurrentWeap = 0x448;
        public static int EquipWeaps = 0x438;
        public static int RecoilInfo = 0xA80;
        public static int Trajectory = 0xA20;
        public static int GunAnim = 0x908;
        public static int WeaponConfig = 0x0538;
        //
        public static int LLocation = 0x70;
        //Actors
        public static int ActorPtr = 0xA0;
        public static int ActorID = 0x18;
        public static int ActorHP = 0x1068;
        public static int RootComponent = 0x180;
        public static int GroggyH = 0x1070;
        public static int ActorLoc = 0x1E0;
        public static int ActorRot = 0x1EC;
        public static int CharacterMovement = 0x408;
        public static int Stance = 0x3D0;


        //Bone ESP
        public static int Mesh = 0x400;//USkeletalMeshComponent;
        public static int CachedBone = 0x958;
        //Camera
        public static int CameraManager = 0x0438;
        public static int CameraCache = 0x410;
        public static int POV = 0x10;
    }
}

