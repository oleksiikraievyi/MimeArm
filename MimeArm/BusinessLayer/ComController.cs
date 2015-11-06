using MimeArm.Models;

namespace MimeArm.BusinessLayer
{
    public class ComController : Controller<LeapData>
    {
        protected override LeapData TransferToView()
        {
            return CurrentLeapData;
        }
    }
}