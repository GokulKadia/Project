/*
*******************************************************************************************
*
* DELL PROPRIETARY INFORMATION
*
* This software is confidential. Dell Inc., or one of its subsidiaries, has supplied this
* software to you under the terms of a license agreement, nondisclosure agreement or both.
* You may not copy, disclose, or use this software except in accordance with those terms.
*
* Copyright 2017 - 2023 Dell Inc. or its subsidiaries. All Rights Reserved.
*
* DELL INC. MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY OF THE SOFTWARE,
* EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF
* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT.
* DELL SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A RESULT OF USING,
* MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS DERIVATIVES.
*
*******************************************************************************************
*
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static PInvoke.User32;

namespace CitrixWorkspaceLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        [System.Runtime.InteropServices.DllImport("Shell32.dll")]
        private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        int uFlags);

        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;

        [DllImport("user32.dll")]
        private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public MainWindow()
        {
            InitializeComponent();
            Call();
        }
        void Call()
        {
            foreach (var process in Process.GetProcessesByName("SelfService"))
            {
                try
                {
                    process.Kill();
                    break;
                }
                catch (Exception ex) { }
            }
            foreach (var process in Process.GetProcessesByName("SelfServicePlugin"))
            {
                try
                {
                    process.Kill();
                    break;
                }
                catch (Exception ex) { }
            }
            System.Threading.Thread.Sleep(100);
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            start.Arguments = "-showAppPicker";
            // Enter the executable to run, including the complete path
            start.FileName = "C:\\Program Files (x86)\\Citrix\\ICA Client\\SelfServicePlugin\\SelfService.exe";
            // Do you want to show a console window?            
            //start.CreateNoWindow = true;
            start.UseShellExecute = true;
            start.WindowStyle = ProcessWindowStyle.Maximized;
            start.CreateNoWindow = true;           

            int exitCode;
            // Run the external process & wait for it to finish
            using (Process proc = Process.Start(start))
            {
                System.Threading.Thread.Sleep(2000);
                MaximizeProcessWindow("SelfService");
                System.Threading.Thread.Sleep(500);
            }
            IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;
            SetWindowPos(hWnd,
            new IntPtr(0),
            0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE);
            //System.Threading.Thread.Sleep(5000);
            this.Close();
        }


        //foreach (var process in processes)
        //{
        //    IDictionary<IntPtr, string> windows = List_Windows_By_PID(process.Id);
        //    foreach (KeyValuePair<IntPtr, string> pair in windows)
        //    {
        //        //var placement = new WINDOWPLACEMENT();
        //        GetWindowPlacement(pair.Key, ref placement);

        //        if (placement.showCmd == SW_SHOWMINIMIZED)
        //        {
        //            //if minimized, show maximized
        //            ShowWindowAsync(pair.Key, SW_SHOWMAXIMIZED);
        //        }
        //        else
        //        {
        //            //default to minimize
        //            ShowWindowAsync(pair.Key, SW_SHOWMAXIMIZED);
        //        }
        //    }
        //}
        public static void MaximizeProcessWindow(string processName)
        {
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.ProcessName.Equals(processName))
                {
                    try
                    {
                        WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
                        GetWindowPlacement(proc.MainWindowHandle, ref wp);
                        System.Threading.Thread.Sleep(200);
                        wp.showCmd = WindowShowStyle.SW_SHOWMAXIMIZED;
                        System.Threading.Thread.Sleep(200);                       
                        SetWindowPlacement(proc.MainWindowHandle, ref wp);
                        break;
                    }
                    catch (Exception ex)
                    {
                        // log exception here and do something
                    }
                }
            }
        }



    }
}
