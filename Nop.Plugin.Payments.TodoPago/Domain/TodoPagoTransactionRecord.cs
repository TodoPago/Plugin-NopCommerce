using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Payments.TodoPago.Domain
{
    public partial class TodoPagoTransactionRecord : BaseEntity
    {

        public int ordenId { get; set; }

        public string firstStep { get; set; }

        public string paramsSAR { get; set; }

        public string responseSAR { get; set; }

        public string secondStep { get; set; }

        public string paramsGAA { get; set; }

        public string responseGAA { get; set; }

        public string requestKey { get; set; }

        public string publicRequestKey { get; set; }

        public string answerKey { get; set; }
 
    }
}
