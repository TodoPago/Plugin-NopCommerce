using Nop.Data.Mapping;
using Nop.Plugin.Payments.TodoPago.Domain;

namespace Nop.Plugin.Payments.TodoPago.Data
{
    public partial class TodoPagoAddressBookRecordMap : NopEntityTypeConfiguration<TodoPagoAddressBookRecord>
    {
        public TodoPagoAddressBookRecordMap()
        {
            this.ToTable("todopago_address_book");
            this.HasKey(x => x.Id);
            this.Property(x => x.hash);
            this.Property(x => x.street);
            this.Property(x => x.city);
            this.Property(x => x.state);
            this.Property(x => x.country);
            this.Property(x => x.postal);
        }
    }
}
