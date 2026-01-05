namespace Infraestructure.Exceptions
{
    [Serializable]
    public class TransactionIsNotOpen : Exception
    {

        public TransactionIsNotOpen() { }

        public TransactionIsNotOpen(string message) : base(message) { }
    }
}
