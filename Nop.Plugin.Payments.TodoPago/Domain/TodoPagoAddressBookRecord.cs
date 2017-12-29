using Nop.Core;

namespace Nop.Plugin.Payments.TodoPago.Domain
{
    public partial class TodoPagoAddressBookRecord : BaseEntity
    {
        public string hash { get; set; }

        public string street { get; set; }

        public string state { get; set; }

        public string city { get; set; }

        public string country { get; set; }

        public string postal { get; set; }
    }
}
