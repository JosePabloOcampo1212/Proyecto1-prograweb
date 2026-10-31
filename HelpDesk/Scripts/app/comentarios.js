$(function () {
    var $detalle = $('#detalleTicket');
    if (!$detalle.length) {
        return;
    }

    var idTicket = $detalle.data('id');
    var urlListar = $detalle.data('url-listar');
    var urlAgregar = $detalle.data('url-agregar');

    var $lista = $('#listaComentarios');
    var $total = $('#totalComentarios');
    var $form = $('#formComentario');
    var $texto = $('#textoComentario');
    var $boton = $('#btnComentar');
    var $mensaje = $('#mensajeComentario');
    var $contador = $('#contadorComentario');

    function crearComentario(c) {
        var $item = $('<div class="comentario border rounded p-3 mb-2"></div>');
        if (c.esSoporte) {
            $item.addClass('comentario-soporte');
        }

        var $autor = $('<strong></strong>').text(c.autor);
        if (c.esSoporte) {
            $autor.append(' <span class="badge text-bg-info ms-1">Soporte</span>');
        }

        var $encabezado = $('<div class="d-flex justify-content-between gap-2 small mb-1"></div>')
            .append($autor)
            .append($('<span class="text-muted text-nowrap"></span>').text(c.fecha));

        return $item
            .append($encabezado)
            .append($('<div class="texto-multilinea"></div>').text(c.texto));
    }

    function actualizarTotal() {
        $total.text($lista.children('.comentario').length);
    }

    function mostrarSinComentarios() {
        $lista.html('<div class="text-muted sin-comentarios">Todavía no hay comentarios.</div>');
    }

    function cargarComentarios() {
        $.ajax({
            url: urlListar,
            type: 'GET',
            data: { idTicket: idTicket },
            dataType: 'json',
            cache: false
        })
            .done(function (respuesta) {
                $lista.empty();
                if (respuesta.comentarios.length === 0) {
                    mostrarSinComentarios();
                } else {
                    $.each(respuesta.comentarios, function (_, c) {
                        $lista.append(crearComentario(c));
                    });
                }
                actualizarTotal();
            })
            .fail(function (xhr) {
                HelpDesk.aviso($lista, 'danger', HelpDesk.mensajeError(xhr));
            });
    }

    $texto.on('input', function () {
        $contador.text($texto.val().length);
    });

    $form.on('submit', function (e) {
        e.preventDefault();

        var texto = $.trim($texto.val());
        if (!texto) {
            HelpDesk.aviso($mensaje, 'warning', 'Escriba el comentario antes de enviarlo.');
            return;
        }

        $boton.prop('disabled', true).text('Enviando...');

        HelpDesk.post(urlAgregar, { idTicket: idTicket, texto: texto })
            .done(function (respuesta) {
                $lista.find('.sin-comentarios').remove();
                $lista.append(crearComentario(respuesta.comentario).addClass('comentario-nuevo'));
                actualizarTotal();
                $('#infoActualizacion').text(respuesta.fechaActualizacion);
                $texto.val('');
                $contador.text('0');
                $mensaje.empty();
            })
            .fail(function (xhr) {
                HelpDesk.aviso($mensaje, 'danger', HelpDesk.mensajeError(xhr));
            })
            .always(function () {
                $boton.prop('disabled', false).text('Agregar comentario');
            });
    });

    $(document).on('helpdesk:ticketActualizado', function (e, ticket) {
        $form.toggleClass('d-none', !ticket.puedeComentar);
        $('#avisoCerrado').toggleClass('d-none', ticket.puedeComentar);
    });

    cargarComentarios();
});
