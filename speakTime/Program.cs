using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace speakTime
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        // http://stackoverflow.com/questions/19147/what-is-the-correct-way-to-create-a-single-instance-application

        static Mutex mutex = new Mutex(true, "{2f2b78a0-6d21-45e3-b7e2-1539426e17b2}");
        static bool mustReleaseMutex = false;
        
        [STAThread]
        static void Main()
        {
            try
            {
                int error;
                if (mutex.WaitOne(TimeSpan.Zero, true))
                {
                    mustReleaseMutex = true;
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Form1());
                    mutex.ReleaseMutex();
                }
                else
                {
                    IntPtr winHandle = NativeMethods.FindWindowByCaption(IntPtr.Zero, NativeMethods.Window_Caption);
                    error = Marshal.GetLastWin32Error();
                    if (error != 0)
                    {
                        MessageBox.Show(new Win32Exception(error).Message);
                    }
                    else
                    {
                        // notificar a nossa app que já está a correr
                        NativeMethods.PostMessage(winHandle, NativeMethods.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (mustReleaseMutex)
                {
                    try
                    {
                        mutex.ReleaseMutex();
                    }
                    catch (Exception)
                    {}
                }
            }
            
        }
    }
}
