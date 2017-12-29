using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Nop.Services.Payments;
using Nop.Plugin.Payments.TodoPago.Utils;
using Nop.Services.Logging;
using TodoPagoConnector;
using Nop.Core.Domain.Payments;
using Nop.Core;
using Nop.Plugin.Payments.TodoPago.Services;
using System.Security.Cryptography;
using TodoPagoConnector.Utils;
using System.Globalization;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;

namespace Nop.Plugin.Payments.TodoPago.Models
{
    internal class Payment : TodoPagoModel
    {
        private IWebHelper _webHelper;
        private IOrderService _orderService;
        private HttpContextBase _httpContext;

        private const string TODOPAGO_SAR_SESSION = "ABCDEF-1234-12221-FDE1-00000200";
        private const string TODOPAGO_SAR_XML = "XML";
        private const string TODOPAGO_SAR_CURRENCY_CODE = "032";
        private const string TODOPAGO_SAR_ARS = "ARS";

        protected TodoPagoAddressBookDto addressBillingRecord = null;
        protected TodoPagoAddressBookDto addressShippingRecord = null;
        protected string addressBillingHash = null;
        protected string addressShippingHash = null;

        public Payment(TodoPagoPaymentSettings _todoPagoPaymentSettings, ILogger _logger, IWebHelper _webHelper, TodoPagoBusinessService todoPagoBusinessService, HttpContextBase _httpContext) : base(todoPagoBusinessService, _todoPagoPaymentSettings, _logger)
        {
            this._webHelper = _webHelper;
            this._httpContext = _httpContext;
        }

        public Payment(TodoPagoPaymentSettings _todoPagoPaymentSettings, ILogger _logger, IWebHelper _webHelper, TodoPagoBusinessService todoPagoBusinessService, HttpContextBase _httpContext, IOrderService _orderService) : this(_todoPagoPaymentSettings, _logger, _webHelper, todoPagoBusinessService, _httpContext)
        {
            this._orderService = _orderService;
        }

        internal string SendAuthorizeRequest(PostProcessPaymentRequest postProcessPaymentRequest, string pluginVersion)
        {
            Dictionary<string, Object> responseSar = new Dictionary<string, Object>();
            Dictionary<string, string> sendAuthorizeRequestParams = GenerateSendAuthorizeRequestParams(postProcessPaymentRequest.Order.Id.ToString());
            Dictionary<string, string> sendAuthorizeRequestPayload = GenerateSendAuthorizeRequestPayload(postProcessPaymentRequest, pluginVersion);

            bool addressHasBeenUpdated = false;
            string paramSarSereal, paramSarPayLoadSereal;
            bool googleMapsIsActive = _todoPagoPaymentSettings.GoogleMaps;

            if (googleMapsIsActive) //Activado Google
                addressHasBeenUpdated = SetGoogleMapsData(sendAuthorizeRequestPayload);

            paramSarSereal = todoPagoBusinessService.serealizar(sendAuthorizeRequestParams);
            paramSarPayLoadSereal = todoPagoBusinessService.serealizar(sendAuthorizeRequestPayload);
            _logger.Information("TodoPago ParamSar : " + paramSarSereal + " " + paramSarPayLoadSereal);
            
            responseSar = this.connector.SendAuthorizeRequest(sendAuthorizeRequestParams, sendAuthorizeRequestPayload);
            
            SaveSendAuthorizeRequest(responseSar, postProcessPaymentRequest.Order.Id, paramSarSereal, paramSarPayLoadSereal);

            if (googleMapsIsActive && addressHasBeenUpdated) //Activado Google
                UpdateGoogleMapsData();

            return GetSarRedirectURL(postProcessPaymentRequest, responseSar);
        }

        internal Dictionary<string, object> GetAuthorizeAnswer(string answerKey, Order order)
        {
            Dictionary<string, Object> responseGaa = new Dictionary<string, Object>();
            Dictionary<string, string> paramsGAA = GenerateGAARequestParams(answerKey, order.Id);

            bool aprobada = false;
            string paramGAASereal, responseGAASereal;

            paramGAASereal = todoPagoBusinessService.serealizar(paramsGAA);
            _logger.Information("TodoPago ParamsGAA : " + paramGAASereal);

            responseGaa = this.connector.GetAuthorizeAnswer(paramsGAA);

            responseGAASereal = todoPagoBusinessService.serealizarGAA(responseGaa);
            _logger.Information("TodoPago ResponseGAA :" + responseGAASereal);

            aprobada = UpdateOrderInformation(order, responseGaa);

            SaveGetAuthorizeAnswer(order.Id, answerKey, paramGAASereal, responseGAASereal);
            
            return responseGaa;
        }
        
        #region SendAuthorizeRequest
        private Dictionary<string, string> GenerateSendAuthorizeRequestParams(String operationId)
        {
            Dictionary<string, string> sendAuthorizeRequestParams = new Dictionary<string, string>();
            string URL_OK = String.Empty;
            string URL_ERROR = String.Empty;
            string storeLocation;

            if (_webHelper.IsCurrentConnectionSecured())
                storeLocation = _webHelper.GetStoreLocation(true);
            else
                storeLocation = _webHelper.GetStoreLocation(false);

            sendAuthorizeRequestParams.Add(ElementNames.SECURITY, this.security);
            sendAuthorizeRequestParams.Add(ElementNames.SESSION, TODOPAGO_SAR_SESSION);
            sendAuthorizeRequestParams.Add(ElementNames.MERCHANT, this.merchant);
            
            URL_OK = storeLocation + "Plugins/PaymentTodoPago/OrderReturn?ordenId=" + operationId + "&";

            //_httpContext.Session["OrderPaymentInfo"] = "Ha ocurrido un error al realizar el pago";
            //URL_ERROR = storeLocation + "Plugins/PaymentTodoPago/OrderStatusTP/" + operationId;
            URL_ERROR = URL_OK;

            sendAuthorizeRequestParams.Add(ElementNames.URL_OK, URL_OK);
            sendAuthorizeRequestParams.Add(ElementNames.URL_ERROR, URL_ERROR);
            sendAuthorizeRequestParams.Add(ElementNames.ENCODING_METHOD, TODOPAGO_SAR_XML);

            return sendAuthorizeRequestParams;
        }

        private Dictionary<string, string> GenerateSendAuthorizeRequestPayload(PostProcessPaymentRequest postProcessPaymentRequest, string pluginVersion)
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();
            String operationId = postProcessPaymentRequest.Order.Id.ToString();
            String mailClient = postProcessPaymentRequest.Order.BillingAddress.Email;
            long n;
            var orderTotal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
            String amount = orderTotal.ToString("0.00", CultureInfo.InvariantCulture);

            payload.Add(ElementNames.MERCHANT, this.merchant);
            payload.Add(ElementNames.OPERATIONID, operationId);
            payload.Add(ElementNames.CURRENCYCODE, TODOPAGO_SAR_CURRENCY_CODE);
            payload.Add(ElementNames.AMOUNT, amount);
            payload.Add(ElementNames.EMAILCLIENTE, mailClient);

            if (_todoPagoPaymentSettings.SetCuotas)
                foreach (int value in Enum.GetValues(typeof(MaxCuotas)))
                    if (((MaxCuotas)value).Equals(_todoPagoPaymentSettings.MaxCuotas))
                        payload.Add(ElementNames.MAXINSTALLMENTS, value.ToString());

            if (_todoPagoPaymentSettings.SetTimeout)
                if (!String.IsNullOrEmpty(_todoPagoPaymentSettings.Timeout))
                    if (Int64.TryParse(_todoPagoPaymentSettings.Timeout, out n))
                        payload.Add(ElementNames.TIMEOUT, _todoPagoPaymentSettings.Timeout);

            AddEcommerceDataToPayload(payload, pluginVersion);

            payload = todoPagoBusinessService.completePayLoad(payload, postProcessPaymentRequest);

            return payload;
        }

        private void AddEcommerceDataToPayload(Dictionary<string, string> payload, string pluginVersion)
        {
            payload.Add("ECOMMERCENAME", "NOPCOMMERCE");
            payload.Add("ECOMMERCEVERSION", Nop.Core.NopVersion.CurrentVersion);

            if(_todoPagoPaymentSettings.Hibrido)
                payload.Add("PLUGINVERSION", pluginVersion + "-H");
            else
                payload.Add("PLUGINVERSION", pluginVersion + "-E");
        }

        private bool SetGoogleMapsData(Dictionary<string, string> sendAuthorizeRequestPayload)
        {
            bool updateAddress = false;
            string sourceBilling = sendAuthorizeRequestPayload["CSBTSTREET1"] + sendAuthorizeRequestPayload["CSBTCITY"] + sendAuthorizeRequestPayload["CSBTSTATE"] + sendAuthorizeRequestPayload["CSBTCOUNTRY"];
            string sourceShipping = sendAuthorizeRequestPayload["CSSTSTREET1"] + sendAuthorizeRequestPayload["CSSTCITY"] + sendAuthorizeRequestPayload["CSSTSTATE"] + sendAuthorizeRequestPayload["CSSTCOUNTRY"];

            addressBillingHash = GetMd5Hash(sourceBilling);
            addressShippingHash = GetMd5Hash(sourceShipping);

            addressBillingRecord = todoPagoBusinessService.findTodoPagoAddressBookRecord(addressBillingHash);
            addressShippingRecord = todoPagoBusinessService.findTodoPagoAddressBookRecord(addressShippingHash);

            if ((addressBillingRecord == null || addressShippingRecord == null) || (addressBillingRecord.hash != addressBillingHash || addressShippingRecord.hash != addressShippingHash))
            {
                updateAddress = true;
                connector.SetGoogleClient(new Google());
            }

            if (addressBillingRecord.hash == addressBillingHash)
                SetGoogleDataPayload(sendAuthorizeRequestPayload, addressShippingRecord, true);

            if (addressShippingRecord.hash == addressShippingHash)
                SetGoogleDataPayload(sendAuthorizeRequestPayload, addressShippingRecord, false);

            return updateAddress;
        }

        private void SetGoogleDataPayload(Dictionary<string, string> SendAuthorizeRequestPayload, TodoPagoAddressBookDto addressRecord, bool isBilling)
        {
            if (isBilling)
            {
                SendAuthorizeRequestPayload["CSBTSTREET1"] = addressRecord.street;
                SendAuthorizeRequestPayload["CSBTCITY"] = addressRecord.city;
                SendAuthorizeRequestPayload["CSBTSTATE"] = addressRecord.state;
                SendAuthorizeRequestPayload["CSBTCOUNTRY"] = addressRecord.country;
                SendAuthorizeRequestPayload["CSBTPOSTALCODE"] = addressRecord.postal;
            }
            else
            {
                SendAuthorizeRequestPayload["CSSTSTREET1"] = addressRecord.street;
                SendAuthorizeRequestPayload["CSSTCITY"] = addressRecord.city;
                SendAuthorizeRequestPayload["CSSTSTATE"] = addressRecord.state;
                SendAuthorizeRequestPayload["CSSTCOUNTRY"] = addressRecord.country;
                SendAuthorizeRequestPayload["CSSTPOSTALCODE"] = addressRecord.postal;
            }
        }

        private void SaveSendAuthorizeRequest(Dictionary<string, object> result, int orderId, string paramSarSereal, string paramSarPayLoadSereal)
        {
            TodoPagoTransactionDto todoPagoTransactionDto = new TodoPagoTransactionDto();
            string requestKey = GetValueByKey(result, TODOPAGO_REQUEST_KEY);
            string publicRequestKey = GetValueByKey(result, TODOPAGO_PUBLIC_REQUEST_KEY);
            string responseSarSereal = todoPagoBusinessService.serealizar(result);
            _logger.Information("TodoPago ResponseSar : " + responseSarSereal);

            todoPagoTransactionDto.ordenId = orderId;
            todoPagoTransactionDto.paramsSAR = paramSarSereal + " " + paramSarPayLoadSereal;
            todoPagoTransactionDto.firstStep = DateTime.Now.ToString();
            todoPagoTransactionDto.responseSAR = responseSarSereal;
            todoPagoTransactionDto.requestKey = requestKey;
            todoPagoTransactionDto.publicRequestKey = publicRequestKey;

            todoPagoBusinessService.insertTodoPagoTransactionRecord(todoPagoTransactionDto);
        }

        private void UpdateGoogleMapsData()
        {
            Google clientGoogle = connector.GetGoogleClient();

            if (clientGoogle != null)
            {
                if (addressBillingRecord == null || addressBillingRecord.hash != addressBillingHash)
                {
                    TodoPagoAddressBookDto todoPagoBillingDto = new TodoPagoAddressBookDto();
                    todoPagoBillingDto.hash = addressBillingHash;
                    todoPagoBillingDto.street = clientGoogle.GetFinalAddress()["billing"].Street;
                    todoPagoBillingDto.state = clientGoogle.GetFinalAddress()["billing"].State;
                    todoPagoBillingDto.city = clientGoogle.GetFinalAddress()["billing"].City;
                    todoPagoBillingDto.country = clientGoogle.GetFinalAddress()["billing"].Country;
                    todoPagoBillingDto.postal = clientGoogle.GetFinalAddress()["billing"].PostalCode;
                    todoPagoBusinessService.insertTodoPagoAddressBookRecord(todoPagoBillingDto);
                }

                if (addressBillingHash != addressShippingHash)
                {
                    if (addressShippingRecord == null || addressShippingRecord.hash != addressShippingHash)
                    {
                        TodoPagoAddressBookDto todoPagoShippingDto = new TodoPagoAddressBookDto();
                        todoPagoShippingDto.hash = addressShippingHash;
                        todoPagoShippingDto.street = clientGoogle.GetFinalAddress()["shipping"].Street;
                        todoPagoShippingDto.state = clientGoogle.GetFinalAddress()["shipping"].State;
                        todoPagoShippingDto.city = clientGoogle.GetFinalAddress()["shipping"].City;
                        todoPagoShippingDto.country = clientGoogle.GetFinalAddress()["shipping"].Country;
                        todoPagoShippingDto.postal = clientGoogle.GetFinalAddress()["shipping"].PostalCode;
                        todoPagoBusinessService.insertTodoPagoAddressBookRecord(todoPagoShippingDto);
                    }
                }
            }
        }

        private string GetSarRedirectURL(PostProcessPaymentRequest postProcessPaymentRequest, Dictionary<string, object> result)
        {
            string urlRedirect = String.Empty;

            if (_webHelper.IsCurrentConnectionSecured())
                urlRedirect = _webHelper.GetStoreLocation(true);
            else
                urlRedirect = _webHelper.GetStoreLocation(false);

            if (result.ContainsKey(TODOPAGO_STATUS_CODE))
            {
                int statusCode = (int)result[TODOPAGO_STATUS_CODE];
                String urlRequest = GetValueByKey(result, TODOPAGO_URL_REQUEST);

                if (!statusCode.Equals(-1))
                {
                    string message = String.Empty;

                    if (result[TODOPAGO_STATUS_MESSAGE] != null && !String.IsNullOrEmpty(result[TODOPAGO_STATUS_MESSAGE].ToString()))
                        message = result[TODOPAGO_STATUS_MESSAGE].ToString().Replace(".", "");

                    if ((int)result[TODOPAGO_STATUS_CODE] >= 98000 && (int)result[TODOPAGO_STATUS_CODE] < 99000)
                    {
                        ErrorMessageCS dictionary = new ErrorMessageCS();
                        _logger.Information("TodoPago ResponseSar :" + statusCode.ToString() + " " + dictionary.GetErrorInfo((int)result[TODOPAGO_STATUS_CODE]));
                    }

                    postProcessPaymentRequest.Order.OrderStatus = _todoPagoPaymentSettings.TransaccionRechazada;
                    postProcessPaymentRequest.Order.PaymentStatus = PaymentStatus.Pending;

                    // SAR CON ERRORES
                    _httpContext.Session["OrderPaymentInfo"] = message;
                    urlRedirect = urlRedirect + "Plugins/PaymentTodoPago/OrderStatusTP/" + postProcessPaymentRequest.Order.Id.ToString();
                }
                else
                {
                    // SAR BIEN
                    if (_todoPagoPaymentSettings.Hibrido)
                        urlRedirect = urlRedirect + "Plugins/PaymentTodoPago/HybridForm/" + postProcessPaymentRequest.Order.Id.ToString() + "/" + result[TODOPAGO_PUBLIC_REQUEST_KEY];
                    else
                        urlRedirect = urlRequest;
                }
            }

            return urlRedirect;
        }

        private string GetMd5Hash(string input)
        {
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        #endregion

        #region GetAuthorizeAnswer
        private Dictionary<string, string> GenerateGAARequestParams(String answerKey, int orderId)
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

        private void SaveGetAuthorizeAnswer(int orderId, string answerKey, string paramGAASereal, string responseGAASereal)
        {
            TodoPagoTransactionDto todoPagoTransactionDto = new TodoPagoTransactionDto();

            todoPagoTransactionDto.ordenId = orderId;
            todoPagoTransactionDto.secondStep = DateTime.Now.ToString();
            todoPagoTransactionDto.paramsGAA = paramGAASereal;
            todoPagoTransactionDto.responseGAA = responseGAASereal;
            todoPagoTransactionDto.answerKey = answerKey;

            todoPagoBusinessService.updateTodoPagoTransactionRecord(todoPagoTransactionDto);
        }

        private bool UpdateOrderInformation(Order order, Dictionary<string, object> responseGaa)
        {
            int statusCode = 0;
            Boolean pagosOffline = false;
            string amountBuyer = String.Empty;
            string amount = String.Empty;
            Boolean aprobada = false;

            order.PaidDateUtc = DateTime.UtcNow;

            if (responseGaa.ContainsKey(TODOPAGO_STATUS_CODE))
            {
                statusCode = (int)responseGaa[TODOPAGO_STATUS_CODE];

                System.Xml.XmlNode[] aux = (System.Xml.XmlNode[])responseGaa["Payload"];

                if (aux != null)
                {
                    for (int i = 0; i < aux.Length; i++)
                    {
                        System.Xml.XmlNodeList inner = aux[i].ChildNodes;
                        for (int j = 0; j < inner.Count; j++)
                        {
                            if (inner.Item(j).Name.Equals("ASSOCIATEDDOCUMENTATION"))
                                if (!(inner.Item(j).InnerText.Equals(String.Empty)))
                                    pagosOffline = true;

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
                            decimal amountValue = order.OrderTotal;
                            decimal amountBuyerValue = Convert.ToDecimal(amountBuyer);

                            order.OrderTotal = amountBuyerValue;
                            order.OrderSubtotalInclTax = amountBuyerValue;
                            order.OrderTax += amountBuyerValue - amountValue;
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

            _orderService.UpdateOrder(order);

            return aprobada;
        }
        #endregion
    }
}
