using System;
using System.Collections.Generic;

namespace Nop.Plugin.Payments.TodoPago.Utils
{
    public class ErrorMessageCS
    {
        private Dictionary<string, string> messages;

        public ErrorMessageCS()
        {
            this.messages = new Dictionary<string, string>();

            this.messages.Add("98001", "El campo CSBTCITY es requerido");
            this.messages.Add("98002", "El campo CSBTCOUNTRY es requerido");
            this.messages.Add("98003", "El campo CSBTCUSTOMERID es requerido");
            this.messages.Add("98004", "El campo CSBTIPADDRESS es requerido");
            this.messages.Add("98005", "El campo CSBTEMAIL es requerido");
            this.messages.Add("98006", "El campo CSBTFIRSTNAME es requerido");
            this.messages.Add("98007", "El campo CSBTLASTNAME es requerido");
            this.messages.Add("98008", "El campo CSBTPHONENUMBER es requerido");
            this.messages.Add("98009", "El campo CSBTPOSTALCODE es requerido");
            this.messages.Add("98010", "El campo CSBTSTATE es requerido");
            this.messages.Add("98011", "El campo CSBTSTREET1 es requerido");
            this.messages.Add("98012", "El campo CSBTSTREET2 es requerido");
            this.messages.Add("98013", "El campo CSPTCURRENCY es requerido");
            this.messages.Add("98014", "El campo CSPTGRANDTOTALAMOUNT es requerido");
            this.messages.Add("98020", "El campo CSSTCITY es requerido");
            this.messages.Add("98021", "El campo CSSTCOUNTRY es requerido");
            this.messages.Add("98022", "El campo CSSTEMAIL es requerido");
            this.messages.Add("98023", "El campo CSSTFIRSTNAME es requerido");
            this.messages.Add("98024", "El campo CSSTLASTNAME es requerido");
            this.messages.Add("98025", "El campo CSSTPHONENUMBER es requerido");
            this.messages.Add("98026", "El campo CSSTPOSTALCODE es requerido");
            this.messages.Add("98027", "El campo CSSTSTATE es requerido");
            this.messages.Add("98028", "El campo CSSTSTREET1 es requerido");
            this.messages.Add("98034", "El campo CSITPRODUCTCODE es requerido");
            this.messages.Add("98035", "El campo CSITPRODUCTDESCRIPTION es requerido");
            this.messages.Add("98036", "El campo CSITPRODUCTNAME es requerido");
            this.messages.Add("98037", "El campo CSITPRODUCTSKU es requerido");
            this.messages.Add("98038", "El campo CSITTOTALAMOUNT es requerido");
            this.messages.Add("98039", "El campo CSITQUANTITY es requerido");
            this.messages.Add("98040", "El campo CSITUNITPRICE es requerido");
            this.messages.Add("98101", "El formato del campo CSBTCITY es incorrecto");
            this.messages.Add("98102", "El formato del campo CSBTCOUNTRY es incorrecto");
            this.messages.Add("98103", "El formato del campo CSBTCUSTOMERID es incorrecto");
            this.messages.Add("98104", "El formato del campo CSBTIPADDRESS es incorrecto");
            this.messages.Add("98105", "El formato del campo CSBTEMAIL es incorrecto");
            this.messages.Add("98106", "El formato del campo CSBTFIRSTNAME es incorrecto");
            this.messages.Add("98107", "El formato del campo CSBTLASTNAME es incorrecto");
            this.messages.Add("98108", "El formato del campo CSBTPHONENUMBER es incorrecto");
            this.messages.Add("98109", "El formato del campo CSBTPOSTALCODE es incorrecto");
            this.messages.Add("98110", "El formato del campo CSBTSTATE es incorrecto");
            this.messages.Add("98111", "El formato del campo CSBTSTREET1 es incorrecto");
            this.messages.Add("98112", "El formato del campo CSBTSTREET2 es incorrecto");
            this.messages.Add("98113", "El formato del campo CSPTCURRENCY es incorrecto");
            this.messages.Add("98114", "El formato del campo CSPTGRANDTOTALAMOUNT es incorrecto");
            this.messages.Add("98115", "El formato del campo CSMDD7 es incorrecto");
            this.messages.Add("98116", "El formato del campo CSMDD8 es incorrecto");
            this.messages.Add("98117", "El formato del campo CSMDD9 es incorrecto");
            this.messages.Add("98118", "El formato del campo CSMDD10 es incorrecto");
            this.messages.Add("98119", "El formato del campo CSMDD11 es incorrecto");
            this.messages.Add("98120", "El formato del campo CSSTCITY es incorrecto");
            this.messages.Add("98121", "El formato del campo CSSTCOUNTRY es incorrecto");
            this.messages.Add("98122", "El formato del campo CSSTEMAIL es incorrecto");
            this.messages.Add("98123", "El formato del campo CSSTFIRSTNAME es incorrecto");
            this.messages.Add("98124", "El formato del campo CSSTLASTNAME es incorrecto");
            this.messages.Add("98125", "El formato del campo CSSTPHONENUMBER es incorrecto");
            this.messages.Add("98126", "El formato del campo CSSTPOSTALCODE es incorrecto");
            this.messages.Add("98127", "El formato del campo CSSTSTATE es incorrecto");
            this.messages.Add("98128", "El formato del campo CSSTSTREET1 es incorrecto");
            this.messages.Add("98129", "El formato del campo CSMDD12 es incorrecto");
            this.messages.Add("98130", "El formato del campo CSMDD13 es incorrecto");
            this.messages.Add("98131", "El formato del campo CSMDD14 es incorrecto");
            this.messages.Add("98132", "El formato del campo CSMDD15 es incorrecto");
            this.messages.Add("98133", "El formato del campo CSMDD16 es incorrecto");
            this.messages.Add("98134", "El formato del campo CSITPRODUCTCODE es incorrecto");
            this.messages.Add("98135", "El formato del campo CSITPRODUCTDESCRIPTION es incorrecto");
            this.messages.Add("98136", "El formato del campo CSITPRODUCTNAME es incorrecto");
            this.messages.Add("98137", "El formato del campo CSITPRODUCTSKU es incorrecto");
            this.messages.Add("98138", "El formato del campo CSITTOTALAMOUNT es incorrecto");
            this.messages.Add("98139", "El formato del campo CSITQUANTITY es incorrecto");
            this.messages.Add("98140", "El formato del campo CSITUNITPRICE es incorrecto");
            this.messages.Add("98141", "El formato del campo CSSTSTREET2 es incorrecto");
            this.messages.Add("98201", "Existen errores en la información de los productos");
            this.messages.Add("98202", "Existen errores en la información de CSITPRODUCTDESCRIPTION los productos");
            this.messages.Add("98203", "Existen errores en la información de CSITPRODUCTNAME los productos");
            this.messages.Add("98204", "Existen errores en la información de CSITPRODUCTSKU los productos");
            this.messages.Add("98205", "Existen errores en la información de CSITTOTALAMOUNT los productos");
            this.messages.Add("98206", "Existen errores en la información de CSITQUANTITY los productos");
            this.messages.Add("98207", "Existen errores en la información de CSITUNITPRICE de los productos");
        }

        public string GetErrorInfo(int errorCode)
        {
            string result = String.Empty;

            if (this.messages.ContainsKey(errorCode.ToString()))
                result = this.messages[errorCode.ToString()];

            return result;
        }
    }
}
