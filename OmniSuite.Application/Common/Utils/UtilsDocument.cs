namespace OmniSuite.Application.Common.Utils
{
    public static class UtilsDocument
    {
        public static string formatCPFToSaveOnDb(string cpf)
        {
            return new string(cpf?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
        }

        public static string FormatPhoneToSaveOnDb(string cpf)
        {
            return new string(cpf?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
        }

        public static string FormaPixValueToSave(string key, string type)
        {
            if (string.IsNullOrWhiteSpace(key)) return string.Empty;

            return type.ToLower() switch
            {
                "cpf" or "cnpj" or "phone" => new string(key.Where(char.IsLetterOrDigit).ToArray()),
                "email" => key.Trim().ToLower(),
                "random" => key.Trim(),
                _ => key.Trim()
            };
        }

        public static bool IsValidCpf(string cpf)
        {
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11 || cpf.All(d => d == cpf[0]))
                return false;

            var numbers = cpf.Select(c => int.Parse(c.ToString())).ToArray();

            var firstCheckDigit = (numbers.Take(9).Select((n, i) => n * (10 - i)).Sum() * 10 % 11) % 10;
            var secondCheckDigit = (numbers.Take(10).Select((n, i) => n * (11 - i)).Sum() * 10 % 11) % 10;

            return numbers[9] == firstCheckDigit && numbers[10] == secondCheckDigit;
        }

        public static bool IsValidCnpj(string cnpj)
        {
            cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

            if (cnpj.Length != 14 || cnpj.All(d => d == cnpj[0]))
                return false;

            var multipliers1 = new int[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multipliers2 = new int[] { 6 }.Concat(multipliers1).ToArray();

            var numbers = cnpj.Select(c => int.Parse(c.ToString())).ToArray();

            var sum1 = numbers.Take(12).Select((n, i) => n * multipliers1[i]).Sum();
            var checkDigit1 = sum1 % 11 < 2 ? 0 : 11 - sum1 % 11;

            var sum2 = numbers.Take(13).Select((n, i) => n * multipliers2[i]).Sum();
            var checkDigit2 = sum2 % 11 < 2 ? 0 : 11 - sum2 % 11;

            return numbers[12] == checkDigit1 && numbers[13] == checkDigit2;
        }

        public static bool IsValidDocument(string document)
        {
            if (string.IsNullOrWhiteSpace(document))
                return false;

            var digits = new string(document.Where(char.IsDigit).ToArray());

            if (digits.Length == 11)
                return IsValidCpf(digits);

            if (digits.Length == 14)
                return IsValidCnpj(digits);

            return false;
        }
    }
}
