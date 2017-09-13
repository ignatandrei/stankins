using System;
using System.Collections.Generic;
using System.Text;

namespace ReceiverAzureIoTHub
{
    public struct LastReceivedMessage
    {
        public DateTime EnqueuedTimeUtc;
        public long EnqueuedOffset;
    }
}
