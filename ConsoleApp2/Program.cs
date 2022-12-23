// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Linq;
using ROOT.CIMV2.Win32;
using System.IO.Ports;


namespace WTBConnect

{
    enum Ports
    {

    }
 
    class Program
    {
     
       class WTBI
        {
            private SerialPort serialPort = new SerialPort();
            private string PortName;

            public bool Initialize()
            {
                bool success = false;
                PortName = GetPortName();
                if (!string.IsNullOrEmpty(PortName))
                {
                    Console.WriteLine($"Connecting WTB device on {PortName}");
                    serialPort.PortName = PortName;
                    serialPort.ReadTimeout = 500;
                    serialPort.WriteTimeout = 500;
                    DateTime Zeit = DateTime.Now;
                    Byte[] sendBytes = { 01, Convert.ToByte(Zeit.Year / 100), Convert.ToByte(Zeit.Year % 100), Convert.ToByte(Zeit.Day), Convert.ToByte(Zeit.Month), Convert.ToByte(Zeit.Hour), Convert.ToByte(Zeit.Minute), Convert.ToByte(Zeit.Second) };

                    try
                    {
                        serialPort.Open();

                        serialPort.Write(sendBytes, 0, 8);
                        success = true;
                    }
                    catch (TimeoutException) { success = false; }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("Error opening COM-Port!");
                        success = false;
                    }

                }                    
                return (success);
            }
            public void Close()
            {
                serialPort.Close();
            }

            public void GetDump()
            {
                byte[] sendBytes = { 02, 00, 00, 00, 00, 00, 00, 00 };
                serialPort.Write(sendBytes,0, 8);
                string receiveString = serialPort.ReadLine();
                while (!receiveString.Contains("/END"))
                {
                    Console.WriteLine(receiveString);
                    receiveString = serialPort.ReadLine();
                }
                
            }
        }
 
       static void Main(string[] args)
        {
            // SerialPort serialPort = new SerialPort();
            Console.WriteLine("WTB Connect V1.1");
            WTBI myWTBI = new WTBI();
 
            if (myWTBI.Initialize())
            {
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("***ERROR***");
            }
            bool userExit = false;
            string userInput;
            string[] userArgs;
            while (!userExit)
            {
                Console.Write(">> ");
                userInput = Console.ReadLine();
                userArgs = userInput.Split(' ');
                if (userArgs.Length == 0)
                {
                    userExit = true;
                }
                else
                {
                    switch (userArgs[0].ToUpper())
                    {
                        case "EXIT":
                            userExit = true;
                            break;
                        case "SET":
                            if (userArgs.Length > 1)
                            {
                                switch(userArgs[1].ToUpper())
                                { 
                                    
                                    case "COLOR":
                                        Console.WriteLine("<1> Work base color");
                                        Console.WriteLine("<2> Work overtime color");
                                        Console.WriteLine("<3> Work exceed color");
                                        Console.WriteLine("<4> Break base color");
                                        Console.WriteLine("<5> Break overtime color");
                                        Console.WriteLine("<6> Daytime color");
                                        Console.WriteLine("<0> Exit");
                                        userInput = Console.ReadLine();
                                        break;
                                    case "/?":
                                    case "/h":
                                    default:
                                        Console.WriteLine("SET <COLOR>|<LIMIT>|</?>|</h> [WORK]|[BREAK]|[TIME]");
                                        break;
                                }  

                            }
                            else
                            {
                                Console.WriteLine("Missing arguments for command SET. Type 'SET /?' or 'SET /h' for help");
                            }
                            break;
                        case "GET":
                            if (userArgs.Length > 1)
                            {
                                switch (userArgs[1].ToUpper())
                                {
                                    case "DUMP":
                                        myWTBI.GetDump();
                                        break;
                                    default:
                                        Console.WriteLine("Invalid argument for command GET. Type 'GET /?' or 'GET /h' for help");
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Missing arguments for command GET. Type 'GET /?' or 'GET /h' for help");
                            }
                            break;
                        default:
                            Console.WriteLine("Unknown command! Type 'Help' for instructions or 'EXIT'.");
                            break;


                    }

                }


            }
            // string PortName = GetPortName();
            //serialPort = new SerialPort();
            //PortName = GetPortName();

            //if (!string.IsNullOrEmpty(PortName))
            //{
            //    Console.WriteLine($"Connecting WTB device on {PortName}");
            //    serialPort.PortName = PortName;
            //    serialPort.ReadTimeout = 500;
            //    serialPort.WriteTimeout = 500;  
            //    DateTime Zeit = DateTime.Now;
            //    int i = 27;
            //    Byte[] sendBytes = {01, Convert.ToByte(Zeit.Year / 100), Convert.ToByte(Zeit.Year % 100), Convert.ToByte(Zeit.Day), Convert.ToByte(Zeit.Month), Convert.ToByte(Zeit.Hour), Convert.ToByte(Zeit.Minute), Convert.ToByte(Zeit.Second) };
            //    try
            //    {
            //        serialPort.Open();
                    
            //        serialPort.Write(sendBytes, 0, 8);
            //        //serialPort.DiscardInBuffer();
            //        string Message = serialPort.ReadLine();
            //        while (true)
            //        {
            //            Console.WriteLine(Message);
            //            Message = serialPort.ReadLine();
            //        }
            //        //Console.WriteLine(Message);
            //        //Message = serialPort.ReadLine();
            //        //Console.WriteLine(Message);

            //        serialPort.Close();
            //    }
            //    catch (TimeoutException) { }
            //    catch (UnauthorizedAccessException)
            //    {
            //        Console.WriteLine("Error opening COM-Port!");
            //    }
           
            
            //}
            
            // Console.ReadLine();
            myWTBI.Close();
            //Anzeige(pnpGeräte); Console.ReadLine();
        }

        private static string GetPortName()
            // Get name of USB port where Arduino is connected via CH340 chip
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
