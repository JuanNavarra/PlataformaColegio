$(document).ready(function () {
    $('#datemask').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' });
    $('[data-mask]').inputmask();

    $("#tipo_insumo_crear").on("click keypress blur", function () {
        if ($(this).val() == "za") {
            $("#zapatos_insumos").removeAttr("style");
            $("#tallas_zapatos_insumo_crear").addClass("agregar-suministro");
            $("#ropa_insumos").css("display", "none");
            $("#tallas_ropa_insumo_crear").removeClass("agregar-suministro");
            $("#tallas_ropa_insumo_crear").val("0");
        }
        else if ($(this).val() == "ro") {
            $("#ropa_insumos").removeAttr("style");
            $("#zapatos_insumos").css("display", "none");
            $("#tallas_zapatos_insumo_crear").removeClass("agregar-suministro");
            $("#tallas_zapatos_insumo_crear").val("0");
            $("#tallas_ropa_insumo_crear").addClass("agregar-suministro");
        } else {
            $("#zapatos_insumos").css("display", "none").removeClass("agregar-suministro");
            $("#ropa_insumos").css("display", "none")
            $("#tallas_zapatos_insumo_crear").removeClass("agregar-suministro");
            $("#tallas_ropa_insumo_crear").removeClass("agregar-suministro");
            $("#tallas_zapatos_insumo_crear").val("0");
            $("#tallas_ropa_insumo_crear").val("0");
        }
    });

    $("#buscar_persona_insumo").on("keypress", function (e) {
        buscarEmpleado(e);
    })
})

function guardarSuministros() {
    if (validarCampos("agregar-suministro")) {
        var suministrosObj = new Object({
            Nombre: $("#nombre_insumo_crear").val(),
            Descripcion: $("#descripcion_insumo_crear").val(),
            Stock: $("#stock_crear_insumo").val(),
            TipoSuministro: $("#tipo_insumo_crear").val(),
            Talla: $("#tipo_insumo_crear").val() == "ro" ? $("#tallas_ropa_insumo_crear").val() : $("#tipo_insumo_crear").val() == "za" ? $("#tallas_zapatos_insumo_crear").val() : "0",
        })

        $.ajax({
            type: "POST",
            url: "Almacen/GuardarSuministros",
            data: {
                suministros: JSON.stringify(suministrosObj),
            },
            dataType: "json",
            async: true,
            success: function (res) {
                if (res.status) {
                    $("#nombre_insumo_crear").val("").focus(); $("#descripcion_insumo_crear").val("");
                    $("#stock_crear_insumo").val(""); $("#tipo_insumo_crear").val("0");
                    $("#tallas_ropa_insumo_crear").val(""); $("#tallas_zapatos_insumo_crear").val("");
                    $("#tipo_insumo_crear").click();
                    toastr.success(res.title + ":", res.message);
                    setTimeout(function () { location.reload(); }, 700);
                } else {
                    toastr.error(res.title + ":", res.message);
                }
            },
            error: function (error) {
                toastr.error("No se ha podido obtener la información");
            }
        })
    } else {
        toastr.warning("Advertencia", "Falta por llenar campos obligatorios");
    }
}

function buscarEmpleado(e) {
    if (e.keyCode == 13) {
        if ($("#buscar_persona_insumo").val() != "") {
            $.ajax({
                type: "GET",
                url: "Almacen/BuscarEmpleado",
                data: {
                    documento: $("#buscar_persona_insumo").val(),
                },
                dataType: "json",
                async: true,
                success: function (res) {
                },
                error: function (error) {
                    toastr.error("No se ha podido obtener la información");
                }
            })
        } else {
            toastr.warning("El campo de busqueda esta vacío");
        }
    }
}