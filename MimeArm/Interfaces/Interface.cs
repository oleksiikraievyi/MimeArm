using System;
using MimeArm.BusinessLayer;

namespace MimeArm.Interfaces
{
    public abstract class Interface<T> : IDisposable
    {
        public Controller<T> Controller { get; set; }

        protected Interface(Controller<T> controller)
        {
            Controller = controller;
            controller.OnRecievedDataFromReader += SendDataToInterface;
        }

        public virtual void SendDataToInterface(object sender, T t)
        {
            Console.WriteLine("Send data to interface");
        }

        public void Dispose()
        {
            Controller.Dispose();
        }
    }
}