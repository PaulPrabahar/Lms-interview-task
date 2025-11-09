namespace BuildingBlocks.Exceptions;

public class NotFoundException:Exception
{
    public NotFoundException(string message):base(message)
    {

    }

    public NotFoundException(string name, object Key) :base($"Entity\"{name}\"({Key}) was not found.")
    { 
    }
}
