using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Nop.Core;
using Nop.Web.Framework.Menu;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.TodoPago.Controllers;
using Nop.Plugin.Payments.TodoPago.Services;
using Nop.Plugin.Payments.TodoPago.Data;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Logging;
using TodoPagoConnector.Model;
using Nop.Plugin.Payments.TodoPago.DTO;
using Nop.Plugin.Payments.TodoPago.Models;

namespace Nop.Plugin.Payments.TodoPago
{
    /// <summary>
    /// AuthorizeNet payment processor
    /// </summary>
    public class TodoPagoPaymentProcessor : BasePlugin, IPaymentMethod, IAdminMenuPlugin
    {
        private readonly ISettingService _settingService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IWebHelper _webHelper;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IEncryptionService _encryptionService;
        private readonly CurrencySettings _currencySettings;
        private readonly TodoPagoPaymentSettings _todoPagoPaymentSettings;
        private readonly TodoPagoTransactionObjectContex _contex;
        private readonly TodoPagoAddressBookObjectContex _contexAddressBook;
        private readonly HttpContextBase _httpContext;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;

        //TodoPago
        private TodoPagoBusinessService todoPagoBusinessService;

        public TodoPagoPaymentProcessor(ISettingService settingService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IWebHelper webHelper,
            IOrderTotalCalculationService orderTotalCalculationService,
            IEncryptionService encryptionService,
            CurrencySettings currencySettings,
            TodoPagoPaymentSettings todoPagoPaymentSettings,
            TodoPagoTransactionObjectContex contex,
            TodoPagoAddressBookObjectContex contexAddressBook,
            HttpContextBase httpContext,
            ILogger logger,
            IOrderService orderService,
            ITodoPagoTransactionService todoPagoTransactionService,
            ITodoPagoAddressBookService todoPagoAddressBookService)
        {
            this._todoPagoPaymentSettings = todoPagoPaymentSettings;
            this._settingService = settingService;
            this._currencyService = currencyService;
            this._customerService = customerService;
            this._currencySettings = currencySettings;
            this._webHelper = webHelper;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._encryptionService = encryptionService;
            this._contex = contex;
            this._contexAddressBook = contexAddressBook;
            this._httpContext = httpContext;
            this._logger = logger;
            this._orderService = orderService;
            this.todoPagoBusinessService = new TodoPagoBusinessService(todoPagoTransactionService, todoPagoAddressBookService);
        }

        public void ManageSiteMap(Nop.Web.Framework.Menu.SiteMapNode rootNode)
        {

            var menuItem = new Nop.Web.Framework.Menu.SiteMapNode()
            {
                SystemName = "TodoPago",
                Title = "TodoPago",
                ControllerName = "Plugins/PaymentTodoPago",
                ActionName = "List",
                Visible = true,
                //RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };
            //var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            //if (pluginNode != null)
            //pluginNode.ChildNodes.Add(menuItem);
            // else
            rootNode.ChildNodes.Add(menuItem);
        }

        public Dictionary<string, object> TodoPagoSecondStep(String answerKey, Order order)
        {
            Payment paymentModel = new Payment(_todoPagoPaymentSettings, _logger, _webHelper, todoPagoBusinessService, _httpContext, _orderService);

            return paymentModel.GetAuthorizeAnswer(answerKey, order);
        }

        public CredentialsResponse getCredentials(String user, String password, String ambiente)
        {
            Account accountModel = new Account();

            return accountModel.GetCredentials(new User(user, password), ambiente);
        }

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return result;
        }

        /// <summary>
        /// Post process payment (used by payment ginateways that require redirecting to a third-party URL)
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            String urlRedirect = String.Empty;

            Payment paymentModel = new Payment(_todoPagoPaymentSettings, _logger, _webHelper, todoPagoBusinessService, _httpContext);

            urlRedirect = paymentModel.SendAuthorizeRequest(postProcessPaymentRequest, this.PluginDescriptor.Version);

            _httpContext.Response.Redirect(urlRedirect);
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            //var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart, _authorizeNetPaymentSettings.AdditionalFee, _authorizeNetPaymentSettings.AdditionalFeePercentage);
            var result = 0;
            return result;
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            Refund refundModel = new Models.Refund(todoPagoBusinessService, _todoPagoPaymentSettings, _logger);

            return refundModel.ExecuteRefund(refundPaymentRequest);
        }

        public StatusModel getStatus(Order order)
        {
            Status statusModel = new Status(todoPagoBusinessService, _todoPagoPaymentSettings, _logger);

            return statusModel.GetStatus(order);
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //it's not a redirection payment method. So we always return false
            return false;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentTodoPago";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.TodoPago.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentTodoPago";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.TodoPago.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentTodoPagoController);
        }

        public override void Install()
        {
            //settings 
            var settings = new TodoPagoPaymentSettings
            {
                //Configuracion general
                Titulo = "TodoPago",
                Descripcion = "Pague de manera segura mediante TodoPago, solo para republica argentina",
                Ambiente = Ambiente.Developers,
                Segmento = Segmento.Retail,
                //DeadLine = "",
                //Formulario = Formulario.Externo,
                SetCuotas = false,
                MaxCuotas = MaxCuotas.Doce,
                Chart = true,

                User = "",
                Password = "",

                //Ambiente Developer
                ApiKeyDeveloper = "",
                SecurityDeveloper = "",
                MerchantDeveloper = "",

                //Ambiente Production
                ApiKeyProduction = "",
                SecurityProduction = "",
                MerchantProduction = "",

                //Estdos del pedido
                TransaccionIniciada = OrderStatus.Pending,
                TransaccionAprobada = OrderStatus.Complete,
                TransaccionRechazada = OrderStatus.Cancelled,
                TransaccionOffline = OrderStatus.Processing
            };

            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Notes", "Descripcion de TodoPago");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Titulo", "Titulo");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Titulo.Hint", "Titulo que el usuario ve durante el checkout.");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Descripcion", "Descripcion");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Descripcion.Hint", "Descripcion que el usuario ve durante el checkout.");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.AmbienteValues", "Ambiente");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.AmbienteValues.Hint", "Seleccione el ambiente con el que desea trabajar.");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SegmentoValues", "Tipo de segmento");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SegmentoValues.Hint", "Seleccione el tipo de segmento con el que desea trabajar.");

            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.DeadLine", "DeadLine");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.DeadLine.Hint", "Dias maximos para la entrega."); 

            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.FormularioValues", "Elija el formulario que desea utilizar");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.FormularioValues.Hint", "Puede esscojer entre un formulario integrado al comercio o redireccionar al formulario externo");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SetCuotas", "Habilitar/Desabilitar cantidad de cuotas");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SetCuotas.Hint", "Habilita el ingreso de la cantidad de cuotas maxima ofrecida");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.MaxCuotasValues", "Numero maximo de cuotas");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.MaxCuotasValues.Hint", "Puede escojer entre 1 o 12 cuotas");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SetTimeout", "Habilitar/Desabilitar timeout");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SetTimeout.Hint", "Habilita el ingreso del timeout");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Timeout", "Tiempo maximo en milisegundos para realizar el pago en el formulario (300000 a 21600000)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Timeout.Hint", "Los valores posibles van de 300000 (5 minutos) a 21600000 (6hs) en milisegundos. En caso de que no se envíe el parámetro el valor por defecto es 1800000 (30 minutos).");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Chart", "¿Desea vaciar carrito de compras al fallar el pago?");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Chart.Hint", "Si falla el pago, vacía el carrito de compras");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.GoogleMaps", "¿Desea validar la dirección de compra con Google Maps?");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.GoogleMaps.Hint", "Verifica la dirección con Google");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Hibrido", "¿Desea utilizar el formulario híbrido?");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Hibrido.Hint", "Verifica la dirección con Google");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.User", "User");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.User.Hint", "User de Todo Pago.");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Password", "Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Password.Hint", "Password de Todo Pago.");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.BtnCredentials", "Obtener Credenciales");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ApiKeyDeveloper", "HTTP Header");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ApiKeyDeveloper.Hint", "Authorization para el header. Ejemplo: PRISMA 2894UK59JD6SKL56J40O487EH3KW8943");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SecurityDeveloper", "Security");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SecurityDeveloper.Hint", "Codigo provisto por Todo Pago");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.MerchantDeveloper", "Merchant ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.MerchantDeveloper.Hint", "Numero de comercio provisto por Todo Pago");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ApiKeyProduction", "HTTP Header");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ApiKeyProduction.Hint", "Authorization para el header. Ejemplo: PRISMA 2894UK59JD6SKL56J40O487EH3KW8943");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SecurityProduction", "Security");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SecurityProduction.Hint", "Codigo provisto por Todo Pago");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.MerchantProduction", "Merchant ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.MerchantProduction.Hint", "Numero de comercio provisto por Todo Pago");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionIniciadaValues", "Estado cuando la transaccion ha sido iniciada");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionIniciadaValues.Hint", "Valor por defecto: Pendiente");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionAprobadaValues", "Estado cuando la transaccion ha sido Aprobada");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionAprobadaValues.Hint", "Valor por defecto: Completado");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionRechazadaValues", "Estado cuando la transaccion ha sido Rechazada");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionRechazadaValues.Hint", "Valor por defecto: Fallido");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionOfflineValues", "Estado cuando la transaccion ha sido Offline");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionOfflineValues.Hint", "Valor por defecto: Pendiente");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.RedirectionTip", "Todo Pago va a redireccionar al formulario de pago.");

            //Status Fields
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.OrderStatus", "Estado de la Transaccion");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ResultMessage", "Mensaje de la Transaccion");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Close", "Cerrar");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ResultCode", "ResultCode");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.DateTime", "DateTime");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.OperationId", "OperationId");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CurrencyCode", "CurrencyCode");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Amount", "Amount");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Type", "Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.InstallmentPayments", "InstallmentPayments");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CustomerEmail", "CustomerEmail");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.IdentificationType", "IdentificationType");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Identification", "Identification");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CardNumber", "CardNumber");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CardHolderName", "CardHolderName");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TicketNumber", "TicketNumber");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.AuthorizationCode", "AuthorizationCode");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.BarCode", "BarCode");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CouponExpDate", "CouponExpDate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CouponSecexpDate", "CouponSecexpDate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CouponSubscriber", "CouponSubscriber");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.BankId", "BankId");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PaymentMethodType", "PaymentMethodType");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PaymentMethodCode", "PaymentMethodCode");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PromotionId", "PromotionId");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.AmountBuyer", "AmountBuyer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PaymentMethodName", "PaymentMethodName");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PushNotifyEndpoint", "PushNotifyEndpoint");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PushNotifyMethod", "PushNotifyMethod");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PushNotifyStates", "PushNotifyStates");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Refunded", "Refunded");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Refunds", "Refunds");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.FeeAmount", "FeeAmount");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TaxAmount", "TaxAmount");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ServiceChargeAmount", "ServiceChargeAmount");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CreditedAmount", "CreditedAmount");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.FeeAmountBuyer", "FeeAmountBuyer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TaxAmountBuyer", "TaxAmountBuyer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CreditedAmountBuyer", "CreditedAmountBuyer");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.EstadoContraCargo", "EstadoContraCargo");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Comision", "Comision");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.IdContracargo", "IdContracargo");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.FechaNotificacionCuenta", "FechaNotificacionCuenta");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TEA", "TEA");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CFT", "CFT");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.RELEASESTATUS", "RELEASESTATUS");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.RELEASEDATETIME", "RELEASEDATETIME");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PHONENUMBER", "PHONENUMBER");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ADDRESS", "ADDRESS");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.POSTALCODE", "POSTALCODE");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CUSTOMERID", "CUSTOMERID");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PRODUCTCODE", "PRODUCTCODE");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PRODUCTDESCRIPTION", "PRODUCTDESCRIPTION");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PRODUCTNAME", "PRODUCTNAME");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.QUANTITY", "QUANTITY");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PRODUCTSKU", "PRODUCTSKU");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.UNITPRICE", "UNITPRICE");
            //this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TOTALAMOUNT", "TOTALAMOUNT");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Items", "Items");

            _contex.Install();
            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<TodoPagoPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Notes");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Titulo");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Titulo.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Descripcion");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Descripcion.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.AmbienteValues");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.AmbienteValues.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SegmentoValues");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SegmentoValues.Hint");
            //this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.DeadLine");
            //this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.DeadLine.Hint");
            //this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.FormularioValues");
            //this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.FormularioValues.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SetCuotas");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SetCuotas.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.MaxCuotasValues");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.MaxCuotasValues.Hint");

            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SetTimeout");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SetTimeout.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Timeout");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Timeout.Hint");

            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Chart");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Chart.Hint");

            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.GoogleMaps");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.GoogleMaps.Hint");

            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Hibrido");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Hibrido.Hint");

            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.User");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.User.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Password");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Password.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.BtnCredentials");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ApiKeyDeveloper");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ApiKeyDeveloper.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SecurityDeveloper");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SecurityDeveloper.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.MerchantDeveloper");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.MerchantDeveloper.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ApiKeyProduction");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ApiKeyProduction.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SecurityProduction");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.SecurityProduction.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.MerchantProduction");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.MerchantProduction.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionIniciadaValues");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionIniciadaValues.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionAprobadaValues");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionAprobadaValues.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionRechazadaValues");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionRechazadaValues.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionOfflineValues");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TransaccionOfflineValues.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.RedirectionTip");

            //Status Fields
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.OrderStatus");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Close");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ResultCode");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.DateTime");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.OperationId");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CurrencyCode");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Amount");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Type");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.InstallmentPayments");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CustomerEmail");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.IdentificationType");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Identification");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CardNumber");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CardHolderName");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TicketNumber");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.AuthorizationCode");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.BarCode");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CouponExpDate");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CouponSecexpDate");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CouponSubscriber");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.BankId");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PaymentMethodType");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PaymentMethodCode");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PromotionId");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.AmountBuyer");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PaymentMethodName");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PushNotifyEndpoint");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PushNotifyMethod");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PushNotifyStates");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Refunded");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Refunds");

            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ResultMessage");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.FeeAmount");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TaxAmount");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ServiceChargeAmount");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CreditedAmount");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.FeeAmountBuyer");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TaxAmountBuyer");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CreditedAmountBuyer");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.EstadoContraCargo");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Comision");

            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.IdContracargo");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.FechaNotificacionCuenta");

            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TEA");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CFT");

            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.RELEASESTATUS");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.RELEASEDATETIME");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PHONENUMBER");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.ADDRESS");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.POSTALCODE");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.CUSTOMERID");
            //this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PRODUCTCODE");
            //this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PRODUCTDESCRIPTION");
            //this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PRODUCTNAME");
            //this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.QUANTITY");
            //this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.PRODUCTSKU");
            //this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.UNITPRICE");
            //this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.TOTALAMOUNT");
            this.DeletePluginLocaleResource("Plugins.Payments.TodoPago.Fields.Items");

            _contex.Uninstall();
            base.Uninstall();
        }

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                //if (_todoPagoPaymentSettings.Hibrido)
                //    return PaymentMethodType.Standard;
                //else
                return PaymentMethodType.Redirection;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }
    }
}
