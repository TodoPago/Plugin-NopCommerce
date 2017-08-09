jQuery(function ($) {

    if ($("#SetCuotas").prop('checked')) {
        $("#MaxCuotasId").prop('disabled', false);
    } else {
        $("#MaxCuotasId").prop('disabled', true);
    }

    $("#SetCuotas").click(function () {
        if ($("#SetCuotas").prop('checked')) {
            $("#MaxCuotasId").prop('disabled', false);
        } else {
            $("#MaxCuotasId").prop('disabled', true);
        }
    });

    if ($("#SetTimeout").prop('checked')) {
        $("#Timeout").prop('disabled', false);
    } else {
        $("#Timeout").prop('disabled', true);
    }

    $("#SetTimeout").click(function () {
        if ($("#SetTimeout").prop('checked')) {
            $("#Timeout").prop('disabled', false);
        } else {
            $("#Timeout").prop('disabled', true);
        }
    });

    $("#Todopago_btnCredentials").click(function () {

        var user = $("#User").val();
        var password = $("#Password").val();

        getCredentials(user, password, "prod");

    });

    function getCredentials(user, password, mode) {

        $.ajax({
            type: 'POST',
            url: rootPath + "Plugins/PaymentTodoPago/GetCredentials",
            data: {
                'user': user,
                'password': password,
                'mode': mode
            },
            success: function (data) {
                setCredentials(data, mode);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr);

                switch (xhr.status) {
                    case 404: alert("Verifique la correcta instalación del plugin");
                        break;
                    default: alert("Verifique la conexion a internet y su proxy");
                        break;
                }
            },
        });
    }

    function setCredentials(data, ambiente) {
        var response = data;

        if (response.success === false) {
            alert(response.message);
        } else {
            $("#ApiKeyProduction").val(response.apikey);
            $("#SecurityProduction").val(response.security);
            $("#MerchantProduction").val(response.merchandid);
        }
    }


    $("#Todopago_btnCredentialsDev").click(function () {
        var user = $("#UserDev").val();
        var password = $("#PasswordDev").val();

        getCredentialsDev(user, password, "dev");
    });

    function getCredentialsDev(user, password, mode) {

        $.ajax({
            type: 'POST',
            url: rootPath + "Plugins/PaymentTodoPago/GetCredentials",
            data: {
                'user': user,
                'password': password,
                'mode': mode
            },
            success: function (data) {
                setCredentialsDev(data, mode);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr);

                switch (xhr.status) {
                    case 404: alert("Verifique la correcta instalación del plugin");
                        break;
                    default: alert("Verifique la conexion a internet y su proxy");
                        break;
                }
            },
        });
    }

    function setCredentialsDev(data, ambiente) {
        var response = data;

        if (response.success === false) {
            alert(response.message);
        } else {
            $("#ApiKeyDeveloper").val(response.apikey);
            $("#SecurityDeveloper").val(response.security);
            $("#MerchantDeveloper").val(response.merchandid);
        }
    }
});