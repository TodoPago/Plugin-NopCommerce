using Nop.Plugin.Payments.TodoPago.Services;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using TodoPagoConnector;

namespace Nop.Plugin.Payments.TodoPago.Models
{
    internal abstract class TodoPagoModel
    {
        protected const string TODOPAGO_STATUS_CODE = "StatusCode";
        protected const string TODOPAGO_URL_REQUEST = "URL_Request";
        protected const string TODOPAGO_STATUS_MESSAGE = "StatusMessage";
        protected const string TODOPAGO_REQUEST_KEY = "RequestKey";
        protected const string TODOPAGO_PUBLIC_REQUEST_KEY = "PublicRequestKey";

        protected TPConnector connector;
        protected TodoPagoBusinessService todoPagoBusinessService;
        protected ILogger _logger;
        protected TodoPagoPaymentSettings _todoPagoPaymentSettings;
        protected string merchant;
        protected string security;

        public TodoPagoModel(TodoPagoBusinessService todoPagoBusinessService, TodoPagoPaymentSettings _todoPagoPaymentSettings, ILogger _logger)
        {
            this.todoPagoBusinessService = todoPagoBusinessService;
            this._logger = _logger;
            this._todoPagoPaymentSettings = _todoPagoPaymentSettings;

            TodoPagoConnectorPrepare();
        }

        protected string GetValueArrayByKey(Dictionary<string, Object> map, String key)
        {
            String result = String.Empty;

            if (map.ContainsKey(key))
            {
                result = Newtonsoft.Json.JsonConvert.SerializeObject(map[key], Newtonsoft.Json.Formatting.Indented);
            }

            return result;
        }

        protected string GetValueByKey(Dictionary<string, Object> map, String key)
        {
            String result = String.Empty;

            if (map.ContainsKey(key))
            {
                result = (String)map[key];
            }
            return result;
        }

        protected void TodoPagoConnectorPrepare()
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
    }
}
