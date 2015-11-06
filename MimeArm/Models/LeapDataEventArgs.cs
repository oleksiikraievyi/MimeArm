namespace MimeArm.Models
{
    public class LeapDataEventArgs
    {
        public LeapData CurrentLeapData { get; private set; }

        public LeapDataEventArgs(LeapData currentLeapData)
        {
            CurrentLeapData = currentLeapData;
        }
    }
}