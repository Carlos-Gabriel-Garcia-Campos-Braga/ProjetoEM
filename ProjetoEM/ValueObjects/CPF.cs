namespace ProjetoEM.ValueObjects
{
    public class CPF
    {
        public string Value { get; set; }

        public CPF()
        {
            Value = string.Empty;
        }

        public CPF(string value)
        {
            Value = value;
        }

        public static implicit operator string(CPF cpf) => cpf.Value;
        public static explicit operator CPF(string value) => new CPF(value);

        public override string ToString() => Value;
    }
}
