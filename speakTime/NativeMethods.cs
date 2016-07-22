using System;
using System.Runtime.InteropServices;

namespace speakTime
{

    // http://stackoverflow.com/questions/19147/what-is-the-correct-way-to-create-a-single-instance-application

    internal class NativeMethods
    {
        public const string Window_Caption = "Say time every...";
        public static readonly uint WM_SHOWME = RegisterWindowMessage("WM_SHOWME");
        
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint msg, uint action, IntPtr param);
        
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hwnd, uint msg, IntPtr wparam, IntPtr lparam);
        
        [DllImport("user32", SetLastError = true)]
        public static extern uint RegisterWindowMessage(string message);

    }
}
