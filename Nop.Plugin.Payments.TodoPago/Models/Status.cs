using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.TodoPago.Services;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.TodoPago.Models
{
    internal class Status : TodoPagoModel
    {
        public Status(TodoPagoBusinessService todoPagoBusinessService, TodoPagoPaymentSettings _todoPagoPaymentSettings, ILogger _logger) : base(todoPagoBusinessService, _todoPagoPaymentSettings, _logger)
        {
        }

        internal StatusModel GetStatus(Order order)
        {
            Dictionary<string, Object> result = new Dictionary<string, Object>();
            List<Dictionary<string, object>> res = new List<Dictionary<string, object>>();

            res = this.connector.GetStatus(this.merchant, order.Id.ToString());

            for (int i = 0; i < res.Count; i++)
            {
                Dictionary<string, object> dic = res[i];
                foreach (Dictionary<string, object> aux in dic.Values)
                    result = aux;
            }

            _logger.Information("TodoPago ResponseGetStatus : " + todoPagoBusinessService.serealizar(result));

            return PrepareStatusModel(result);
        }

        private StatusModel PrepareStatusModel(Dictionary<string, Object> response)
        {
            StatusModel model = new StatusModel();

            model.RESULTCODE = GetValueByKey(response, "RESULTCODE");
            model.RESULTMESSAGE = GetValueByKey(response, "RESULTMESSAGE");
            model.DATETIME = GetValueByKey(response, "DATETIME");
            model.OPERATIONID = GetValueByKey(response, "OPERATIONID");
            model.CURRENCYCODE = GetValueByKey(response, "CURRENCYCODE");
            model.AMOUNT = GetValueByKey(response, "AMOUNT");
            model.TYPE = GetValueByKey(response, "TYPE");
            model.INSTALLMENTPAYMENTS = GetValueByKey(response, "INSTALLMENTPAYMENTS");
            model.CUSTOMEREMAIL = GetValueByKey(response, "CUSTOMEREMAIL");
            model.IDENTIFICATIONTYPE = GetValueByKey(response, "IDENTIFICATIONTYPE");
            model.IDENTIFICATION = GetValueByKey(response, "IDENTIFICATION");
            model.CARDNUMBER = GetValueByKey(response, "CARDNUMBER");
            model.CARDHOLDERNAME = GetValueByKey(response, "CARDHOLDERNAME");
            model.TICKETNUMBER = GetValueByKey(response, "TICKETNUMBER");
            model.AUTHORIZATIONCODE = GetValueByKey(response, "AUTHORIZATIONCODE");
            model.BARCODE = GetValueByKey(response, "BARCODE");
            model.COUPONEXPDATE = GetValueByKey(response, "COUPONEXPDATE");
            model.COUPONSECEXPDATE = GetValueByKey(response, "COUPONSECEXPDATE");
            model.COUPONSUBSCRIBER = GetValueByKey(response, "COUPONSUBSCRIBER");
            model.BANKID = GetValueByKey(response, "BANKID");
            model.PAYMENTMETHODTYPE = GetValueByKey(response, "PAYMENTMETHODTYPE");
            model.PAYMENTMETHODCODE = GetValueByKey(response, "PAYMENTMETHODCODE");
            model.PROMOTIONID = GetValueByKey(response, "PROMOTIONID");
            model.AMOUNTBUYER = GetValueByKey(response, "AMOUNTBUYER");
            model.PAYMENTMETHODNAME = GetValueByKey(response, "PAYMENTMETHODNAME");
            model.PUSHNOTIFYENDPOINT = GetValueByKey(response, "PUSHNOTIFYENDPOINT");
            model.PUSHNOTIFYMETHOD = GetValueByKey(response, "PUSHNOTIFYMETHOD");
            model.PUSHNOTIFYSTATES = GetValueByKey(response, "PUSHNOTIFYSTATES");
            model.REFUNDED = GetValueByKey(response, "REFUNDED");

            // Nuevos campos del GetStatus
            model.REFUNDS = GetValueArrayByKey(response, "REFUNDS");
            if (model.REFUNDS == "{}") model.REFUNDS = "";
            model.FEEAMOUNT = GetValueByKey(response, "FEEAMOUNT");
            model.TAXAMOUNT = GetValueByKey(response, "TAXAMOUNT");
            model.SERVICECHARGEAMOUNT = GetValueByKey(response, "SERVICECHARGEAMOUNT");
            model.CREDITEDAMOUNT = GetValueByKey(response, "CREDITEDAMOUNT");
            model.FEEAMOUNTBUYER = GetValueByKey(response, "FEEAMOUNTBUYER");
            model.TAXAMOUNTBUYER = GetValueByKey(response, "TAXAMOUNTBUYER");
            model.CREDITEDAMOUNTBUYER = GetValueByKey(response, "CREDITEDAMOUNTBUYER");
            model.ESTADOCONTRACARGO = GetValueByKey(response, "ESTADOCONTRACARGO");
            model.COMISION = GetValueByKey(response, "COMISION");

            model.IDCONTRACARGO = GetValueByKey(response, "IDCONTRACARGO");
            model.FECHANOTIFICACIONCUENTA = GetValueByKey(response, "FECHANOTIFICACIONCUENTA");

            model.TEA = GetValueByKey(response, "TEA");
            model.CFT = GetValueByKey(response, "CFT");

            model.RELEASESTATUS = GetValueByKey(response, "RELEASESTATUS");
            model.RELEASEDATETIME = GetValueByKey(response, "RELEASEDATETIME");
            model.PHONENUMBER = GetValueByKey(response, "PHONENUMBER");
            model.ADDRESS = GetValueByKey(response, "ADDRESS");
            model.POSTALCODE = GetValueByKey(response, "POSTALCODE");
            model.CUSTOMERID = GetValueByKey(response, "CUSTOMERID");
            
            model.ITEMS = GetValueArrayByKey(response, "ITEMS");
            if (model.ITEMS == "{}") model.ITEMS = "";

            //model.PRODUCTCODE = GetValueByKey(response, "PRODUCTCODE");
            //model.PRODUCTDESCRIPTION = GetValueByKey(response, "PRODUCTDESCRIPTION");
            //model.PRODUCTNAME = GetValueByKey(response, "PRODUCTNAME");
            //model.QUANTITY = GetValueByKey(response, "QUANTITY");
            //model.PRODUCTSKU = GetValueByKey(response, "PRODUCTSKU");
            //model.UNITPRICE = GetValueByKey(response, "UNITPRICE");
            //model.TOTALAMOUNT = GetValueByKey(response, "TOTALAMOUNT");

            return model;
        }
    }
}
