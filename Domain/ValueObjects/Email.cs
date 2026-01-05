using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public sealed class Email : IEquatable<Email>
    {
        public string Value { get; }

        public Email(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.");

            var cleanedEmail = email.Trim();

            if (!IsValid(cleanedEmail))
                throw new ArgumentException("Invalid email format.");

            Value = cleanedEmail;
        }

        private static bool IsValid(string email)
        {
            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Email);
        }

        public bool Equals(Email? other)
        {
            if (other is null) return false;
            return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Value.ToLowerInvariant().GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
