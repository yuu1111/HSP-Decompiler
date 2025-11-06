using System;
using System.IO;
using System.Text;
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
            // Shift-JISサポートのためにCodePagesエンコーディングプロバイダーを登録
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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

        internal static string ExeName { get; } = Path.GetFileName(Application.ExecutablePath);

        internal static System.Diagnostics.FileVersionInfo ExeVer { get; } = System.Diagnostics.FileVersionInfo.GetVersionInfo(Application.ExecutablePath);

        internal static string ExeDir { get; } = Path.GetDirectoryName(Application.ExecutablePath) + @"\";
    }
}
