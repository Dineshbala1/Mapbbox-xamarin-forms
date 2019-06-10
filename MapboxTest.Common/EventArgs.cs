using System;

namespace MapboxTest.Common
{
    public class EventArgs<T> : EventArgs
    {
        public T Data { get; }

        public EventArgs(T data)
        {
            Data = data;
        }
    }
}