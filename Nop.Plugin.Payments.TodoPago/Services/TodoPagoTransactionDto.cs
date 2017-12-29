namespace Nop.Plugin.Payments.TodoPago.Services
{
    public class TodoPagoTransactionDto
    {
        public int ordenId { get; set; }

        public string firstStep { get; set; }

        public string paramsSAR { get; set; }

        public string responseSAR { get; set; }

        public string requestKey { get; set; }

        public string publicRequestKey { get; set; }

        public string secondStep { get; set; }

        public string paramsGAA { get; set; }

        public string responseGAA { get; set; }

        public string answerKey { get; set; }

    }
}
