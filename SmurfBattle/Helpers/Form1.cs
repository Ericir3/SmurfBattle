using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.Direct2D1;
using Factory = SharpDX.Direct2D1.Factory;
using FontFactory = SharpDX.DirectWrite.Factory;
using Format = SharpDX.DXGI.Format;
using SharpDX;
using SharpDX.Mathematics.Interop;
using SharpDX.DirectWrite;
using System.Threading;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Windows.Input;
using System.Diagnostics;

namespace SmurfBattle
{

    public partial class Form1 : Form
    {

        public static WindowRenderTarget device = null;
        private HwndRenderTargetProperties renderProperties;
        //private SolidColorBrush solidColorBrush;
        private Factory factory;
        // public static bool Hile = true;
        //text fonts
        private TextFormat font, fontSmall;
        private FontFactory fontFactory;
        private const string fontFamily = "Arial";//you can edit this of course
        private const float fontSize = 12.0f;
        private const float fontSizeSmall = 10.0f;

        private IntPtr handle;
        private Thread sDX = null;
        private bool ArabaTH = false;
        //DllImports
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("dwmapi.dll")]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref int[] pMargins);
        //Styles
        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;
        public const UInt32 TOPMOST_FLAGS = SWP_NOSIZE;
        public static IntPtr HWND_TOPMOST = new IntPtr(-1);

        public Form1()
        {
            this.handle = Handle;
            int initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            OnResize(null);

            this.SuspendLayout();
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "I am free";
            this.Text = "";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Black;

            this.Load += new System.EventHandler(this.Form1_Load);
            //this.Opacity = 0;
            this.ResumeLayout(false);

        }
        static TimeSpan last;
        private static readonly Stopwatch Stopwatch = Stopwatch.StartNew();
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == 1)
            {
                var timeStamp = Stopwatch.Elapsed;
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
                if (timeStamp - last > TimeSpan.FromMilliseconds(1000 / 5))
                {
                    if (AMK.Hile == true)AMK.Hile = false;
                    else AMK.Hile = true;
                    last = Stopwatch.Elapsed;
                }
            }

            if (m.Msg == 0x0312 && m.WParam.ToInt32() == 2)
            {
                var timeStamp = Stopwatch.Elapsed;
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
                if (timeStamp - last > TimeSpan.FromMilliseconds(1000 / 5))
                {
                    if (ArabaTH == true) ArabaTH = false;
                    else ArabaTH = true;
                    last = Stopwatch.Elapsed;
                }
            }
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == 3)
            {
                var timeStamp = Stopwatch.Elapsed;
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
                if (timeStamp - last > TimeSpan.FromMilliseconds(1000 / 5))
                {
                    if (AMK.Reco == true)AMK.Reco = false;
                    else AMK.Reco = true;
                    last = Stopwatch.Elapsed;
                }
            }
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == 4)
            {
                var timeStamp = Stopwatch.Elapsed;
                SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
                if (timeStamp - last > TimeSpan.FromMilliseconds(1000 / 5))
                {
                    if (AMK.Bone == true)AMK.Bone = false;
                    else AMK.Bone = true;
                    last = Stopwatch.Elapsed;
                }
            }
            base.WndProc(ref m);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

            this.DoubleBuffered = true;
            this.Width = 1920;// set your own size
            this.Height = 1080;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |// this reduce the flicker
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.Opaque |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
            this.TopMost = true;
            this.Visible = true;
            factory = new Factory();
            fontFactory = new FontFactory();
            renderProperties = new HwndRenderTargetProperties()
            {
                Hwnd = this.Handle,
                PixelSize = new Size2(1920, 1090),
                PresentOptions = PresentOptions.None
            };
            RegisterHotKey(this.Handle, 1, 2, (int)Keys.F2);//CTRL + F2
            RegisterHotKey(this.Handle, 2, 2, (int)Keys.F3);//CTRL + F3
            RegisterHotKey(this.Handle, 3, 2, (int)Keys.F4);//CTRL + F4
            RegisterHotKey(this.Handle, 4, 2, (int)Keys.F5);//CTRL + F5

            //Init DirectX
            device = new WindowRenderTarget(factory, new RenderTargetProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)), renderProperties)
            {
                AntialiasMode = AntialiasMode.PerPrimitive
            };
            // Init font's
            font = new TextFormat(fontFactory, fontFamily, fontSize);
            fontSmall = new TextFormat(fontFactory, fontFamily, fontSizeSmall);
            // line = new device.DrawLine;

            //sDX.Start();
            sDX = new Thread(new ParameterizedThreadStart(SDXThread))
            {
                //Priority = ThreadPriority.,
                IsBackground = true
            };
            sDX.Start();

        }

        public unsafe struct FTransform
        {
            public SharpDX.Vector4 Rotation;
            public SharpDX.Vector3 Translation;
            public fixed byte UnknownData00[0x4];
            public SharpDX.Vector3 Scale3D;
            public FTransform(SharpDX.Vector4 rot, SharpDX.Vector3 translation, SharpDX.Vector3 scale)
            {
                Rotation = rot;
                Translation = translation;
                Scale3D = scale;
            }

            public Matrix ToMatrixWithScale()
            {
                Matrix m = new Matrix
                {
                    M41 = Translation.X,
                    M42 = Translation.Y,
                    M43 = Translation.Z
                };

                float x2 = Rotation.X + Rotation.X;
                float y2 = Rotation.Y + Rotation.Y;
                float z2 = Rotation.Z + Rotation.Z;

                float xx2 = Rotation.X * x2;
                float yy2 = Rotation.Y * y2;
                float zz2 = Rotation.Z * z2;
                m.M11 = (1.0f - (yy2 + zz2)) * Scale3D.X;
                m.M22 = (1.0f - (xx2 + zz2)) * Scale3D.Y;
                m.M33 = (1.0f - (xx2 + yy2)) * Scale3D.Z;


                float yz2 = Rotation.Y * z2;
                float wx2 = Rotation.W * x2;
                m.M32 = (yz2 - wx2) * Scale3D.Z;
                m.M23 = (yz2 + wx2) * Scale3D.Y;


                float xy2 = Rotation.X * y2;
                float wz2 = Rotation.W * z2;
                m.M21 = (xy2 - wz2) * Scale3D.Y;
                m.M12 = (xy2 + wz2) * Scale3D.X;


                float xz2 = Rotation.X * z2;
                float wy2 = Rotation.W * y2;
                m.M31 = (xz2 + wy2) * Scale3D.Z;
                m.M13 = (xz2 - wy2) * Scale3D.X;

                m.M14 = 0.0f;
                m.M24 = 0.0f;
                m.M34 = 0.0f;
                m.M44 = 1.0f;

                return m;
            }
        }


        enum Bone : int

        {
            Root = 0,
            pelvis = 1,
            spine_01 = 2,
            spine_02 = 3,
            spine_03 = 4,
            neck_01 = 5,
            Head = 6,
            face_root = 7,
            eyebrows_pos_root = 8,
            eyebrows_root = 9,
            eyebrows_r = 10,
            eyebrows_l = 11,
            eyebrow_l = 12,
            eyebrow_r = 13,
            forehead_root = 14,
            forehead = 15,
            jaw_pos_root = 16,
            jaw_root = 17,
            jaw = 18,
            mouth_down_pos_root = 19,
            mouth_down_root = 20,
            lip_bm_01 = 21,
            lip_bm_02 = 22,
            lip_br = 23,
            lip_bl = 24,
            jaw_01 = 25,
            jaw_02 = 26,
            cheek_pos_root = 27,
            cheek_root = 28,
            cheek_r = 29,
            cheek_l = 30,
            nose_side_root = 31,
            nose_side_r_01 = 32,
            nose_side_r_02 = 33,
            nose_side_l_01 = 34,
            nose_side_l_02 = 35,
            eye_pos_r_root = 36,
            eye_r_root = 37,
            eye_rot_r_root = 38,
            eye_lid_u_r = 39,
            eye_r = 40,
            eye_lid_b_r = 41,
            eye_pos_l_root = 42,
            eye_l_root = 43,
            eye_rot_l_root = 44,
            eye_lid_u_l = 45,
            eye_l = 46,
            eye_lid_b_l = 47,
            nose_pos_root = 48,
            nose = 49,
            mouth_up_pos_root = 50,
            mouth_up_root = 51,
            lip_ul = 52,
            lip_um_01 = 53,
            lip_um_02 = 54,
            lip_ur = 55,
            lip_l = 56,
            lip_r = 57,
            hair_root = 58,
            hair_b_01 = 59,
            hair_b_02 = 60,
            hair_l_01 = 61,
            hair_l_02 = 62,
            hair_r_01 = 63,
            hair_r_02 = 64,
            hair_f_02 = 65,
            hair_f_01 = 66,
            hair_b_pt_01 = 67,
            hair_b_pt_02 = 68,
            hair_b_pt_03 = 69,
            hair_b_pt_04 = 70,
            hair_b_pt_05 = 71,
            camera_fpp = 72,
            GunReferencePoint = 73,
            GunRef = 74,
            breast_l = 75,
            breast_r = 76,
            clavicle_l = 77,
            upperarm_l = 78,
            lowerarm_l = 79,
            hand_l = 80,
            thumb_01_l = 81,
            thumb_02_l = 82,
            thumb_03_l = 83,
            thumb_04_l_MBONLY = 84,
            index_01_l = 85,
            index_02_l = 86,
            index_03_l = 87,
            index_04_l_MBONLY = 88,
            middle_01_l = 89,
            middle_02_l = 90,
            middle_03_l = 91,
            middle_04_l_MBONLY = 92,
            ring_01_l = 93,
            ring_02_l = 94,
            ring_03_l = 95,
            ring_04_l_MBONLY = 96,
            pinky_01_l = 97,
            pinky_02_l = 98,
            pinky_03_l = 99,
            pinky_04_l_MBONLY = 100,
            item_l = 101,
            lowerarm_twist_01_l = 102,
            upperarm_twist_01_l = 103,
            clavicle_r = 104,
            upperarm_r = 105,
            lowerarm_r = 106,
            hand_r = 107,
            thumb_01_r = 108,
            thumb_02_r = 109,
            thumb_03_r = 110,
            thumb_04_r_MBONLY = 111,
            index_01_r = 112,
            index_02_r = 113,
            index_03_r = 114,
            index_04_r_MBONLY = 115,
            middle_01_r = 116,
            middle_02_r = 117,
            middle_03_r = 118,
            middle_04_r_MBONLY = 119,
            ring_01_r = 120,
            ring_02_r = 121,
            ring_03_r = 122,
            ring_04_r_MBONLY = 123,
            pinky_01_r = 124,
            pinky_02_r = 125,
            pinky_03_r = 126,
            pinky_04_r_MBONLY = 127,
            item_r = 128,
            lowerarm_twist_01_r = 129,
            upperarm_twist_01_r = 130,
            BackPack = 131,
            backpack_01 = 132,
            backpack_02 = 133,
            Slot_Primary = 134,
            Slot_Secondary = 135,
            Slot_Melee = 136,
            slot_throwable = 137,
            coat_l_01 = 138,
            coat_l_02 = 139,
            coat_l_03 = 140,
            coat_l_04 = 141,
            coat_fl_01 = 142,
            coat_fl_02 = 143,
            coat_fl_03 = 144,
            coat_fl_04 = 145,
            coat_b_01 = 146,
            coat_b_02 = 147,
            coat_b_03 = 148,
            coat_b_04 = 149,
            coat_r_01 = 150,
            coat_r_02 = 151,
            coat_r_03 = 152,
            coat_r_04 = 153,
            coat_fr_01 = 154,
            coat_fr_02 = 155,
            coat_fr_03 = 156,
            coat_fr_04 = 157,
            thigh_l = 158,
            calf_l = 159,
            foot_l = 160,
            ball_l = 161,
            calf_twist_01_l = 162,
            thigh_twist_01_l = 163,
            thigh_r = 164,
            calf_r = 165,
            foot_r = 166,
            ball_r = 167,
            calf_twist_01_r = 168,
            thigh_twist_01_r = 169,
            Slot_SideArm = 170,
            skirt_l_01 = 171,
            skirt_l_02 = 172,
            skirt_l_03 = 173,
            skirt_f_01 = 174,
            skirt_f_02 = 175,
            skirt_f_03 = 176,
            skirt_b_01 = 177,
            skirt_b_02 = 178,
            skirt_b_03 = 179,
            skirt_r_01 = 180,
            skirt_r_02 = 181,
            skirt_r_03 = 182,
            ik_hand_root = 183,
            ik_hand_gun = 184,
            ik_hand_r = 185,
            ik_hand_l = 186,
            ik_aim_root = 187,
            ik_aim_l = 188,
            ik_aim_r = 189,
            ik_foot_root = 190,
            ik_foot_l = 191,
            ik_foot_r = 192,
            camera_tpp = 193,
            ik_target_root = 194,
            ik_target_l = 195,
            ik_target_r = 196,
            VB_spine_03_spine_03 = 197,
            VB_upperarm_r_lowerarm_r = 198
        }
        static Bone[] skeletonUpper = new Bone[] {
            Bone.forehead, Bone.Head, Bone.neck_01,
        };
        static Bone[] skeletonRightArm = new Bone[] {
            Bone.neck_01, Bone.upperarm_r, Bone.lowerarm_r//, Bone.hand_r
        };
        static Bone[] skeletonLeftArm = new Bone[] {
            Bone.neck_01, Bone.upperarm_l, Bone.lowerarm_l//, Bone.hand_l
        };
        static Bone[] skeletonSpine = new Bone[] {
            Bone.neck_01, Bone.spine_01, Bone.spine_02, Bone.pelvis
        };
        static Bone[] skeletonLowerRight = new Bone[] {
            Bone.pelvis, Bone.thigh_r, Bone.calf_r//, Bone.foot_r
        };
        static Bone[] skeletonLowerLeft = new Bone[] {
            Bone.pelvis, Bone.thigh_l, Bone.calf_l//, Bone.foot_l
        };
        static Bone[][] skeleton = new Bone[][] {
            skeletonUpper, skeletonLeftArm, skeletonRightArm, skeletonSpine, skeletonLowerRight, skeletonLowerLeft
        };
        public RawColor4 RawColorFromColor(System.Drawing.Color color) => new RawColor4(color.R, color.G, color.B, color.A);
        private SharpDX.Vector3 LastPunch { get; set; }

        public unsafe struct FRecoilInfo
        {
            public float VerticalRecoilMin;
            public float VerticalRecoilMax;
            public float VerticalRecoilVariation;
            public float VerticalRecoveryModifier;
            public float VerticalRecoveryClamp;
            public float VerticalRecoveryMax;
            public float LeftMax;
            public float RightMax;
            public float HorizontalTendency;
            public fixed byte UnknownData00[0x4];
            public IntPtr RecoilCurve;
            public int BulletsPerSwitch;
            public float TimePerSwitch;
            public byte bSwitchOnTime;
            public fixed byte UnknownData01[0x3];
            public float RecoilSpeed_Vertical;
            public float RecoilSpeed_Horizontal;
            public float RecoverySpeed_Vertical;
            public float RecoilValue_Climb;
            public float RecoilValue_Fall;
            public float RecoilModifier_Stand;
            public float RecoilModifier_Crouch;
            public float RecoilModifier_Prone;
            public float RecoilHorizontalMinScalar;
            public fixed byte UnknownData02[0x8];
        }
        public unsafe struct FTrajectoryWeaponData
        {
            public float WeaponSpread;                                             // 0x0000(0x0004) (CPF_Edit, CPF_BlueprintVisible, CPF_BlueprintReadOnly, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float AimingSpreadModifier;                                     // 0x0004(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float ScopingSpreadModifier;                                    // 0x0008(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float FiringSpreadBase;                                         // 0x000C(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float StandRecoveryTime;                                        // 0x0010(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float CrouchRecoveryTime;                                       // 0x0014(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float ProneRecoveryTime;                                        // 0x0018(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float RecoveryInterval;                                         // 0x001C(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float RecoilSpeed;                                              // 0x0020(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float RecoilRecoverySpeed;                                      // 0x0024(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float RecoilPatternScale;                                       // 0x0028(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float InitialSpeed;                                             // 0x002C(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            int HitDamage;                                                // 0x0030(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float RangeModifier;                                            // 0x0034(0x0004) (CPF_Edit, CPF_BlueprintVisible, CPF_BlueprintReadOnly, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float ReferenceDistance;                                        // 0x0038(0x0004) (CPF_Edit, CPF_BlueprintVisible, CPF_BlueprintReadOnly, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float TravelDistanceMax;                                        // 0x003C(0x0004) (CPF_Edit, CPF_BlueprintVisible, CPF_BlueprintReadOnly, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            char IsPenetrable;                                         // 0x0040(0x0001) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public fixed byte UnknownData00[0x7];
            IntPtr DamageType;                                               // 0x0048(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr RecoilPatterns;                                           // 0x0050(0x0010) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance)
        }
        public unsafe struct FWeaponGunAnim
        {
            IntPtr Fire;                                                     // 0x0000(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            byte bLoopedFire;                                          // 0x0008(0x0001) (CPF_Edit, CPF_DisableEditOnInstance)
            fixed byte UnknownData00[0x7];
            IntPtr Reload;                                                   // 0x0010(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr CharacterGripBlendspace;                                  // 0x0018(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr CharacterLHGripBlendspace;                                // 0x0020(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr CharacterFire;                                            // 0x0028(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr CharacterFireCycle;                                       // 0x0030(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr CharacterFireSelector;                                    // 0x0038(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr CharacterReloadTactical;                                  // 0x0040(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr CharacterReloadCharge;                                    // 0x0048(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr CharacterReloadByOneStart;                                // 0x0050(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr CharacterReloadByOneStop;                                 // 0x0058(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr CharacterReloadByOneSingle;                               // 0x0060(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr WeaponReloadTactical;                                     // 0x0068(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr WeaponReloadCharge;                                       // 0x0070(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float ReloadDurationTactical;                                   // 0x0078(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float ReloadDurationCharge;                                     // 0x007C(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float ReloadDurationStart;                                      // 0x0080(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float ReloadDurationLoop;                                       // 0x0084(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float ReloadDurationMagOut;                                     // 0x0088(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            byte bUseBoltAction;                                       // 0x008C(0x0001) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            fixed byte UnknownData01[0x3];
            float FireCycleDelay;                                           // 0x0090(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float FireCycleDuration;                                        // 0x0094(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            byte bCycleAfterLastShot;                                  // 0x0098(0x0001) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            byte bCycleDuringReload;                                   // 0x0099(0x0001) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            fixed byte UnknownData02[0x6];
            public IntPtr ShotCameraShake;                                          // 0x00A0(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public IntPtr ShotCameraShakeIronsight;                                 // 0x00A8(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public IntPtr ShotCameraShakeADS;                                       // 0x00B0(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr CycleCameraAnim;                                          // 0x00B8(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float RecoilKickADS;                                            // 0x00C0(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            SharpDX.Vector3 RecoilADSSocketOffsetScale;                               // 0x00C4(0x000C) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            SharpDX.Vector3 MagDropLinearVelocity;                                    // 0x00D0(0x000C) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            SharpDX.Vector3 MagDropAngularVelocity;                                   // 0x00DC(0x000C) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float InertiaInterpMultiplier;                                  // 0x00E8(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float InertiaClampMultiplier;                                   // 0x00EC(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
        }
        public unsafe struct FWeaponData
        {
            float TargetingFOV;                                             // 0x0000(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float HoldBreathFOV;                                            // 0x0004(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr Rarity;                                                   // 0x0008(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            SharpDX.Vector3 SocketOffset_Shoulder;                                    // 0x0010(0x000C) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            SharpDX.Vector3 SocketOffset_Hand;                                        // 0x001C(0x000C) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr bApplyGripPoseLeft;                                 // 0x0028(0x0001) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr WeaponGripLeft;                                           // 0x0029(0x0001) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr WeaponClass;                                              // 0x002A(0x0001) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr bUseDefaultScoreMultiplier;                           // 0x002B(0x0001) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float ScoreMultiplierByDamage;                                  // 0x002C(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float ScoreMultiplierByKill;                                    // 0x0030(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float SwayModifier_Pitch;                                       // 0x0034(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float SwayModifier_YawOffset;                                   // 0x0038(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float SwayModifier_Movement;                                     // 0x003C(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float SwayModifier_Stand;                                       // 0x0040(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float SwayModifier_Crouch;                                      // 0x0044(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            public float SwayModifier_Prone;                                       // 0x0048(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float CameraDOF_Range;                                          // 0x004C(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float CameraDOF_NearRange;                                      // 0x0050(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float CameraDOF_Power;                                          // 0x0054(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr bUseDynamicReverbAK;                                  // 0x0058(0x0001) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            fixed byte UnknownData00[0x3];                                       // 0x0059(0x0003) MISSED OFFSET
            float CurrentWeaponZero;                                        // 0x005C(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float MinWeaponZero;                                            // 0x0060(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float MaxWeaponZero;                                            // 0x0064(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float AnimationKick;                                            // 0x0068(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            fixed byte UnknownData01[0x4];                                       // 0x006C(0x0004) MISSED OFFSET
            IntPtr RecoilMontage;                                            // 0x0070(0x0008) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr DestructibleDoor;                                     // 0x0078(0x0001) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr ThrownType;                                               // 0x0079(0x0001) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            fixed byte UnknownData02[0x2];                                       // 0x007A(0x0002) MISSED OFFSET
            float WeaponEquipDuration;                                      // 0x007C(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            float WeaponReadyDuration;                                      // 0x0080(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            IntPtr bForceFireAfterEquip;                                 // 0x0084(0x0001) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
            fixed byte UnknownData03[0x3];                                       // 0x0085(0x0003) MISSED OFFSET
            float PhysicalBodyHitImpactPower;                               // 0x0088(0x0004) (CPF_Edit, CPF_ZeroConstructor, CPF_DisableEditOnInstance, CPF_IsPlainOldData)
        }
        public void DrawLines(RawVector2[] point0)
        {
            if (point0.Length < 2)
                return;
            //device.DrawLine(point0[0], point0[1], (new SolidColorBrush(device, RawColorFromColor(System.Drawing.Color.Purple))));
            for (int x = 0; x < point0.Length - 1; x++)
                device.DrawLine(point0[x], point0[x + 1], (new SolidColorBrush(device, RawColorFromColor(System.Drawing.Color.Red))));
        }
        private void SDXThread(object sender)
        {
            while (true)
            {
                Thread.Sleep(15);
                Console.Clear();
                device.BeginDraw();
                device.Clear(null);
                if (AMK.Hile == true)
                {
                    int EntityCount = AMK.Mem.Read<int>(AMK.ULevel + 0xA8);
                    Console.WriteLine($"EntityCount:{EntityCount}");
                    if (EntityCount < 1 || EntityCount > 10000) continue;
                    SharpDX.Vector3 LocalPlayerPosition = AMK.Mem.Read<SharpDX.Vector3>(AMK.LocalPlayer + Offsets.LLocation);
                    //if (LocalPlayerPosition == null) continue;
                    IntPtr aactorPtr = AMK.Mem.Read<IntPtr>(AMK.ULevel + Offsets.ActorPtr);
                    for (int i = 0; i < EntityCount; i++)
                    { 
                        IntPtr curActor = AMK.Mem.Read<IntPtr>(aactorPtr + (i * IntPtr.Size));
                        //if (curActor == null) continue;

                        int PlayerID = AMK.Mem.Read<int>(curActor + Offsets.ActorID);
                        if (!AMK.tGnames.ContainsKey(PlayerID)) continue;
                        
                        if (AMK.tGnames[PlayerID] != "" && !ArabaTH) continue;

                        float hp = AMK.Mem.Read<float>(curActor + Offsets.ActorHP);
                        if (AMK.tGnames[PlayerID] == "" && hp < 1 && AMK.Mem.Read<float>(curActor + Offsets.GroggyH) < 1) continue;

                        IntPtr rootCmpPtr = AMK.Mem.Read<IntPtr>(curActor + Offsets.RootComponent);

                        SharpDX.Vector3 actorLocation = AMK.Mem.Read<SharpDX.Vector3>(rootCmpPtr + Offsets.ActorLoc);

                        var lDeltaInMeters = (LocalPlayerPosition - actorLocation).Length() /100;
                        if (lDeltaInMeters > 800 || lDeltaInMeters < 3) continue;
                        AMK.CameraManager = (AMK.Mem.Read<IntPtr>(AMK.LocalPlayerControl + Offsets.CameraManager));
                        Matriks.WorldToScreen2(actorLocation, AMK.CameraManager, out SharpDX.Vector2 GelBakem);
                        if (AMK.tGnames[PlayerID] == "" && lDeltaInMeters < 100 && AMK.Bone == true)
                        {
                            IntPtr mesh = AMK.Mem.Read<IntPtr>(curActor + 0x400);
                            IntPtr bonearray = AMK.Mem.Read<IntPtr>(mesh + 0x790);
                            FTransform ComponentToWorld = AMK.Mem.Read<FTransform>(mesh + 0x190);
                            foreach (Bone[] part in skeleton)
                            {
                                SharpDX.Vector2 previousBone = new SharpDX.Vector2();
                                foreach (Bone b in part)
                                {
                                    FTransform bone = AMK.Mem.Read<FTransform>(bonearray + ((int)b * 0x30));
                                    Matrix Matrix = bone.ToMatrixWithScale() * ComponentToWorld.ToMatrixWithScale();
                                    SharpDX.Vector3 currentBone = new SharpDX.Vector3(Matrix.M41, Matrix.M42, Matrix.M43);
                                    Matriks.WorldToScreen2(currentBone, AMK.CameraManager, out SharpDX.Vector2 cr);
                                    if (previousBone.X == 0)
                                    {
                                        previousBone = cr;
                                        continue;
                                    }
                                    else
                                    {
                                        //if (b == Bone.forehead) Ekranabas(cr, "O", System.Drawing.Color.Red, font);
                                        device.DrawLine(previousBone, cr, (new SolidColorBrush(device, RawColorFromColor(System.Drawing.Color.DarkRed))));
                                        previousBone = cr;
                                    }
                                }
                            }
                        }
                        if (AMK.tGnames[PlayerID] == "" && hp < 1) Ekranabas(GelBakem, $"[{lDeltaInMeters.ToString("N2")}]", System.Drawing.Color.Purple, fontSmall);
                        else if (lDeltaInMeters > 150) Ekranabas(GelBakem, $"{AMK.tGnames[PlayerID]}[{lDeltaInMeters.ToString("N2")}]", System.Drawing.Color.Green, fontSmall);
                        else Ekranabas(GelBakem, $"{AMK.tGnames[PlayerID]}[{lDeltaInMeters.ToString("N2")}]", System.Drawing.Color.Red, font);
                    }
                }
                device.TryEndDraw(out long w, out w);
            }
        }
        private bool Ekranabas(SharpDX.Vector2 ekran,string yazi,System.Drawing.Color renk,TextFormat qfont)
        {
            TextLayout TL = new SharpDX.DirectWrite.TextLayout(fontFactory, yazi, qfont, float.MaxValue, float.MaxValue);
            device.DrawTextLayout(ekran,TL,(new SolidColorBrush(device, RawColorFromColor(renk))), DrawTextOptions.NoSnap);
            TL.Dispose();
            return true;
        }

        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public static void OvAC()
        {  
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static class Win32
        {
            [DllImport("user32.dll")]
            public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        }

    }

}
