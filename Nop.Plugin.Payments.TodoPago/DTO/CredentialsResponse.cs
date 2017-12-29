namespace Nop.Plugin.Payments.TodoPago.DTO
{
    public class CredentialsResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string merchandid { get; set; }
        public string apikey { get; set; }
        public string security { get; set; }
    }
}
