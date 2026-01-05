namespace Domain.Exceptions
{
    public class DuplicateEntryException : Exception
    {
        public DuplicateEntryException(string message) : base(message)
        {
        }
        public DuplicateEntryException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public DuplicateEntryException() : base("An entry with the same key already exists.")
        {
        }
    }
}
