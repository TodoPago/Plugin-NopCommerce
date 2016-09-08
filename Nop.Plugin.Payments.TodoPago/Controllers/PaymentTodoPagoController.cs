using System;
using System.Collections.Generic;
using System.Linq;
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
using TodoPagoConnector.Model;
using TodoPagoConnector.Exceptions;
using Nop.Plugin.Payments.TodoPago.Utils;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.TodoPago.Controllers
{
    public class PaymentTodoPagoController : BasePaymentController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IPaymentService _paymentService;
        private readonly PaymentSettings _paymentSettings;


        public PaymentTodoPagoController(ILocalizationService localizationService,
            ISettingService settingService,
            IStoreService storeService,
            IWorkContext workContext,
            IPaymentService paymentService,
            IOrderService orderService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            PaymentSettings paymentSettings

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
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure() {
            //load settings for a chosen store scope
            var storeScope = GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var todoPagoPaymentSettings = _settingService.LoadSetting<TodoPagoPaymentSettings>(storeScope);
            ActionResult result;
            double version;

            var model = new ConfigurationModel {

                //Configuracion general
                Titulo = todoPagoPaymentSettings.Titulo,
                Descripcion = todoPagoPaymentSettings.Descripcion,
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

            if (storeScope > 0) {

                //Configuracion general
                model.Titulo_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Titulo, storeScope);
                model.Descripcion_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Descripcion, storeScope);
                model.AmbienteId_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Ambiente, storeScope);
                model.SegmentoId_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Segmento, storeScope);
                //model.DeadLine_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.DeadLine, storeScope);
                //model.FormularioId_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.Formulario, storeScope);
                model.SetCuotas_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.SetCuotas, storeScope);
                model.MaxCuotasId_OverrideForStore = _settingService.SettingExists(todoPagoPaymentSettings, x => x.MaxCuotas, storeScope);
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
        public ActionResult PaymentInfo() {

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
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form) {
            var paymentInfo = new ProcessPaymentRequest();
            paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        [ValidateInput(false)]
        public ActionResult OrderReturn(FormCollection form) {

            var answerKey = _webHelper.QueryString<string>("answer");
            var ordenId = _webHelper.QueryString<string>("ordenId");

            int id;
            Int32.TryParse(ordenId, out id);

            String b = _webHelper.GetThisPageUrl(true);

            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.TodoPago") as TodoPagoPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("TodoPago module cannot be loaded");

            //var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
            //            customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();

            var order = _orderService.GetOrderById(id);
            if (order == null || order.Deleted){
                //No order found with the specified id
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            if (order != null) {
                Boolean aprobada = processor.TodoPagoSecondStep(answerKey, order);

                if (aprobada) {
                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                } else {
                    return RedirectToRoute("OrderDetails", new { orderId = order.Id });
                }
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetCredentials(string user = "", string password = "", string mode = "") {

            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.TodoPago") as TodoPagoPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("TodoPago module cannot be loaded");

            String security = String.Empty;
            String message = String.Empty;
            User resultUser = new User();
            Boolean success = false;

            try {
                if (processor != null) {
                    resultUser = processor.getCredentials(user, password, mode);
                }

                string[] securityD = resultUser.getApiKey().Split(' ');
                security = securityD[1];
                success = true;

            } catch (ResponseException ex) {
                success = false;
                message = ex.Message;
            } 
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new{
                            success = success,
                            message = message,
                            merchandid = resultUser.getMerchant(),
                            apikey = resultUser.getApiKey(),
                            security = security
            });
        }

        [ValidateInput(false)]
        public ActionResult List(FormCollection form) {

            var model = new OrderListModel();

            var pm = _paymentService.LoadPaymentMethodBySystemName("Payments.TodoPago");
            model.AvailablePaymentMethods.Add(new SelectListItem { Text = pm.PluginDescriptor.FriendlyName, Value = pm.PluginDescriptor.SystemName });

            ActionResult result = View("~/Plugins/Payments.TodoPago/Views/PaymentTodoPago/Status.cshtml", model);

            return result;
        }

        [ValidateInput(false)]
        public ActionResult GetStatus(int id) {

            Dictionary<string, Object> response = new Dictionary<string, Object>();

            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.TodoPago") as TodoPagoPaymentProcessor;
            if (processor == null ||
                !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("TodoPago module cannot be loaded");


            var order = _orderService.GetOrderById(id);
            if (order == null || order.Deleted){
                //No order found with the specified id
                return RedirectToAction("List");
            }

            if (processor != null) {
                    response = processor.getStatus(order);
            }

            var model = new StatusModel();
            model = prepareStatusModel(model, response);

            ActionResult result = View("~/Plugins/Payments.TodoPago/Views/PaymentTodoPago/StatusPopUp.cshtml", model);

            return result;
        }

        private StatusModel prepareStatusModel(StatusModel model, Dictionary<string, Object> response)  {

            model.RESULTCODE = getValueByKey(response, "RESULTCODE");
            model.DATETIME = getValueByKey(response, "DATETIME");
            model.OPERATIONID = getValueByKey(response, "OPERATIONID");
            model.CURRENCYCODE = getValueByKey(response, "CURRENCYCODE");
            model.AMOUNT = getValueByKey(response, "AMOUNT");
            model.TYPE = getValueByKey(response, "TYPE");
            model.INSTALLMENTPAYMENTS = getValueByKey(response, "INSTALLMENTPAYMENTS");
            model.CUSTOMEREMAIL = getValueByKey(response, "CUSTOMEREMAIL");
            model.IDENTIFICATIONTYPE = getValueByKey(response, "IDENTIFICATIONTYPE");
            model.IDENTIFICATION = getValueByKey(response, "IDENTIFICATION");
            model.CARDNUMBER = getValueByKey(response, "CARDNUMBER");
            model.CARDHOLDERNAME = getValueByKey(response, "CARDHOLDERNAME");
            model.TICKETNUMBER = getValueByKey(response, "TICKETNUMBER");
            model.AUTHORIZATIONCODE = getValueByKey(response, "AUTHORIZATIONCODE");
            model.BARCODE = getValueByKey(response, "BARCODE");
            model.COUPONEXPDATE = getValueByKey(response, "COUPONEXPDATE");
            model.COUPONSECEXPDATE = getValueByKey(response, "COUPONSECEXPDATE");
            model.COUPONSUBSCRIBER = getValueByKey(response, "COUPONSUBSCRIBER");
            model.BANKID = getValueByKey(response, "BANKID");
            model.PAYMENTMETHODTYPE = getValueByKey(response, "PAYMENTMETHODTYPE");
            model.PAYMENTMETHODCODE = getValueByKey(response, "PAYMENTMETHODCODE");
            model.PROMOTIONID = getValueByKey(response, "PROMOTIONID");
            model.AMOUNTBUYER = getValueByKey(response, "AMOUNTBUYER");
            model.PAYMENTMETHODNAME = getValueByKey(response, "PAYMENTMETHODNAME");
            model.PUSHNOTIFYENDPOINT = getValueByKey(response, "PUSHNOTIFYENDPOINT");
            model.PUSHNOTIFYMETHOD = getValueByKey(response, "PUSHNOTIFYMETHOD");
            model.PUSHNOTIFYSTATES = getValueByKey(response, "PUSHNOTIFYSTATES");
            model.REFUNDED = getValueByKey(response, "REFUNDED");

            return model;
        }

        private string getValueByKey(Dictionary<string, Object> map, String key){
            String result = String.Empty;

            if (map.ContainsKey(key)){
                result = (String)map[key];
            }
            return result;
        }
    }
}