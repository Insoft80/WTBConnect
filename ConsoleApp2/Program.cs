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
            SerialPort serialPort = new SerialPort();
            serialPort.PortName = "COM8";
            serialPort.Open();
            
            string Message = serialPort.ReadLine();
            Console.WriteLine(Message);
            serialPort.Close();

            var usbController = USBControllerDevice.GetInstances().Cast<USBControllerDevice>();
            var pnpGeräte = PnPEntity.GetInstances().Cast<PnPEntity>().
              Where(p => usbController.Any(uc => uc.Dependent.RelativePath.
                Split('\"')[1].Replace(@"\\", @"\") == p.DeviceID) && p.Status == "OK");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("    {0}",s);
            }
            Anzeige(pnpGeräte); Console.ReadLine();
        }

        private static void Anzeige(IEnumerable<PnPEntity> pnpGeräte)
        {
            foreach (var gerät in pnpGeräte)
                if (gerät.Caption.Contains("CH340"))
                {
                    Console.WriteLine($"******* Hab das Ding {gerät.Caption} gefunden ************");
                    //gerät.Enable(false);
                }
                else
                {
                    Console.WriteLine(gerät.Caption + "    DeviceID: " + gerät.DeviceID + "    Beschreibung: " + gerät.Description + "    Hersteller: " + gerät.Manufacturer);
                }
        }
    }
}
