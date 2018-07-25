using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.TodoPago.Models
{
    public class PaymentInfoModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.UrlBilletera")]
        public string UrlBilletera { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.BilleteraCheckoutActiva")]
        public bool BilleteraCheckoutActiva { get; set; }
    }
}
