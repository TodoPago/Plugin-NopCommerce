using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.TodoPago.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        //Configuracion general

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Titulo")]
        public string Titulo { get; set; }
        public bool Titulo_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Descripcion")]
        public string Descripcion { get; set; }
        public bool Descripcion_OverrideForStore { get; set; }

        public int AmbienteId { get; set; }
        public bool AmbienteId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.AmbienteValues")]
        public SelectList AmbienteValues { get; set; }

        public int SegmentoId { get; set; }
        public bool SegmentoId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.SegmentoValues")]
        public SelectList SegmentoValues { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.DeadLine")]
        //public string DeadLine { get; set; }
        //public bool DeadLine_OverrideForStore { get; set; }

        //public int FormularioId { get; set; }
        //public bool FormularioId_OverrideForStore { get; set; }
        //[NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.FormularioValues")]
        //public SelectList FormularioValues { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.SetCuotas")]
        public bool SetCuotas { get; set; }
        public bool SetCuotas_OverrideForStore { get; set; }

        public int MaxCuotasId { get; set; }
        public bool MaxCuotasId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.MaxCuotasValues")]
        public SelectList MaxCuotasValues { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.SetTimeout")]
        public bool SetTimeout { get; set; }
        public bool SetTimeout_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Timeout")]
        public string Timeout { get; set; }
        public bool Timeout_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Chart")]
        public bool Chart { get; set; }
        public bool Chart_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.GoogleMaps")]
        public bool GoogleMaps { get; set; }
        public bool GoogleMaps_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Hibrido")]
        public bool Hibrido { get; set; }
        public bool Hibrido_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.UrlBannerBilletera")]
        public string UrlBannerBilletera { get; set; }
        public bool UrlBannerBilletera_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.User")]
        public string UserDev { get; set; }
        public bool UserDev_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Password")]
        public string PasswordDev { get; set; }
        public bool PasswordDev_OverrideForStore { get; set; }

        //Ambiente Developer

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.ApiKeyDeveloper")]
        public string ApiKeyDeveloper { get; set; }
        public bool ApiKeyDeveloper_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.SecurityDeveloper")]
        public string SecurityDeveloper { get; set; }
        public bool SecurityDeveloper_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.MerchantDeveloper")]
        public string MerchantDeveloper { get; set; }
        public bool MerchantDeveloper_OverrideForStore { get; set; }


        //Ambiente Production
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.User")]
        public string User { get; set; }
        public bool User_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Password")]
        public string Password { get; set; }
        public bool Password_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.ApiKeyProduction")]
        public string ApiKeyProduction { get; set; }
        public bool ApiKeyProduction_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.SecurityProduction")]
        public string SecurityProduction { get; set; }
        public bool SecurityProduction_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.MerchantProduction")]
        public string MerchantProduction { get; set; }
        public bool MerchantProduction_OverrideForStore { get; set; }


        //Estdos del pedido

        public int TransaccionIniciadaId { get; set; }
        public bool TransaccionIniciadaId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.TransaccionIniciadaValues")]
        public SelectList TransaccionIniciadaValues { get; set; }

        public int TransaccionAprobadaId { get; set; }
        public bool TransaccionAprobadaId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.TransaccionAprobadaValues")]
        public SelectList TransaccionAprobadaValues { get; set; }

        public int TransaccionRechazadaId { get; set; }
        public bool TransaccionRechazadaId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.TransaccionRechazadaValues")]
        public SelectList TransaccionRechazadaValues { get; set; }

        public int TransaccionOfflineId { get; set; }
        public bool TransaccionOfflineId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.TransaccionOfflineValues")]
        public SelectList TransaccionOfflineValues { get; set; }


    }
}