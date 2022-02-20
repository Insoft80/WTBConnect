// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.CIMV2.Win32;
using System.IO.Ports;


namespace WTBConnect

{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WTB Connect V0.9");
            SerialPort serialPort = new SerialPort();
            string PortName = GetPortName();
            if (!string.IsNullOrEmpty(PortName))
            {
                Console.WriteLine($"Connecting WTB device on {PortName}");
                serialPort.PortName = PortName;
           
                serialPort.Open();
            
                string Message = serialPort.ReadLine();
                Console.WriteLine(Message);
                serialPort.Close();

           
            
            }
            Console.ReadLine();
            //Anzeige(pnpGeräte); Console.ReadLine();
        }

        private static string GetPortName()
            // Get name of USB port where Arduion is connected via CH340 chip
        {
            string portName = null;
            var usbController = USBControllerDevice.GetInstances().Cast<USBControllerDevice>();
            var pnpGeräte = PnPEntity.GetInstances().Cast<PnPEntity>().
              Where(p => usbController.Any(uc => uc.Dependent.RelativePath.
                Split('\"')[1].Replace(@"\\", @"\") == p.DeviceID) && p.Status == "OK");
            Boolean found = false;
            Console.WriteLine("Detecting connected PNP Entities ...");
            Console.CursorVisible = false;
            int cl = Console.CursorLeft;
            int ct = Console.CursorTop;
            foreach (var gerät in pnpGeräte)
            {
                Console.CursorLeft = cl;
                Console.CursorTop = ct;
                Console.WriteLine("    " + gerät.Caption + "    DeviceID: " + gerät.DeviceID);
                if (gerät.Caption.Contains("CH340"))
                {
                    Console.Write("        found - trying to detect PortName ... ");
                    int pos1 = gerät.Caption.LastIndexOf("COM");
                    if (pos1 > 0)
                    {
                        int pos2 = gerät.Caption.IndexOf(")", pos1);
                        if (pos2 > pos1)
                        {
                            portName = gerät.Caption.Substring(pos1, pos2 - pos1);
                            found = true;
                            Console.WriteLine(portName + "!");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"ERROR - No COM-Port found ({pos1}/{pos2})");
                        }
                    }
                  

                }
            }
            if (!found)
                Console.WriteLine("ERROR detecting WTB device - not connected or no port information found!");
            return(portName);
        }
        //private static void Anzeige(IEnumerable<PnPEntity> pnpGeräte)
        //{
        //    foreach (var gerät in pnpGeräte)
        //        if (gerät.Caption.Contains("CH340"))
        //        {
        //            Console.WriteLine($"******* Hab das Ding {gerät.Caption} gefunden ************");
        //            //gerät.Enable(false);
        //        }
        //        else
        //        {
        //            Console.WriteLine(gerät.Caption + "    DeviceID: " + gerät.DeviceID + "    Beschreibung: " + gerät.Description + "    Hersteller: " + gerät.Manufacturer);
        //        }
        //}
    }
}
