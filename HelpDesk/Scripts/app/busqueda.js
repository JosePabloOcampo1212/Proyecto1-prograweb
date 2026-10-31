$(function () {
    var $form = $('#formBusqueda');
    if (!$form.length) {
        return;
    }

    var $texto = $('#texto');
    var $estado = $('#idEstado');
    var $contenedor = $('#contenedorTabla');
    var $info = $('#estadoBusqueda');
    var espera = null;
    var peticionActual = null;

    function buscar() {
        if (peticionActual) {
            peticionActual.abort();
        }

        $info.text('Buscando...');

        var peticion = $.ajax({
            url: $form.attr('action'),
            type: 'GET',
            data: { texto: $.trim($texto.val()), idEstado: $estado.val() },
            dataType: 'html',
            cache: false
        });
        peticionActual = peticion;

        peticion
            .done(function (html) {
                $contenedor.html(html);
                $info.text('');
            })
            .fail(function (xhr, estado) {
                if (estado !== 'abort') {
                    $info.text(HelpDesk.mensajeError(xhr));
                }
            })
            .always(function () {
                if (peticionActual === peticion) {
                    peticionActual = null;
                }
            });
    }

    $texto.on('input', function () {
        clearTimeout(espera);
        espera = setTimeout(buscar, 300);
    });

    $estado.on('change', buscar);

    $form.on('submit', function (e) {
        e.preventDefault();
        clearTimeout(espera);
        buscar();
    });

    $('#btnLimpiar').on('click', function () {
        $texto.val('');
        $estado.val('');
        buscar();
    });
});
