namespace OmniSuite.Application.Common.Utils
{
    public static class UtilsMoney
    {
        /// <summary>
        /// Converte um valor monetário em reais (ex: 1234.56) para centavos (ex: 123456).
        /// </summary>
        public static long ToCents(decimal amount)
        {
            return (long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Converte um valor em centavos (ex: 123456) para reais (ex: 1234.56).
        /// </summary>
        public static decimal FromCents(long cents)
        {
            return cents / 100m;
        }
    }
}
