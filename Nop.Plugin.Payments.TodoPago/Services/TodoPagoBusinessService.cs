using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Plugin.Payments.TodoPago.Domain;
using TodoPagoConnector.Utils;

namespace Nop.Plugin.Payments.TodoPago.Services
{
    public class TodoPagoBusinessService {

        private Dictionary<string, string> stateCodeDictionary;
        private const string TODOPAGO_SAR_ARS = "ARS";
        private const string TODOPAGO_NUMERAL = "#";

        //private TodoPagoTransactionDto todoPagoTransactionDto;

        private readonly ITodoPagoTransactionService _todoPagoTransactionService;

        public TodoPagoBusinessService(ITodoPagoTransactionService todoPagoTransactionService){
            this._todoPagoTransactionService = todoPagoTransactionService;
            setState();
        }


        public void insertTodoPagoTransactionRecord(TodoPagoTransactionDto todoPagoTransactionDto) {
            _todoPagoTransactionService.insertTodoPagoTransactionRecord(this.toRecord(todoPagoTransactionDto));
        }

        public TodoPagoTransactionDto findTodoPagoTransactionRecord(int orderId) {

            TodoPagoTransactionDto result = null;

            if (orderId != null) {
                result = this.toDto(_todoPagoTransactionService.findByOrdenId(orderId));
            }

            return result;
        }

        public void updateTodoPagoTransactionRecord(TodoPagoTransactionDto todoPagoTransactionDto){

            TodoPagoTransactionRecord record = _todoPagoTransactionService.findByOrdenId(todoPagoTransactionDto.ordenId);
            //TodoPagoTransactionDto result = findTodoPagoTransactionRecord(todoPagoTransactionDto.ordenId);

            if(record != null){

                record.secondStep = todoPagoTransactionDto.secondStep;
                record.paramsGAA = todoPagoTransactionDto.paramsGAA;
                record.responseGAA = todoPagoTransactionDto.responseGAA;
                record.answerKey = todoPagoTransactionDto.answerKey;

                _todoPagoTransactionService.updateTodoPagoTransactionRecord(record);

            }
        }

        public Dictionary<string, string> completePayLoad(Dictionary<string, string> payload, PostProcessPaymentRequest postProcessPaymentRequest) {

            var orderTotal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
            String amount = orderTotal.ToString("0.00", CultureInfo.InvariantCulture);

            payload.Add("CSBTCITY", postProcessPaymentRequest.Order.BillingAddress.City);
            payload.Add("CSBTCOUNTRY", postProcessPaymentRequest.Order.BillingAddress.Country.TwoLetterIsoCode ?? "AR");//MANDATORIO. Código ISO.
            payload.Add("CSBTEMAIL", postProcessPaymentRequest.Order.BillingAddress.Email); //MANDATORIO.
            payload.Add("CSBTFIRSTNAME", postProcessPaymentRequest.Order.BillingAddress.FirstName);//MANDATORIO.
            payload.Add("CSBTLASTNAME", postProcessPaymentRequest.Order.BillingAddress.LastName);//MANDATORIO.
            payload.Add("CSBTPHONENUMBER", postProcessPaymentRequest.Order.BillingAddress.PhoneNumber);//MANDATORIO.
            payload.Add("CSBTPOSTALCODE", postProcessPaymentRequest.Order.BillingAddress.ZipPostalCode);//MANDATORIO.
            //payload.Add("CSBTSTATE", postProcessPaymentRequest.Order.BillingAddress.StateProvince.Name);//MANDATORIO
            payload.Add("CSBTSTATE", "C");//MANDATORIO
            payload.Add("CSBTSTREET1", postProcessPaymentRequest.Order.BillingAddress.Address1);//MANDATORIO.
            payload.Add("CSBTSTREET2", postProcessPaymentRequest.Order.BillingAddress.Address2);//NO MANDATORIO
            payload.Add("CSBTCUSTOMERID", postProcessPaymentRequest.Order.Customer.Id.ToString()); //MANDATORIO.
            payload.Add("CSBTIPADDRESS", postProcessPaymentRequest.Order.CustomerIp.ToString()); //MANDATORIO.
            payload.Add("CSPTCURRENCY", TODOPAGO_SAR_ARS);//MANDATORIO.
            payload.Add("CSPTGRANDTOTALAMOUNT", amount);//MANDATORIO.

            payload.Add("CSMDD6", "");//NO MANDATORIO.
            payload.Add("CSMDD7", "");//NO MANDATORIO.
            payload.Add("CSMDD8", ""); //NO MANDATORIO.
            payload.Add("CSMDD9", "");//NO MANDATORIO.
            payload.Add("CSMDD10", "");//NO MANDATORIO.
            payload.Add("CSMDD11", "");//NO MANDATORIO.

            //retail
            if (postProcessPaymentRequest.Order.PickUpInStore || postProcessPaymentRequest.Order.ShippingStatusId == 10) {
                payload.Add("CSSTCITY", postProcessPaymentRequest.Order.BillingAddress.City); //MANDATORIO.
                payload.Add("CSSTCOUNTRY", postProcessPaymentRequest.Order.BillingAddress.Country.TwoLetterIsoCode ?? "AR");//MANDATORIO. Código ISO.
                payload.Add("CSSTEMAIL", postProcessPaymentRequest.Order.BillingAddress.Email); //MANDATORIO.
                payload.Add("CSSTFIRSTNAME", postProcessPaymentRequest.Order.BillingAddress.FirstName);//MANDATORIO.
                payload.Add("CSSTLASTNAME", postProcessPaymentRequest.Order.BillingAddress.LastName);//MANDATORIO.
                payload.Add("CSSTPHONENUMBER", postProcessPaymentRequest.Order.BillingAddress.PhoneNumber);//MANDATORIO.
                payload.Add("CSSTPOSTALCODE", postProcessPaymentRequest.Order.BillingAddress.ZipPostalCode);//MANDATORIO.
                //payload.Add("CSSTSTATE", postProcessPaymentRequest.Order.BillingAddress.StateProvince.Name);//MANDATORIO
                payload.Add("CSSTSTATE", "C");//MANDATORIO
                payload.Add("CSSTSTREET1", postProcessPaymentRequest.Order.BillingAddress.Address1);//MANDATORIO.
                payload.Add("CSSTSTREET2", postProcessPaymentRequest.Order.BillingAddress.Address2);//NO MANDATORIO.

            } else{
                payload.Add("CSSTCITY", postProcessPaymentRequest.Order.ShippingAddress.City); //MANDATORIO.
                payload.Add("CSSTCOUNTRY", postProcessPaymentRequest.Order.ShippingAddress.Country.TwoLetterIsoCode ?? "AR");//MANDATORIO. Código ISO.
                payload.Add("CSSTEMAIL", postProcessPaymentRequest.Order.ShippingAddress.Email); //MANDATORIO.
                payload.Add("CSSTFIRSTNAME", postProcessPaymentRequest.Order.ShippingAddress.FirstName);//MANDATORIO.
                payload.Add("CSSTLASTNAME", postProcessPaymentRequest.Order.ShippingAddress.LastName);//MANDATORIO.
                payload.Add("CSSTPHONENUMBER", postProcessPaymentRequest.Order.ShippingAddress.PhoneNumber);//MANDATORIO.
                payload.Add("CSSTPOSTALCODE", postProcessPaymentRequest.Order.ShippingAddress.ZipPostalCode);//MANDATORIO.
                //payload.Add("CSSTSTATE", postProcessPaymentRequest.Order.ShippingAddress.StateProvince.Name);//MANDATORIO
                payload.Add("CSSTSTATE", "C");//MANDATORIO
                payload.Add("CSSTSTREET1", postProcessPaymentRequest.Order.ShippingAddress.Address1);//MANDATORIO.
                payload.Add("CSSTSTREET2", postProcessPaymentRequest.Order.ShippingAddress.Address2);//NO MANDATORIO.
            }



            StringBuilder csitProductCode = new StringBuilder();
            StringBuilder csitProductDescription = new StringBuilder();
            StringBuilder csitProductName = new StringBuilder();
            StringBuilder csitProductsSKU = new StringBuilder();
            StringBuilder csitTotalAmount = new StringBuilder();
            StringBuilder csitQuantity = new StringBuilder();
            StringBuilder csitUnitPrice = new StringBuilder();

            var cartItems = postProcessPaymentRequest.Order.OrderItems;

            foreach (var item in cartItems){
                csitProductCode.Append(item.ProductId + TODOPAGO_NUMERAL);
                csitProductDescription.Append(getDescription(item.Product.FullDescription, item.Product.ShortDescription, item.Product.Name) + TODOPAGO_NUMERAL);
                csitProductName.Append(item.Product.Name + TODOPAGO_NUMERAL);
                csitProductsSKU.Append(getSKU(item.Product.Sku, item.ProductId.ToString()) + TODOPAGO_NUMERAL);
                csitTotalAmount.Append(Math.Round(item.PriceInclTax, 2).ToString("0.00", CultureInfo.InvariantCulture) + TODOPAGO_NUMERAL);
                csitQuantity.Append(item.Quantity + TODOPAGO_NUMERAL);
                csitUnitPrice.Append(Math.Round(item.UnitPriceInclTax, 2).ToString("0.00", CultureInfo.InvariantCulture) + TODOPAGO_NUMERAL);
            }

            //csitProductCode.ToString().Substring(0, csitProductCode.Length - 1);

            payload.Add("CSITPRODUCTCODE", csitProductCode.ToString().Substring(0, csitProductCode.Length - 1));//CONDICIONAL
            payload.Add("CSITPRODUCTDESCRIPTION", csitProductDescription.ToString().Substring(0, csitProductDescription.Length - 1));//CONDICIONAL.
            payload.Add("CSITPRODUCTNAME", csitProductName.ToString().Substring(0, csitProductName.Length - 1));//CONDICIONAL.
            payload.Add("CSITPRODUCTSKU", csitProductsSKU.ToString().Substring(0, csitProductsSKU.Length - 1));//CONDICIONAL.
            payload.Add("CSITTOTALAMOUNT", csitTotalAmount.ToString().Substring(0, csitTotalAmount.Length - 1));//CONDICIONAL.
            payload.Add("CSITQUANTITY", csitQuantity.ToString().Substring(0, csitQuantity.Length - 1));//CONDICIONAL.
            payload.Add("CSITUNITPRICE", csitUnitPrice.ToString().Substring(0, csitUnitPrice.Length - 1));

            payload.Add("CSMDD12", "");//NO MADATORIO.
            payload.Add("CSMDD13", "");//NO MANDATORIO.
            payload.Add("CSMDD14", "");//NO MANDATORIO.
            payload.Add("CSMDD15", "");//NO MANDATORIO.
            payload.Add("CSMDD16", "");//NO MANDATORIO.

            return payload;
        
        }

        private String getDescription (String description, String shortDescription, String name){

            String result = "description";

            if (description.Equals(String.Empty)) {
                if (shortDescription.Equals(String.Empty)) {
                    result = name;
                } else {
                    result = shortDescription;
                }
            }else {
                // result = description;
                result = shortDescription;
            }

            return result;
        }

        private String getSKU(String SKU, String code){

            String result = "SKU";

            if (SKU != null && !SKU.Equals(String.Empty)){
                result = SKU;
            }else {
                result = code;
            }

            return result;
        }

        private void setState(){
            this.stateCodeDictionary = new Dictionary<string, string>();
            this.stateCodeDictionary["A"] = "4400";
            this.stateCodeDictionary["B"] = "1900";
            this.stateCodeDictionary["C"] = "1000";
            this.stateCodeDictionary["D"] = "5700";
            this.stateCodeDictionary["E"] = "3100";
            this.stateCodeDictionary["F"] = "5300";
            this.stateCodeDictionary["G"] = "4200";
            this.stateCodeDictionary["H"] = "3500";
            this.stateCodeDictionary["J"] = "5400";
            this.stateCodeDictionary["K"] = "4700";
            this.stateCodeDictionary["L"] = "6300";
            this.stateCodeDictionary["M"] = "5500";
            this.stateCodeDictionary["N"] = "3300";
            this.stateCodeDictionary["P"] = "3600";
            this.stateCodeDictionary["Q"] = "8300";
            this.stateCodeDictionary["R"] = "8500";
            this.stateCodeDictionary["S"] = "3000";
            this.stateCodeDictionary["T"] = "4001";
            this.stateCodeDictionary["U"] = "9103";
            this.stateCodeDictionary["V"] = "9410";
            this.stateCodeDictionary["W"] = "3400";
            this.stateCodeDictionary["X"] = "5000";
            this.stateCodeDictionary["Y"] = "4600";
            this.stateCodeDictionary["Z"] = "9400";
        }

        //private String findState()
        //{
        //    String result = "1000";

        //    if (this.resultDictionary.ContainsKey(CSBTSTATE))
        //    {
        //        if (this.stateCodeDictionary.ContainsKey(this.resultDictionary[CSBTSTATE]))
        //        {
        //            result = this.stateCodeDictionary[(this.resultDictionary[CSBTSTATE])];
        //        }
        //    }
        //    return result;
        //}

        //public void setTodoPagoTransactionDto(TodoPagoTransactionDto todoPagoTransactionDto){
        //    this.todoPagoTransactionDto = todoPagoTransactionDto;
        //}

        //public TodoPagoTransactionDto getTodoPagoTransactionDto(){
        //    return this.todoPagoTransactionDto;
        //}

    
        public String serealizar(Dictionary<string, object> dict){

            StringBuilder builder = new StringBuilder();
    
            foreach (KeyValuePair<string, object> pair in dict){
                builder.Append(pair.Key).Append(":").Append(pair.Value).Append(',');
            }

            String result = builder.ToString();
            result = result.TrimEnd(',');

            return result;
         }



        public String serealizar(Dictionary<string, string> dict){

            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<string, string> pair in dict) {
                 builder.Append(pair.Key).Append(":").Append(pair.Value).Append(',');
            }

            String result = builder.ToString();
            result = result.TrimEnd(',');

            return result;

        }

        public String serealizarRefund(Dictionary<string, object> dict)
        {

            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<string, object> pair in dict)
            {
                if (pair.Value is Dictionary<string, object>)
                {
                    builder.Append(pair.Key).Append(":").Append(serealizar((Dictionary<string, object>)pair.Value)).Append(',');
                }
                else
                {
                    builder.Append(pair.Key).Append(":").Append(pair.Value).Append(',');
                }
            }

            String result = builder.ToString();
            result = result.TrimEnd(',');

            return result;
        }

        public TodoPagoTransactionRecord toRecord (TodoPagoTransactionDto dto) { 

            TodoPagoTransactionRecord record = new TodoPagoTransactionRecord();
            record.ordenId = dto.ordenId;
            record.firstStep = dto.firstStep;
            record.paramsSAR = dto.paramsSAR;
            record.responseSAR = dto.responseSAR;
            record.requestKey = dto.requestKey;
            record.publicRequestKey = dto.publicRequestKey;
            record.secondStep = dto.secondStep;
            record.paramsGAA = dto.paramsGAA;
            record.responseGAA = dto.responseGAA;
            record.answerKey = dto.answerKey;

            return record;

        }

        public TodoPagoTransactionDto toDto(TodoPagoTransactionRecord record) {

            TodoPagoTransactionDto dto = new TodoPagoTransactionDto ();
            dto.ordenId = record.ordenId;
            dto.firstStep = record.firstStep;
            dto.paramsSAR = record.paramsSAR;
            dto.responseSAR = record.responseSAR;
            dto.requestKey = record.requestKey;
            dto.publicRequestKey = record.publicRequestKey;
            dto.secondStep = record.secondStep;
            dto.paramsGAA = record.paramsGAA;
            dto.responseGAA = record.responseGAA;
            dto.answerKey = record.answerKey;

            return dto;
        }



    }
}
