var devolocionArr = new Array();
$(document).ready(function () {
    mostrarInfoSuministros()
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

    $("#buscar_persona_insumo_devolucion").on("keypress", function (e) {
        buscarDevolver(e);
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
                    mostrarInfoSuministros();
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
                                ca += "</tr>"
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

function eliminarFila(id) {
    $("#" + id).remove();
}

function prestarInsumos() {
    if ($("#tbody_prestamo_suministro tr").length > 0) {
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
                    mostrarInfoSuministros();
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

function buscarDevolver(e) {
    if (e.keyCode == 13) {
        if ($("#buscar_persona_insumo_devolucion").val() != "") {
            $.ajax({
                type: "GET",
                url: "Almacen/BuscarDevolver",
                data: {
                    documento: $("#buscar_persona_insumo_devolucion").val(),
                },
                dataType: "json",
                async: true,
                success: function (res) {
                    if (res.length > 0) {
                        devolocionArr.length = 0;
                        $("#tbody_prestamo_suministro_devolucion").empty();
                        $("#tbody_prestamo_suministro_devolucion").removeAttr("style");
                        $("#div_resultado_busqueda_empleado_insumo_devolucion").removeAttr("style");
                        $("#buscar_persona_insumo_devolucion").val("");
                        $.each(res, function (index, item) {
                            ca = "<tr id='" + item.idPrestamo + "'>"
                            ca += "<td style='text-align:center'>" + (index + 1) + "</td>"
                            ca += "<td style='text-align:center'>" + item.insumo + "</td>"
                            ca += "<td style='text-align:center'>" + item.cantidad + "</td>"
                            ca += "<td style='text-align:center'><a href='#' onclick=\"incremetar('" + item.idPrestamo + "'," + item.cantidad + "," + index + ");\"><i class='fas fa-plus'></i></a><a name='cantidad' style='margin-left: 10px;margin-right: 10px;font-size: 22px;'>0</a><a href='#' onclick=\"decrementar('" + item.idPrestamo + "'," + index + ");\"><i class='fas fa-minus'></i></a></td>"
                            ca += "</tr>"
                            $("#tbody_prestamo_suministro_devolucion").append(ca);
                            var item = {
                                idPrestamo: item.idPrestamo, cantidadActual: item.cantidad,
                                incremento: 0, idPersona: item.idPersona, suministroId: item.suministroId
                            }
                            devolocionArr.push(item);
                        });
                    } else {
                        toastr.warning("No hay insumos por devolver para este empleado");
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

function incremetar(id, max, index) {
    if (devolocionArr[index].incremento < max) {
        devolocionArr.map(function (dato) {
            if (dato.idPrestamo == id) {
                dato.incremento = dato.incremento + 1;
                $("#" + id).children().eq(3).children()[1].innerHTML = dato.incremento
            }
        });
    }
}

function decrementar(id, index) {
    if (devolocionArr[index].incremento > 0) {
        devolocionArr.map(function (dato) {
            if (dato.idPrestamo == id) {
                dato.incremento = dato.incremento - 1;
                $("#" + id).children().eq(3).children()[1].innerHTML = dato.incremento
            }
        });
    }
}

function devolverInsumos() {
    $.ajax({
        type: "POST",
        url: "Almacen/DevolverInsumos",
        data: {
            devoluciones: JSON.stringify(devolocionArr),
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.status) {
                $("#tbody_prestamo_suministro_devolucion").empty();
                $("#tbody_prestamo_suministro_devolucion").css("display", "none");
                $("#div_resultado_busqueda_empleado_insumo_devolucion").css("display", "none");
                toastr.success(res.message, res.title + ":");
                mostrarInfoSuministros();
            } else {
                toastr.warning(res.message, res.title + ":");
            }
        },
        error: function (error) {
            console.log("No se ha podido obtener la información");
        }
    })
}

function mostrarInfoSuministros() {
    $.ajax({
        type: "GET",
        url: "Almacen/MostrarInfoSuministros",
        data: {},
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.length > 0) {
                $("#tbody_prestamo_suministros").empty();
                $.each(res, function (index, item) {
                    var ultimoPrestamo = item.ultimoPrestamo.split("T")
                    var fechaCreacion = item.fechaCreacion.split("T")
                    ca = "<tr id=" + item.suministroId + ">"
                    ca += "<td style='text-align:center'>" + (index + 1) + "</td>"
                    ca += "<td style='text-align:center'>" + item.nombre + "</td>"
                    ca += "<td style='text-align:center'>" + item.descripcion + "</td>"
                    ca += "<td style='text-align:center'>" + item.linea + "</td>"
                    ca += "<td style='text-align:center'>" + fechaCreacion[0].replace(/^(\d{4})-(\d{2})-(\d{2})$/g, '$3/$2/$1') + "</td>"
                    ca += "<td style='text-align:center'>"
                    ca += item.ultimoPrestamo.includes("0001-01-01T") ? "NO REGISTRA" : ultimoPrestamo[0].replace(/^(\d{4})-(\d{2})-(\d{2})$/g, '$3/$2/$1')
                    ca += "</td>"
                    ca += "<td style='text-align:center;background-color:#d9edf7;color:#31708f'>" + item.prestado + "</td>"
                    ca += "<td style='text-align:center;background-color:#d9edf7;color:#31708f'>" + item.stock + "</td>"
                    ca += "<td style='text-align:center;background-color:#d9edf7;color:#31708f'>"
                    ca += item.total == 0 ? item.stock : item.total
                    ca += "</td>"
                    ca += "<td style='text-align:center'>"
                    ca += "<a href='#' onclick=\"eliminarSuministros('" + item.suministroId + "');\"><i class='fas fa-trash'></i></a>"
                    ca += item.stock != 0 ? "<a href='#' style='margin-left: -32px;' onclick=\"vaciarStock('" + item.suministroId + "');\"><i class='far fa-folder-open'></i></a>" : ""
                    ca += "</td >"
                    ca += "</tr>"
                    $("#tbody_prestamo_suministros").append(ca);
                });
            }
        },
        error: function (error) {
            console.log("No se ha podido obtener la información");
        }
    })
}

function eliminarSuministros(suministroId) {
    var prestado = $("#" + suministroId).children().eq(6).text();
    var stock = $("#" + suministroId).children().eq(7).text();
    if ((prestado && stock) == "0") {
        swal({
            title: "¿Está seguro que desea eliminar esto(s) suminstros?",
            buttons: {
                cancel: "Cancelar",
                catch: {
                    text: "Eliminar",
                    value: "catch",
                },
            },
            icon: "warning",
        }).then((value) => {
            if (value == "catch") {
                $.ajax({
                    type: "DELETE",
                    url: "Almacen/EliminarSuministros",
                    data: {
                        suministroId: suministroId
                    },
                    dataType: "json",
                    async: true,
                    success: function (res) {
                        if (res.status) {
                            toastr.success(res.message, res.title + ":");
                            mostrarInfoSuministros();
                        } else {
                            toastr.error(res.message, res.title + ":");
                        }
                    },
                    error: function (error) {
                        console.log("No se ha podido obtener la información");
                    }
                })
            }
        });
    } else {
        swal({
            title: "Para eliminar el suministro, no debe tener INSUMOS PRESTADOS, ni en STOCK",
            icon: "warning",
        });
    }
}

function vaciarStock(suministroId) {
    var stock = $("#" + suministroId).children().eq(7).text(); {
        swal({
            title: "¿Está seguro que desea el STOCK?",
            buttons: {
                cancel: "Cancelar",
                catch: {
                    text: "Vaciar",
                    value: "catch",
                },
            },
            icon: "warning",
        }).then((value) => {
            if (value == "catch") {
                $.ajax({
                    type: "PUT",
                    url: "Almacen/VaciarStock",
                    data: {
                        suministroId: suministroId, stock: stock,
                    },
                    dataType: "json",
                    async: true,
                    success: function (res) {
                        if (res.status) {
                            toastr.success(res.message, res.title + ":");
                            mostrarInfoSuministros();
                        } else {
                            toastr.error(res.message, res.title + ":");
                        }
                    },
                    error: function (error) {
                        console.log("No se ha podido obtener la información");
                    }
                })
            }
        });
    }
}