using Nop.Core.Configuration;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Payments.TodoPago
{
    public class TodoPagoPaymentSettings : ISettings
    {
        //Configuracion general

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public Ambiente Ambiente { get; set; }

        public Segmento Segmento { get; set; }

        //public string DeadLine { get; set; }

        //public Formulario Formulario { get; set; } 

        public bool SetCuotas { get; set; }

        public MaxCuotas MaxCuotas { get; set; }

        public bool SetTimeout { get; set; }

        public string Timeout { get; set; }

        public bool Chart { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        //Ambiente Developer

        public string ApiKeyDeveloper { get; set; }

        public string SecurityDeveloper { get; set; }

        public string MerchantDeveloper { get; set; }


        //Ambiente Production

        public string ApiKeyProduction { get; set; }

        public string SecurityProduction { get; set; }

        public string MerchantProduction { get; set; }


        //Estdos del pedido

        public OrderStatus TransaccionIniciada { get; set; }

        public OrderStatus TransaccionAprobada { get; set; }

        public OrderStatus TransaccionRechazada { get; set; }

        public OrderStatus TransaccionOffline { get; set; }

    }
}
