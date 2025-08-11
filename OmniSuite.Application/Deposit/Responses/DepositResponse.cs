namespace OmniSuite.Application.Deposit.Responses
{
    public class DepositResponse
    {
        public Guid Id { get; set; }

        public string Status { get; set; }

        public string PixCopyPasteCode { get; set; }

        public int Expiration { get; set; }

        public string QrCodeBase64 { get; set; }
    }
}
