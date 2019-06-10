using MapboxTest.Common;
using MapboxTest.Forms.Service;
using Xamarin.Forms;

[assembly:Dependency(typeof(PopupService))]
namespace MapboxTest.Forms.Service
{
    public class PopupService : IPopupService
    {
        public void SendPopupAlertToMessagingCenter(MessageBlock block)
        {
            MessagingCenter.Send(App.MessageAlertSender, App.MessageAlertMessageName, block);
        }
    }
}
