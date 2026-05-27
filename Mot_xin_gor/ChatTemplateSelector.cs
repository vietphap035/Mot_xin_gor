using ShareModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mot_xin_gor
{
    internal class ChatTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextSenderTemplate { get; set; }
        public DataTemplate TextReceiverTemplate { get; set; }
        public DataTemplate ImageSenderTemplate { get; set; }
        public DataTemplate ImageReceiverTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var message = item as ShareModel.Messages;
            if (message == null)
                return null;

            string currentUserId = Preferences.Default.Get("CurrentUserId", string.Empty);

            bool isSender = message.UId == currentUserId;
            bool isImage = message.MessageType == MessageType.Image; // 👈 rất quan trọng

            Debug.WriteLine($"Render: {message.MessageType} | Sender={isSender}");

            if (isImage)
                return isSender ? ImageSenderTemplate : ImageReceiverTemplate;

            return isSender ? TextSenderTemplate : TextReceiverTemplate;
        }
    }
}
