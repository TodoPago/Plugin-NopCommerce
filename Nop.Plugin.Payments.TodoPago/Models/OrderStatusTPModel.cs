using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.TodoPago.Models
{
    public class OrderStatusTPModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.OrderStatusId")]
        public int ORDERSTATUSID { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.OrderStatusMessage")]
        public string ORDERSTATUSMESSAGE { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.OrderDetails")]
        public string ORDERDETAILS { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Order")]
        public string ORDER { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.OrderMessage")]
        public string ORDERMESSAGE { get; set; }

        public OrderStatusTPModel()
        {
            this.ORDERDETAILS = "Order Details";
            this.ORDER = "Order #";
            this.ORDERMESSAGE = "Message";
        }
    }
}
