using System;
using System.IO;
using System.Windows.Forms;

namespace KttK.HspDecompiler
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            HspConsole.Initialize();
            if (!HspDecoder.Initialize())
            {
                HspConsole.Close();
                return;
            }

            string? filename = null;
            if ((args != null) && (args.Length > 0))
            {
                foreach (string file in args)
                {
                    if (File.Exists(file))
                    {
                        filename = file;
                        break;
                    }
                }
            }

            try
            {
                Application.Run(new DeHspDialog(filename));
            }
            catch (Exception e)
            {
                try
                {
                    HspConsole.ExceptionHandlingClose(e);
                }
                catch
                {
                }

                return;
            }

            HspConsole.Close();
        }

        private static readonly string ExeDirValue = Path.GetDirectoryName(Application.ExecutablePath) + @"\";
        private static readonly string ExeNameValue = Path.GetFileName(Application.ExecutablePath);
        private static readonly System.Diagnostics.FileVersionInfo ExeVerValue =
            System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath);

        internal static string ExeName
        {
            get { return ExeNameValue; }
        }

        internal static System.Diagnostics.FileVersionInfo ExeVer
        {
            get { return ExeVerValue; }
        }

        internal static string ExeDir
        {
            get { return ExeDirValue; }
        }
    }
}
