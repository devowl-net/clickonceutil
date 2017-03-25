using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.Properties;

namespace ClickOnceUtil4UI.Utils
{
    /// <summary>
    /// Certificate utilities.
    /// </summary>
    public static class CertificateUtils
    {
        /// <summary>
        /// Generate certificate file and create <see cref="X509Certificate2"/>.
        /// </summary>
        /// <returns><see cref="X509Certificate2"/> certificate instance.</returns>
        public static X509Certificate2 GenerateSelfSignedCertificate()
        {
            var temporary = PrepareCertificateResourceFiles();
            const string PvkFileName = "TempCA.pvk";
            const string CerFileName = "TempCA.cer";
            const string PfxFileName = "TempCA.pfx";
            const string PublisherName = "CN=TempCA";
            
            // Generate PVK and CER files
            // ..\makecert -n "CN=TempCA" -r -sv TempCA.pvk TempCA.cer
            var makecertStart = new ProcessStartInfo(
                Path.Combine(temporary, Constants.MakecertFileName),
                $"-n {PublisherName} -r -sv {PvkFileName} {CerFileName}");
            StartProcess(makecertStart, temporary, MakecertDialogBot);

            // Generate PFX file
            // ..\pvk2pfx.exe -pvk ..\TempCA.pvk -spc ..\TempCA.cer -pfx ..\TempCA.pfx
            var pvk2PfxStart = new ProcessStartInfo(
                Path.Combine(temporary, Constants.Pvk2PfxFileName),
                $"-pvk {PvkFileName} -spc {CerFileName} -pfx {PfxFileName}");
            StartProcess(pvk2PfxStart, temporary, null);

            var pfxFullPath = Path.Combine(temporary, PfxFileName);
            var certificate = new X509Certificate2(X509Certificate.CreateFromSignedFile(pfxFullPath));

            CleanTemporaryFolder(temporary);

            return certificate;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int FindWindow(string className, string windowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int FindWindowEx(int parent, int childAfter, string className, string windowTitle);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(int hWnd, int msg, int wParam, int lParam);

        private static void MakecertDialogBot(Process process)
        {
            // TODO Prevent to use a MUI C:\Windows\System32\ru-RU\mssign32.dll.mui

            // Spy++ :
            // title: "Create Private Key Password"
            // None button: "None"
            // TODO Set random password next time
            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_LBUTTONUP = 0x0202;

            const string DialogTitle = "Create Private Key Password";
            const string DialogNoneButton = "None";

            Task.Factory.StartNew(
                () =>
                {
                    do
                    {
                        var dialogHandler = FindWindow(null, DialogTitle);
                        if (dialogHandler != 0)
                        {
                            int buttonHandler = FindWindowEx(dialogHandler, 0, null, DialogNoneButton);
                            if (buttonHandler != 0)
                            {
                                SendMessage(buttonHandler, WM_LBUTTONDOWN, 0, 0);
                                SendMessage(buttonHandler, WM_LBUTTONUP, 0, 0);
                            }
                        }

                        Thread.Sleep(50);
                    } while (!process.HasExited);
                });
        }

        private static void StartProcess(ProcessStartInfo processStartInfo, string workingDirectory, Action<Process> handler)
        {
            processStartInfo.WorkingDirectory = workingDirectory;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            var process = Process.Start(processStartInfo);
            if (process != null)
            {
                handler?.Invoke(process);
                process.WaitForExit();
            }
        }

        private static void CleanTemporaryFolder(string temporary)
        {
            Directory.Delete(temporary, true);
        }

        private static string PrepareCertificateResourceFiles()
        {
            var temporary = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(temporary);
            File.WriteAllBytes(Path.Combine(temporary, Constants.MakecertFileName), Resources.makecert);
            File.WriteAllBytes(Path.Combine(temporary, Constants.Pvk2PfxFileName), Resources.pvk2pfx);
            return temporary;
        }
    }
}