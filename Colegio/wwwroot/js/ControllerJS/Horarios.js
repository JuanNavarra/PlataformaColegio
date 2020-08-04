$(document).ready(function () {
    $('.select2').select2();
    $('#timepicker').datetimepicker({
        format: 'LT'
    })
    $('#timepicker_fin').datetimepicker({
        format: 'LT'
    })
    mostarMaterias();
})

function mostarMaterias() {
    $.ajax({
        type: "GET",
        url: "Horarios/MostrarMarterias",
        data: {},
        dataType: "json",
        async: true,
        success: function (res) {
            $("#listado_materias").empty();
            $("#materias_formulario").empty();
            $.each(res, function (index, item) {
                ca = "<div class='badge badge-" + item.color + " materias-listas'><p style='margin-top:1px;color: white;'>" + item.nombre + "</p></div>"
                $("#listado_materias").append(ca);
            });
            ca = "<option value='0'>Seleccione</option>"
            $("#materias_formulario").append(ca);
            $.each(res, function (index, item) {
                ca = "<option value='" + item.materiaId + "'>" + item.nombre + "</option>"
                $("#materias_formulario").append(ca);
            });
        },
        error: function (error) {
            toastr.error("No se ha podido obtener la información");
        }
    })
}


function guardarMaterias(color) {
    if (validarCampos("materia-color")) {
        var materiaObj = new Object({
            Color: color, Codigo: $("#codigo_materia").val(),
            Nombre: $("#nombre_materia").val(), Descripcion: $("#desc_materia").val()
        })
        $.ajax({
            type: "POST",
            url: "Horarios/GuardarMarterias",
            data: {
                materia: JSON.stringify(materiaObj)
            },
            dataType: "json",
            async: true,
            success: function (res) {
                if (res.status) {
                    $("#codigo_materia").val("");
                    $("#nombre_materia").val(""); $("#desc_materia").val("");
                    toastr.success(res.message, res.title + ":");
                    mostarMaterias();
                } else {
                    toastr.error(res.message, res.title + ":");
                }
            },
            error: function (error) {
                toastr.error("No se ha podido obtener la información");
            }
        })
    } else {
        toastr.warning("¡Hay campos obligatorios vacios!");
    }
}

function agregarMateriaHorario() {
    if (validarCampos("agregar-horario")) {
        var horaIni = $("#hora_ini").val().replace(" ", "-");
        var horaFin = $("#hora_fin").val().replace(" ", "-");
        if ((horaFin.includes(":00") || horaFin.includes(":30")) && (horaIni.includes(":00") || horaIni.includes(":30"))) {
            var materiaObj = new Object({
                materia: $("#materias_formulario").val(), dia: $("#dia_semana_horario").val(),
                horaIni: horaIni, horaFin: horaFin
            })
            $.ajax({
                type: "POST",
                url: "Horarios/AgregarMateriasHorario",
                data: {
                    materia: JSON.stringify(materiaObj)
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
                    toastr.error("No se ha podido obtener la información");
                }
            })
        } else {
            toastr.warning("Las horas de las materias tienen que ser :30 ó :00 para guardarse en el horarios", "¡Atención!");
        }
    } else {
        toastr.warning("¡Hay campos obligatorios vacios!");
    }
}