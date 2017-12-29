using System;
using Nop.Plugin.Payments.TodoPago.Services;
using Nop.Services.Payments;
using System.Collections.Generic;
using Nop.Services.Logging;
using System.Globalization;
using Nop.Core.Domain.Payments;
using TodoPagoConnector.Utils;

namespace Nop.Plugin.Payments.TodoPago.Models
{
    internal class Refund : TodoPagoModel
    {
        public Refund(TodoPagoBusinessService todoPagoBusinessService, TodoPagoPaymentSettings _todoPagoPaymentSettings, ILogger _logger) : base(todoPagoBusinessService, _todoPagoPaymentSettings, _logger)
        {
        }

        internal RefundPaymentResult ExecuteRefund(RefundPaymentRequest refundPaymentRequest)
        {
            RefundPaymentResult result = new RefundPaymentResult();
            Dictionary<string, Object> responseRefund = new Dictionary<string, Object>();
            Dictionary<string, Object> response = new Dictionary<string, Object>();

            if (refundPaymentRequest.IsPartialRefund)
                responseRefund = ReturnRequest(refundPaymentRequest);
            else
                responseRefund = VoidRequest(refundPaymentRequest);

            if (responseRefund.ContainsKey("VoidResponse"))
                response = (Dictionary<string, Object>)responseRefund["VoidResponse"];

            if (responseRefund.ContainsKey("ReturnResponse"))
                response = (Dictionary<string, Object>)responseRefund["ReturnResponse"];

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
                        result.NewPaymentStatus = PaymentStatus.PartiallyRefunded;
                    else
                        result.NewPaymentStatus = PaymentStatus.Refunded;
                }
            }

            return result;
        }

        private Dictionary<string, Object> VoidRequest(RefundPaymentRequest refundPaymentRequest)
        {
            Dictionary<string, string> refundParams = new Dictionary<string, string>();
            Dictionary<string, Object> responseRefund = new Dictionary<string, Object>();

            refundParams = GenerateVoidRequestParams(refundPaymentRequest.Order.Id);
            _logger.Information("TodoPago ParamsRefund : " + todoPagoBusinessService.serealizar(refundParams));

            responseRefund = this.connector.VoidRequest(refundParams);
            _logger.Information("TodoPago resultRefund : " + todoPagoBusinessService.serealizarRefund(responseRefund));

            return responseRefund;
        }

        private Dictionary<string, Object> ReturnRequest(RefundPaymentRequest refundPaymentRequest)
        {
            Dictionary<string, string> refundParams = new Dictionary<string, string>();
            Dictionary<string, Object> responseRefund = new Dictionary<string, Object>();

            var amountToRefund = (refundPaymentRequest.AmountToRefund / refundPaymentRequest.Order.OrderTotal) * (refundPaymentRequest.Order.OrderTotal - refundPaymentRequest.Order.OrderTax);
            var orderTotal = Math.Round(amountToRefund, 2);
            String amount = orderTotal.ToString("0.00", CultureInfo.InvariantCulture);

            refundParams = GenerateReturnRequestParams(refundPaymentRequest.Order.Id, amount);
            _logger.Information("TodoPago ParamsRefund : " + todoPagoBusinessService.serealizar(refundParams));

            responseRefund = this.connector.ReturnRequest(refundParams);
            _logger.Information("TodoPago resultRefund : " + todoPagoBusinessService.serealizarRefund(responseRefund));

            return responseRefund;
        }

        private Dictionary<string, string> GenerateVoidRequestParams(int orderId)
        {
            Dictionary<string, string> voidRequestParams = new Dictionary<string, string>();
            TodoPagoTransactionDto todoPagoTransactionDto = todoPagoBusinessService.findTodoPagoTransactionRecord(orderId);

            voidRequestParams.Add(ElementNames.SECURITY, this.security);
            voidRequestParams.Add(ElementNames.MERCHANT, this.merchant);
            
            if (todoPagoTransactionDto.requestKey != null)
                voidRequestParams.Add(ElementNames.REQUESTKEY, todoPagoTransactionDto.requestKey);
            else
                voidRequestParams.Add(ElementNames.REQUESTKEY, "");

            return voidRequestParams;
        }

        private Dictionary<string, string> GenerateReturnRequestParams(int orderId, String amount)
        {
            Dictionary<string, string> returnRequestParams = new Dictionary<string, string>();
            TodoPagoTransactionDto todoPagoTransactionDto = todoPagoBusinessService.findTodoPagoTransactionRecord(orderId);

            returnRequestParams.Add(ElementNames.SECURITY, this.security);
            returnRequestParams.Add(ElementNames.MERCHANT, this.merchant);
            returnRequestParams.Add(ElementNames.AMOUNT, amount);
            
            if (todoPagoTransactionDto.requestKey != null)
                returnRequestParams.Add(ElementNames.REQUESTKEY, todoPagoTransactionDto.requestKey);
            else
                returnRequestParams.Add(ElementNames.REQUESTKEY, "");

            return returnRequestParams;
        }
    }
}
