$(document).ready(function () {
    $('.select2').select2();
    $('#timepicker').datetimepicker({
        format: 'LT'
    })
    $('#timepicker_fin').datetimepicker({
        format: 'LT'
    })
    mostrarMaterias();
    mostarCursos();

    $('#cursos_formulario_horario').on('select2:select', function (e) {
        if ($(this).val() == "0") {
            $("#hora_ini").prop("disabled", true).val("");
            $("#hora_fin").prop("disabled", true).val("");
            $("#materias_formulario").prop("disabled", true).select2('val', "0");
            $("#dia_semana_horario").prop("disabled", true).val("0");
            $("#btn_formulario_horarios").prop("disabled", true);
        } else {
            $("#hora_ini").prop("disabled", false).val("");
            $("#hora_fin").prop("disabled", false).val("");
            $("#materias_formulario").prop("disabled", false).select2('val', "0");
            $("#dia_semana_horario").prop("disabled", false).val("0");
            $("#btn_formulario_horarios").prop("disabled", false);
        }
        mostrarHorasMaterias();
    });
    mostrarHorasMaterias();

    $("#btn_add_horario").on("click", function () {
        if ($('#cursos_formulario_horario').val() != "0") {
            $("#hora_ini").prop("disabled", true).val("");
            $("#hora_fin").prop("disabled", true).val("");
            $("#materias_formulario").prop("disabled", true).select2('val', "0");
            $("#dia_semana_horario").prop("disabled", true).val("0");
            $("#btn_formulario_horarios").prop("disabled", true);
            $('#cursos_formulario_horario').select2('val', "0");
            mostrarHorasMaterias();
        }
    });
})

function mostrarMaterias() {
    $.ajax({
        type: "GET",
        url: "MostrarMarterias",
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
            url: "GuardarMarterias",
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
                    mostrarMaterias();
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
            var horarioObj = new Object({
                MateriaId: $("#materias_formulario").val(), CursoId: $('#cursos_formulario_horario').val(),
                HoraIni: $("#dia_semana_horario").val() + "-" + horaIni.replace(" ", "-").replace(":", "_"),
                HoraFin: $("#dia_semana_horario").val() + "-" + horaFin.replace(" ", "-").replace(":", "_"),
            })
            $.ajax({
                type: "POST",
                url: "AgregarMateriasHorario",
                data: {
                    horario: JSON.stringify(horarioObj)
                },
                dataType: "json",
                async: true,
                success: function (res) {
                    if (res.status) {
                        $('#cursos_formulario_horario').prop("disabled", true);
                        toastr.success(res.message, res.title + ":");
                        mostrarHorasMaterias();
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

function mostarCursos() {
    $.ajax({
        type: "GET",
        url: "MostrarCursos",
        data: {},
        dataType: "json",
        async: true,
        success: function (res) {
            $("#cursos_formulario_horario").empty();
            ca = "<option value='0'>Seleccione</option>"
            $("#cursos_formulario_horario").append(ca);
            $.each(res, function (index, item) {
                ca = "<option value='" + item.cursoId + "'>" + item.nombre + "</option>"
                $("#cursos_formulario_horario").append(ca);
            });
        },
        error: function (error) {
            toastr.error("No se ha podido obtener la información");
        }
    })
}

function mostrarHorasMaterias() {
    $.ajax({
        type: "GET",
        url: "MostrarHorasMaterias",
        data: {
            cursoId: $('#cursos_formulario_horario').val(),
        },
        dataType: "json",
        async: true,
        success: function (res) {
            $("td").children().remove();
            if (res.length > 0) {
                $.each(res.sort(sort_by('orden', false, parseInt)), function (index, item) {
                    $("#" + item.horaIni).append("<div style='cursor: pointer;' class='badge badge-" + item.color + " materias-listas' onclick=\"eliminarHoriorMateria('" + item.materia + "'," + item.id + ");\"><p style='margin-top:1px;color: white;'>" + item.materia + "</p></div>");
                    $("#" + item.horaFin).append("<div style='cursor: pointer;'class='badge badge-" + item.color + " materias-listas' onclick=\"eliminarHoriorMateria('" + item.materia + "'," + item.id + ");\"><p style='margin-top:1px;color: white;'>" + item.materia + "</p></div>");
                    $.each(item.intervalo, function (_index, _item) {
                        $("#" + _item).append("<div style='cursor: pointer;' class='badge badge-" + item.color + " materias-listas' onclick=\"eliminarHoriorMateria('" + item.materia + "'," + item.id + ");\"><p style='margin-top:1px;color: white;'>" + item.materia + "</p></div>");
                    });
                })
            }
        },
        error: function (error) {
            toastr.error("No se ha podido obtener la información");
        }
    })
}

function eliminarHoriorMateria(materia, id) {
    swal({
        title: "Está seguro que desea eliminar " + materia + " de éste horario",
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
                url: "EliminarHoriorMateria",
                data: {
                    horarioId: id,
                },
                dataType: "json",
                async: true,
                success: function (res) {
                    if (res.status) {
                        toastr.success(res.message, res.title + ":");
                        mostrarHorasMaterias();
                    } else {
                        toastr.error(res.message, res.title + ":");
                    }
                },
                error: function (error) {
                    toastr.error("No se ha podido obtener la información");
                }
            })
        }
    });
}