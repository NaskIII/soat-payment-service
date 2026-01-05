using System;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public sealed class Cpf : IEquatable<Cpf>
    {
        public string Value { get; }

        public Cpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                throw new ArgumentException("CPF cannot be null or empty.");

            var cleanedCpf = Clean(cpf);

            if (!IsValid(cleanedCpf))
                throw new ArgumentException("Invalid CPF.");

            Value = cleanedCpf;
        }

        private static string Clean(string cpf)
        {
            return Regex.Replace(cpf, @"[^\d]", "");
        }

        private static bool IsValid(string cpf)
        {
            if (cpf.Length != 11)
                return false;

            if (new string(cpf[0], cpf.Length) == cpf)
                return false;

            if (!ValidateDigit(cpf, 9)) return false;
            if (!ValidateDigit(cpf, 10)) return false;

            return true;
        }

        private static bool ValidateDigit(string cpf, int position)
        {
            int sum = 0;
            int weight = position + 1;

            for (int i = 0; i < position; i++)
            {
                sum += (cpf[i] - '0') * weight--;
            }

            int mod = sum % 11;
            int digit = mod < 2 ? 0 : 11 - mod;

            return digit == (cpf[position] - '0');
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Cpf);
        }

        public bool Equals(Cpf? other)
        {
            if (other is null) return false;
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Convert.ToUInt64(Value).ToString(@"000\.000\.000\-00");
        }
    }
}
