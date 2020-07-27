$(document).ready(function () {
    ejecutarDataTable("detalle_perfiles_table");
    $('#datemask').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' });
    $('[data-mask]').inputmask();
    bsCustomFileInput.init();
    $('.select2').select2();
    CargarRoles();

    $("#btn_continuar_contrato").on("click", function (e) {
        e.preventDefault();
        if ($("#tab_datos_personales").hasClass("active")) {
            if (validarCampos("personal")) {
                if ($("#tbody_estudios_personales tr").length > 0) {
                    if ($("#id_persona").val() != "" && $("#persona_actualizar").val() == "") {
                        $("#tab_datos_personales").removeClass("active");
                        $("#datos_personales").removeClass("active");
                        $("#tab_datos_experiencia").addClass("active");
                        $("#datos_experiencia").addClass("active");
                        $("#btn_atras_contrato").removeAttr('style');
                    } else {
                        guardarDatosPersonales();
                    }
                } else {
                    toastr.warning("¡Llenar la tabla de información académica es obligatoria!");
                }
            } else {
                toastr.warning("¡Campos resaltados son obligatorios!");
            }
        } else if ($("#tab_datos_experiencia").hasClass("active")) {
            if ($("#tbody_experiencia_laboral tr").length > 0) {
                if ($("#id_pesrona_experiencia").val() != "" && $("#id_persona_actualizar").val() == "") {
                    $("#tab_datos_experiencia").removeClass("active");
                    $("#datos_experiencia").removeClass("active");
                    $("#datos_laborales").addClass("active");
                    $("#tab_datos_laborales").addClass("active");
                } else {
                    guardarExperiencia();
                }
            } else {
                toastr.warning("¡Llenar la tabla de experiencia laboral es obligatoria!");
            }
        } else if ($("#tab_datos_laborales").hasClass("active")) {
            if (validarCampos("laborales")) {
                if ($("#id_laboral_afiliacion").val() != "" && $("#id_pesrona_experiencia_actualizar").val() == "") {
                    $("#tab_datos_laborales").removeClass("active");
                    $("#datos_laborales").removeClass("active");
                    $("#datos_afiliacion").addClass("active");
                    $("#tab_datos_afiliacion").addClass("active");
                    $("#btn_continuar_contrato").css("display", "none");
                    $("#btn_terminar_actualizar").css("display", "none");
                    $("#btn_terminar_contrato").removeAttr('style');
                } else {
                    $("#btn_terminar_contrato").css("display", "none");
                    $("#btn_terminar_actualizar").removeAttr('style');
                    $("#btn_continuar_contrato").css("display", "none");
                    guardarLaboral();
                }
            } else {
                toastr.warning("¡Campos resaltados son obligatorios!");
            }
        }
    });
    $("#btn_atras_contrato").on("click", function (e) {
        e.preventDefault();
        if ($("#tab_datos_afiliacion").hasClass("active")) {
            $("#tab_datos_afiliacion").removeClass("active");
            $("#datos_afiliacion").removeClass("active");
            $("#tab_datos_laborales").addClass("active");
            $("#datos_laborales").addClass("active");
            $("#btn_terminar_contrato").css('display', 'none');
            $("#btn_terminar_actualizar").css('display', 'none');
            $("#btn_continuar_contrato").removeAttr('style');
            $("#btn_atras_contrato").removeAttr('style');
        } else if ($("#tab_datos_laborales").hasClass("active")) {
            $("#tab_datos_laborales").removeClass("active");
            $("#datos_laborales").removeClass("active");
            $("#datos_experiencia").addClass("active");
            $("#tab_datos_experiencia").addClass("active");
        } else if ($("#tab_datos_experiencia").hasClass("active")) {
            $("#tab_datos_experiencia").removeClass("active");
            $("#datos_experiencia").removeClass("active");
            $("#datos_personales").addClass("active");
            $("#tab_datos_personales").addClass("active");
            $("#btn_atras_contrato").css("display", "none");
        }
    });

    $("#btn_crear_personal").on("click", function () {
        if (!$("#listone").hasClass("show")) {
            $("#collapse_bar_formulario").click();
        }
        limpiarFormulario()
    })
});

function limpiarFormulario() {
    $("#tab_datos_afiliacion").removeClass("active");
    $("#datos_afiliacion").removeClass("active");
    $("#btn_continuar_contrato").removeAttr("style");
    $("#tab_datos_laborales").removeClass("active");
    $("#datos_laborales").removeClass("active");
    $("#btn_atras_contrato").css('display', 'none');
    $("#tab_datos_experiencia").removeClass("active");
    $("#datos_experiencia").removeClass("active");
    $("#btn_terminar_contrato").css('display', 'none');
    $("#datos_personales").addClass("active");
    $("#tab_datos_personales").addClass("active");
    $("#tbody_estudios_personales").empty();
    $("#persona_actualizar").val(""); $("#id_persona").val("");
    $("#primer_nombre").val(""); $("#numero_documento").val("");
    $("#segundo_nombre").val(""); $("#celular").val("");
    $("#primer_apellido").val(""); $("#estado_civil").val("0");
    $("#segundo_apellido").val(""); $("#correo_personal").val("");
    $("#fecha_nacimiento").val(""); $("#direccion").val("");
    $("#barrio").val(""); $("#tipo_documento").val("0");
    $("#tbody_experiencia_laboral").empty();
    $("#tbody_insumolaboral_formurlario").empty();
    $("#id_laboral_afiliacion").val(""); $("#id_pesrona_experiencia_actualizar").val("");
    $("#nombre_cargo").val(""); $("#fecha_alta").val("");
    $("#salario").val(""); $("#fecha_baja").val(""); $("#id_pesrona_experiencia").val("");
    $("#tipo_contrado").val("0"); $("#correo_corporativo").val("");
    $("#horas").val(""); $("#auxilio_transporte").val("");
    $("#tbody_afiliacion").empty(); $("#id_persona_actualizar").val("")
    $("#id_laboral_afiliacion_actualizar").val("");
}

function agregarInfAca() {
    if (validarCampos("info-academica")) {
        var numero = $("#tbody_estudios_personales tr").length + 1;
        var id = (numero + $("#formacion").val() + $("#institucion").val() + $("#titulo").val());
        ca = "<tr id=" + id.replace(/ /g, "") + ">"
        ca += "<td>" + numero + "</td>"
        ca += "<td>" + $("#formacion").val() + "</td>"
        ca += "<td>" + $("#titulo").val() + "</td>"
        ca += "<td>" + $("#institucion").val() + "</td>"
        ca += "<td>" + $("#fecha_graduacion").val() + "</td>"
        ca += "<td style='text-align:center'><a href='#' onclick=\"eliminarFila('" + id.replace(/ /g, "") + "');\"><i class='fas fa-trash'></i></a></td>"
        ca += "</tr>"
        $("#tbody_estudios_personales").append(ca);

        $("#formacion").val("");
        $("#titulo").val("");
        $("#institucion").val("");
        $("#fecha_graduacion").val("");
    } else {
        toastr.warning("¡Campos resaltados son obligatorios!");
    }
}

function agreagarExperiencia() {
    if (validarCampos("experiencia")) {
        var numero = $("#tbody_experiencia_laboral tr").length + 1;
        var id = (numero + $("#empresa").val() + $("#cargo_empleado").val() + $("#tiempo_laborado").val());
        var meses = cantidadMeses($("#fecha_inicio").val(), $("#fecha_fin").val()) + " M";
        var funciones = $("#descripcion_funciones").val().length > 20 ? $("#descripcion_funciones").val().substring(0, 20) + "..."
            : $("#descripcion_funciones").val();
        var logros = $("#logros_obtenidos").val().length > 20 ? $("#logros_obtenidos").val().substring(0, 20) + "..."
            : $("#logros_obtenidos").val();

        ca = "<tr id=" + id.replace(/ /g, "") + ">"
        ca += "<td style='text-align:center'>" + numero + "</td>"
        ca += "<td style='text-align:center'>" + $("#empresa").val() + "</td>"
        ca += "<td style='text-align:center'>" + $("#cargo_empleado").val() + "</td>"
        ca += "<td style='text-align:center'>" + $("#fecha_inicio").val() + "</td>"
        ca += "<td style='text-align:center'>" + $("#fecha_fin").val() + "</td>"
        ca += "<td style='text-align:center'>" + meses + "</td>"
        ca += "<td funciones=" + $("#descripcion_funciones").val() + " style='text-align:center'>" + funciones + "</td>"
        ca += "<td logro=" + $("#logros_obtenidos").val() + " style='text-align:center'>" + logros + "</td>"
        ca += "<td style='text-align:center'><a href='#' onclick=\"eliminarFila('" + id.replace(/ /g, "") + "');\"><i class='fas fa-trash'></i></a></td>"
        ca += "</tr>"
        $("#tbody_experiencia_laboral").append(ca);

        $("#empresa").val("");
        $("#cargo_empleado").val("");
        $("#fecha_inicio").val("");
        $("#fecha_fin").val("");
        $("#descripcion_funciones").val("");
        $("#logros_obtenidos").val("");
    } else {
        toastr.warning("¡Campos resaltados son obligatorios!");
    }
}

function eliminarFila(id) {
    $("#" + id).remove();
}

function guardarDatosPersonales() {
    var personalObj = new Object({
        PrimerNombre: $("#primer_nombre").val(), NumeroDocumento: $("#numero_documento").val(),
        SegundoNombre: $("#segundo_nombre").val(), Celular: $("#celular").val(),
        PrimerApellido: $("#primer_apellido").val(), EstadoCivil: $("#estado_civil").val(),
        SegundoApellido: $("#segundo_apellido").val(), CorreoPersonal: $("#correo_personal").val(),
        FechaNacimiento: $("#fecha_nacimiento").val(), Direccion: $("#direccion").val(),
        Barrio: $("#barrio").val(), TipoDocumento: $("#tipo_documento").val(),
    })

    var academicaArr = new Array();
    var elemento = document.getElementById("tbody_estudios_personales");
    for (var i = 0; i < elemento.children.length; i++) {
        var nivel = elemento.children[i].cells[1].textContent;
        var titulo = elemento.children[i].cells[2].textContent;
        var nombreIns = elemento.children[i].cells[3].textContent;
        var fecha = elemento.children[i].cells[4].textContent;
        var item = {
            NivelFormacion: nivel,
            TituloObtenido: titulo,
            NombreIns: nombreIns,
            FechaGradua: fecha,
        };
        academicaArr.push(item);
    }

    $.ajax({
        type: "POST",
        url: "Contratacion/GuardarCambiosPersonales",
        data: {
            personal: JSON.stringify(personalObj), academico: JSON.stringify(academicaArr),
            personaActualizar: $("#persona_actualizar").val()
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (res == -1) {
                toastr.warning("Ya existe un empleado con este numero de documento");
            }
            else if (res == 0) {
                toastr.error("Error al llenar los datos", "Contacte con el administrador");
            }
            else {
                $("#id_persona").val(res);
                $("#tab_datos_personales").removeClass("active");
                $("#datos_personales").removeClass("active");
                $("#tab_datos_experiencia").addClass("active");
                $("#datos_experiencia").addClass("active");
                $("#btn_atras_contrato").removeAttr('style');
                if ($("#persona_actualizar").val() == "") {
                    $("#primer_nombre").prop("disabled", true); $("#numero_documento").prop("disabled", true);
                    $("#segundo_nombre").prop("disabled", true); $("#celular").prop("disabled", true);
                    $("#primer_apellido").prop("disabled", true); $("#estado_civil").prop("disabled", true);
                    $("#segundo_apellido").prop("disabled", true); $("#correo_personal").prop("disabled", true);
                    $("#fecha_nacimiento").prop("disabled", true); $("#direccion").prop("disabled", true);
                    $("#barrio").prop("disabled", true); $("#tipo_documento").prop("disabled", true);
                    $("#formacion").prop("disabled", true); $("#titulo").prop("disabled", true);
                    $("#institucion").prop("disabled", true); $("#fecha_graduacion").prop("disabled", true);
                    $("#anexo_documento_identidad").prop("disabled", true); $("#btn_add_formacion").prop("disabled", true);
                }
                toastr.success("¡Primer paso completado!", "Ingresa los datos de la experiencia laboral");
            }
        },
        error: function (error) {
            toastr.error("No se ha podido obtener la información");
        }
    })
}

function guardarExperiencia() {
    var experienciaArr = new Array();
    var elemento = document.getElementById("tbody_experiencia_laboral");
    for (var i = 0; i < elemento.children.length; i++) {
        var empresa = elemento.children[i].cells[1].textContent;
        var cargo = elemento.children[i].cells[2].textContent;
        var fechaInicio = elemento.children[i].cells[3].textContent;
        var fechaFin = elemento.children[i].cells[4].textContent;
        var funciones = elemento.children[i].cells[6].attributes[0].textContent;
        var logros = elemento.children[i].cells[7].attributes[0].textContent;
        var item = {
            Empresa: empresa,
            Cargo: cargo,
            FechaInicio: fechaInicio,
            FechaFin: fechaFin,
            Funciones: funciones,
            Logros: logros,
        };
        experienciaArr.push(item);
    }

    $.ajax({
        type: "POST",
        url: "Contratacion/GuardarExperiencia",
        data: {
            experiencia: JSON.stringify(experienciaArr), personaId: $("#id_persona").val(),
            experienciaActualzar: $("#id_persona_actualizar").val()
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (!res) {
                toastr.error("Error al llenar los datos", "Contacte con el administrador");
            }
            else {
                $("#id_pesrona_experiencia").val($("#id_persona").val());
                $("#tab_datos_experiencia").removeClass("active");
                $("#datos_experiencia").removeClass("active");
                $("#datos_laborales").addClass("active");
                $("#tab_datos_laborales").addClass("active");
                if ($("#id_persona_actualizar").val() == "") {
                    $("#empresa").prop("disabled", true); $("#cargo_empleado").prop("disabled", true);
                    $("#fecha_inicio").prop("disabled", true); $("#fecha_fin").prop("disabled", true);
                    $("#descripcion_funciones").prop("disabled", true); $("#logros_obtenidos").prop("disabled", true);
                    $("#btn_agregar_experiencia").prop("disabled", true);
                }
                toastr.success("¡Segundo paso completado!", "Ingresa los datos laborales del empleado");
            }
        },
        error: function (error) {
            toastr.error("No se ha podido obtener la información");
        }
    })
}

function guardarLaboral() {
    var laboralObj = new Object({
        NombreCargo: $("#nombre_cargo").val(), FechaIngreso: $("#fecha_alta").val(),
        Salario: $("#salario").val(), FechaBaja: $("#fecha_baja").val(),
        TipoContrato: $("#tipo_contrado").val(), CorreoCorporativo: $("#correo_corporativo").val(),
        Horas: $("#horas").val(), AuxilioTransporte: $("#auxilio_transporte").val(),
        JornadaLaboral: $("#jornada_laboral").val(),
    });

    var insumosArr = new Array();
    var elemento = document.getElementById("tbody_insumolaboral_formurlario");
    if (elemento.children.length != 0) {
        for (var i = 0; i < elemento.children.length; i++) {
            var insumo = elemento.children[i].cells[1].textContent;
            item = {
                Nombre: insumo,
            };
            insumosArr.push(item);
        }
    }
    $.ajax({
        type: "POST",
        url: "Contratacion/GuardarLaboral",
        data: {
            laboral: JSON.stringify(laboralObj), insumos: JSON.stringify(insumosArr),
            personaId: $("#id_pesrona_experiencia").val(), laboralActualizar: $("#id_pesrona_experiencia_actualizar").val()
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (res == 0) {
                toastr.error("Error al llenar los datos", "Contacte con el administrador");
            }
            else {
                $("#id_laboral_afiliacion").val(res);
                $("#tab_datos_laborales").removeClass("active");
                $("#datos_laborales").removeClass("active");
                $("#datos_afiliacion").addClass("active");
                $("#tab_datos_afiliacion").addClass("active");
                if ($("#id_pesrona_experiencia_actualizar").val() == "") {
                    $("#btn_terminar_contrato").removeAttr('style');
                    $("#nombre_cargo").prop("disabled", true); $("#fecha_alta").prop("disabled", true);
                    $("#salario").prop("disabled", true); $("#fecha_baja").prop("disabled", true);
                    $("#tipo_contrado").prop("disabled", true); $("#correo_corporativo").prop("disabled", true);
                    $("#horas").prop("disabled", true); $("#auxilio_transporte").prop("disabled", true);
                    $("#jornada_laboral").prop("disabled", true); $("#btn_insumo_add").prop("disabled", true);
                }
                toastr.success("¡Segundo paso completado!", "Ingresa los datos laborales del empleado");
            }
        },
        error: function (error) {
            toastr.error("No se ha podido obtener la información");
        }
    })
}

function agregarAfiliacion() {
    if (validarCampos("afiliacion")) {
        var numero = $("#tbody_afiliacion tr").length + 1;
        var id = (numero + $("#tipo_entidad").val() + $("#entidad").val());
        ca = "<tr id=" + id.replace(/ /g, "") + ">"
        ca += "<td>" + numero + "</td>"
        ca += "<td tipo=" + $("#tipo_entidad").val() + ">" + $("#tipo_entidad option:selected").text() + "</td>"
        ca += "<td>" + $("#entidad").val() + "</td>"
        ca += "<td>" + $("#fecha_afiliacion").val() + "</td>"
        ca += "<td style='text-align:center'><a href='#' onclick=\"eliminarFila('" + id.replace(/ /g, "") + "');\"><i class='fas fa-trash'></i></a></td>"
        ca += "</tr>"
        $("#tbody_afiliacion").append(ca);

        $("#tipo_entidad").val("");
        $("#entidad").val("");
        $("#fecha_afiliacion").val("");
    } else {
        toastr.warning("¡Campos resaltados son obligatorios!");
    }
}

function agregarInsumo() {
    if (validarCampos("insumos")) {
        var numero = $("#tbody_insumolaboral_formurlario tr").length + 1;
        var id = (numero + $("#insumo_add").val());
        ca = "<tr id=" + id.replace(/ /g, "") + ">"
        ca += "<td>" + numero + "</td>"
        ca += "<td>" + $("#insumo_add").val() + "</td>"
        ca += "<td style='text-align:center'><a href='#' onclick=\"eliminarFila('" + id.replace(/ /g, "") + "');\"><i class='fas fa-trash'></i></a></td>"
        ca += "</tr>"
        $("#tbody_insumolaboral_formurlario").append(ca);

        $("#insumo_add").val("");
    } else {
        toastr.warning("¡Campos resaltados son obligatorios!");
    }
}

function guardarAfiliacion() {
    if (validarCampos("asignar-rol")) {
        var afiliacionArr = new Array();
        var elemento = document.getElementById("tbody_afiliacion");
        if (elemento.children.length != 0) {
            for (var i = 0; i < elemento.children.length; i++) {
                var entidad = elemento.children[i].cells[1].attributes[0].textContent;
                var nombre = elemento.children[i].cells[2].textContent;
                var fecha = elemento.children[i].cells[3].textContent;
                item = {
                    TipoEntidad: entidad,
                    NombreEntidad: nombre,
                    FechaAfiliacion: fecha
                };
                afiliacionArr.push(item);
            }
        }

        var personaObj = new Object({
            PrimerNombre: $("#primer_nombre").val(), PrimerApellido: $("#primer_apellido").val(),
            NumeroDocumento: $("#numero_documento").val(), TipoDocumento: $("#tipo_documento").val(),
        })
        $.ajax({
            type: "POST",
            url: "Contratacion/GuardarAfiliacion",
            data: {
                afiliaciones: JSON.stringify(afiliacionArr), rol: $("#escoger_rol").val(), laboralId: $("#id_laboral_afiliacion").val(),
                persona: JSON.stringify(personaObj), afiliacioneActualizar: $("#id_laboral_afiliacion_actualizar").val(),
            },
            dataType: "json",
            async: true,
            success: function (res) {
                if (!res.status) {
                    toastr.error(res.title + ": " + res.message);
                }
                else {
                    if ($("#id_laboral_afiliacion_actualizar").val() == "") {
                        $('#asignar_rol_modal').modal('toggle');
                    }
                    toastr.success(res.title + ": " + res.message);
                    setTimeout(function () { location.reload(); }, 1000);
                }
            },
            error: function (error) {
                toastr.error("No se ha podido obtener la información");
            }
        })
    } else {
        toastr.warning("Atención", "¡Asignar un rol es requeriso!");
    }
}

function CargarRoles() {
    $.ajax({
        type: "GET",
        url: "Contratacion/CargarRoles",
        data: {},
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.result == "ok") {
                var ca = "";
                $("#escoger_rol").empty();
                $.each(res.data, function (index, item) {
                    ca = "<option value=" + item.rolId + ">" + item.nombreRol + "</option>"
                    $("#escoger_rol").append(ca);
                });
            }
        },
        error: function (error) {
            toastr.error("No se ha podido obtener la información");
        }
    })
}

function mostrarPendientes(idPersona, progreso) {
    $.ajax({
        type: "GET",
        url: "Contratacion/MostrarPendientes",
        data: {
            progreso: progreso, idPersona: idPersona
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (res != null) {
                if (!$("#tab_datos_experiencia").hasClass("active")) {
                    $("#tab_datos_afiliacion").removeClass("active");
                    $("#datos_afiliacion").removeClass("active");
                    $("#btn_continuar_contrato").removeAttr('style');
                    $("#tab_datos_laborales").removeClass("active");
                    $("#datos_laborales").removeClass("active");
                    $("#tab_datos_experiencia").addClass("active");
                    $("#datos_experiencia").addClass("active");
                    $("#btn_atras_contrato").removeAttr("style");
                    $("#btn_terminar_contrato").css('display', 'none');
                    $("#datos_personales").removeClass("active");
                    $("#tab_datos_personales").removeClass("active");
                }
                if (!$("#listone").hasClass("show")) {
                    $("#collapse_bar_formulario").click();
                }
                $("#tbody_estudios_personales").empty();
                $("#persona_actualizar").val(res.persona.personaId); $("#id_persona").val(res.persona.personaId);
                $("#primer_nombre").val(res.persona.primerNombre); $("#numero_documento").val(res.persona.numeroDocumento);
                $("#segundo_nombre").val(res.persona.segundoNombre); $("#celular").val(res.persona.celular);
                $("#primer_apellido").val(res.persona.primerApellido); $("#estado_civil").val(res.persona.estadoCivil);
                $("#segundo_apellido").val(res.persona.segundoApellido); $("#correo_personal").val(res.persona.correoPersonal);
                $("#fecha_nacimiento").val(res.persona.fechaNacimiento); $("#direccion").val(res.persona.direccion);
                $("#barrio").val(res.persona.barrio); $("#tipo_documento").val(res.persona.tipoDocumento);
                $.each(res.infoAcademicas, function (index, item) {
                    var id = ((index + 1) + item.NivelFormacion + item.nombreIns + $("#titulo").val());
                    ca = "<tr id=" + id.replace(/ /g, "") + ">"
                    ca += "<td>" + (index + 1) + "</td>"
                    ca += "<td>" + item.nivelFormacion + "</td>"
                    ca += "<td>" + item.tituloObtenido + "</td>"
                    ca += "<td>" + item.nombreIns + "</td>"
                    ca += "<td>" + convertirFechaEspecifica(item.fechaGradua).replace("0:00:00", "") + "</td>"
                    ca += "<td style='text-align:center'><a href='#' onclick=\"eliminarFila('" + id.replace(/ /g, "") + "');\"><i class='fas fa-trash'></i></a></td>"
                    ca += "</tr>"
                    $("#tbody_estudios_personales").append(ca);
                })
                if (res.persona.progreso == "E" || res.persona.progreso == "L" || res.persona.progreso == "A") {
                    if (!$("#tab_datos_laborales").hasClass("active")) {
                        $("#tab_datos_afiliacion").removeClass("active");
                        $("#datos_afiliacion").removeClass("active");
                        $("#btn_continuar_contrato").removeAttr('style');
                        $("#tab_datos_laborales").addClass("active");
                        $("#datos_laborales").addClass("active");
                        $("#tab_datos_experiencia").removeClass("active");
                        $("#datos_experiencia").removeClass("active");
                        $("#btn_atras_contrato").removeAttr("style");
                        $("#btn_terminar_contrato").css('display', 'none');
                        $("#datos_personales").removeClass("active");
                        $("#tab_datos_personales").removeClass("active");
                    }
                    $("#tbody_experiencia_laboral").empty();
                    $("#id_persona_actualizar").val(res.persona.personaId); $("#id_pesrona_experiencia").val(res.persona.personaId);
                    $.each(res.experiencias, function (index, item) {
                        var id = ((index + 1) + item.empresa + item.cargo + item.meses);
                        var funciones = item.funciones.length > 20 ? item.funciones.substring(0, 20) + "..." : item.funciones;
                        var logros = item.logros.length > 20 ? item.logros.substring(0, 20) + "..." : item.logros;
                        ca = "<tr id=" + id.replace(/ /g, "") + ">"
                        ca += "<td style='text-align:center'>" + (index + 1) + "</td>"
                        ca += "<td style='text-align:center'>" + item.empresa + "</td>"
                        ca += "<td style='text-align:center'>" + item.cargo + "</td>"
                        ca += "<td style='text-align:center'>" + convertirFechaEspecifica(item.fechaInicio).replace("0:00:00", "") + "</td>"
                        ca += "<td style='text-align:center'>" + convertirFechaEspecifica(item.fechaFin).replace("0:00:00", "") + "</td>"
                        ca += "<td style='text-align:center'>" + item.meses + "</td>"
                        ca += "<td funciones=" + item.funciones + " style='text-align:center'>" + funciones + "</td>"
                        ca += "<td logro=" + item.logros + " style='text-align:center'>" + logros + "</td>"
                        ca += "<td style='text-align:center'><a href='#' onclick=\"eliminarFila('" + id.replace(/ /g, "") + "');\"><i class='fas fa-trash'></i></a></td>"
                        ca += "</tr>"
                        $("#tbody_experiencia_laboral").append(ca);
                    })
                    if (res.persona.progreso == "L" || res.persona.progreso == "A") {
                        var ingreso = res.laboral.fechaIngreso.split("T");
                        if (!$("#tab_datos_afiliacion").hasClass("active")) {
                            $("#tab_datos_afiliacion").addClass("active");
                            $("#datos_afiliacion").addClass("active");
                            $("#btn_continuar_contrato").removeAttr('style');
                            $("#tab_datos_laborales").removeClass("active");
                            $("#datos_laborales").removeClass("active");
                            $("#btn_atras_contrato").removeAttr('style');
                            $("#tab_datos_experiencia").removeClass("active");
                            $("#datos_experiencia").removeClass("active");
                            $("#btn_terminar_contrato").css('display', 'none');
                            $("#datos_personales").removeClass("active");
                            $("#tab_datos_personales").removeClass("active");
                        }
                        $("#tbody_insumolaboral_formurlario").empty();
                        $("#id_laboral_afiliacion").val(res.laboral.laboralId); $("#id_pesrona_experiencia_actualizar").val(res.laboral.laboralId);
                        $("#nombre_cargo").val(res.laboral.nombreCargo); $("#fecha_alta").val(ingreso[0].replace(/^(\d{4})-(\d{2})-(\d{2})$/g, '$3/$2/$1'));
                        $("#salario").val(res.laboral.salario); $("#fecha_baja").val(res.laboral.fechaBaja);
                        $("#tipo_contrado").val(res.laboral.tipoContrato); $("#correo_corporativo").val(res.laboral.correoCorporativo);
                        $("#horas").val(res.laboral.horas); $("#auxilio_transporte").val(res.laboral.auxilioTransporte);
                        $("#jornada_laboral").val(res.laboral.jornadaLaboral);
                        if (res.insumosLaborales.length > 0) {
                            $.each(res.insumosLaborales, function (index, item) {
                                var id = ((index + 1) + item.nombre);
                                ca = "<tr id=" + id.replace(/ /g, "") + ">"
                                ca += "<td>" + (index + 1) + "</td>"
                                ca += "<td>" + item.nombre + "</td>"
                                ca += "<td style='text-align:center'><a href='#' onclick=\"eliminarFila('" + id.replace(/ /g, "") + "');\"><i class='fas fa-trash'></i></a></td>"
                                ca += "</tr>"
                                $("#tbody_insumolaboral_formurlario").append(ca);
                            })
                        }
                        if (res.persona.progreso == "A") {
                            if (!$("#tab_datos_personales").hasClass("active")) {
                                $("#tab_datos_afiliacion").removeClass("active");
                                $("#datos_afiliacion").removeClass("active");
                                $("#btn_continuar_contrato").removeAttr("style");
                                $("#tab_datos_laborales").removeClass("active");
                                $("#datos_laborales").removeClass("active");
                                $("#btn_atras_contrato").css('display', 'none');
                                $("#tab_datos_experiencia").removeClass("active");
                                $("#datos_experiencia").removeClass("active");
                                $("#btn_terminar_contrato").css('display', 'none');
                                $("#datos_personales").addClass("active");
                                $("#tab_datos_personales").addClass("active");
                            }
                            $("#id_laboral_afiliacion_actualizar").val(res.laboral.laboralId);
                            $("#tbody_afiliacion").empty();
                            $.each(res.afiliaciones, function (index, item) {
                                var afiliacion = item.fechaAfiliacion.split("T");
                                var id = ((index + 1) + item.tipoEntidad + item.nombreEntidad);
                                ca = "<tr id=" + id.replace(/ /g, "") + ">"
                                ca += "<td>" + (index + 1) + "</td>"
                                ca += "<td tipo=" + item.tipoEntidad + ">" + item.tipoEntidad + "</td>"
                                ca += "<td>" + item.nombreEntidad + "</td>"
                                ca += "<td>" + afiliacion[0].replace(/^(\d{4})-(\d{2})-(\d{2})$/g, '$3/$2/$1') + "</td>"
                                ca += "<td style='text-align:center'><a href='#' onclick=\"eliminarFila('" + id.replace(/ /g, "") + "');\"><i class='fas fa-trash'></i></a></td>"
                                ca += "</tr>"
                                $("#tbody_afiliacion").append(ca);
                            })
                        }
                    }
                }
            } else {
                toastr.error("Error al procesar los datos", "Contacte con el administrador")
            }
        },
        error: function (error) {
            toastr.error("No se ha podido obtener la información");
        }
    })
}

function modalEliminarEmpleado(personaId) {
    var estado = $("#emp_" + personaId).children().eq(9).text();
    if (estado == "ACTIVO") {
        swal({
            title: "¿Desea eliminar éste empleado?",
            buttons: ["Cancelar", "Continuar"],
            icon: "warning",
        }).then((value) => {
            if (value) {
                swal({
                    title: "Inactivar un empleado o eliminarlo para siempre",
                    buttons: {
                        cancel: "Cancelar",
                        defeat: {
                            text: "Inactivar",
                            value: "defeat",
                        },
                        catch: {
                            text: "Eliminar",
                            value: "catch",
                        },
                    },
                    icon: "warning",
                }).then((value) => {
                    if (value == "defeat" || value == "catch") {
                        var op = value == "catch" ? true : false
                        eliminarEmpleado(personaId, op);
                    }
                });
            }
        })
    } else if (estado == "PENDIENTE" || estado == "INACTIVO") {
        swal({
            title: "Eliminar el empleado",
            icon: "warning",
            buttons: [true, "Eliminar"]
        }).then((value) => {
            if (value) {
                alert("else")
                eliminarEmpleado(personaId, true);
            }
        });
    }
}

function eliminarEmpleado(personaId, op) {
    $.ajax({
        type: "POST",
        url: "Contratacion/EliminarEmpleado",
        data: {
            personaId: personaId, op: op
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.status) {
                if (op) {
                    $("#emp_" + personaId).hide();
                }
                else {
                    $("#emp_" + personaId).children().eq(10).append("<a href='#' onclick='activarEmpleado(" + personaId + ")' id='icon_activars_" + personaId + "'><i class='far fa-hand-point-up' ></i></a >");
                    $("#icon_activo_" + personaId).remove();
                    $("#emp_" + personaId).children().eq(9).text("INACTIVO");
                }
                if ($("#listone").hasClass("show")) {
                    $("#collapse_bar_formulario").click();
                }
                limpiarFormulario();
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

function activarEmpleado(personaId, progreso) {
    $.ajax({
        type: "PUT",
        url: "Contratacion/ActivarEmpleado",
        data: {
            personaId: personaId
        },
        dataType: "json",
        async: true,
        success: function (res) {
            if (res.status) {
                $("#emp_" + personaId).children().eq(10).append("<a href='#' onclick=\"mostrarPendientes(" + personaId + ",'A');\" id='icon_activo_" + personaId + "' ><i class='fas fa-edit'></i></a>");
                $("#icon_activars_" + personaId).remove();
                $("#emp_" + personaId).children().eq(9).text("ACTIVO");
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