$(document).ready(function () {
    $('#datemask').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' });
    $('[data-mask]').inputmask();
    bsCustomFileInput.init();

    $("#btn_continuar_contrato").on("click", function (e) {
        e.preventDefault();
        if ($("#tab_datos_personales").hasClass("active")) {
            if (validarCampos("personal")) {
                if ($("#tbody_estudios_personales").find('tbody tr').length > 0) {
                    $("#tab_datos_personales").removeClass("active");
                    $("#datos_personales").removeClass("active");
                    $("#tab_datos_experiencia").addClass("active");
                    $("#datos_experiencia").addClass("active");
                    $("#btn_atras_contrato").removeAttr('style');
                } else {
                    toastr.warning("¡Llenar la tabla de información académica es obligatoria!");
                }
            } else {
                toastr.warning("¡Campos resaltados son obligatorios!");
            }
        } else if ($("#tab_datos_experiencia").hasClass("active")) {
            if (validarCampos("experiencia")) {
                if ($("#tbody_experiencia_laboral").find('tbody tr').length > 0) {
                    $("#tab_datos_experiencia").removeClass("active");
                    $("#datos_experiencia").removeClass("active");
                    $("#datos_laborales").addClass("active");
                    $("#tab_datos_laborales").addClass("active");
                } else {
                    toastr.warning("¡Llenar la tabla de experiencia laboral es obligatoria!");
                }
            } else {
                toastr.warning("¡Campos resaltados son obligatorios!");
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
})