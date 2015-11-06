using System;
using Leap;

namespace MimeArm.Models
{
    public class FrameEventArgs : EventArgs
    {
        public Frame CurrentFrame { get; private set; }

        public FrameEventArgs(Frame frame)
        {
            CurrentFrame = frame;
        }
    }
}