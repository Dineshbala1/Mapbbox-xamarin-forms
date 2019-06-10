using System;
using MapboxTest.Common;
using Xamarin.Forms;

namespace MapboxTest.Forms.Behaviors
{
    public class PageBehavior : Behavior<Page>
    {
        private Page _page;

        protected override void OnAttachedTo(Page bindable)
        {
            base.OnAttachedTo(bindable);
            _page = bindable;
            bindable.Appearing += OnPageAppearingInvoked;
            bindable.Disappearing += OnPageDisappearingInvoked;
        }

        private void OnPageAppearingInvoked(object sender, EventArgs e)
        {
            MessagingCenter.Subscribe<string, MessageBlock>(App.MessageAlertSender, App.MessageAlertMessageName,
                DialogCreationCallback);
        }

        private void OnPageDisappearingInvoked(object sender, EventArgs e)
        {
            MessagingCenter.Unsubscribe<string, MessageBlock>(App.MessageAlertSender, App.MessageAlertMessageName);
        }

        protected override void OnDetachingFrom(Page bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Appearing -= OnPageAppearingInvoked;
            bindable.Disappearing -= OnPageDisappearingInvoked;
            _page = null;
        }

        #region Private Methods

        private async void DialogCreationCallback(string messageSenderName, MessageBlock messageBlock)
        {
            await _page.DisplayAlert(messageBlock.Title, messageBlock.Message, cancel: messageBlock.OkButton);
        }

        #endregion
    }
}
