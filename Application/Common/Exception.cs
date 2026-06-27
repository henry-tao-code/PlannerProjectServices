namespace ProjectPlanner.Application.Common;

public class ConcurrencyException : Exception
{
    public ConcurrencyException()
        : base("The data has been modified by another user or process. Please reload and try again.")
    {
    }

    public ConcurrencyException(string message)
        : base(message)
    {
    }

    public ConcurrencyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class PersistenceException : Exception
{
    public PersistenceException()
    {
    }

    public PersistenceException(string message)
        : base(message)
    {
    }

    public PersistenceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}

