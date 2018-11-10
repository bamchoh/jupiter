using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace Jupiter.Events
{
    public class ErrorNotificationEvent : PubSubEvent<ErrorNotification>
    {
    }

    public class ErrorNotification
    {
        public ErrorNotification(Exception ex)
        {
            if (ex.InnerException != null)
            {
                Message = ex.InnerException.Message;
            }
            else
            {
                Message = ex.Message;
            }
        }

        public string Message { get; set; }
    }
}
