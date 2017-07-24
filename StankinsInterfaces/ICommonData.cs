using System;

namespace StankinsInterfaces
{
    public interface ICommonData
    {
        //TODO: modify UTCDateReceived into UTCDate
        DateTime UTCDateReceived { get; }
        DateTime LocalDateReceived { get; }
        string UserName { get; }
        string DeviceName { get; }
        string DeviceType { get; }
    }
}