namespace Domain.ValueObjects
{
    public sealed class Name : IEquatable<Name>
    {
        public string Value { get; }

        public Name(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            Value = name.Trim();
        }

        public bool Equals(Name? other)
        {
            if (other is null) return false;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as Name);
        }
        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }
}
