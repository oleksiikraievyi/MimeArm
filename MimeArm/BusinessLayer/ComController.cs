using MimeArm.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MimeArm.BusinessLayer
{
    public class ComController : Controller<LeapData>, IDisposable
    {
        public SerialPort Port { get; private set; }

        private static Random random = new Random();
        public static int CurrentBackoffLevel { get; set; }
        private const int STANDARD_BAUD_RATE = 38400;

        public ComController()
        {
            CurrentBackoffLevel = -1;                   // when calculating backoff time, first increment, then calculate time 
            TryConnectToArm();
            RequestIDPacket();
            SetCartesianCoordinateSystem();
            SendMoveCommand(512, 200, 200, 150);
        }

        private bool TryConnectToArm()
        {
            Port = null;

            while (Port == null)
            {
                var portNamesList = SerialPort.GetPortNames();

                foreach (var portName in portNamesList)
                {
                    if (RequestIDPacket())
                        Port = new SerialPort(portName, STANDARD_BAUD_RATE);
                }

                if (Port == null)
                {
                    Console.WriteLine("Arm is not connected, please connect the arm.");
                    Thread.Sleep(GetExponentialBackoffTime());
                }
                else
                {
                    Port.Open();
                    return true;
                }
            }

            return false;
        }

        public static int GetExponentialBackoffTime()
        {
            if (random == null)
                random = new Random();

            CurrentBackoffLevel++;
            return random.Next(0, CurrentBackoffLevel) * 1000;          // return random amount of seconds in according to exponential backoff idea
        }

        protected override LeapData TransferToView()
        {
            return CurrentLeapData;
        }

        private void SetCartesianCoordinateSystem()
        {
            Console.Write("Setting cartesian coordinate system...");
            Console.WriteLine(string.Join(",", SendExternalCommand(0x20)));
        }

        private void GoToHomePosition()
        {
            Console.Write("Setting home position...");
            Console.WriteLine(string.Join(",", SendExternalCommand(0x50)));
        }

        private void GoToSleepMode()
        {
            Console.Write("Sending arm to sleep mode...");
            Console.WriteLine(string.Join(",", SendExternalCommand(0x60)));

            byte[] armStatusBytes = null;
            armStatusBytes = ReadArmStatus();

            Console.Write("Arm status: ");
            Console.WriteLine(string.Join(",", armStatusBytes));
            Console.WriteLine("Package OK: " + IsPackageOk(armStatusBytes));
            Console.WriteLine(Port.ReadExisting());
        }

        private bool RequestIDPacket()
        {
            Console.Write("Requesting ID Packet...");
            Console.WriteLine(string.Join(",", SendExternalCommand(0x70)));

            byte[] armStatusBytes = null;
            armStatusBytes = ReadArmStatus();

            if (IsPackageOk(armStatusBytes))
            {
                Console.Write("Arm status: ");
                Console.WriteLine(string.Join(",", armStatusBytes));
                Console.WriteLine("Package OK: " + IsPackageOk(armStatusBytes));
                Console.WriteLine(Port.ReadExisting());
            }

            return IsPackageOk(armStatusBytes) && armStatusBytes[1] == 2;
        }

        public static bool IsPackageOk(byte[] package)
        {
            if (package[0] != 0xFF)
                return false;

            int checksum = 0;

            for (var i = 1; i < package.Length - 1; i++)
            {
                checksum += package[i];
            }

            checksum = 0xFF - checksum % 256;

            return (checksum == package[package.Length - 1]);
        }

        public static byte[] PrepareByteArrayToSend(byte header, float x, float y, float z, short wristAngle, short wristRotation, short gripper, byte speed, byte external)
        {
            List<byte> returnValue = new List<byte>();

            if (header == 0xFF)
            {
                short xShort = Convert.ToInt16(Math.Round(x));
                short yShort = Convert.ToInt16(Math.Round(y));
                short zShort = Convert.ToInt16(Math.Round(z));
                byte xHigh = BitConverter.GetBytes(xShort)[1];
                byte xLow = BitConverter.GetBytes(xShort)[0];
                byte yHigh = BitConverter.GetBytes(zShort)[1];              // arm bug, consider that y and z are swapped!
                byte yLow = BitConverter.GetBytes(zShort)[0];
                byte zHigh = BitConverter.GetBytes(yShort)[1];
                byte zLow = BitConverter.GetBytes(yShort)[0];
                byte wristAngleHigh = BitConverter.GetBytes(wristAngle)[1];
                byte wristAngleLow = BitConverter.GetBytes(wristAngle)[0];
                byte wristRotationHigh = BitConverter.GetBytes(wristRotation)[1];
                byte wristRotationLow = BitConverter.GetBytes(wristRotation)[0];
                byte gripperAngleHigh = BitConverter.GetBytes(gripper)[1];
                byte gripperAngleLow = BitConverter.GetBytes(gripper)[0];
                byte DTIME = speed;                                         // set arm movement time 
                byte BUTTONS = 0x00;                                        //should be 0 as we don't have any buttons
                byte EXT = external;                                        // additional commands
                byte checksum = Convert.ToByte(0xFF - (xHigh + xLow + yHigh + yLow + zHigh + zLow + wristAngleHigh +
                    wristAngleLow + wristRotationHigh + wristRotationLow + gripperAngleHigh + gripperAngleLow +
                    DTIME + BUTTONS + EXT) % 256);

                //create COM package
                returnValue.Add(header);
                returnValue.Add(xHigh);
                returnValue.Add(xLow);
                returnValue.Add(yHigh);
                returnValue.Add(yLow);
                returnValue.Add(zHigh);
                returnValue.Add(zLow);
                returnValue.Add(wristAngleHigh);
                returnValue.Add(wristAngleLow);
                returnValue.Add(wristRotationHigh);
                returnValue.Add(wristRotationLow);
                returnValue.Add(gripperAngleHigh);
                returnValue.Add(gripperAngleLow);
                returnValue.Add(DTIME);
                returnValue.Add(BUTTONS);
                returnValue.Add(EXT);
                returnValue.Add(checksum);
            } 
            else
            {
                byte checksum = Convert.ToByte(0xFF - external % 256);
                returnValue.AddRange(new byte[15]);
                returnValue.Add(external);
                returnValue.Add(checksum);
            }

            return returnValue.ToArray();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] SendMoveCommand(float x, float y, float z, byte speed)
        {
            var commandBytes = PrepareByteArrayToSend(255, x, y, z, 0x005A, 0x0200, 0x0100, speed, 0);
            Port.Write(commandBytes, 0, commandBytes.Length);

            return commandBytes;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] SendExternalCommand(byte ext)
        {
            var commandBytes = PrepareByteArrayToSend(0, 0, 0, 0, 0, 0, 0, 0, ext);
            Port.Write(commandBytes, 0, commandBytes.Length);

            return commandBytes;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] ReadArmStatus()
        {
            Port.RtsEnable = true;
            var returnValue = new byte[5];
            Thread.Sleep(1000);
            Port.Read(returnValue, 0, returnValue.Length);
            Port.RtsEnable = false;

            return returnValue;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] SendEmeregencyStopCommand()
        {
            var commandBytes = PrepareByteArrayToSend(0, 0, 0, 0, 0, 0, 0, 0, 0x11);
            Port.Write(commandBytes, 0, commandBytes.Length);

            return commandBytes;
        }

        public override void Dispose()
        {
            GoToSleepMode();
            Port.Close();
            Port.Dispose();
            base.Dispose();
        }

        public void testTrue_prepareByteArrayToSend()
        {
            var byteArray = PrepareByteArrayToSend(0, 100, 100, 100, 50, 100, 100, 30, 11);
            //assert equals byte[] {255, 0, 100, 0, 100, 0, 100, 0, 50, 0, 100, 0, 100, 30, 0, 11, 176}
        }
        
        public void testTrue_prepareByteArrayToSendWithEmptyHeader()
        {
            var byteArray = PrepareByteArrayToSend(0, 100, 100, 100, 50, 100, 100, 30, 11);
            //assert equals byte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 176}
        }

        public void testTrue_isPackageOk()
        {
            byte[] testValue = { 255, 0, 100, 0, 100, 0, 100, 0, 50, 0, 100, 0, 100, 30, 0, 11, 176 };
            bool isOk = IsPackageOk(testValue);
            //assert equals true
        }

        public void testFalse_isPackageOk()
        {
            byte[] testValue = { 255, 0, 100, 0, 100, 0, 100, 1, 50, 0, 100, 0, 100, 30, 0, 11, 176 };
            bool isOk = IsPackageOk(testValue);
            //assert equals false
        }

        public void test_getExponentialBackoffTime()
        {
            CurrentBackoffLevel = 0;
            int timeout = GetExponentialBackoffTime();
            //assert equals 0
        }
    }
}