namespace Infraestructure.Exceptions
{
    public class IndexUpdateException : Exception
    {
        public IndexUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public IndexUpdateException() : base("An error occurred while updating the index in the database.")
        {
        }
    }
}
