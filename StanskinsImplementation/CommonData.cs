using StankinsInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace StanskinsImplementation
{
    public class CommonData : ICommonData
    {
        public CommonData()
        {
            this.UTCDateReceived = DateTime.UtcNow;
            this.LocalDateReceived = DateTime.Now;
            this.DeviceName = Environment.MachineName;
            //this.UserName = Environment.UserDomainName;
            
        }
        public DateTime UTCDateReceived { get; protected set; }
        public DateTime LocalDateReceived { get; protected set; }

        public string UserName { get; protected set; }

        public string DeviceName { get; protected set; }

        public string DeviceType { get; protected set; }
    }
}
