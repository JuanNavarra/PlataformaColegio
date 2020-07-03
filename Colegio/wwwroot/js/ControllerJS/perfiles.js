$(document).ready(function () {
    ejecutarDataTable("detalle_perfiles_table")
    //cargarRoles()
    cargarModulos();
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
        type: "POST",
        url: "Perfiles/CargarModulos",
        data: {},
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.result = "ok") {
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
            if (res.result = "ok") {
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
    var selectModulos = document.getElementById("modal_nombre_modulo");
    for (var i = 0; i < selectModulos.length; i++) {

        var item = {
            ModuloId: $("#modal_nombre_modulo").val()[i],
        };
        if (item.ModuloId != undefined) {
            moduloArray.push(item);
        }
    }

    var subModuloArray = new Array();
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
            autorizacion: $("#modal_nombre_autorizacion").val(), descripcion: $("#modal_descripcion_autorizacion").val()
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.result) {
                $(document).Toasts('create', {
                    class: 'bg-success',
                    title: res.title,
                    body: res.message
                })
            } else {
                $(document).Toasts('create', {
                    class: 'bg-danger',
                    title: res.title,
                    body: res.message
                })
            }
        },
        error: function (error) {
            console.log("No se ha podido obtener la información");
        }
    })
}

//aun no se usa
function cargarRoles() {
    $.ajax({
        type: "POST",
        url: "Perfiles/CargarRoles",
        data: {},
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.result = "ok") {
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
