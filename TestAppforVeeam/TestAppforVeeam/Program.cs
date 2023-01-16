using System;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Threading;


namespace ProcessKiller
{
    
    class Program
    {
        private static bool stopFlag = false;
        static void processKill(Process process)
        {
            process.Kill();
            process.WaitForExit();
            Console.WriteLine("Process has been terminated");
        }

        static bool checkProcessLifeTime(Process process, int maxLifetime)
        {
            TimeSpan processWorkTime = DateTime.Now.Subtract(process.StartTime);
            if (timeSpanToMinutes(processWorkTime) >= maxLifetime)
            {
                return true;
            }
            return false;
        }
        static int timeSpanToMinutes(TimeSpan timeSpan)
        {
            return timeSpan.Days * 1440 + timeSpan.Hours * 60 + timeSpan.Minutes + timeSpan.Seconds / 60;
        }

        static void processSearch(string processName, int maxLifetime)
        {
            Process[] procList = Process.GetProcesses();
            foreach (Process process in procList)
            {
                if (process.ProcessName == processName)
                {
                    try
                    {
                        Console.WriteLine("{0} ID: {1} Start time:{2}", process.ProcessName, process.Id, process.StartTime);
                        if (checkProcessLifeTime(process, maxLifetime))
                        {
                            processKill(process);
                        }
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine($"{e.Message}: {process.ProcessName}");
                    }
                }
            }
           
        }
        static void threadWork(string processName, int processLifetime, int checkFrequency)
        {
            while (!stopFlag)
            {
                processSearch(processName, processLifetime);
                Thread.Sleep(checkFrequency*60*200);
            }

        }
        static void exitProgramm()
        {
            while (Console.ReadKey(false).Key != ConsoleKey.Q)
            {
                Thread.Sleep(0);
            }
            stopFlag = true;
            Console.WriteLine("Exit button pressed");
        }

        static void Main(string[] args)
        {
            if (args == null || args.Length<3)
            {
                Console.WriteLine("Wrong input args");
            }
            else
            {
                Console.WriteLine("Press Q to exit programm");
                Thread threadProcessKill = new Thread(() => threadWork(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2])));
                Thread threadPressToExit = new Thread(() => exitProgramm());
                threadPressToExit.Start();
                //Console.WriteLine($"Thread1 is a Background thread:  {threadProcessKill.IsBackground}");
                threadProcessKill.Start();
                threadProcessKill.Join();
                threadPressToExit.Join();
            }
        }
    }
}

