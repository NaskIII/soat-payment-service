using Domain.ValueObjects;

namespace Domain.Entities
{
    public class ServiceAccounts
    {
        public Guid Id { get; private set; }
        public Name ServiceName { get; private set; }
        public Guid ClientId { get; private set; }
        public string ClientSecretHash { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected ServiceAccounts() 
        {
            ClientSecretHash = string.Empty;
            ServiceName = default!;
        }

        public ServiceAccounts(Name serviceName, Guid clientId, string clientSecretHash)
        {
            Id = Guid.NewGuid();
            ServiceName = serviceName;
            ClientId = clientId;
            ClientSecretHash = clientSecretHash;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public ServiceAccounts(Guid id, Name serviceName, Guid clientId, string clientSecretHash, DateTime dateTime)
        {
            Id = id;
            ServiceName = serviceName;
            ClientId = clientId;
            ClientSecretHash = clientSecretHash;
            IsActive = true;
            CreatedAt = dateTime;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void RotateSecret(string newSecretHash)
        {
            ClientSecretHash = newSecretHash;
        }
    }
}