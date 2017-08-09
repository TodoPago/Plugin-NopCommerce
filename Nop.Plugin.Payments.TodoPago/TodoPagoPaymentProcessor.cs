using System;
using System.Collections.Generic;
using System.Globalization;
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
using TodoPagoConnector;
using TodoPagoConnector.Model;
using TodoPagoConnector.Exceptions;
using TodoPagoConnector.Utils;
using Nop.Plugin.Payments.TodoPago.Utils;


//using AuthorizeNetSDK = AuthorizeNet;


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
        private readonly HttpContextBase _httpContext;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;

        //TodoPago
        private TPConnector connector;
        private TodoPagoBusinessService todoPagoBusinessService;
        private String merchant;
        private String security;

        private const string TODOPAGO_SAR_SESSION = "ABCDEF-1234-12221-FDE1-00000200";
        private const string TODOPAGO_SAR_XML = "XML";
        private const string TODOPAGO_SAR_CURRENCY_CODE = "032";
        private const string TODOPAGO_SAR_ARS = "ARS";

        private const string TODOPAGO_STATUS_CODE = "StatusCode";
        private const string TODOPAGO_STATUS_MESSAGE = "StatusMessage";
        private const string TODOPAGO_REQUEST_KEY = "RequestKey";
        private const string TODOPAGO_PUBLIC_REQUEST_KEY = "PublicRequestKey";
        private const string TODOPAGO_URL_REQUEST = "URL_Request";


        public TodoPagoPaymentProcessor(ISettingService settingService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IWebHelper webHelper,
            IOrderTotalCalculationService orderTotalCalculationService,
            IEncryptionService encryptionService,
            CurrencySettings currencySettings,
            TodoPagoPaymentSettings todoPagoPaymentSettings,
            TodoPagoTransactionObjectContex contex,
            HttpContextBase httpContext,
            ILogger logger,
            IOrderService orderService,
            ITodoPagoTransactionService todoPagoTransactionService)
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
            this._httpContext = httpContext;
            this._logger = logger;
            this._orderService = orderService;
            this.todoPagoBusinessService = new TodoPagoBusinessService(todoPagoTransactionService);
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

        private void TodoPagoConnectorPrepare()
        {
            String authorization = string.Empty;
            var headers = new Dictionary<String, String>();

            if (_todoPagoPaymentSettings.Ambiente == Ambiente.Production)
            {
                authorization = _todoPagoPaymentSettings.ApiKeyProduction;
                this.security = _todoPagoPaymentSettings.SecurityProduction;
                this.merchant = _todoPagoPaymentSettings.MerchantProduction;
                headers.Add("Authorization", authorization);
                this.connector = new TPConnector(TPConnector.productionEndpoint, headers);
            }
            else
            {
                authorization = _todoPagoPaymentSettings.ApiKeyDeveloper;
                headers.Add("Authorization", authorization);
                this.security = _todoPagoPaymentSettings.SecurityDeveloper;
                this.merchant = _todoPagoPaymentSettings.MerchantDeveloper;
                this.connector = new TPConnector(TPConnector.developerEndpoint, headers);
            }

        }

        private Dictionary<string, Object> TodoPagoFirstStep(PostProcessPaymentRequest postProcessPaymentRequest)
        {

            String operationId = postProcessPaymentRequest.Order.Id.ToString();

            Dictionary<string, Object> result = new Dictionary<string, Object>();
            Dictionary<string, string> sendAuthorizeRequestParams = generateSendAuthorizeRequestParams(operationId);
            Dictionary<string, string> sendAuthorizeRequestPayload = generateSendAuthorizeRequestPayload(postProcessPaymentRequest);

            String paramSarSereal = todoPagoBusinessService.serealizar(sendAuthorizeRequestParams);
            String paramSarPayLoadSereal = todoPagoBusinessService.serealizar(sendAuthorizeRequestPayload);
            _logger.Information("TodoPago ParamSar : " + paramSarSereal + " " + paramSarPayLoadSereal);

            result = this.connector.SendAuthorizeRequest(sendAuthorizeRequestParams, sendAuthorizeRequestPayload);

            int statusCode = 0;
            if (result.ContainsKey(TODOPAGO_STATUS_CODE))
            {
                statusCode = (int)result[TODOPAGO_STATUS_CODE];
            }
            String statusMessage = getValueByKey(result, TODOPAGO_STATUS_MESSAGE);
            String requestKey = getValueByKey(result, TODOPAGO_REQUEST_KEY);
            String publicRequestKey = getValueByKey(result, TODOPAGO_PUBLIC_REQUEST_KEY);

            String responseSarSereal = todoPagoBusinessService.serealizar(result);
            _logger.Information("TodoPago ResponseSar : " + responseSarSereal);

            TodoPagoTransactionDto todoPagoTransactionDto = new TodoPagoTransactionDto();

            todoPagoTransactionDto.ordenId = postProcessPaymentRequest.Order.Id;
            todoPagoTransactionDto.firstStep = DateTime.Now.ToString();
            todoPagoTransactionDto.paramsSAR = paramSarSereal + " " + paramSarPayLoadSereal;
            todoPagoTransactionDto.responseSAR = responseSarSereal;
            todoPagoTransactionDto.requestKey = requestKey;
            todoPagoTransactionDto.publicRequestKey = publicRequestKey;

            todoPagoBusinessService.insertTodoPagoTransactionRecord(todoPagoTransactionDto);

            return result;
        }

        private Dictionary<string, string> generateSendAuthorizeRequestParams(String operationId)
        {

            Dictionary<string, string> sendAuthorizeRequestParams = new Dictionary<string, string>();

            sendAuthorizeRequestParams.Add(ElementNames.SECURITY, this.security);
            sendAuthorizeRequestParams.Add(ElementNames.SESSION, TODOPAGO_SAR_SESSION);
            sendAuthorizeRequestParams.Add(ElementNames.MERCHANT, this.merchant);

            string URL_OK = String.Empty;
            string URL_ERROR = String.Empty;

            if (_webHelper.IsCurrentConnectionSecured())
            {
                URL_OK = _webHelper.GetStoreLocation(true) + "Plugins/PaymentTodoPago/OrderReturn?ordenId=" + operationId + "&";
                if (_todoPagoPaymentSettings.Chart)
                    URL_ERROR = URL_OK;
                else
                    URL_ERROR = _webHelper.GetStoreLocation(true) + "Plugins/PaymentTodoPago/OrderStatusFailTP/" + operationId + "/Ha ocurrido un error al realizar el pago";
            }
            else
            {
                URL_OK = _webHelper.GetStoreLocation(false) + "Plugins/PaymentTodoPago/OrderReturn?ordenId=" + operationId + "&";

                if (_todoPagoPaymentSettings.Chart)
                    URL_ERROR = URL_OK;
                else
                    URL_ERROR = _webHelper.GetStoreLocation(false) + "Plugins/PaymentTodoPago/OrderStatusFailTP/" + operationId + "/Ha ocurrido un error al realizar el pago";
            }

            sendAuthorizeRequestParams.Add(ElementNames.URL_OK, URL_OK);
            sendAuthorizeRequestParams.Add(ElementNames.URL_ERROR, URL_ERROR);
            sendAuthorizeRequestParams.Add(ElementNames.ENCODING_METHOD, TODOPAGO_SAR_XML);

            return sendAuthorizeRequestParams;
        }


        private Dictionary<string, string> generateSendAuthorizeRequestPayload(PostProcessPaymentRequest postProcessPaymentRequest)
        {

            Dictionary<string, string> payload = new Dictionary<string, string>();

            String operationId = postProcessPaymentRequest.Order.Id.ToString();
            String mailClient = postProcessPaymentRequest.Order.BillingAddress.Email;

            var orderTotal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
            String amount = orderTotal.ToString("0.00", CultureInfo.InvariantCulture);

            payload.Add(ElementNames.MERCHANT, this.merchant);
            payload.Add(ElementNames.OPERATIONID, operationId);
            payload.Add(ElementNames.CURRENCYCODE, TODOPAGO_SAR_CURRENCY_CODE);
            payload.Add(ElementNames.AMOUNT, amount);
            payload.Add(ElementNames.EMAILCLIENTE, mailClient);

            if (_todoPagoPaymentSettings.SetCuotas)
            {
                foreach (int value in Enum.GetValues(typeof(MaxCuotas)))
                {
                    if (((MaxCuotas)value).Equals(_todoPagoPaymentSettings.MaxCuotas))
                    {
                        payload.Add(ElementNames.MAXINSTALLMENTS, value.ToString());
                    }
                }
            }

            if (_todoPagoPaymentSettings.SetTimeout)
            {
                if (!String.IsNullOrEmpty(_todoPagoPaymentSettings.Timeout))
                {
                    long n;

                    if (Int64.TryParse(_todoPagoPaymentSettings.Timeout, out n))
                    {
                        // It's a number!
                        payload.Add(ElementNames.TIMEOUT, _todoPagoPaymentSettings.Timeout);
                    }
                }
            }

            payload = todoPagoBusinessService.completePayLoad(payload, postProcessPaymentRequest);
            return payload;
        }

        private string getValueByKey(Dictionary<string, Object> map, String key)
        {
            String result = String.Empty;

            if (map.ContainsKey(key))
            {
                result = (String)map[key];
            }
            return result;
        }

        public Boolean TodoPagoSecondStep(String answerKey, Order order)
        {

            Boolean aprobada = false;
            Boolean pagosOffline = false;
            string amountBuyer = String.Empty;
            string amount = String.Empty;

            TodoPagoConnectorPrepare();

            Dictionary<string, Object> result = new Dictionary<string, Object>();
            Dictionary<string, string> paramsGAA = generateGAARequestParams(answerKey, order.Id);
            String paramGAASereal = todoPagoBusinessService.serealizar(paramsGAA);
            _logger.Information("TodoPago ParamsGAA : " + paramGAASereal);

            result = this.connector.GetAuthorizeAnswer(paramsGAA);
            String responseGAASereal = todoPagoBusinessService.serealizar(result);
            _logger.Information("TodoPago ResponseGAA :" + responseGAASereal);

            int statusCode = 0;
            if (result.ContainsKey(TODOPAGO_STATUS_CODE))
            {
                statusCode = (int)result[TODOPAGO_STATUS_CODE];

                System.Xml.XmlNode[] aux = (System.Xml.XmlNode[])result["Payload"];

                if (aux != null)
                {
                    for (int i = 0; i < aux.Length; i++)
                    {
                        System.Xml.XmlNodeList inner = aux[i].ChildNodes;
                        for (int j = 0; j < inner.Count; j++)
                        {
                            if (inner.Item(j).Name.Equals("ASSOCIATEDDOCUMENTATION"))
                            {
                                if (!(inner.Item(j).InnerText.Equals(String.Empty))) { pagosOffline = true; }
                            }

                            if (inner.Item(j).Name.Equals("AMOUNTBUYER"))
                                amountBuyer = inner.Item(j).InnerText;

                            if (inner.Item(j).Name.Equals("AMOUNT"))
                                amount = inner.Item(j).InnerText;

                        }
                    }
                }

                if (statusCode == -1)
                {
                    if (pagosOffline)
                    {
                        order.OrderStatus = _todoPagoPaymentSettings.TransaccionOffline;
                        order.PaymentStatus = PaymentStatus.Pending;
                        aprobada = true;
                    }
                    else
                    {
                        order.OrderStatus = _todoPagoPaymentSettings.TransaccionAprobada;
                        order.PaymentStatus = PaymentStatus.Paid;

                        if (!String.IsNullOrEmpty(amountBuyer) && !String.IsNullOrEmpty(amount))
                        {
                            //decimal amountValue = Convert.ToDecimal(amount);
                            decimal amountValue = order.OrderTotal;
                            decimal amountBuyerValue = Convert.ToDecimal(amountBuyer);

                            order.OrderTotal = amountBuyerValue;
                            order.OrderSubtotalInclTax = amountBuyerValue;
                            order.OrderTax += amountBuyerValue - amountValue;
                            //order.PaymentMethodAdditionalFeeInclTax = amountBuyerValue - amountValue;
                            //order.PaymentMethodAdditionalFeeExclTax = amountBuyerValue - amountValue;
                        }

                        aprobada = true;
                    }
                }
                else
                {
                    order.OrderStatus = _todoPagoPaymentSettings.TransaccionRechazada;
                    order.PaymentStatus = PaymentStatus.Pending;
                    aprobada = false;
                }
            }
            order.PaidDateUtc = DateTime.UtcNow;
            _orderService.UpdateOrder(order);

            TodoPagoTransactionDto todoPagoTransactionDto = new TodoPagoTransactionDto();

            todoPagoTransactionDto.ordenId = order.Id;
            todoPagoTransactionDto.secondStep = DateTime.Now.ToString();
            todoPagoTransactionDto.paramsGAA = paramGAASereal;
            todoPagoTransactionDto.responseGAA = responseGAASereal;
            todoPagoTransactionDto.answerKey = answerKey;

            todoPagoBusinessService.updateTodoPagoTransactionRecord(todoPagoTransactionDto);

            return aprobada;

        }


        private Dictionary<string, string> generateGAARequestParams(String answerKey, int orderId)
        {

            Dictionary<string, string> paramsGAA = new Dictionary<string, string>();

            paramsGAA.Add(ElementNames.SECURITY, this.security);
            paramsGAA.Add(ElementNames.SESSION, TODOPAGO_SAR_SESSION);
            paramsGAA.Add(ElementNames.MERCHANT, this.merchant);
            paramsGAA.Add(ElementNames.ANSWERKEY, answerKey);

            TodoPagoTransactionDto todoPagoTransactionDto = todoPagoBusinessService.findTodoPagoTransactionRecord(orderId);

            if (todoPagoTransactionDto.requestKey != null)
                paramsGAA.Add(ElementNames.REQUESTKEY, todoPagoTransactionDto.requestKey);
            else
                paramsGAA.Add(ElementNames.REQUESTKEY, "");

            return paramsGAA;
        }

        public User getCredentials(String user, String password, String ambiente)
        {

            if (ambiente.Equals("dev"))
            {
                this.connector = new TPConnector(TPConnector.developerEndpoint);
            }
            else
            {
                this.connector = new TPConnector(TPConnector.productionEndpoint);
            }

            User userParam = new User(user, password);
            User resultUser = this.connector.getCredentials(userParam);

            return resultUser;
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

            //Instancio el conector
            this.TodoPagoConnectorPrepare();

            //llamo al first Step, hace el sar
            Dictionary<string, Object> responseSar = this.TodoPagoFirstStep(postProcessPaymentRequest);

            if (responseSar.ContainsKey(TODOPAGO_STATUS_CODE))
            {
                int statusCode = (int)responseSar[TODOPAGO_STATUS_CODE];
                String urlRequest = getValueByKey(responseSar, TODOPAGO_URL_REQUEST);

                if (!statusCode.Equals(-1))
                {
                    string message = String.Empty;

                    if (responseSar[TODOPAGO_STATUS_MESSAGE] != null && !String.IsNullOrEmpty(responseSar[TODOPAGO_STATUS_MESSAGE].ToString()))
                        message = responseSar[TODOPAGO_STATUS_MESSAGE].ToString().Replace(".", "");

                    if ((int)responseSar[TODOPAGO_STATUS_CODE] >= 98000 && (int)responseSar[TODOPAGO_STATUS_CODE] < 99000)
                    {
                        ErrorMessageCS dictionary = new ErrorMessageCS();
                        _logger.Information("TodoPago ResponseSar :" + statusCode.ToString() + " " + dictionary.GetErrorInfo((int)responseSar[TODOPAGO_STATUS_CODE]));
                    }

                    postProcessPaymentRequest.Order.OrderStatus = _todoPagoPaymentSettings.TransaccionRechazada;
                    postProcessPaymentRequest.Order.PaymentStatus = PaymentStatus.Pending;

                    // SAR CON ERRORES
                    if (_webHelper.IsCurrentConnectionSecured())
                    {
                        urlRedirect = _webHelper.GetStoreLocation(true);
                    }
                    else
                    {
                        urlRedirect = _webHelper.GetStoreLocation(false);
                    }

                    if (_todoPagoPaymentSettings.Chart)
                    {
                        urlRedirect = urlRedirect + "Plugins/PaymentTodoPago/OrderStatusTP/" + postProcessPaymentRequest.Order.Id.ToString() + "/" + message;
                    }
                    else
                    {
                        urlRedirect = urlRedirect + "Plugins/PaymentTodoPago/OrderStatusFailTP/" + postProcessPaymentRequest.Order.Id.ToString() + "/" + message;
                    }
                    //urlRedirect = urlRedirect + "orderdetails/" + postProcessPaymentRequest.Order.Id.ToString();
                    
                }
                else
                {
                    // SAR BIEN
                    urlRedirect = urlRequest;
                }
            }

            //Se utiliza para redireccionar al formulario de TodoPago
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

            var result = new RefundPaymentResult();

            //Instancio el conector
            this.TodoPagoConnectorPrepare();

            Dictionary<string, string> refundParams = new Dictionary<string, string>();
            Dictionary<string, Object> responseRefund = new Dictionary<string, Object>();
            Dictionary<string, Object> response = new Dictionary<string, Object>();

            if (refundPaymentRequest.IsPartialRefund)
            {
                //String amount = this.GetPesosAmount(refundPaymentRequest.AmountToRefund).ToString("F", new CultureInfo("es-AR"));

                var amountToRefund = (refundPaymentRequest.AmountToRefund / refundPaymentRequest.Order.OrderTotal) * (refundPaymentRequest.Order.OrderTotal - refundPaymentRequest.Order.OrderTax) ;

                var orderTotal = Math.Round(amountToRefund, 2);
                String amount = orderTotal.ToString("0.00", CultureInfo.InvariantCulture);

                refundParams = generateReturnRequestParams(refundPaymentRequest.Order.Id, amount);
                String paramRefund = todoPagoBusinessService.serealizar(refundParams);
                _logger.Information("TodoPago ParamsRefund : " + paramRefund);

                responseRefund = this.connector.ReturnRequest(refundParams);
                String resultRefund = todoPagoBusinessService.serealizarRefund(responseRefund);
                _logger.Information("TodoPago resultRefund : " + resultRefund);

            }
            else
            {
                refundParams = generateVoidRequestParams(refundPaymentRequest.Order.Id);
                String paramRefund = todoPagoBusinessService.serealizar(refundParams);
                _logger.Information("TodoPago ParamsRefund : " + paramRefund);

                responseRefund = this.connector.VoidRequest(refundParams);
                String resultRefund = todoPagoBusinessService.serealizarRefund(responseRefund);
                _logger.Information("TodoPago resultRefund : " + resultRefund);
            }

            if (responseRefund.ContainsKey("VoidResponse"))
            {
                response = (Dictionary<string, Object>)responseRefund["VoidResponse"];
            }

            if (responseRefund.ContainsKey("ReturnResponse"))
            {
                response = (Dictionary<string, Object>)responseRefund["ReturnResponse"];
            }

            if (response.ContainsKey(TODOPAGO_STATUS_CODE))
            {
                System.Int64 statusCode = (System.Int64)response[TODOPAGO_STATUS_CODE];

                if (!statusCode.Equals(2011))
                {
                    // REFUND CON ERRORES
                    String statusMessage = (String)response[TODOPAGO_STATUS_MESSAGE];
                    result.AddError(statusCode + " - " + statusMessage);
                }
                else
                {
                    // REFUND BIEN
                    if (refundPaymentRequest.IsPartialRefund && refundPaymentRequest.Order.RefundedAmount + refundPaymentRequest.AmountToRefund < refundPaymentRequest.Order.OrderTotal)
                    {
                        result.NewPaymentStatus = PaymentStatus.PartiallyRefunded;
                    }
                    else
                    {
                        result.NewPaymentStatus = PaymentStatus.Refunded;
                    }
                }
            }

            return result;
        }

        private Dictionary<string, string> generateVoidRequestParams(int orderId)
        {

            Dictionary<string, string> voidRequestParams = new Dictionary<string, string>();

            voidRequestParams.Add(ElementNames.SECURITY, this.security);
            voidRequestParams.Add(ElementNames.MERCHANT, this.merchant);

            TodoPagoTransactionDto todoPagoTransactionDto = todoPagoBusinessService.findTodoPagoTransactionRecord(orderId);

            if (todoPagoTransactionDto.requestKey != null)
                voidRequestParams.Add(ElementNames.REQUESTKEY, todoPagoTransactionDto.requestKey);
            else
                voidRequestParams.Add(ElementNames.REQUESTKEY, "");

            return voidRequestParams;
        }

        private Dictionary<string, string> generateReturnRequestParams(int orderId, String amount)
        {

            Dictionary<string, string> returnRequestParams = new Dictionary<string, string>();

            returnRequestParams.Add(ElementNames.SECURITY, this.security);
            returnRequestParams.Add(ElementNames.MERCHANT, this.merchant);
            returnRequestParams.Add(ElementNames.AMOUNT, amount);

            TodoPagoTransactionDto todoPagoTransactionDto = todoPagoBusinessService.findTodoPagoTransactionRecord(orderId);
            if (todoPagoTransactionDto.requestKey != null)
                returnRequestParams.Add(ElementNames.REQUESTKEY, todoPagoTransactionDto.requestKey);
            else
                returnRequestParams.Add(ElementNames.REQUESTKEY, "");

            return returnRequestParams;
        }

        private decimal GetPesosAmount(decimal amount)
        {
            Currency pesos = _currencyService.GetCurrencyByCode("ARG");
            pesos = _currencyService.GetCurrencyByCode("AR");

            if (pesos == null)
            {
                pesos = new Currency();
                pesos.CurrencyCode = "32";
            }
            // throw new Exception("Pesos currency cannot be loaded");

            return _currencyService.ConvertFromPrimaryStoreCurrency(amount, pesos);
        }

        public Dictionary<string, Object> getStatus(Order order)
        {

            TodoPagoConnectorPrepare();

            Dictionary<string, Object> result = new Dictionary<string, Object>();
            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();

            res = this.connector.GetStatus(this.merchant, order.Id.ToString());

            for (int i = 0; i < res.Count; i++)
            {
                Dictionary<string, object> dic = res[i];
                foreach (Dictionary<string, object> aux in dic.Values)
                {
                    //foreach (string k in aux.Keys) {
                    //    if (aux[k].GetType().IsInstanceOfType(aux)) {
                    //        Dictionary<string, object> a = (Dictionary<string, object>)aux[k];
                    //        Console.WriteLine("- " + k + ": ");
                    //        foreach (Dictionary<string, object> aux2 in a.Values) {
                    //            Console.WriteLine("- REFUND: ");
                    //            foreach (string b in aux2.Keys) {
                    //                Console.WriteLine("- " + b + ": " + aux2[b]);
                    //            }
                    //        }
                    //    } else {
                    //        Console.WriteLine("- " + k + ": " + aux[k]);
                    //    }
                    //}

                    result = aux;

                }
            }

            String responseGetStatus = todoPagoBusinessService.serealizar(result);
            _logger.Information("TodoPago ResponseGetStatus : " + responseGetStatus);

            return result;

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

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.TodoPago.Fields.RedirectionTip", "TodoPago va a redireccionar al formulario de pago.");

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
