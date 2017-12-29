namespace Nop.Plugin.Payments.TodoPago.Services
{
    public class TodoPagoAddressBookDto
    {
        public string hash { get; set; }
        public string street { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string postal { get; set; }
    }
}
