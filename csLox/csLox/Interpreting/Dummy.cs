using System;

public sealed class Dummy
{
    Dummy()
    {
        throw new InvalidOperationException("Don't instantiate Dummy.");
    }
}