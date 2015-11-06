using Leap;

namespace MimeArm.Models
{
    public class LeapData
    {
        public Frame CurrentFrame { get; }
        public double GrabStrength { get; private set; }
        public Vector PalmPosition { get; private set; }

        public LeapData(Frame frame)
        {
            CurrentFrame = frame;
            GrabStrength = CurrentFrame.Hands.Frontmost.GrabStrength;
            PalmPosition = CurrentFrame.InteractionBox.NormalizePoint(CurrentFrame.Hands.Frontmost.PalmPosition);
        }
    }
}