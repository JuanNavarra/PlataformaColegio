﻿$(document).ready(function () {
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
    });

    $("#prestar_insumo").on('select2:select', function () {
        if ($(this).val() != "0") {
            $("#cantidad_prestar_insumo")
                .prop("disabled", false)
                .val($("#prestar_insumo option:selected").attr("stockAttr"))
                .attr("maximo", $("#prestar_insumo option:selected").attr("stockAttr"));
            $("#motivo_prestar_insumo").prop("disabled", false).val("");
            $("#fecha_prestamo_insumo").prop("disabled", false).val("");
            numeroMaximo("cantidad_prestar_insumo");
        } else {
            $("#cantidad_prestar_insumo").prop("disabled", true).val("");
            $("#motivo_prestar_insumo").prop("disabled", true).val("");;
            $("#fecha_prestamo_insumo").prop("disabled", true).val("");;
        }
    });
})

function guardarSuministros() {
    if (validarCampos("agregar-suministro")) {
        var suministrosObj = new Object({
            Nombre: $("#nombre_insumo_crear").val(),
            Descripcion: $("#descripcion_insumo_crear").val(),
            Stock: $("#stock_crear_insumo").val(),
            TipoSuministro: $("#tipo_insumo_crear").val(),
            Linea: $("#linea_crear_insumo").val(),
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
                    toastr.success(res.message, res.title + ":");
                    setTimeout(function () { location.reload(); }, 700);
                } else {
                    toastr.error(res.message, res.title + ":");
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
                    if (res.data != null) {
                        $("#div_resultado_busqueda_empleado_insumo").removeAttr("style");
                        $("#buscar_persona_insumo").val("");
                        $("#documento_persona_insumo").val(res.data.empleado.documento);
                        $("#nombre_persona_insumo").val(res.data.empleado.nombre);
                        $("#cargo_persona_insumo").val(res.data.empleado.nombreCargo);
                        MostrarSuministros();
                        if (res.data.insumos.length > 0) {
                            $("#div_tabla_insumos_necesarios").removeAttr("style");
                            $("#tbody_insumolaboral_formurlario").empty();
                            $.each(res.data.insumos, function (index, item) {
                                ca = "<tr>"
                                ca += "<td style='text-align:center'>" + (index + 1) + "</td>"
                                ca += "<td style='text-align:center'>" + item.nombre + "</td>"
                                $("#tbody_insumolaboral_formurlario").append(ca);
                            });
                        } else {
                            $("#div_tabla_insumos_necesarios").css("display", "none");
                        }
                    } else {
                        $("#div_resultado_busqueda_empleado_insumo").css("display", "none");
                        $("#div_tabla_insumos_necesarios").css("display", "none");
                        toastr.warning("No se encontro ningún empleado con éste documento");
                    }
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

function MostrarSuministros() {
    $.ajax({
        type: "GET",
        url: "Almacen/MostrarSuministros",
        data: {},
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.data != null) {
                var ca = "";
                $("#prestar_insumo").empty();
                ca = '<option value="0">Seleccione</option>'
                $("#prestar_insumo").append(ca);
                $.each(res.data, function (index, item) {
                    ca = "<option stockAttr=" + item.stock + " value=" + item.suministroId + ">" + item.nombre + " - (" + item.linea + ")</option>"
                    $("#prestar_insumo").append(ca);
                });
            }
        },
        error: function (error) {
            console.log("No se ha podido obtener la información");
        }
    })
    $('.select2').select2();
}

function agregarInsumoPrestar() {
    if (validarCampos("insumos-prestar")) {
        var numero = $("#tbody_prestamo_suministro tr").length + 1;
        var id = (numero + $("#cantidad_prestar_insumo").val() + $("#motivo_prestar_insumo").val() + $("#prestar_insumo").val());
        ca = "<tr id=" + id.replace(/ /g, "") + ">"
        ca += "<td>" + numero + "</td>"
        ca += "<td>" + $("#documento_persona_insumo").val() + "</td>"
        ca += "<td insumoNombre=" + $("#prestar_insumo").val() + ">" + $("#prestar_insumo option:selected").text() + "</td>"
        ca += "<td>" + $("#nombre_persona_insumo").val() + "</td>"
        ca += "<td>" + $("#cargo_persona_insumo").val() + "</td>"
        ca += "<td>" + $("#cantidad_prestar_insumo").val() + "</td>"
        ca += "<td>" + $("#motivo_prestar_insumo").val() + "</td>"
        ca += "<td>" + $("#fecha_prestamo_insumo").val() + "</td>"
        ca += "<td style='text-align:center'><a href='#' onclick=\"eliminarFila('" + id.replace(/ /g, "") + "');\"><i class='fas fa-trash'></i></a></td>"
        ca += "</tr>"
        $("#tbody_prestamo_suministro").append(ca);

        $("#prestar_insumo").select2('val', "0");
        $("#cantidad_prestar_insumo").val("");
        $("#motivo_prestar_insumo").prop("disabled", false).val("");
        $("#fecha_prestamo_insumo").prop("disabled", false).val("");

    } else {
        toastr.warning("¡Campos resaltados son obligatorios!");
    }
}

function prestarInsumos() {
    if ($("#tbody_prestamo_suministro").length > 0) {
        var prestamoArr = new Array();
        var elemento = document.getElementById("tbody_prestamo_suministro");
        for (var i = 0; i < elemento.children.length; i++) {
            var insumo = elemento.children[i].cells[2].attributes[0].textContent;
            var cantidad = elemento.children[i].cells[5].textContent;
            var motivo = elemento.children[i].cells[6].textContent;
            var fecha = elemento.children[i].cells[7].textContent;
            var item = {
                SuministroId: insumo,
                Cantidad: cantidad,
                Motivo: motivo,
                FechaPrestamo: fecha,
            };
            prestamoArr.push(item);
        }
        $.ajax({
            type: "POST",
            url: "Almacen/PrestarInsumos",
            data: {
                prestamos: JSON.stringify(prestamoArr), documento: $("#documento_persona_insumo").val(),
            },
            dataType: "json",
            async: true,
            success: function (res) {
                $("#prestar_insumo").select2('val', "0");
                $("#cantidad_prestar_insumo").val("");
                $("#motivo_prestar_insumo").val("");
                $("#fecha_prestamo_insumo").val("");
                if (res.status) {
                    $("#tbody_prestamo_suministro").empty();
                    $("#div_resultado_busqueda_empleado_insumo").css("display", "none");
                    toastr.success(res.message, res.title + ":");
                } else {
                    if (res.title == "Error de stock") {
                        $("td[insumoNombre='" + res.comodin + "']").parent().remove();
                    }
                    toastr.warning(res.message, res.title + ":");
                }
            },
            error: function (error) {
                console.log("No se ha podido obtener la información");
            }
        })
    } else {
        toastr.warning("!La tabla de suministros debe tener al menos un registro para ser prestado!");
    }
}