using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using SmurfBattle;
using System.Drawing;
using System.Threading;
using System.Windows.Input.Manipulations;
using Memory;
using System.Text.RegularExpressions;

namespace SmurfBattle
{
    class Program
    {

        static void Main(string[] args)
        {

            if (Environment.GetCommandLineArgs().Length < 2)
            {
                MessageBox.Show("No handle");
                return;
            }
            else
            {
                AMK.AMKProcessi = (IntPtr)Convert.ToUInt32(Environment.GetCommandLineArgs()[1]);
                //System.IO.File.WriteAllText(System.IO.Path.Combine(Environment.CurrentDirectory, "handle"), Environment.GetCommandLineArgs()[1]);
            }
            // pExceptionHandler

            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => MessageBox.Show(e.ExceptionObject.ToString());

                // GET BASE ADDRESS OF GAME PROCESS
                AMK.hMods = new IntPtr[1024];
            var pModules = GCHandle.Alloc(AMK.hMods, GCHandleType.Pinned);

            uint size = (uint)IntPtr.Size * 1024;
            if (Win32.EnumProcessModules(AMK.AMKProcessi, pModules.AddrOfPinnedObject(), size, out uint cbNeeded))
            {
                int cb = Marshal.SizeOf(typeof(Win32._MODULEINFO));
                Win32.GetModuleInformation(AMK.AMKProcessi, AMK.hMods[0], out Win32._MODULEINFO modinfo, cb);
                AMK.Mem = new ExternalProcessMemory(AMK.AMKProcessi);

                SIE:
                AMK.GNames = AMK.Mem.Read<IntPtr>(AMK.hMods[0] + Offsets.GNames);
                AMK.UWorld = AMK.Mem.Read<IntPtr>(AMK.hMods[0] + Offsets.UWorld);
                AMK.GameInstance = AMK.Mem.Read<IntPtr>(AMK.UWorld + Offsets.GameInst);
                IntPtr ULocalPlayer = AMK.Mem.Read<IntPtr>(AMK.GameInstance + Offsets.ULocalP);
                if (ULocalPlayer == null || ULocalPlayer == (IntPtr)0)goto SIE;
                AMK.LocalPlayer = AMK.Mem.Read<IntPtr>(ULocalPlayer);
                IntPtr viewportclient = AMK.Mem.Read<IntPtr>(AMK.LocalPlayer + Offsets.ViewPort);
                IntPtr PWorld = AMK.Mem.Read<IntPtr>(viewportclient + Offsets.pUWorld);
                AMK.ULevel = AMK.Mem.Read<IntPtr>(PWorld + Offsets.ULevel);//PersistentLevel

                AMK.LocalPlayerControl = AMK.Mem.Read<IntPtr>(AMK.LocalPlayer + Offsets.LPlayerControl);
                Console.WriteLine("GNames Starting");
                string[] isim = { "PlayerMale_A" , "PlayerMale_A_C", "PlayerFemale_A", "PlayerFemale_A_C" };
                int uc = 0;
                // int ilk = 0;
                for (int y = 0; y < 5; y++)
                {
                    IntPtr chunk = AMK.Mem.Read<IntPtr>(AMK.GNames + (y * 8));

                    for (int x = 0; x < 100000; x++)//100000
                    {
                        IntPtr fnameEntryPtr = AMK.Mem.Read<IntPtr>(chunk + (x * 8));
                        if (fnameEntryPtr == null || fnameEntryPtr == default(IntPtr) || fnameEntryPtr.ToInt64() < 1) continue;
                        int id = AMK.Mem.Read<int>(fnameEntryPtr);
                        if (AMK.tGnames.ContainsKey(id/2)) continue;
                        StringBuilder sb = new StringBuilder();
                        byte readChar = 0xFF;
                        int i = 0;
                        while (true)
                        {
                            var readBytes = AMK.Mem.ReadBytes(fnameEntryPtr + 0x10 + i++, 1);
                            if (readBytes == null)
                                break;

                            readChar = readBytes[0];

                            if (readChar == 00)
                                break;
                            sb.Append((char)readChar);
                        }

                        id = id / 2;
                        
                        string wisim = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(sb.ToString())).Replace("\r", "").Replace("\n", "");
                        
                        if (isim.Contains(wisim))
                        {
                            AMK.tGnames.Add(id, "");
                            Console.WriteLine("{0}, {1} x:{2} y: {3}", wisim, id,x,y);
                            uc++;                  
                        }
                        else if (ArabaKontrol(wisim, out string qisim))
                        {
                            AMK.tGnames.Add(id, qisim);
                            Console.WriteLine("{0}, {1} x:{2} y: {3}", qisim, id,x,y);
                        }

                          if (!AMK.tGnames.ContainsKey(id))
                              AMK.tGnames.Add(id, Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(sb.ToString())).Replace("\r", "").Replace("\n", ""));
                    }
                }


                
                Console.WriteLine("GNames Finish");
                Thread thread2 = new Thread(Genel);
                thread2.Start();

                Form1.OvAC();

                //thread1.Start();
            }

            // NO NASTY MEMORY LEAKS HERE
            pModules.Free();

            Console.ReadLine();
        }
        //static int CRW = -1;
       
        private static void Genel()
        {
            while (true)
            {
                if (AMK.Hile == true)
                {

                    AMK.UWorld = AMK.Mem.Read<IntPtr>(AMK.hMods[0] + Offsets.UWorld);
                    AMK.GameInstance = AMK.Mem.Read<IntPtr>(AMK.UWorld + Offsets.GameInst);
                    IntPtr ULocalPlayer = AMK.Mem.Read<IntPtr>(AMK.GameInstance + Offsets.ULocalP);
                    if (ULocalPlayer == null || ULocalPlayer == (IntPtr)0) continue;
                    AMK.LocalPlayer = AMK.Mem.Read<IntPtr>(ULocalPlayer);
                    IntPtr viewportclient = AMK.Mem.Read<IntPtr>(AMK.LocalPlayer + Offsets.ViewPort);
                    IntPtr PWorld = AMK.Mem.Read<IntPtr>(viewportclient + Offsets.pUWorld);
                    AMK.ULevel = AMK.Mem.Read<IntPtr>(PWorld + Offsets.ULevel);
                    AMK.LocalPlayerControl = AMK.Mem.Read<IntPtr>(AMK.LocalPlayer + Offsets.LPlayerControl);

                    IntPtr pLocalPlayer = AMK.Mem.Read<IntPtr>(AMK.LocalPlayerControl + Offsets.LocalPlayer);
                    IntPtr WeapP = AMK.Mem.Read<IntPtr>(pLocalPlayer + Offsets.WeaponProcess);
                    
                    int CurrentWp = AMK.Mem.Read<int>(WeapP + Offsets.CurrentWeap);
                    if (CurrentWp < 0 || CurrentWp > 2) goto UYU;//continue;
                    //Console.WriteLine($"CR: {CurrentWp}");
                    IntPtr EquippedWeapons = AMK.Mem.Read<IntPtr>(WeapP + Offsets.EquipWeaps);
                    if (EquippedWeapons != default(IntPtr))
                    {   
                        IntPtr Weapon = AMK.Mem.Read<IntPtr>(EquippedWeapons + CurrentWp * IntPtr.Size);
                        if (Weapon == default(IntPtr)) goto UYU;
                        AMK.Weapon = Weapon;
                        if (!AMK.weapons.ContainsKey(Weapon) && AMK.Reco)
                        {
                            Sekmeme w;
                            Form1.FRecoilInfo RecoilInfo = AMK.Mem.Read<Form1.FRecoilInfo>(Weapon + Offsets.RecoilInfo);
                            w.Recoil = RecoilInfo;
                            RecoilInfo.VerticalRecoilMin = 0;
                            RecoilInfo.VerticalRecoilMax = 0;
                            RecoilInfo.RecoilValue_Climb = 0;
                            RecoilInfo.RecoilModifier_Crouch = 0;
                            RecoilInfo.RecoilModifier_Prone = 0;
                            RecoilInfo.RecoilSpeed_Horizontal = 0;
                            RecoilInfo.RecoilSpeed_Vertical = 0;
                            RecoilInfo.RecoverySpeed_Vertical = 0;
                            RecoilInfo.VerticalRecoveryModifier = 0;
                            AMK.Mem.Write(Weapon + Offsets.RecoilInfo, RecoilInfo);

                            Form1.FTrajectoryWeaponData Trac = AMK.Mem.Read<Form1.FTrajectoryWeaponData>(Weapon + Offsets.Trajectory);
                            w.Trac = Trac;
                            Trac.RecoilPatternScale = 0;
                            Trac.RecoilRecoverySpeed = 0;
                            Trac.RecoilSpeed = 0;
                           //Console.WriteLine($"Speed:{Trac.InitialSpeed}");
                           // Trac.InitialSpeed = 9885;
                            AMK.Mem.Write(Weapon + Offsets.Trajectory, Trac);

                            Form1.FWeaponGunAnim Anim = AMK.Mem.Read<Form1.FWeaponGunAnim>(Weapon + Offsets.GunAnim);
                            w.GunAnim = Anim;
                            Anim.ShotCameraShake = IntPtr.Zero;
                            Anim.ShotCameraShakeADS = IntPtr.Zero;
                            Anim.ShotCameraShakeIronsight = IntPtr.Zero;
                            AMK.Mem.Write(Weapon + Offsets.GunAnim, Anim);

                            AMK.weapons.Add(Weapon, w);
                            /*
                            RecoilInfo = AMK.Mem.Read<Form1.FRecoilInfo>(Weapon + Offsets.RecoilInfo);
                            Console.WriteLine("DEBUG");
                            Console.WriteLine($"2: {RecoilInfo.VerticalRecoilMin} || {RecoilInfo.VerticalRecoilMax} || {RecoilInfo.RecoilValue_Climb} || {RecoilInfo.RecoilModifier_Crouch}  || {RecoilInfo.RecoilModifier_Prone}");
                            Console.WriteLine($"2: {RecoilInfo.RecoilSpeed_Horizontal} || {RecoilInfo.RecoilSpeed_Vertical} || {RecoilInfo.RecoverySpeed_Vertical} || {RecoilInfo.VerticalRecoveryModifier}\n");
                            */
                        }
                        else if(AMK.weapons.ContainsKey(Weapon))
                        {
                            Form1.FRecoilInfo RecoilInfo = AMK.Mem.Read<Form1.FRecoilInfo>(Weapon + Offsets.RecoilInfo);
                            
                            if (AMK.Reco == false && RecoilInfo.VerticalRecoilMax == 0 && RecoilInfo.RecoilValue_Climb == 0)
                            {
                                AMK.weapons.TryGetValue(Weapon, out Sekmeme w);
                                AMK.Mem.Write(Weapon + Offsets.RecoilInfo, w.Recoil);
                                AMK.Mem.Write(Weapon + Offsets.Trajectory, w.Trac);
                                AMK.Mem.Write(Weapon + Offsets.GunAnim, w.GunAnim);
                                //AMK.Mem.Write(Weapon + Offsets.WeaponConfig, w.WeapC);
                            }
                            else if (AMK.Reco == true && (RecoilInfo.VerticalRecoilMax > 0 || RecoilInfo.RecoilValue_Climb > 0))
                            {
                                RecoilInfo.VerticalRecoilMin = 0;
                                RecoilInfo.VerticalRecoilMax = 0;
                                RecoilInfo.RecoilValue_Climb = 0;
                                RecoilInfo.RecoilModifier_Crouch = 0;
                                RecoilInfo.RecoilModifier_Prone = 0;
                                RecoilInfo.RecoilSpeed_Horizontal = 0;
                                RecoilInfo.RecoilSpeed_Vertical = 0;
                                RecoilInfo.RecoverySpeed_Vertical = 0;
                                RecoilInfo.VerticalRecoveryModifier = 0;
                                AMK.Mem.Write(Weapon + Offsets.RecoilInfo, RecoilInfo);

                                Form1.FTrajectoryWeaponData Trac = AMK.Mem.Read<Form1.FTrajectoryWeaponData>(Weapon + Offsets.Trajectory);
                                Trac.RecoilPatternScale = 0;
                                Trac.RecoilRecoverySpeed = 0;
                                Trac.RecoilSpeed = 0;
                                AMK.Mem.Write(Weapon + Offsets.Trajectory, Trac);

                                Form1.FWeaponGunAnim Anim = AMK.Mem.Read<Form1.FWeaponGunAnim>(Weapon + Offsets.GunAnim);
                                Anim.ShotCameraShake = IntPtr.Zero;
                                Anim.ShotCameraShakeADS = IntPtr.Zero;
                                Anim.ShotCameraShakeIronsight = IntPtr.Zero;
                                AMK.Mem.Write(Weapon + Offsets.GunAnim, Anim);
                            }

                        }
                    }
                }
                UYU:
                Thread.Sleep(2000);
            }
        }

        private static bool ArabaKontrol(string ActorNameFromID, out string isim)
        {
            //string ActorNameFromID = AMK.tGnames[PlayerID];
            if (ActorNameFromID == "Uaz_B_01_C" || ActorNameFromID == "Uaz_A_01_C" || ActorNameFromID == "Uaz_C_01_C")
            {
                isim = "[UAZ]";
                return true;
            }
            else if (ActorNameFromID == "Dacia_A_01_C" || ActorNameFromID == "Dacia_A_02_C" || ActorNameFromID == "Dacia_A_03_C" || ActorNameFromID == "Dacia_A_04_C")
            {
                isim = "[TOROS]";
                return true;
            }
            else if (ActorNameFromID == "ABP_Motorbike_03_C" || ActorNameFromID == "ABP_Motorbike_04_C" || ActorNameFromID == "BP_Motorbike_03_C" || ActorNameFromID == "BP_Motorbike_04_C"
               || ActorNameFromID == "ABP_Motorbike_03_Sidecart_C")
            {
                isim = "[MOTOR]";
                return true;
            }
            else if (ActorNameFromID == "Buggy_A_03_C" || ActorNameFromID == "Buggy_A_02_C" || ActorNameFromID == "Buggy_A_01_C")
            {
                isim = "[BUGGY]";
                return true;
            }
            else if (ActorNameFromID == "Boat_PG117_C")
            {
                isim = "[BOT]";
                return true;
            }
            isim = null;
            return false;
        }
        private static class Win32
        {
            [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            public static extern bool EnumProcessModules(IntPtr hProcess, [Out] IntPtr lphModule, uint cb, out uint lpcbNeeded);

            [DllImport("psapi.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            public static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, out _MODULEINFO lpModInfo, int cb);

            [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
            private static extern int LstrcpyW(string lpString1, int StringPointer);

            [StructLayout(LayoutKind.Sequential)]
            public struct _MODULEINFO
            {
                public IntPtr lpBaseOfDll;
                public uint SizeOfImage;
                public IntPtr EntryPoint;
            }
        }
    }
}
