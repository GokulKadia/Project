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
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace CitrixWorkspaceLauncher
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("USER32.DLL")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public Main()
        {
            InitializeComponent();            
            call();
        }
        public static IDictionary<IntPtr, string> List_Windows_By_PID(int processID)
        {
            IntPtr hShellWindow = GetShellWindow();
            Dictionary<IntPtr, string> dictWindows = new Dictionary<IntPtr, string>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                //ignore the shell window
                if (hWnd == hShellWindow)
                {
                    return true;
                }

                //ignore non-visible windows
                if (!IsWindowVisible(hWnd))
                {
                    return true;
                }

                //ignore windows with no text
                int length = GetWindowTextLength(hWnd);
                if (length == 0)
                {
                    return true;
                }

                uint windowPid;
                GetWindowThreadProcessId(hWnd, out windowPid);

                //ignore windows from a different process
                if (windowPid != processID)
                {
                    return true;
                }

                StringBuilder stringBuilder = new StringBuilder(length);
                GetWindowText(hWnd, stringBuilder, length + 1);
                dictWindows.Add(hWnd, stringBuilder.ToString());

                return true;

            }, 0);

            return dictWindows;
        }

        //void call()
        //{
        //    //Kill citrix process before running this application
        //    Process.GetProcesses().Where(x => x.ProcessName == "SelfService" || x.ProcessName == "SelfServicePlugin")
        //        .ToList().ForEach(y=>y.Kill());

        //    System.Threading.Thread.Sleep(100);
        //    ProcessStartInfo start = new ProcessStartInfo();
        //    // Enter in the command line arguments, everything you would enter after the executable name itself
        //    start.Arguments = "-showAppPicker";
        //    // Enter the executable to run, including the complete path
        //    start.FileName = "C:\\Program Files (x86)\\Citrix\\ICA Client\\SelfServicePlugin\\SelfService.exe";
        //    // Do you want to show a console window?            
        //    //start.CreateNoWindow = true;
        //    start.UseShellExecute = true;
        //    start.WindowStyle = ProcessWindowStyle.Maximized;
        //    start.CreateNoWindow = true;
        //    Process.Start(start);

        //    System.Threading.Thread.Sleep(10000);
        //    Process[] processes = Process.GetProcessesByName("AuthManSvr");
        //    //Process[] processes = Process.GetProcessesByName("SelfService");
        //    if (processes.Length > 0)
        //    {                
        //        var placement = new WINDOWPLACEMENT();
        //        processes.Select(x =>
        //        {
        //            IDictionary<IntPtr, string> windows = List_Windows_By_PID(x.Id);
        //            windows.Select(y =>
        //            {
        //                GetWindowPlacement(y.Key, ref placement);
        //                if (placement.showCmd == SW_SHOWMINIMIZED)
        //                {
        //                    ShowWindowAsync(y.Key, SW_SHOWMAXIMIZED);
        //                }
        //                else
        //                {
        //                    ShowWindowAsync(y.Key, SW_SHOWMAXIMIZED);
        //                }
        //                return windows;
        //            }).ToList();
        //            return processes;
        //        }).ToList();
        //    }
        //}
        void call()
        {
            //Kill citrix process before running this application
            Process.GetProcesses().Where(x => x.ProcessName == "SelfService" || x.ProcessName == "SelfServicePlugin" || x.ProcessName == "AuthManSvr")
                .ToList().ForEach(y => y.Kill());

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
            Process.Start(start);

            //System.Threading.Thread.Sleep(10000);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                //var names = new[] { "SelfService", "AuthManSvr" };
                //var localByName = names.SelectMany(name => Process.GetProcessesByName(name)).ToArray();
                //Process[] processes = Process.GetProcessesByName("SelfService");
                //Process[] processes1 = Process.GetProcessesByName("AuthManSvr");
                Process[] processes = Process.GetProcesses();
                if (processes.Length > 0)
                {
                    var placement = new WINDOWPLACEMENT(); //x.ProcessName== "SelfService" ||
                    processes.Where(x=> x.ProcessName== "AuthManSvr").Select(x =>
                    {
                        IDictionary<IntPtr, string> windows = List_Windows_By_PID(x.Id);
                        windows.Select(y =>
                        {
                            GetWindowPlacement(y.Key, ref placement);
                            if (placement.showCmd == SW_SHOWMINIMIZED)
                            {
                                ShowWindowAsync(y.Key, SW_SHOWMAXIMIZED);
                            }
                            else
                            {
                                ShowWindowAsync(y.Key, SW_SHOWMAXIMIZED);
                            }
                            return windows;
                        }).ToList();
                        return processes;
                    }).ToList();
                    if (sw.Elapsed > TimeSpan.FromMinutes(2))
                    {
                        Application.Current.Shutdown();
                        //break;
                    }
                    //break;
                }
            }
            
        }

        
    }


}
