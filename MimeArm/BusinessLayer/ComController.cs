using MimeArm.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace MimeArm.BusinessLayer
{
    public class ComController : Controller<LeapData>, IDisposable
    {
        public SerialPort Port { get; private set; }

        public ComController()
        {
            Port = new SerialPort("COM3", 38400);
            Port.Open();
        }

        protected override LeapData TransferToView()
        {
            return CurrentLeapData;
        }

        public static byte[] PrepareByteArrayToSend(byte header, float x, float y, float z, short wristAngle, short wristRotation, short gripper, byte speed, byte external)
        {
            List<byte> returnValue = new List<byte>();
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

            return returnValue.ToArray();
        }

        public override void Dispose()
        {
            Port.Close();
            Port.Dispose();
            base.Dispose();
        }
    }
}