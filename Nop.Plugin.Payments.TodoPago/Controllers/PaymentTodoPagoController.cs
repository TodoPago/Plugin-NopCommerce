using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.TodoPago.Models;
using Nop.Admin.Models.Orders;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.TodoPago.Utils;
using Nop.Web.Framework.Controllers;
using System.Net;

namespace Nop.Plugin.Payments.TodoPago.Controllers
{
    public class PaymentTodoPagoController : BasePaymentController
    {
        protected const string TODOPAGO_STATUS_CODE = "StatusCode";
        protected const string TODOPAGO_STATUS_MESSAGE = "StatusMessage";

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IPaymentService _paymentService;
        private readonly PaymentSettings _paymentSettings;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly TodoPagoPaymentSettings _todoPagoPaymentSettings;
        private System.Web.HttpContextBase _httpContext;

        public PaymentTodoPagoController(ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            IPaymentService paymentService,
            IOrderService orderService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            PaymentSettings paymentSettings,
            TodoPagoPaymentSettings todoPagoPaymentSettings,
            IOrderProcessingService orderProcessingService
            )
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._orderService = orderService;
            this._storeContext = storeContext;
            this._webHelper = webHelper;
            this._paymentService = paymentService;
            this._paymentSettings = paymentSettings;
            this._todoPagoPaymentSettings = todoPagoPaymentSettings;
            this._orderProcessingService = orderProcessingService;
            this._httpContext = Nop.Core.Infrastructure.EngineContext.Current.Resolve<System.Web.HttpContextBase>();
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var todoPagoPaymentSettings = _settingService.LoadSetting<TodoPagoPaymentSettings>(storeScope);
            ActionResult result;
            double version;

            var model = new ConfigurationModel
            {

                //Configuracion general
                //Titulo = todoPagoPaymentSettings.Titulo,
                //Descripcion = todoPagoPaymentSettings.Descripcion,
                AmbienteId = Convert.ToInt32(todoPagoPaymentSettings.Ambiente),
                AmbienteValues = todoPagoPaymentSettings.Ambiente.GetSelectList(),
                SegmentoId = Convert.ToInt32(todoPagoPaymentSettings.Segmento),
                SegmentoValues = todoPagoPaymentSettings.Segmento.GetSelectList(),
                //DeadLine = todoPagoPaymentSettings.DeadLine,
                //FormularioId = Convert.ToInt32(todoPagoPaymentSettings.Formulario),
                //FormularioValues = todoPagoPaymentSettings.Formulario.GetSelectList(),
                SetCuotas = todoPagoPaymentSettings.SetCuotas,
                MaxCuotasId = Convert.ToInt32(todoPagoPaymentSettings.MaxCuotas),
                MaxCuotasValues = todoPagoPaymentSettings.MaxCuotas.GetSelectList(),

                SetTimeout = todoPagoPaymentSettings.SetTimeout,
                Timeout = todoPagoPaymentSettings.Timeout,

                Chart = todoPagoPaymentSettings.Chart,
                GoogleMaps = todoPagoPaymentSettings.GoogleMaps,

                Hibrido = todoPagoPaymentSettings.Hibrido,

                User = todoPagoPaymentSettings.User,
                Password = todoPagoPaymentSettings.Password,

                //Ambiente Developer
                ApiKeyDeveloper = todoPagoPaymentSettings.ApiKeyDeveloper,
                SecurityDeveloper = todoPagoPaymentSettings.SecurityDeveloper,
                MerchantDeveloper = todoPagoPaymentSettings.MerchantDeveloper,

                //Ambiente Production
                ApiKeyProduction = todoPagoPaymentSettings.ApiKeyProduction,
                SecurityProduction = todoPagoPaymentSettings.SecurityProduction,
                MerchantProduction = todoPagoPaymentSettings.MerchantProduction,

                //Estdos del pedido
                TransaccionIniciadaId = Convert.ToInt32(todoPagoPaymentSettings.TransaccionIniciada),
                TransaccionIniciadaValues = todoPagoPaymentSettings.TransaccionIniciada.GetSelectList(),
                TransaccionAprobadaId = Convert.ToInt32(todoPagoPaymentSettings.TransaccionAprobada),
                TransaccionAprobadaValues = todoPagoPaymentSettings.TransaccionAprobada.GetSelectList(),
                TransaccionRechazadaId = Convert.ToInt32(todoPagoPaymentSettings.TransaccionRechazada),
                TransaccionRechazadaValues = todoPagoPaymentSettings.TransaccionRechazada.GetSelectList(),
                TransaccionOfflineId = Convert.ToInt32(todoPagoPaymentSettings.TransaccionOffline),
                TransaccionOfflineValues = todoPagoPaymentSettings.TransaccionOffline.GetSelectList(),

                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {

                //Configuracion general
                //model.Titulo_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Titulo, storeScope);
                //model.Descripcion_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Descripcion, storeScope);
                model.AmbienteId_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Ambiente, storeScope);
                model.SegmentoId_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Segmento, storeScope);
                //model.DeadLine_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.DeadLine, storeScope);
                //model.FormularioId_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Formulario, storeScope);
                model.SetCuotas_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.SetCuotas, storeScope);
                model.MaxCuotasId_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.MaxCuotas, storeScope);

                model.SetTimeout_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.SetTimeout, storeScope);
                model.Timeout_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Timeout, storeScope);

                model.Chart_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Chart, storeScope);
                model.GoogleMaps_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.GoogleMaps, storeScope);

                model.Hibrido_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Hibrido, storeScope);

                model.User_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.User, storeScope);
                model.Password_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Password, storeScope);

                //Ambiente Developer
                model.ApiKeyDeveloper_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.ApiKeyDeveloper, storeScope);
                model.SecurityDeveloper_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.SecurityDeveloper, storeScope);
                model.MerchantDeveloper_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.MerchantDeveloper, storeScope);

                //Ambiente Production
                model.ApiKeyProduction_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.ApiKeyProduction, storeScope);
                model.SecurityProduction_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.SecurityProduction, storeScope);
                model.MerchantProduction_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.MerchantProduction, storeScope);

                //Estdos del pedido
                model.TransaccionIniciadaId_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.TransaccionIniciada, storeScope);
                model.TransaccionAprobadaId_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.TransaccionAprobada, storeScope);
                model.TransaccionRechazadaId_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.TransaccionRechazada, storeScope);
                model.TransaccionOfflineId_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.TransaccionOffline, storeScope);
            }

            // check plugin is installed
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.TodoPago") as TodoPagoPaymentProcessor;
            if (processor == null || !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                ViewBag.getDescriptionError = "El plugin se encuentra desactivado, actívelo para continuar.";

            Double.TryParse(Nop.Core.NopVersion.CurrentVersion, out version);

            if (version >= 3.8)
                result = View("~/Plugins/Payments.TodoPago/Views/PaymentTodoPago/Configure.cshtml", model);
            else
                result = View("~/Plugins/Payments.TodoPago/Views/PaymentTodoPago/Configure37.cshtml", model);

            return result;
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var todoPagoPaymentSettings = _settingService.LoadSetting<TodoPagoPaymentSettings>(storeScope);

            //save settings
            todoPagoPaymentSettings.Titulo = model.Titulo;
            todoPagoPaymentSettings.Descripcion = model.Descripcion;
            todoPagoPaymentSettings.Ambiente = (Ambiente)model.AmbienteId;
            todoPagoPaymentSettings.Segmento = (Segmento)model.SegmentoId;
            //todoPagoPaymentSettings.DeadLine = model.DeadLine;
            //todoPagoPaymentSettings.Formulario = (Formulario)model.FormularioId;
            todoPagoPaymentSettings.SetCuotas = model.SetCuotas;
            todoPagoPaymentSettings.MaxCuotas = (MaxCuotas)model.MaxCuotasId;

            todoPagoPaymentSettings.SetTimeout = model.SetTimeout;

            if (todoPagoPaymentSettings.SetTimeout)
            {
                if (!String.IsNullOrEmpty(model.Timeout))
                {
                    long n;
                    if (Int64.TryParse(model.Timeout, out n))
                    {
                        // It's a number!
                        todoPagoPaymentSettings.Timeout = model.Timeout;
                    }
                    else
                    {
                        todoPagoPaymentSettings.Timeout = String.Empty;
                        todoPagoPaymentSettings.SetTimeout = false;
                    }
                }
                else
                {
                    todoPagoPaymentSettings.Timeout = String.Empty;
                    todoPagoPaymentSettings.SetTimeout = false;
                }
            }
            else
            {
                todoPagoPaymentSettings.Timeout = String.Empty;
            }

            todoPagoPaymentSettings.Chart = model.Chart;
            todoPagoPaymentSettings.GoogleMaps = model.GoogleMaps;

            todoPagoPaymentSettings.Hibrido = model.Hibrido;

            todoPagoPaymentSettings.User = model.User;
            todoPagoPaymentSettings.Password = model.Password;

            todoPagoPaymentSettings.ApiKeyDeveloper = model.ApiKeyDeveloper;
            todoPagoPaymentSettings.SecurityDeveloper = model.SecurityDeveloper;
            todoPagoPaymentSettings.MerchantDeveloper = model.MerchantDeveloper;

            todoPagoPaymentSettings.ApiKeyProduction = model.ApiKeyProduction;
            todoPagoPaymentSettings.SecurityProduction = model.SecurityProduction;
            todoPagoPaymentSettings.MerchantProduction = model.MerchantProduction;

            todoPagoPaymentSettings.TransaccionIniciada = (OrderStatus)model.TransaccionIniciadaId;
            todoPagoPaymentSettings.TransaccionAprobada = (OrderStatus)model.TransaccionAprobadaId;
            todoPagoPaymentSettings.TransaccionRechazada = (OrderStatus)model.TransaccionRechazadaId;
            todoPagoPaymentSettings.TransaccionOffline = (OrderStatus)model.TransaccionOfflineId;


            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.Titulo_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.Titulo, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.Titulo, storeScope);

            if (model.Descripcion_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.Descripcion, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.Descripcion, storeScope);

            if (model.AmbienteId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.Ambiente, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.Ambiente, storeScope);

            if (model.SegmentoId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.Segmento, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.Segmento, storeScope);

            // if (model.DeadLine_OverrideForStore || storeScope == 0)
            // _settingService.SaveSetting(todoPagoPaymentSettings, x => x.DeadLine, storeScope, false);
            //else if (storeScope > 0)
            //_settingService.DeleteSetting(todoPagoPaymentSettings, x => x.DeadLine, storeScope);

            //if (model.FormularioId_OverrideForStore || storeScope == 0)
            //_settingService.SaveSetting(todoPagoPaymentSettings, x => x.Formulario, storeScope, false);
            //else if (storeScope > 0)
            //_settingService.DeleteSetting(todoPagoPaymentSettings, x => x.Formulario, storeScope);

            if (model.SetCuotas_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.SetCuotas, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.SetCuotas, storeScope);

            if (model.MaxCuotasId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.MaxCuotas, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.MaxCuotas, storeScope);

            if (model.SetTimeout_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.SetTimeout, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.SetTimeout, storeScope);

            if (model.Timeout_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.Timeout, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.Timeout, storeScope);

            if (model.SetTimeout_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.Chart, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.Chart, storeScope);

            if (model.SetTimeout_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.GoogleMaps, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.GoogleMaps, storeScope);

            if (model.SetTimeout_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.Hibrido, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.Hibrido, storeScope);

            if (model.User_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.User, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.User, storeScope);

            if (model.Password_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.Password, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.Password, storeScope);

            if (model.ApiKeyDeveloper_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.ApiKeyDeveloper, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.ApiKeyDeveloper, storeScope);

            if (model.SecurityDeveloper_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.SecurityDeveloper, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.SecurityDeveloper, storeScope);

            if (model.MerchantDeveloper_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.MerchantDeveloper, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.MerchantDeveloper, storeScope);

            if (model.ApiKeyProduction_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.ApiKeyProduction, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.ApiKeyProduction, storeScope);

            if (model.SecurityProduction_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.SecurityProduction, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.SecurityProduction, storeScope);

            if (model.MerchantProduction_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.MerchantProduction, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.MerchantProduction, storeScope);

            if (model.TransaccionIniciadaId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.TransaccionIniciada, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.TransaccionIniciada, storeScope);

            if (model.TransaccionAprobadaId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.TransaccionAprobada, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.TransaccionAprobada, storeScope);

            if (model.TransaccionRechazadaId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.TransaccionRechazada, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.TransaccionRechazada, storeScope);

            if (model.TransaccionOfflineId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(todoPagoPaymentSettings, x => x.TransaccionOffline, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(todoPagoPaymentSettings, x => x.TransaccionOffline, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            //FORMULARIO EXTERNO, REDIRECCION
            ActionResult result = View("~/Plugins/Payments.TodoPago/Views/PaymentTodoPago/PaymentInfo.cshtml");

            return result;
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        [ValidateInput(false)]
        public ActionResult OrderReturn(FormCollection form)
        {
            var answerKey = _webHelper.QueryString<string>("answer");
            var ordenId = _webHelper.QueryString<string>("ordenId");

            int id;
            Int32.TryParse(ordenId, out id);

            String b = _webHelper.GetThisPageUrl(true);

            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.TodoPago") as TodoPagoPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("TodoPago module cannot be loaded");

            var order = _orderService.GetOrderById(id);
            if (order == null || order.Deleted)
            {
                //No order found with the specified id
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            if (order != null)
            {
                Dictionary<string, object> responseGaa = processor.TodoPagoSecondStep(answerKey, order);
                int statusCode = (int)responseGaa[TODOPAGO_STATUS_CODE];
                
                if (statusCode == -1)
                {
                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                }
                else
                {
                    _httpContext.Session["OrderPaymentInfo"] = (string)responseGaa[TODOPAGO_STATUS_MESSAGE];
                    return RedirectToAction("OrderStatusTP", new { id = order.Id });
                }
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetCredentials(string user = "", string password = "", string mode = "")
        {
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.TodoPago") as TodoPagoPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("TodoPago module cannot be loaded");

            return Json(processor.getCredentials(user, password, mode));
        }

        [ValidateInput(false)]
        public ActionResult List(FormCollection form)
        {
            var model = new OrderListModel();

            var pm = _paymentService.LoadPaymentMethodBySystemName("Payments.TodoPago");
            model.AvailablePaymentMethods.Add(new SelectListItem { Text = pm.PluginDescriptor.FriendlyName, Value = pm.PluginDescriptor.SystemName });

            ActionResult result = View("~/Plugins/Payments.TodoPago/Views/PaymentTodoPago/Status.cshtml", model);

            return result;
        }

        [ValidateInput(false)]
        public ActionResult GetStatus(int id)
        {
            Dictionary<string, Object> response = new Dictionary<string, Object>();

            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.TodoPago") as TodoPagoPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("TodoPago module cannot be loaded");

            var order = _orderService.GetOrderById(id);
            if (order == null || order.Deleted)
            {
                //No order found with the specified id
                return RedirectToAction("List");
            }

            ActionResult result = View("~/Plugins/Payments.TodoPago/Views/PaymentTodoPago/StatusPopUp.cshtml", processor.getStatus(order));

            return result;
        }

        [HttpGet]
        public ActionResult OrderStatusTP(int id)
        {
            ActionResult result;

            var model = new OrderStatusTPModel();

            model.ORDERSTATUSID = id;

            if (!_todoPagoPaymentSettings.Chart)
            {
                var order = _orderService.GetOrderById(id);
                if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                    return new HttpUnauthorizedResult();

                _orderProcessingService.ReOrder(order);
            }

            if (_httpContext.Session["OrderPaymentInfo"] != null)
                model.ORDERSTATUSMESSAGE = (string)_httpContext.Session["OrderPaymentInfo"];

            result = View("~/Plugins/Payments.TodoPago/Views/PaymentTodoPago/OrderStatusTP.cshtml", model);

            return result;
        }

        [HttpPost]
        public ActionResult OrderStatusMessage(string message)
        {
            _httpContext.Session["OrderPaymentInfo"] = message;

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpGet]
        public ActionResult HybridForm(int id, string publicRequestKey)
        {
            ActionResult result;
            var model = new HybridModel();
            var order = _orderService.GetOrderById(id);
            string storeLocation;

            model.ID = id;
            model.PUBLICREQUESTKEY = publicRequestKey;
            model.EMAIL = order.BillingAddress.Email;
            model.NOMBRECOMPLETO = order.BillingAddress.FirstName + " " + order.BillingAddress.LastName;

            if (_todoPagoPaymentSettings.Ambiente == Ambiente.Production)
            {
                model.URL_HYBRIDFORM = "https://forms.todopago.com.ar/resources/v2/TPBSAForm.min.js";
            }
            else
            {
                model.URL_HYBRIDFORM = "https://developers.todopago.com.ar/resources/v2/TPBSAForm.min.js";
            }

            if (_webHelper.IsCurrentConnectionSecured())
                storeLocation = _webHelper.GetStoreLocation(true);
            else
                storeLocation = _webHelper.GetStoreLocation(false);

            model.URL_OK = storeLocation + "Plugins/PaymentTodoPago/OrderReturn?ordenId=" + id;
            model.URL_ERROR = storeLocation + "Plugins/PaymentTodoPago/OrderStatusTP/" + id;

            result = View("~/Plugins/Payments.TodoPago/Views/PaymentTodoPago/HybridForm.cshtml", model);

            return result;
        }
    }
}