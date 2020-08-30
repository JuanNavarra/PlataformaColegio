$(document).ready(function () {
    $('.select2').select2();

    $("#buscar_profesor_horario").on("keyup", function () {
        if ($(this).val() != "") {
            $("#buscar_nombre_profesor").prop("disabled", true).select2('val', "0");
        } else {
            $("#buscar_nombre_profesor").prop("disabled", false).select2('val', "0");
        }
    });

    $("#buscar_nombre_profesor").on("select2:select", function () {
        if ($(this).val() != "0") {
            $("#buscar_profesor_horario").prop("disabled", true).val("");
        } else {
            $("#buscar_profesor_horario").prop("disabled", false).val("");
        }
    });
    cargarProfesores();

    $("#nombre_curso").on("select2:select", function () {
        if ($(this).val() != "0") {
            $("#nombre_materia").prop("disabled", false).select2('val', "0");
            mostrarMaterias($(this).val());
        } else {
            $("#nombre_materia").prop("disabled", true).select2('val', "0");
        }
        $("#dia_semana").prop("disabled", true).select2('val', "0");
        $("#horario_inicial_final").prop("disabled", true).select2('val', "0");
        $("#btn_agregar_materia_profesor").prop("disabled", true);
    });

    $("#nombre_materia").on("select2:select", function () {
        if ($(this).val() != "0") {
            $("#dia_semana").prop("disabled", false).select2('val', "0");
            mostrarDiasSemana($(this).val(), $("#nombre_curso").val());
        } else {
            $("#dia_semana").prop("disabled", true).select2('val', "0");
        }
        $("#horario_inicial_final").prop("disabled", true).select2('val', "0");
        $("#btn_agregar_materia_profesor").prop("disabled", true);
    });

    $("#dia_semana").on("select2:select", function () {
        if ($(this).val() != "0") {
            $("#horario_inicial_final").prop("disabled", false).select2('val', "0");
            MostrarHorarios($("#dia_semana option:selected").text(), $("#nombre_materia").val(), $("#nombre_curso").val());
            $("#btn_agregar_materia_profesor").prop("disabled", false);
        } else {
            $("#horario_inicial_final").prop("disabled", true).select2('val', "0");
            $("#btn_agregar_materia_profesor").prop("disabled", true);
        }
    });
})

function buscarProfesor() {
    var busqueda = $("#buscar_profesor_horario").val() != "" ? $("#buscar_profesor_horario").val() :
        $("#buscar_nombre_profesor").val() != "0" ? $("#buscar_nombre_profesor").val() : "";
    if (busqueda != "") {
        $("#tbody_horarios_profesores").empty();
        $.ajax({
            type: "GET",
            url: "MostrarHorariosProfesor",
            data: {
                busqueda: busqueda,
            },
            dataType: "json",
            async: true,
            success: function (res) {
                if (res != null) {
                    mostrarInfoSuministros(busqueda);
                    $("#card_horarios").empty();
                    $("#div_horarios_enlazados").removeAttr("style");
                    $("#div_table_horarios_asignados").removeAttr("style");
                    $("#formulario_horarios_disponibles").removeAttr("style");
                    $("#documento_profesor_enlace").val(res.profesor.numeroDocumento);
                    $("#nombre_profesor_enlace").val(res.profesor.primerNombre);
                    $("#usuario_profesor_enlace").val(res.profesor.usuario);
                    $("#correo_profesor_enlace").val(res.profesor.correoPersonal);
                    $("#buscar_profesor_horario").val("").prop("disabled", false);
                    $("#buscar_nombre_profesor").prop("disabled", false).select2('val', "0");
                    if (res.cursos != null) {
                        $("#nombre_curso").empty();
                        ca = '<option value="0">Seleccione</option>'
                        $("#nombre_curso").append(ca);
                        $.each(res.cursos, function (index, item) {
                            cv = "<option value=" + item.cursoId + ">" + item.nombre + "</option>"
                            $("#nombre_curso").append(cv);
                        })
                    }
                } else {
                    $("#card_horarios").empty();
                    $("#div_horarios_enlazados").css("display", "none");
                    $("#div_table_horarios_asignados").css("display", "none");
                    $("#formulario_horarios_disponibles").css("display", "none");
                    $("#documento_profesor_enlace").val("");
                    $("#nombre_profesor_enlace").val("");
                    $("#usuario_profesor_enlace").val("");
                    $("#correo_profesor_enlace").val("");
                    $("#buscar_profesor_horario").val("").prop("disabled", false);
                    $("#buscar_nombre_profesor").prop("disabled", false).select2('val', "0");
                    toastr.error("No existe ningun profesor relacionado con esta busqueda");
                }
            },
            error: function (error) {
                toastr.error("No se ha podido obtener la información");
            }
        })
    } else {
        toastr.warning("Necesitas buscar por algun parametro");
    }
}

function cargarProfesores() {
    $.ajax({
        type: "GET",
        url: "CargarProfesores",
        data: {},
        dataType: "json",
        async: true,
        success: function (res) {
            if (res != null) {
                var ca = "";
                $("#buscar_nombre_profesor").empty();
                ca = '<option value="0">Seleccione</option>'
                $("#buscar_nombre_profesor").append(ca);
                $.each(res, function (index, item) {
                    cv = "<option value=" + item.personaId + ">" + item.primerNombre + "</option>"
                    $("#buscar_nombre_profesor").append(cv);
                });
            }
        },
        error: function (error) {
            toastr.error("No se ha podido obtener la información");
        }
    })
}

function mostrarMaterias(cursoId) {
    $.ajax({
        type: "GET",
        url: "MostrarMarterias",
        data: {
            cursoId: cursoId
        },
        dataType: "json",
        async: true,
        success: function (res) {
            $("#nombre_materia").empty();
            ca = "<option value='0'>Seleccione</option>"
            $("#nombre_materia").append(ca);
            $.each(res, function (index, item) {
                ca = "<option value='" + item.materiaId + "'>" + item.nombre + "</option>"
                $("#nombre_materia").append(ca);
            });
        },
        error: function (error) {
            toastr.error("No se ha podido obtener la información");
        }
    })
}

function mostrarDiasSemana(materiaId, cursoId) {
    $.ajax({
        type: "GET",
        url: "MostrarDiasSemana",
        data: {
            materiaId: materiaId, cursoId: cursoId,
        },
        dataType: "json",
        async: true,
        success: function (res) {
            $("#dia_semana").empty();
            ca = "<option value='0'>Seleccione</option>"
            $("#dia_semana").append(ca);
            $.each(res, function (index, item) {
                ca = "<option value='" + item.horarioId + "'>" + item.dia + "</option>"
                $("#dia_semana").append(ca);
            });
        },
        error: function (error) {
            toastr.error("No se ha podido obtener la información");
        }
    })
}

function MostrarHorarios(dia, materiaId, cursoId) {
    $.ajax({
        type: "GET",
        url: "MostrarHorariosForm",
        data: {
            dia: dia, materiaId: materiaId, cursoId: cursoId,
        },
        dataType: "json",
        async: true,
        success: function (res) {
            $("#horario_inicial_final").empty();
            ca = "<option value='0'>Seleccione</option>"
            $("#horario_inicial_final").append(ca);
            $.each(res, function (index, item) {
                ca = "<option value='" + item.id + "'>" + item.horario + "</option>"
                $("#horario_inicial_final").append(ca);
            });
        },
        error: function (error) {
            toastr.error("No se ha podido obtener la información");
        }
    })
}

function agregarHorario() {
    if (validarCampos("enlace-horario")) {
        var idHorario = $("#horario_inicial_final").val();
        var documento = $("#documento_profesor_enlace").val();
        if (checkId(idHorario)) {
            $("#nombre_curso").select2('val', "0");
            $("#nombre_materia").prop("disabled", true).select2('val', "0");
            $("#dia_semana").prop("disabled", true).select2('val', "0");
            $("#horario_inicial_final").prop("disabled", true).select2('val', "0");
            return toastr.warning("¡Ya existe un horario creado con estos datos!"
                , "Cambiar datos del horario");
        }
        $.ajax({
            type: "POST",
            url: "AgregarEnlaceProfesorHorario",
            data: {
                idHorario: idHorario, documento: documento,
            },
            dataType: "json",
            async: true,
            success: function (res) {
                if (res.status) {
                    $("#nombre_curso").select2('val', "0");
                    $("#nombre_materia").prop("disabled", true).select2('val', "0");
                    $("#dia_semana").prop("disabled", true).select2('val', "0");
                    $("#horario_inicial_final").prop("disabled", true).select2('val', "0");
                    mostrarInfoSuministros(documento);
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
        toastr.warning("¡Campos resaltados son obligatorios!");
    }
}

function checkId(id) {
    var table = document.getElementById("tbody_horarios_profesores");
    var igual = false;
    for (var i = 0; i < table.children.length; i++) {
        igual = table.children[i].id == id ? true : false;
    }
    return igual
}


function mostrarInfoSuministros(documento) {
    $.ajax({
        type: "GET",
        url: "MostrarEnlaceProfesorHorario",
        data: {
            documento: documento
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.length > 0) {
                $("#tbody_horarios_profesores").empty();
                $.each(res, function (index, item) {
                    ca = "<tr id=" + item.id + ">"
                    ca += "<td style='text-align:center'>" + (index + 1) + "</td>"
                    ca += "<td style='text-align:center'>" + item.curso + "</td>"
                    ca += "<td style='text-align:center'>" + item.materia + "</td>"
                    ca += "<td style='text-align:center'>" + item.dia + "</td>"
                    ca += "<td style='text-align:center'>" + item.horario + "</td>"
                    ca += "<td style='text-align:center'>"
                    ca += "<a href='#' onclick=\"eliminarSuministros('" + item.id + "');\"><i class='fas fa-trash'></i></a>"
                    ca += "</td >"
                    ca += "</tr>"
                    $("#tbody_horarios_profesores").append(ca);
                });
            }
        },
        error: function (error) {
            console.log("No se ha podido obtener la información");
        }
    })
}