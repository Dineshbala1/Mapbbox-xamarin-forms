using MapboxTest.Common;

namespace MapboxTest.Forms.Service
{
    public interface IPopupService
    {
        void SendPopupAlertToMessagingCenter(MessageBlock block);
    }
}