using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.TodoPago
{
    public partial class RouteProvider : IRouteProvider{

        public void RegisterRoutes(RouteCollection routes){

            //OrderOk        
            routes.MapRoute("Plugin.Payments.TodoPago.OrderReturn",
                 "Plugins/PaymentTodoPago/OrderReturn",
                 new { controller = "PaymentTodoPago", action = "OrderReturn" },
                 new[] { "Nop.Plugin.Payments.TodoPago.Controllers" }
            );

            //Cancel
            routes.MapRoute("Plugin.Payments.TodoPago.CancelOrder",
                 "Plugins/PaymentTodoPago/CancelOrder",
                 new { controller = "PaymentTodoPago", action = "CancelOrder" },
                 new[] { "Nop.Plugin.Payments.TodoPago.Controllers" }
            );

            //Credentials
            routes.MapRoute("Plugin.Payments.TodoPago.GetCredentials",
                 "Plugins/PaymentTodoPago/GetCredentials",
                 new { controller = "PaymentTodoPago", action = "GetCredentials" },
                 new[] { "Nop.Plugin.Payments.TodoPago.Controllers" }
            );

            //List
            routes.MapRoute("Plugin.Payments.TodoPago.List",
                 "Plugins/PaymentTodoPago/List",
                 new { controller = "PaymentTodoPago", action = "List" },
                 new[] { "Nop.Plugin.Payments.TodoPago.Controllers" }
            );

            //GetStatus
            routes.MapRoute("Plugin.Payments.TodoPago.GetStatus",
                 "Plugins/PaymentTodoPago/GetStatus/{id}",
                 new { controller = "PaymentTodoPago", action = "GetStatus", id = "" },
                 new[] { "Nop.Plugin.Payments.TodoPago.Controllers" }
            );

        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
