$(document).ready(function () {
    ejecutarDataTable("detalle_perfiles_table");

    $('.select2').select2()

    $("#modal_nombre_modulo").on("select2:close", function (e) {
        if ($(this).val() == "") {
            $("#customCheckbox1").prop("disabled", true);
            if ($("#customCheckbox1").is(':checked')) {
                $("#customCheckbox1").prop('checked', false)
            }
        }
        if ($('#actualizar_no').is(':checked')) {
            cargarSubModulos();
        }
    });

    $('#modal_nombre_modulo').on('select2:select', function (e) {
        if ($(this).val() != "") {
            if ($(this).is(':checked')) {
                cargarSubModulos();
            }
            $("#customCheckbox1").prop("disabled", false);
            if ($("#customCheckbox1").is(':checked')) {
                $("#profile_tab").removeAttr("style");
            }
        }
    });

    $('#actualizar_no').on('switchChange.bootstrapSwitch', function () {
        if ($(this).is(':checked')) {
            vaciarModal(false);
            cargarModulos();
            $("#custom-tabs-one-home-tab").addClass("active").addClass("show");
            $("#custom-tabs-one-home").addClass("show").addClass("active");
            $("#custom-tabs-one-profile-tab").removeClass("active").removeClass("show");
            $("#profile_tab").css("display", "none");
            $("#custom-tabs-one-profile").removeClass("active").removeClass("show");
        } else {
            cargarModalActualizar();
        }
    });

    $('#customCheckbox1').on("click", function () {
        if ($(this).is(':checked')) {
            $("#modal_guardar_cambios").css("display", "none");
            $("#modal_siguiente_detalle").removeAttr("style");
            $("#profile_tab").removeAttr("style");
        } else {
            $("#profile_tab").css("display", "none");
            $("#modal_submodulos").val("");
            $("#modal_submodulos2").html("")
        }
    })

    $("#modal_siguiente_detalle").on("click", function (e) {
        e.preventDefault();
        if ($("#custom-tabs-one-home-tab").hasClass("active")) {
            if ($("#modal_nombre_perfil").val() != "" && $("#modal_nombre_modulo").val() != "" && $("#modal_descripcion_autorizacion").val() != "") {
                if ($("#customCheckbox1").is(":checked")) {
                    $("#custom-tabs-one-profile-tab").addClass("active");
                    $("#custom-tabs-one-profile").addClass("active").addClass("show");
                    $("#modal_guardar_cambios").css("display", "none");
                    if (!$('#actualizar_no').is(':checked') && $("#id_rol_actualizar").val() != "") {
                        cargarModalActualizar();
                        $("#btnAllRight").css("display", "none");
                        $("#btnRight").css("display", "none");
                        $("#btnLeft").css("display", "none");
                        $("#btnAllLeft").css("display", "none");
                    } else {
                        cargarSubModulos();
                        $("#btnAllRight").removeAttr("style");
                        $("#btnRight").removeAttr("style");
                        $("#btnLeft").removeAttr("style");
                        $("#btnAllLeft").removeAttr("style");
                    }
                } else {
                    $("#custom-tabs-one-permisos-tab").addClass("active");
                    $("#custom-tabs-one-permisos").addClass("active").addClass("show");
                    $("#modal_guardar_cambios").removeAttr("style");
                    if ($("#id_rol_actualizar").val() != "") {
                        if ($('#actualizar_no').is(':checked')) {
                            cargarPermisosCRUD();
                        }
                        else {
                            cargarModalActualizar();
                            $("#modal_guardar_cambios").css("display", "none");
                        }
                    } else {
                        cargarPermisosCRUD();
                        $("#modal_guardar_cambios").removeAttr("style");
                    }
                    $(this).css("display", "none");
                }
                $("#custom-tabs-one-home-tab").removeClass("active");
                $("#custom-tabs-one-home").removeClass("show").removeClass("active");
                $("#modal_atras_detalle").removeAttr("style");
                if ($('#actualizar_no').is(':checked')) {
                    cargarSubModulos();
                }
            } else {
                toastr.warning("¡Falta llenar los campos obligatorios!");
            }
        }
        else
            if ($("#custom-tabs-one-profile-tab").hasClass("active")) {
                if ($("#modal_submodulos2").val() != "") {
                    $("#custom-tabs-one-permisos-tab").addClass("active");
                    $("#custom-tabs-one-permisos").addClass("active").addClass("show");
                    $("#custom-tabs-one-profile-tab").removeClass("active");
                    $("#custom-tabs-one-profile").removeClass("show").removeClass("active");
                    $("#modal_guardar_cambios").removeAttr("style");
                    $(this).css("display", "none");
                    $("#modal_atras_detalle").removeAttr("style");
                    if ($('#actualizar_no').is(':checked') || $("#id_rol_actualizar").val() == "") {
                        cargarPermisosCRUD();
                        cargarSubModulos();
                    }
                } else {
                    toastr.warning("¡Al menos tiene que tener un submodulo seleccionado!");
                }
            }
            else {
                $(this).css("display", "none");
                $("#modal_atras_detalle").removeAttr("style");
                if ($('#actualizar_no').is(':checked')) {
                    cargarSubModulos();
                }
            }
    })

    $("#modal_atras_detalle").on("click", function (e) {
        e.preventDefault();
        if ($("#custom-tabs-one-permisos-tab").hasClass("active")) {
            if ($("#customCheckbox1").is(":checked")) {
                $("#custom-tabs-one-profile-tab").addClass("active").addClass("show");
                $("#custom-tabs-one-profile").addClass("show").addClass("active");
                $("#custom-tabs-one-permisos-tab").removeClass("active").removeClass("show");
                $("#custom-tabs-one-permisos").removeClass("active").removeClass("show");
                $("#modal_guardar_cambios").css("display", "none");
                $(this).removeAttr("style");
                $("#modal_siguiente_detalle").removeAttr("style");
                if ($("#id_rol_actualizar").val() != "") {
                    if (!$('#actualizar_no').is(':checked')) {
                        cargarModalActualizar();
                        $("#btnAllRight").css("display", "none");
                        $("#btnRight").css("display", "none");
                        $("#btnLeft").css("display", "none");
                        $("#btnAllLeft").css("display", "none");
                    }
                    else {
                        cargarSubModulos();
                        $("#modal_atras_detalle").removeAttr("style");
                        $("#btnAllRight").removeAttr("style");
                        $("#btnRight").removeAttr("style");
                        $("#btnLeft").removeAttr("style");
                        $("#btnAllLeft").removeAttr("style");
                        $("#modal_submodulos2").html('');
                    }
                } else {
                    cargarSubModulos();
                    $("#modal_atras_detalle").removeAttr("style");
                    $("#btnAllRight").removeAttr("style");
                    $("#btnRight").removeAttr("style");
                    $("#btnLeft").removeAttr("style");
                    $("#btnAllLeft").removeAttr("style");
                    $("#modal_submodulos2").html('');
                }
            } else {
                $("#custom-tabs-one-home-tab").addClass("active").addClass("show");
                $("#custom-tabs-one-home").addClass("show").addClass("active");
                $("#custom-tabs-one-permisos-tab").removeClass("active").removeClass("show");
                $("#custom-tabs-one-permisos").removeClass("active").removeClass("show");
                $("#modal_guardar_cambios").css("display", "none");
                $(this).css("display", "none");
                $("#modal_siguiente_detalle").removeAttr("style");
                if ($("#id_rol_actualizar").val() != "") {
                    if (!$('#actualizar_no').is(':checked')) {
                        cargarModalActualizar();
                    } else {
                        cargarModulos();
                        $(this).css("display", "none");
                        $("#modal_nombre_perfil").val('');
                        $("#modal_descripcion_autorizacion").val('');
                        $("#profile_tab").css("display", "none");
                        $("#customCheckbox1").prop("disabled", true);
                        $("#customCheckbox1").prop('checked', false);
                    }
                }
                else {
                    cargarModulos();
                    $(this).css("display", "none");
                    $("#modal_nombre_perfil").val('');
                    $("#modal_descripcion_autorizacion").val('');
                    $("#profile_tab").css("display", "none");
                    $("#customCheckbox1").prop("disabled", true);
                    $("#customCheckbox1").prop('checked', false);
                }
            }
        }
        else if ($("#custom-tabs-one-profile-tab").hasClass("active")) {
            $("#custom-tabs-one-home-tab").addClass("active").addClass("show");
            $("#custom-tabs-one-home").addClass("show").addClass("active");
            $("#custom-tabs-one-profile-tab").removeClass("active").removeClass("show");
            $("#custom-tabs-one-profile").removeClass("active").removeClass("show");
            $("#modal_guardar_cambios").css("display", "none");
            $("#modal_siguiente_detalle").removeAttr("style");
            if ($("#id_rol_actualizar").val() != "") {
                if (!$('#actualizar_no').is(':checked')) {
                    cargarModalActualizar();
                } else {
                    cargarModulos();
                    $(this).css("display", "none");
                    $("#modal_nombre_perfil").val('');
                    $("#modal_descripcion_autorizacion").val('');
                    $("#profile_tab").css("display", "none");
                    $("#customCheckbox1").prop("disabled", true);
                    $("#customCheckbox1").prop('checked', false);
                    $("#modal_submodulos2").html('');
                }
            } else {
                cargarModulos();
                $(this).css("display", "none");
                $("#modal_nombre_perfil").val('');
                $("#modal_descripcion_autorizacion").val('');
                $("#profile_tab").css("display", "none");
                $("#customCheckbox1").prop("disabled", true);
                $("#customCheckbox1").prop('checked', false);
                $("#modal_submodulos2").html('');
            }
        }

    });

    $('#btnRight').click(function (e) {
        $('select').moveToListAndDelete('#modal_submodulos', '#modal_submodulos2');
        $("#modal_submodulos2 option").prop("selected", "true");
        e.preventDefault();
    });

    $('#btnAllRight').click(function (e) {
        $('select').moveAllToListAndDelete('#modal_submodulos', '#modal_submodulos2');
        $("#modal_submodulos2 option").prop("selected", "true");
        e.preventDefault();
    });

    $('#btnLeft').click(function (e) {
        $('select').moveToListAndDelete('#modal_submodulos2', '#modal_submodulos');
        $("#modal_submodulos option").prop("selected", "true");

        e.preventDefault();
    });

    $('#btnAllLeft').click(function (e) {
        $('select').moveAllToListAndDelete('#modal_submodulos2', '#modal_submodulos');
        $("#modal_submodulos option").prop("selected", "true");
        e.preventDefault();
    });

    $("#btn_crear_autorizacion").on("click", function () {
        limpiarModal(false, "");
    });

    $(".btn_titulo_actualizar").on("click", function () {
        limpiarModal(true, this);
    });

    $("input[data-bootstrap-switch]").each(function () {
        $(this).bootstrapSwitch('state', $(this).prop('checked'));
    });

});

function limpiarModal(verbo, event) {
    if (verbo) {
        $("#titulo_accion_perfil").html("Actualizar perfil: " + $(event).parent().parent().children().eq(1).text());
        $("#id_rol_actualizar").val($(event).parent().parent().attr("id").replace("auth_", ""));
        $("#row_actualizar_no").removeAttr("style");
        $("#actualizar_no").bootstrapSwitch('state', false);
        cargarModalActualizar();
    } else {
        $("#titulo_accion_perfil").html("Creación de perfiles");
        vaciarModal(false);
        cargarModulos();
        $("#customCheckbox1").prop('checked', false);
        $("#row_actualizar_no").css("display", "none");
        $("#id_rol_actualizar").val("");
    }
    $("#profile_tab").css("display", "none");
    $("#custom-tabs-one-home-tab").addClass("active");
    $("#custom-tabs-one-home").addClass("active").addClass("show");
    $("#custom-tabs-one-profile-tab").removeClass("active"); 1
    $("#custom-tabs-one-profile").removeClass("show").removeClass("active");
    $("#custom-tabs-one-permisos-tab").removeClass("active");
    $("#custom-tabs-one-permisos").removeClass("show").removeClass("active");
    $("#modal_guardar_cambios").css("display", "none");
    $("#modal_siguiente_detalle").removeAttr("style");
    $("#modal_atras_detalle").css("display", "none");
    $("#customCheckbox1").prop("disabled", true);
    $("#tbody_permisos").html("")
}

function cargarModulos() {
    $.ajax({
        type: "GET",
        url: "Perfiles/CargarModulos",
        data: {},
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.result == "ok") {
                var ca = "";
                $("#modal_nombre_modulo").empty();
                $.each(res.data, function (index, item) {
                    ca = "<option value=" + item.moduloId + ">" + item.nombre + "</option>"
                    $("#modal_nombre_modulo").append(ca);
                });
            }
        },
        error: function (error) {
            console.log("No se ha podido obtener la información");
        }
    })
}

function cargarSubModulos() {
    $.ajax({
        type: "POST",
        url: "Perfiles/CargarSubModulos",
        data: {
            modulos: $("#modal_nombre_modulo").val()
        },
        dataType: "json",
        async: true,
        success: function (res) {
            $('#modal_submodulos').empty();
            if (res.result == "ok") {
                var ca = "";
                $.each(res.data, function (index, item) {
                    ca = "<option value=" + item.subModuloId + "-" + item.moduloId + ">" + item.nombre + " (" + item.descripcion + ")  </option>"
                    $("#modal_submodulos").append(ca);
                });
            }
        },
        error: function (error) {
            console.log("No se ha podido obtener la información");
        }
    })
}

function guardarCambios() {

    var moduloArray = new Array();
    var subModuloArray = new Array();

    var selectModulos = document.getElementById("modal_nombre_modulo");
    for (var i = 0; i < selectModulos.length; i++) {
        var modulo = $("#null-" + $("#modal_nombre_modulo").val()[i]).children().eq(2);
        if (modulo.length > 0) {
            var repl = modulo[0].childNodes[1].innerText;
            var arrayPermisos = repl.replace("×", "").replace("×", ",").replace("×", ",").replace("×", ",");
            var item = {
                ModuloId: $("#modal_nombre_modulo").val()[i],
                PermisosCrud: arrayPermisos
            };
            if (item.ModuloId != undefined) {
                moduloArray.push(item);
            }
        }
        else {
            var item = {
                ModuloId: $("#modal_nombre_modulo").val()[i],
                PermisosCrud: null
            };
            if (item.ModuloId != undefined) {
                moduloArray.push(item);
            }
        }
    }

    var selectSubModulo = document.getElementById("modal_submodulos2");
    if (selectSubModulo.length > 0) {
        for (var i = 0; i < selectSubModulo.length; i++) {
            var dato = $("#modal_submodulos2").val()[i].split('-')
            var submodulo = $("#" + dato[0] + "-" + dato[1]).children().eq(2);
            if (submodulo.length > 0) {
                var repl = submodulo[0].childNodes[1].innerText;
                var arrayPermisos = repl.replace("×", "").replace("×", ",").replace("×", ",").replace("×", ",");
                var item = {
                    SubModuloId: dato[0],
                    ModuloId: dato[1],
                    PermisosCrud: arrayPermisos
                };
                if (item.SubModuloId != undefined) {
                    subModuloArray.push(item);
                }
            }
        }
    }

    $.ajax({
        type: "POST",
        url: "Perfiles/GuardarCambios",
        data: {
            modulo: JSON.stringify(moduloArray), subModulo: JSON.stringify(subModuloArray),
            rol: $("#modal_nombre_perfil").val(), descripcion: $("#modal_descripcion_autorizacion").val(),
            idRol: $("#id_rol_actualizar").val()
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.result.status) {
                vaciarModal(false);
                $('#modal_crear_autorizacion').modal('toggle');
                toastr.success(res.result.title + ": " + res.result.message);
            } else {
                toastr.error(res.result.title + ": " + res.result.message);
            }
        },
        error: function (error) {
            toastr.error(res.result.title + ": " + res.result.message);
        }
    })
}

function vaciarModal(opcion) {
    $("#modal_submodulos").html('').prop("disabled", opcion);
    $("#modal_submodulos2").html('').prop("disabled", opcion);
    $("#modal_nombre_perfil").val('').prop("disabled", opcion);
    $("#modal_descripcion_autorizacion").val('').prop("disabled", opcion);
    $('#modal_nombre_modulo').find('option:selected').remove().end();
    $("#modal_nombre_modulo").val("").prop("disabled", opcion);
    $("#customCheckbox1").prop('checked', opcion);
    $("#customCheckbox1").prop("disabled", true);
}

function modalVerAutorizacion(rolId) {
    $.ajax({
        type: "GET",
        url: "Perfiles/MostrarDetallePerfil",
        data: {
            rolId: rolId
        },
        datatype: "json",
        async: true,
        success: function (res) {
            if (res.result == "ok") {
                $('#tbody_ver_detalle_autorizacion').empty();
                $("#listado_modulos").empty();
                $("#tbody_ver_personas_perfil").empty();
                $('#modal_ver_autorizacion').modal('show');

                var ca = "<tr>"
                ca += "<td>" + res.data.roles.nombreRol + "</td>"
                ca += "<td>" + res.data.roles.fechaCreacion + "</td>"
                ca += "<td>"
                ca += res.data.roles.fechaActualizacion == null ? "Sin registros" : res.data.roles.fechaActualizacion
                ca += "</td>"
                ca += "<td>"
                ca += res.data.roles.ultimoLogin == null ? "Sin registros" : res.data.roles.ultimoLogin
                ca += "</td>"
                ca += "<td>" + res.data.roles.estado + "</td>"
                ca += "</tr>"
                $('#tbody_ver_detalle_autorizacion').append(ca);
                $('#ver_detalle_descripcion').html(res.data.roles.descripcion);

                ca = ""
                $.each(res.data.modulos, function (index, item) {
                    ca = "<div class='col-4'>"
                    ca += "<ul class='list-group'>"
                    if (res.data.modulos[index].relaciones.length > 0) {
                        ca += "<li class='list-group-item active' style='margin-bottom:11%;'>" + item.nombreModulo + "</li>"
                        $.each(res.data.modulos[index].relaciones, function (index2, item2) {
                            ca += "<li class='list-group-item'>"
                            ca += item2.nombreSubModulo
                            $.each(item2.permisos, function (index3, item3) {
                                if (item3.includes("Crear")) {
                                    ca += "<span class='badge badge-success' style='margin-left: 6px;height: 13px;width: 14px;'>  </span>"
                                }
                                else if (item3.includes("Eliminar")) {
                                    ca += "<span class='badge badge-danger' style='margin-left: 6px;height: 13px;width: 14px;'>  </span>"
                                }
                                else if (item3.includes("Actualizar")) {
                                    ca += "<span class='badge badge-warning' style='margin-left: 6px;height: 13px;width: 14px;'>  </span>"
                                }
                                else {
                                    ca += "<span class='badge badge-info' style='margin-left: 6px;height: 13px;width: 14px;background-color: coral !important;'>  </span>"
                                }
                            })
                            ca += "</li>"
                        });
                    } else {
                        ca += "<li class='list-group-item active' style='margin-bottom:11%;'>"
                        ca += item.nombreModulo
                        $.each(res.data.modulos[index].permisos, function (index2, item2) {
                            if (item2.includes("Crear")) {
                                ca += "<span class='badge badge-success' style='margin-left: 6px;height: 13px;width: 14px;'>  </span>"
                            }
                            else if (item2.includes("Eliminar")) {
                                ca += "<span class='badge badge-danger' style='margin-left: 6px;height: 13px;width: 14px;'>  </span>"
                            }
                            else if (item2.includes("Actualizar")) {
                                ca += "<span class='badge badge-warning' style='margin-left: 6px;height: 13px;width: 14px;'>  </span>"
                            }
                            else {
                                ca += "<span class='badge badge-info' style='margin-left: 6px;height: 13px;width: 14px;background-color: coral !important;'>  </span>"
                            }
                        })
                        ca += "</li>"
                    }
                    ca += "</ul>"
                    ca += "</div>"
                    $("#listado_modulos").append(ca);
                });

                ca = "";
                $.each(res.data.usuarios, function (index, item) {
                    var ca = "<tr>"
                    ca += "<td>" + res.data.usuarios[index].nombreUsuario + "</td>"
                    ca += "<td>" + res.data.usuarios[index].nombrePersona + "</td>"
                    ca += "<td>" + res.data.usuarios[index].fechaCreacion + "</td>"
                    ca += "<td>"
                    ca += res.data.usuarios[index].fechaActualizacion == null ? "Sin registros" : res.data.usuarios[index].fechaActualizacion
                    ca += "</td>"
                    ca += "<td>"
                    ca += res.data.usuarios[index].ultimoLogin == null ? "Sin registros" : res.data.usuarios[index].ultimoLogin
                    ca += "</td>"
                    ca += "<td>" + res.data.usuarios[index].estado + "</td>"
                    ca += "</tr>"
                    $('#tbody_ver_personas_perfil').append(ca);
                })
            }
        },
        error: function (error) {
            console.log("no se ha podido obtener la información");
        }
    })
}

function modalEliminarAuorizacion(rolId) {
    var estado = $("#auth_" + rolId).children().eq(6).text();
    if (estado == "ACTIVO") {
        swal({
            title: "¿Desea eliminar éste perfil?",
            buttons: [true, "Continuar"]
        }).then((value) => {
            if (value) {
                swal({
                    title: "Inactivar un perfil o eliminarlo para siempre",
                    text: "Si hay usuarios con este rol, actualice el rol de de los usuarios," +
                        " de lo contrario los usuarios serán eliminados",
                    buttons: ["Inactivar", "Eliminar"]
                }).then((value) => {
                    var op = value ? value : false
                    eliminarAuorizacion(rolId, op);
                })
            }
        })
    } else {
        swal({
            title: "Eliminar el perfil",
            text: "Si hay usuarios con este rol, actualice el rol de de los usuarios," +
                " de lo contrario los usuarios serán eliminados",
            buttons: [true, "Eliminar"]
        }).then((value) => {
            if (value) {
                eliminarAuorizacion(rolId, true);
            }
        })
    }
}


function eliminarAuorizacion(rolId, op) {
    $.ajax({
        type: "POST",
        url: "Perfiles/EliminarPerfiles",
        data: {
            rolId: rolId, op: op
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.status) {
                if (op) {
                    $("#auth_" + rolId).hide();
                }
                toastr.success(res.title + ": " + res.message);
            } else {
                toastr.error(res.title + ": " + res.message);
            }
        },
        error: function (error) {
            toastr.error("Contacte con el adminstrador");
        }
    })
}

function cargarPermisosCRUD() {
    $("#tbody_permisos").empty();
    var subModuloArray = new Array();
    var selectSubModulo = document.getElementById("modal_submodulos2");
    for (var i = 0; i < selectSubModulo.length; i++) {
        var dato = $("#modal_submodulos2").val()[i].split('-')
        var texto = $("#modal_submodulos2").text().trim().split('  ')
        var item = {
            SubModuloId: dato[0],
            ModuloId: dato[1],
            Texto: texto[i]
        };
        if (item.SubModuloId != undefined) {
            subModuloArray.push(item);
        }
    }
    var modulos = $('#modal_nombre_modulo option:selected')
    if (subModuloArray.length > 0) {
        for (var i in modulos) {
            var moduloNombre = modulos[i].innerHTML;
            var moduloId = modulos[i].value;
            if (subModuloArray.find(f => f.ModuloId != moduloId)) {
                var item = {
                    SubModuloId: null,
                    ModuloId: moduloId,
                    Texto: moduloNombre,
                };
                if (item.ModuloId != undefined) {
                    subModuloArray.push(item);
                }
            }
        }
    } else {
        for (var i in modulos) {
            var moduloNombre = modulos[i].innerHTML;
            var moduloId = modulos[i].value;
            var item = {
                SubModuloId: null,
                ModuloId: moduloId,
                Texto: moduloNombre,
            };
            if (item.ModuloId != undefined) {
                subModuloArray.push(item);
            }
        }
    }

    var ca = "";
    $.each(subModuloArray, function (index, item) {
        ca = "<tr id ='" + item.SubModuloId + "-" + item.ModuloId + "'>"
        ca += "<td>" + (index + 1) + "</td>"
        ca += "<td>" + (item.Texto) + "</td>"
        ca += "<td>"
        ca += "<select class='select2' multiple='multiple' name=tags[] style='width:100%;'>"
        ca += "<option value='R'>Leer</option><option value='C'>Crear</option><option value='U'>Actualizar</option><option value='D'>Eliminar</option>"
        ca += "</select>"
        ca += "</td>"
        ca += "</tr>"
        $("#tbody_permisos").append(ca);
    })
    $('.select2').select2()
}

function cargarModalActualizar() {
    $.ajax({
        type: "GET",
        url: "Perfiles/CargarDatosActualizar",
        data: {
            rolId: $("#id_rol_actualizar").val()
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.result == "ok") {
                $("#modal_submodulos2").empty();
                $("#modal_nombre_modulo").empty();
                $("#modal_submodulos").empty();
                $("#tbody_permisos").empty();
                $("#modal_nombre_perfil").val(res.data.rol.nombreRol).prop("disabled", true);
                $("#modal_descripcion_autorizacion").val(res.data.rol.descripcion).prop("disabled", true);
                $("#customCheckbox1").prop('checked', false).prop("disabled", true);
                if (res.data.rol.restringir) {
                    $("#customCheckbox1").prop('checked', true).prop("disabled", true);
                    $("#profile_tab").removeAttr("style");
                }

                $.each(res.data.modulos, function (index, item) {
                    ca = "<option selected value=" + item.moduloId + ">" + item.nombre + "</option>"
                    $("#modal_nombre_modulo").append(ca);
                });
                $.each(res.data.seleccionados, function (index, item) {
                    ca = "<option value=" + item.subModuloId + "-" + item.moduloId + ">" + item.nombre + "</option>"
                    $("#modal_submodulos2").append(ca);
                });
                $.each(res.data.noSeleccionados, function (index, item) {
                    ca = "<option value=" + item.subModuloId + "-" + item.moduloId + ">" + item.nombre + " (" + item.descripcion + ")</option>"
                    $("#modal_submodulos").append(ca);
                });

                $("#modal_nombre_modulo").prop("disabled", true);
                $("#modal_submodulos").prop("disabled", true);
                $("#modal_submodulos2").prop("disabled", true);
                $("#modal_submodulos2 option").prop("selected", "true");

                var cont = 1
                $.each(res.data.permisos, function (index, item) {
                    if (item.relaciones.length > 0) {
                        $.each(item.relaciones, function (index2, item2) {
                            ca = "<tr id ='" + item2.subModuloId + "-" + item2.moduloId + "'>"
                            ca += "<td>" + cont + "</td>"
                            ca += "<td>" + (item2.nombreSubModulo) + " (" + item.nombreModulo + ")</td>"
                            ca += "<td>"
                            ca += "<select disabled class='select2' multiple='multiple' name=tags[] style='width:100%;'>"
                            $.each(item2.permisos, function (index3, item3) {
                                ca += "<option selected value='" + item3 + "'>" + item3 + "</option>"
                            })
                            ca += "</select>"
                            ca += "<td>"
                            ca += "</tr>"
                            $("#tbody_permisos").append(ca);
                            cont++;
                        })
                    } else {
                        ca = "<tr id ='" + item.subModuloId + "-" + item.moduloId + "'>"
                        ca += "<td>" + cont + "</td>"
                        ca += "<td>" + (item.nombreModulo) + "</td>"
                        ca += "<td>"
                        ca += "<select disabled class='select2' multiple='multiple' name=tags[] style='width:100%;'>"
                        $.each(item.permisos, function (index2, item2) {
                            ca += "<option selected value='" + item2 + "'>" + item2 + "</option>"
                        })
                        ca += "</select>"
                        ca += "<td>"
                        ca += "</tr>"
                        $("#tbody_permisos").append(ca);
                    }
                    cont++;
                })
                $('.select2').select2()

            }
        },
        error: function (error) {
            toastr.error("Contacte con el adminstrador");
        }
    })
}