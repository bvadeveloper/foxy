using System;

namespace Platform.Processor.Coordinator.Exceptions;

public class NoCollectorException : InvalidOperationException
{
    public NoCollectorException(string message) : base(message)
    {
    }
}