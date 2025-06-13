using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PowerSchemeChanger
{
    internal static class Program
    {
        [DllImport("PowrProf.dll")]
        public static extern UInt32 PowerEnumerate(IntPtr RootPowerKey, IntPtr SchemeGuid, IntPtr SubGroupOfPowerSettingGuid, UInt32 AccessFlags, UInt32 Index, ref Guid Buffer, ref UInt32 BufferSize);

        [DllImport("PowrProf.dll")]
        public static extern UInt32 PowerGetActiveScheme(IntPtr UserRootPowerKey, ref IntPtr ActivePolicyGuid);

        [DllImport("PowrProf.dll")]
        public static extern UInt32 PowerSetActiveScheme(IntPtr RootPowerKey, [MarshalAs(UnmanagedType.LPStruct)] Guid SchemeGuid);

        [DllImport("PowrProf.dll")]
        public static extern UInt32 PowerReadFriendlyName(IntPtr RootPowerKey, ref Guid SchemeGuid, IntPtr SubGroupOfPowerSettingGuid, IntPtr PowerSettingGuid, IntPtr Buffer, ref UInt32 BufferSize);

        public enum AccessFlags: uint
        {
            ACCESS_SCHEME = 16,
            ACCESS_SUBGROUP = 17,
            ACCESS_INDIVIDUAL_SETTING = 18
        }

        private static string ReadPowerSchemeFriendlyName(Guid schemeGuid)
        {
            uint sizeName = 1024;
            IntPtr pSizeName = Marshal.AllocHGlobal((int)sizeName);
            string friendlyName;

            try
            {
                PowerReadFriendlyName(IntPtr.Zero, ref schemeGuid, IntPtr.Zero, IntPtr.Zero, pSizeName, ref sizeName);
                friendlyName = Marshal.PtrToStringUni(pSizeName);
            }
            finally
            {
                Marshal.FreeHGlobal(pSizeName);
            }

            return friendlyName;
        }

        private static IEnumerable<Guid> GetPowerSchemeGuids()
        {
            Guid schemeGuid = Guid.Empty;
            uint sizeSchemeGuid = (uint)Marshal.SizeOf(typeof(Guid));
            uint schemeIndex = 0;

            while (PowerEnumerate(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, (uint)AccessFlags.ACCESS_SCHEME, schemeIndex, ref schemeGuid, ref sizeSchemeGuid) == 0)
            {
                yield return schemeGuid;
                schemeIndex++;
            }
        }

        private static IEnumerable<string> GetPowerSchemeNames()
        {
            IEnumerable<Guid> schemeGuids = GetPowerSchemeGuids();

            foreach (Guid guid in schemeGuids)
            {
                yield return ReadPowerSchemeFriendlyName(guid);
            }
        }

        private static Guid GetCurrentPowerSchemeGuid()
        {
            IntPtr pActiveGuid = IntPtr.Zero;
            PowerGetActiveScheme(IntPtr.Zero, ref pActiveGuid);

            return (Guid)Marshal.PtrToStructure(pActiveGuid, typeof(Guid));
        }

        [STAThread]
        static void Main()
        {
            IEnumerable<Guid> schemeGuids = GetPowerSchemeGuids();
            IEnumerable<string> schemeNames = GetPowerSchemeNames();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 mainForm = new Form1();
            mainForm.fillPowerSchemes(schemeGuids, schemeNames);
            mainForm.updateActivePowerScheme(GetCurrentPowerSchemeGuid());

            Application.Run(mainForm);
        }
    }
}
