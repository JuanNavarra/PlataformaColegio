$(document).ready(function () {

});


function agregarCuenta() {
    if (validarCampos("agregar-cta")) {
        var nombre = $("#nombre_banco").val();
        var titular = $("#titular_cuenta").val();
        var numero = $("#numero_cuenta").val();
        $.ajax({
            type: "POST",
            url: "Bancario/GuardarCuenta",
            data: {
                nombre: nombre, titular, titular, numero: numero
            },
            dataType: "json",
            async: true,
            success: function (res) {
                if (res.status) {
                    toastr.success(res.message, res.title + ":");

                } else {
                    toastr.error(res.message, res.title + ":");
                }
            },
            error: function (error) {
                console.log("No se ha podido obtener la información");
            }
        })
    } else {
        toastr.warning("¡Campos resaltados son obligatorios!");
    }
}