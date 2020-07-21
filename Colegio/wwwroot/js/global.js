$(document).ready(function () {
    var time = new Date().getTime();
    $(document.body).bind("mousemove keypress", function (e) {
        time = new Date().getTime();
    });

    function refresh() {
        if (new Date().getTime() - time >= 600000)
            window.location.reload(true);
        else
            setTimeout(refresh, 10000);
    }

    setTimeout(refresh, 10000);
})

function ejecutarDataTable(id) {
    $("#" + id).DataTable({
        "responsive": true,
        "autoWidth": false,
        language: {
            "decimal": "",
            "emptyTable": "No hay información",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
            "infoEmpty": "Mostrando 0 to 0 of 0 Entradas",
            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Mostrar _MENU_ Entradas",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "search": "Buscar:",
            "zeroRecords": "Sin resultados encontrados",
            "paginate": {
                "first": "Primero",
                "last": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
    });
}

function convertirFechaEspecifica(fecha) {
    var convercion = new Date(fecha);
    return convercion.toLocaleString();
}

function validarCampos(clase) {
    var input = document.getElementsByClassName(clase);
    var validar = true;
    if (input.length > 0) {
        for (var i = 0; i < input.length; i++) {
            if (input[i].value == "" || input[i].value == "0") {
                input[i].style.borderColor = "tomato";
                validar = false;
            }
        }
    }
    return validar;
}

function cantidadMeses(_fecha1, _fecha2) {
    _fecha1 = _fecha1.split("/").reverse().join("/");
    _fecha2 = _fecha2.split("/").reverse().join("/");
    let fecha1 = new Date(_fecha1);
    let fecha2 = new Date(_fecha2)

    let resta = fecha2.getTime() - fecha1.getTime()
    var meses = (Math.round(resta / (1000 * 60 * 60 * 24))) / 30
    return meses.toFixed();
}