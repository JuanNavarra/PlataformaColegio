$(document).ready(function () {
    $('#datemask').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' });
    $('[data-mask]').inputmask();
    bsCustomFileInput.init();

    $("#btn_continuar_contrato").on("click", function (e) {
        e.preventDefault();
        if ($("#tab_datos_personales").hasClass("active")) {
            if (validarCampos("personal")) {
                if ($("#tbody_estudios_personales tr").length > 0) {
                    $("#tab_datos_personales").removeClass("active");
                    $("#datos_personales").removeClass("active");
                    $("#tab_datos_experiencia").addClass("active");
                    $("#datos_experiencia").addClass("active");
                    $("#btn_atras_contrato").removeAttr('style');
                    guardarDatosPersonales();
                } else {
                    toastr.warning("¡Llenar la tabla de información académica es obligatoria!");
                }
            } else {
                toastr.warning("¡Campos resaltados son obligatorios!");
            }
        } else if ($("#tab_datos_experiencia").hasClass("active")) {
            if ($("#tbody_experiencia_laboral tr").length > 0) {
                $("#tab_datos_experiencia").removeClass("active");
                $("#datos_experiencia").removeClass("active");
                $("#datos_laborales").addClass("active");
                $("#tab_datos_laborales").addClass("active");
            } else {
                toastr.warning("¡Llenar la tabla de experiencia laboral es obligatoria!");
            }
        } else if ($("#tab_datos_laborales").hasClass("active")) {
            if (validarCampos("laborales")) {
                $("#tab_datos_laborales").removeClass("active");
                $("#datos_laborales").removeClass("active");
                $("#datos_afiliacion").addClass("active");
                $("#tab_datos_afiliacion").addClass("active");
                $("#btn_terminar_contrato").removeAttr('style');
                $("#btn_continuar_contrato").css("display", "none");
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
});

function agregarInfAca() {
    if (validarCampos("info-academica")) {
        var numero = $("#tbody_estudios_personales tr").length + 1;
        var id = (numero + $("#formacion").val() + $("#institucion").val() + $("#fecha_graduacion").val());
        ca = "<tr id=" + id + ">"
        ca += "<td>" + numero + "</td>"
        ca += "<td>" + $("#formacion").val() + "</td>"
        ca += "<td>" + $("#titulo").val() + "</td>"
        ca += "<td>" + $("#institucion").val() + "</td>"
        ca += "<td>" + $("#fecha_graduacion").val() + "</td>"
        ca += "<td style='text-align:center'><a href='#' onclick=\"eliminarFila('" + id + "');\"><i class='fas fa-trash'></i></a></td>"
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
        var meses = cantidadMeses($("#fecha_inicio").val(), $("#fecha_fin").val()) + " Meses";
        ca = "<tr id=" + id + ">"
        ca += "<td>" + numero + "</td>"
        ca += "<td>" + $("#empresa").val() + "</td>"
        ca += "<td>" + $("#cargo_empleado").val() + "</td>"
        ca += "<td>" + $("#fecha_inicio").val() + "</td>"
        ca += "<td>" + $("#fecha_fin").val() + "</td>"
        ca += "<td>" + meses + "</td>"
        ca += "<td>" + $("#descripcion_funciones").val() + "</td>"
        ca += "<td>" + $("#logros_obtenidos").val() + "</td>"
        ca += "<td style='text-align:center'><a href='#' onclick=\"eliminarFila('" + id + "');\"><i class='fas fa-trash'></i></a></td>"
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
    for (var i = 0; i < elemento.attributes.length; i++) {
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
        if (item.NivelFormacion != undefined) {
            academicaArr.push(item);
        }
    }

    $.ajax({
        type: "POST",
        url: "Contratacion/GuardarPersonales",
        data: {
            personal: JSON.stringify(personalObj), academico: JSON.stringify(academicaArr)
        },
        dataType: "json",
        async: true,
        success: function (res) {
        },
        error: function (error) {
            console.log("No se ha podido obtener la información");
        }
    })
}