$(document).ready(function () {
    //  window.TPFORMAPI.hybridForm.constructDefaultForm("formContainer");
    window.TPFORMAPI.hybridForm.initForm({
        callbackValidationErrorFunction: 'validationCollector',
        callbackBilleteraFunction: 'billeteraPaymentResponse',
        callbackCustomSuccessFunction: 'customPaymentSuccessResponse',
        callbackCustomErrorFunction: 'customPaymentErrorResponse',
        botonPagarId: 'MY_btnConfirmarPago',
        botonPagarConBilleteraId: 'MY_btnPagarConBilletera',
        modalCssClass: 'modal-class',
        modalContentCssClass: 'modal-content',
        beforeRequest: 'initLoading',
        afterRequest: 'stopLoading'
    });
    /************* SETEO UN ITEM PARA COMPRAR ******************/
    window.TPFORMAPI.hybridForm.setItem({
        publicKey: PUBLICREQUESTKEY,
        defaultNombreApellido: NOMBRECOMPLETO,
        defaultNumeroDoc: '',
        defaultMail: EMAIL,
        defaultTipoDoc: 'DNI'
    });

    /************ FUNCIONES CALLBACKS ************/
    function validationCollector(parametros) {
        console.log("Validando los datos");
        console.log(parametros.field + " -> " + parametros.error);
        var input = parametros.field;
        if (input.search("Txt") !== -1) {
            label = input.replace("Txt", "Lbl");
        } else {
            label = input.replace("Cbx", "Lbl");
        }
        document.getElementById(label).innerHTML = parametros.error;
    }

    function billeteraPaymentResponse(response) {
        console.log("Iniciando billetera");
        console.log(response.ResultCode + " -> " + response.ResultMessage);
        if (response.AuthorizationKey) {
            window.location.href = URL_OK + "&Answer=" + response.AuthorizationKey;
        } else {
            window.location.href = URL_ERROR + response.ResultMessage;
        }
    }

    function customPaymentSuccessResponse(response) {
        console.log("Success");
        console.log(response.ResultCode + " -> " + response.ResultMessage);
        window.location.href = URL_OK + "&Answer=" + response.AuthorizationKey;
    }

    function customPaymentErrorResponse(response) {
        console.log("Error al pagar.");
        console.log(response.ResultCode + " -> " + response.ResultMessage);
        window.location.href = URL_ERROR + response.ResultMessage;
    }

    function initLoading() {
        console.log('Loading...');
    }

    function stopLoading() {
        console.log('Stop loading...');
    }
});