using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.TodoPago.Models
{
    public class HybridModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Id")]
        public int ID { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PulicRequestKey")]
        public string PUBLICREQUESTKEY { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.NombreCompleto")]
        public string NOMBRECOMPLETO { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Email")]
        public string EMAIL { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.UrlError")]
        public string URL_ERROR { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.UrlOk")]
        public string URL_OK { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.UrlHybridForm")]
        public string URL_HYBRIDFORM { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Amount")]
        public decimal Amount { get; set; }
    }
}
