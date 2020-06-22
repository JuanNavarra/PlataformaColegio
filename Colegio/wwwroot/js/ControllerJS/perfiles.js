$(document).ready(function () {
    ejecutarDataTable("detalle_perfiles_table")
    cargarRoles()
})

function checkearPadre() {
    var modalPadre = document.getElementById("modal_es_padre");
    var submodulos = document.getElementById("row_submodulos");
    if (modalPadre.checked == true) {
        submodulos.style.display = "block";
    } else {
        submodulos.style.display = "none";
    }
}

function guardarModulos() {
    var modulo = new Object({
        Nombre: $("#modal_nombre_modulo").val(),
        EtiquetaDom: $("#modal_nombre_etiqueta").val(),Descripcion: $("#modal_descripcion_modulo").val(), 
        EsPadre: $('#modal_es_padre').is(':checked') ? true : false,
    });

    var subModuloArray = new Array();
    var selectElements = document.getElementById("modal_submodulos");
    for (var i = 0; i < selectElements.length; i++) {
        var item = {
            "SubModuloId": $("#modal_submodulos").val()[i]
        };
        if (item.SubModuloId != undefined) {
            subModuloArray.push(item);
        }
    }

    

    $.ajax({
        type: "POST",
        url: "Perfiles/GuardarAutorizaciones",
        data: {
            modulo: JSON.stringify(modulo), subModulo: JSON.stringify(subModuloArray),
            rol: $("#modal_nombre_perfil").val() == "" ? $("#modal_nombre_perfil_existente").val() : $("#modal_nombre_modulo").val(),
            swRol: $("#customSwitch1").is(':checked') ? true : false,
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
                    class: 'bg-warning',
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

function checkearRol() {
    var sw = document.getElementById("customSwitch1");
    var roles_existentes = document.getElementById("row_roles_existentes");
    var crear_roles = document.getElementById("row_crear_roles");
    if (sw.checked == true) {
        roles_existentes.style.display = "block";
        crear_roles.style.display = "none";
        $("#sw_rol").html("Seleccionar un rol existente")
    } else {
        crear_roles.style.display = "block";
        roles_existentes.style.display = "none";
        $("#sw_rol").html("Crear un nuevo rol")
    }
}