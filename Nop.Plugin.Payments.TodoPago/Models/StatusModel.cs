using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Nop.Plugin.Payments.TodoPago.Models{

    public class StatusModel : BaseNopModel{

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.ResultCode")]
        public string RESULTCODE { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.DateTime")]
        public string DATETIME { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.OperationId")]
        public string OPERATIONID { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.CurrencyCode")]
        public string CURRENCYCODE { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Amount")]
        public string AMOUNT { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Type")]
        public string TYPE { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.InstallmentPayments")]
        public string INSTALLMENTPAYMENTS { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.CustomerEmail")]
        public string CUSTOMEREMAIL { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.IdentificatioType")]
        public string IDENTIFICATIONTYPE { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Identification")]
        public string IDENTIFICATION { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.CardNumber")]
        public string CARDNUMBER { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.CardHolderName")]
        public string CARDHOLDERNAME { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.TicketNumber")]
        public string TICKETNUMBER { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.AuthorizationCode")]
        public string AUTHORIZATIONCODE { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.BarCode")]
        public string BARCODE { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.CouponExpDate")]
        public string COUPONEXPDATE { get; set; }
       
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.CouponSecexpDate")]
        public string COUPONSECEXPDATE { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.CouponSubscriber")]
        public string COUPONSUBSCRIBER { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.BankId")]
        public string BANKID { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PaymentMethodType")]
        public string PAYMENTMETHODTYPE { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PaymentMethodCode")]
        public string PAYMENTMETHODCODE { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PromotionId")]
        public string PROMOTIONID { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.AmountBuyer")]
        public string AMOUNTBUYER { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PaymentMethodName")]
        public string PAYMENTMETHODNAME { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PushNotifyEndpoint")]
        public string PUSHNOTIFYENDPOINT { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PushNotifyMethod")]
        public string PUSHNOTIFYMETHOD { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PushNotifyStates")]
        public string PUSHNOTIFYSTATES { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Refunded")]
        public string REFUNDED { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.Refunds")]
        public Dictionary<string, object> REFUNDS { get; set; }

        }
    }