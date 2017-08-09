using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Data.Mapping;
using Nop.Plugin.Payments.TodoPago.Domain;

namespace Nop.Plugin.Payments.TodoPago.Data
{
    public partial class TodoPagoTransactionRecordMap : NopEntityTypeConfiguration<TodoPagoTransactionRecord>
    {

        public TodoPagoTransactionRecordMap()
        {

            this.ToTable("todopago_transaction");
            this.HasKey(x => x.Id);
            this.Property(x => x.ordenId);
            this.Property(x => x.firstStep);
            this.Property(x => x.paramsSAR);
            this.Property(x => x.responseSAR);
            this.Property(x => x.secondStep);
            this.Property(x => x.paramsGAA);
            this.Property(x => x.responseGAA);
            this.Property(x => x.requestKey);
            this.Property(x => x.publicRequestKey);
            this.Property(x => x.answerKey);
        }

    }
}
