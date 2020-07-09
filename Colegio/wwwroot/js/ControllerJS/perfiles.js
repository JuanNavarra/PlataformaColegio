$(document).ready(function () {
    ejecutarDataTable("detalle_perfiles_table")

    cargarModulos();
    cargarPermisosCRUD()
    $('.select2').select2()

    $("#modal_nombre_modulo").on("select2:close", function (e) {
        if ($(this).val() == "") {
            $("#customCheckbox1").prop("disabled", true);
            $("#modal_siguiente_detalle").css("display", "none");
            $("#modal_guardar_cambios").removeAttr("style");
            if ($("#customCheckbox1").is(':checked')) {
                $("#customCheckbox1").prop('checked', false)
            }
        }
    });

    $('#modal_nombre_modulo').on('select2:select', function (e) {
        if ($(this).val() != "") {
            $("#customCheckbox1").prop("disabled", false);
        }
    });

    $('#customCheckbox1').on("click", function () {
        if ($(this).is(':checked')) {
            $("#modal_guardar_cambios").css("display", "none");
            $("#modal_siguiente_detalle").removeAttr("style");
            cargarSubModulos();
        } else {
            $("#modal_siguiente_detalle").css("display", "none");
            $("#modal_guardar_cambios").removeAttr("style");
            $("#modal_submodulos").val("");
        }
    })

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

    $("#modal_siguiente_detalle").on("click", function (e) {
        e.preventDefault();
        $("#custom-tabs-one-profile-tab").addClass("active");
        $("#custom-tabs-one-profile").addClass("active").addClass("show");
        $("#custom-tabs-one-home-tab").removeClass("active");
        $("#custom-tabs-one-home").removeClass("show").removeClass("active");
        $("#modal_atras_detalle").removeAttr("style");
        $("#modal_guardar_cambios").removeAttr("style");
        $(this).css("display", "none");
        cargarSubModulos();
    })

    $("#modal_atras_detalle").on("click", function (e) {
        e.preventDefault();
        $("#custom-tabs-one-profile-tab").removeClass("active").removeClass("show");
        $("#custom-tabs-one-profile").removeClass("active").removeClass("show");
        $("#custom-tabs-one-home-tab").addClass("active").addClass("show");
        $("#custom-tabs-one-home").addClass("show").addClass("active");
        $("#modal_siguiente_detalle").removeAttr("style");
        $("#modal_guardar_cambios").css("display", "none");
        $(this).css("display", "none");
    });


})

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
                    ca = "<option value=" + item.subModuloId + "-" + item.moduloId + ">" + item.nombre + " (" + item.descripcion + ")</option>"
                    $("#modal_submodulos").append(ca);
                });
            }
        },
        error: function (error) {
            console.log("No se ha podido obtener la información");
        }
    })
}

function guardarModulos() {
    var moduloArray = new Array();
    var subModuloArray = new Array();
    var permisoCRUDArray = new Array();

    var selectModulos = document.getElementById("modal_nombre_modulo");
    for (var i = 0; i < selectModulos.length; i++) {

        var item = {
            ModuloId: $("#modal_nombre_modulo").val()[i],
        };
        if (item.ModuloId != undefined) {
            moduloArray.push(item);
        }
    }

    var selectCRUD = document.getElementById("modal_nombre_crud");
    for (var i = 0; i < selectCRUD.length; i++) {

        var item = {
            PermisoId: $("#modal_nombre_crud").val()[i],
        };
        if (item.PermisoId != undefined) {
            permisoCRUDArray.push(item);
        }
    }

    var selectSubModulo = document.getElementById("modal_submodulos2");
    for (var i = 0; i < selectSubModulo.length; i++) {
        var dato = $("#modal_submodulos2").val()[i].split('-')
        var item = {
            SubModuloId: dato[0],
            ModuloId: dato[1]
        };
        if (item.SubModuloId != undefined) {
            subModuloArray.push(item);
        }
    }

    $.ajax({
        type: "POST",
        url: "Perfiles/GuardarAutorizaciones",
        data: {
            modulo: JSON.stringify(moduloArray), subModulo: JSON.stringify(subModuloArray), rol: $("#modal_nombre_perfil").val(),
            crud: JSON.stringify(permisoCRUDArray), descripcion: $("#modal_descripcion_autorizacion").val()
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.result.status) {
                $("#modal_submodulos").html('');
                $("#modal_submodulos1").html('');
                $("#modal_nombre_perfil").val('');
                $("#modal_nombre_modulo").trigger('change').val("");
                $("#modal_nombre_crud").trigger('change').val("");
                $("#modal_descripcion_autorizacion").val('');
                $("#customCheckbox1").prop('checked', false);
                $("#customCheckbox1").prop("disabled", true);
                $("#modal_siguiente_detalle").css("display", "none");
                $("#modal_guardar_cambios").removeAttr("style");
                $("#modal_crear_autorizacion").toggle();
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

//aun no se usa
function cargarRoles() {
    $.ajax({
        type: "GET",
        url: "Perfiles/CargarRoles",
        data: {},
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.result == "ok") {
                var ca = "";
                $.each(res.roles, function (index, item) {
                    ca = "<option value=" + item.rolId + ">" + item.nombreRol + "</option>"
                    $("#modal_nombre_perfil_existente").append(ca);
                });
            }
        },
        error: function (error) {
            console.log("No se ha podido obtener la información");
        }
    })
}

function cargarPermisosCRUD() {
    $.ajax({
        type: "GET",
        url: "Perfiles/CargarPermisosCRUD",
        data: {},
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.result == "ok") {
                var ca = "";
                $.each(res.data, function (index, item) {
                    ca = "<option value=" + item.permisoId + ">" + item.nombre + " (" + item.descripcion + ")</option>"
                    $("#modal_nombre_crud").append(ca);
                });
            }
        },
        error: function (error) {
            console.log("No se ha podido obtener la información");
        }
    })
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
                    ca += "<li class='list-group-item active'>" + item.nombreModulo + "</li>"
                    if (res.data.modulos[index].relaciones.length > 0) {
                        $.each(res.data.modulos[index].relaciones, function (index, item2) {
                            ca += "<li class='list-group-item'>" + item2.nombreSubModulo + "</li>"
                        });
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
                console.log(res.data.usuarios)
            }
        },
        error: function (error) {
            console.log("no se ha podido obtener la información");
        }
    })
}

function modalEliminarAuorizacion(rolId) {
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
                var op = value == true ? true : false
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
                            toastr.success(res.title + ": " + res.message);
                        } else {
                            toastr.error(res.title + ": " + res.message);
                        }
                    },
                    error: function (error) {
                        toastr.error("Contacte con el adminstrador");
                    }
                })
            })
        }
    })

}