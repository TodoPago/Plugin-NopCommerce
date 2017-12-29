using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.TodoPago.Models
{
    public class StatusModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.ResultCode")]
        public string RESULTCODE { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.ResultMessage")]
        public string RESULTMESSAGE { get; set; }

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

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.IdentificationType")]
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

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Refunds")]
        public string REFUNDS { get; set; }

        //Nuevos campos del GetStatus
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.FeeAmount")]
        public string FEEAMOUNT { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.TaxAmount")]
        public string TAXAMOUNT { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.ServiceChargeAmount")]
        public string SERVICECHARGEAMOUNT { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.CreditedAmount")]
        public string CREDITEDAMOUNT { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.FeeAmountBuyer")]
        public string FEEAMOUNTBUYER { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.TaxAmountBuyer")]
        public string TAXAMOUNTBUYER { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.CreditedAmountBuyer")]
        public string CREDITEDAMOUNTBUYER { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.EstadoContraCargo")]
        public string ESTADOCONTRACARGO { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Comision")]
        public string COMISION { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.IdContracargo")]
        public string IDCONTRACARGO { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.FechaNotificacionCuenta")]
        public string FECHANOTIFICACIONCUENTA { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.TEA")]
        public string TEA { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.CFT")]
        public string CFT { get; set; }

        // SP 46
        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.RELEASESTATUS")]
        public string RELEASESTATUS { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.RELEASEDATETIME")]
        public string RELEASEDATETIME { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PHONENUMBER")]
        public string PHONENUMBER { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.ADDRESS")]
        public string ADDRESS { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.POSTALCODE")]
        public string POSTALCODE { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.CUSTOMERID")]
        public string CUSTOMERID { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PRODUCTCODE")]
        //public string PRODUCTCODE { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PRODUCTDESCRIPTION")]
        //public string PRODUCTDESCRIPTION { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PRODUCTNAME")]
        //public string PRODUCTNAME { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.QUANTITY")]
        //public string QUANTITY { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.PRODUCTSKU")]
        //public string PRODUCTSKU { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.UNITPRICE")]
        //public string UNITPRICE { get; set; }

        //[NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.TOTALAMOUNT")]
        //public string TOTALAMOUNT { get; set; }

        [NopResourceDisplayName("Plugins.Payments.TodoPago.Fields.Items")]
        public string ITEMS { get; set; }
    }
}