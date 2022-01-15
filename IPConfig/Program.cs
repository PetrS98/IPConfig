using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;

namespace IPConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            string IpAddress = "";
            string Mask = "";
            string Gate = "";
            string Prim_DNS = "";
            string Sec_DNS = "";

            string[] configData_All = File.ReadAllLines("config.txt");

            List<string> configData = new List<string>();

            for (int i = 0; i < configData_All.Length; i++)
            {
                if (configData_All[i][0] != '#')
                {
                    configData.Add(configData_All[i]);
                }
            }

            NetworkInterface[] netInterface = NetworkInterface.GetAllNetworkInterfaces();

            Console.WriteLine("[1] - Set DHCP");
            Console.WriteLine("[2] - Set IP");
            Console.WriteLine("[3] - CLOSE");
            Console.WriteLine("");
            Console.WriteLine("Zadejte Volbu:");
            string input = Console.ReadLine();

            if (input == "1")
            {
                string input_1 = WriteAndSetInterface();

                try
                {
                    SendComand("/c netsh interface ipv4 set address name=\"" + netInterface[int.Parse(input_1) - 1].Name + "\" dhcp");
                    SendComand("/c netsh interface ipv4 set dns name=\"" + netInterface[int.Parse(input_1) - 1].Name + "\" dhcp");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Špatně zadán vstup! KONEC APLIKACE!!!");
                    Thread.Sleep(2500);
                    return;
                }


            }
            else if (input == "2")
            {
                Console.Clear();
                Console.WriteLine("[1] - Pre-Set - " + configData[0]);
                Console.WriteLine("[2] - Pre-Set - " + configData[6]);
                Console.WriteLine("[3] - Pre-Set - " + configData[12]);
                Console.WriteLine("[4] - Pre-Set - " + configData[18]);
                Console.WriteLine("[5] - Pre-Set - " + configData[24]);
                Console.WriteLine("[6] - CLOSE");
                Console.WriteLine("");
                Console.WriteLine("Zadejte Volbu:");
                string input_3 = Console.ReadLine();

                if (input_3 == "1") SetData(0);
                else if (input_3 == "2") SetData(6);
                else if (input_3 == "3") SetData(12);
                else if (input_3 == "4") SetData(18);
                else if (input_3 == "5") SetData(24);
                else if (input_3 == "6")
                {
                    Console.WriteLine("KONEC APLIKACE!!!");
                    Thread.Sleep(2000);
                    return;
                }
                else
                {
                    Console.WriteLine("Špatně zadán vstup! KONEC APLIKACE!!!");
                    Thread.Sleep(2500);
                    return;
                }

                string input_2 = WriteAndSetInterface();

                if (input_2 == "END")
                {
                    Console.WriteLine("KONEC APLIKACE!!!");
                    Thread.Sleep(2000);
                    return;
                }

                string Name = netInterface[int.Parse(input_2) - 1].Name;

                SendComand("/c netsh interface ipv4 set address name=\"" + Name + "\" static " + IpAddress + " " + Mask + " " + Gate);
                SendComand("/c netsh interface ipv4 set dns name=\"" + Name + "\" static  " + Prim_DNS + " primary");
                SendComand("/c netsh interface ip add dns \"" + Name + "\" " + Sec_DNS + " INDEX=2");

            }
            else if (input == "3")
            {
                Console.WriteLine("KONEC APLIKACE!!!");
                Thread.Sleep(2000);
                return;
            }
            else
            {
                Console.WriteLine("Špatně zadán vstup! KONEC APLIKACE!!!");
                Thread.Sleep(2500);
                return;
            }

            void SendComand(string cmd)
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
                    psi.UseShellExecute = true;
                    psi.WindowStyle = ProcessWindowStyle.Hidden;
                    psi.Verb = "runas";
                    psi.Arguments = cmd;
                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(50);
            }

            string WriteAndSetInterface()
            {
                int netIndex = 1;

                Console.Clear();

                foreach (var net in netInterface)
                {
                    Console.WriteLine("[" + netIndex.ToString() + "] - " + net.Name);
                    netIndex++;
                }

                Console.WriteLine("[" + netIndex.ToString() + "] - " + "CLOSE");
                Console.WriteLine("");
                Console.WriteLine("Zadejte Volbu");
                string inpt = Console.ReadLine();

                if (inpt == netIndex.ToString())
                {
                    return "END";
                }
                else return inpt;
            }

            void SetData(int offset)
            {
                try
                {
                    IpAddress = configData[1 + offset];
                    Mask = configData[2 + offset];
                    Gate = configData[3 + offset];
                    Prim_DNS = configData[4 + offset];
                    Sec_DNS = configData[5 + offset];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
