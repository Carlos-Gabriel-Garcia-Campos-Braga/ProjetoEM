namespace EM.Domain.Utilitarios
{
    public class CPF
    {
        public string Value { get; set; }
        public string CpfFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Value))
                {
                    return string.Empty;
                }

                string cpfNumbers = RemoverFormatacao(Value);
                if (cpfNumbers.Length == 11 && long.TryParse(cpfNumbers, out long cpfNum))
                {
                    return string.Format("{0:000\\.000\\.000\\-00}", cpfNum);
                }

                return Value;
            }
        }

        public CPF()
        {
            Value = string.Empty;
        }

        public CPF(string value)
        {
            Value = value;
        }

        public bool EhValido()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                return false;
            }

            string? cpfLimpo = RemoverFormatacao(Value);

            if (cpfLimpo.Length != 11)
            {
                return false;
            }

            if (cpfLimpo.All(c => c == cpfLimpo[0]))
            {
                return false;
            }

            return ValidarDigitosVerificadores(cpfLimpo);
        }

        public static string RemoverFormatacao(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                return string.Empty;
            }
                
            return cpf.Replace(".", "").Replace("-", "").Trim();
        }

        private static bool ValidarDigitosVerificadores(string cpf)
        {
            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (10 - i);
            }

            int digito1 = (soma % 11 < 2) ? 0 : 11 - (soma % 11);

            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += int.Parse(cpf[i].ToString()) * (11 - i);
            }

            int digito2 = (soma % 11 < 2) ? 0 : 11 - (soma % 11);

            return digito1 == int.Parse(cpf[9].ToString()) && 
                   digito2 == int.Parse(cpf[10].ToString());
        }

        public static implicit operator string(CPF cpf)
        {
            return cpf.Value;
        }
        
        public static explicit operator CPF(string value)
        {
            return new(value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}

