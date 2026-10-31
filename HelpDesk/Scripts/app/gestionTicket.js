$(function () {
    var $panel = $('#panelAdmin');
    if (!$panel.length) {
        return;
    }

    var $detalle = $('#detalleTicket');
    var idTicket = $detalle.data('id');
    var urlAsignar = $detalle.data('url-asignar');
    var urlEstado = $detalle.data('url-estado');

    var $mensaje = $('#mensajeGestion');
    var $botones = $('#botonesEstado');
    var $select = $('#selectAsignado');
    var cerrado = $panel.data('cerrado') === true;

    function bloquearPanel() {
        $panel.find('button, select').prop('disabled', true);
    }

    function desbloquearPanel() {
        $panel.find('button, select').prop('disabled', false);

        if (cerrado) {
            $('#selectAsignado, #btnAsignar, #btnAsignarme').prop('disabled', true);
        }
    }


    function aplicarCambios(ticket) {
        cerrado = !ticket.puedeComentar;

        $('#badgeEstado').attr('class', 'badge fs-6 ' + ticket.claseEstado).text(ticket.estado);
        $('#infoAsignado').text(ticket.asignado || 'Sin asignar');
        $('#infoActualizacion').text(ticket.fechaActualizacion);
        $('#infoResolucion').text(ticket.fechaResolucion || '—');

        if (ticket.idAsignado) {
            $select.find('option[value=""]').remove();
            $select.val(String(ticket.idAsignado));
        }

        $botones.empty();
        $.each(ticket.estadosSiguientes, function (_, estado) {
            $('<button type="button" class="btn btn-outline-secondary btn-estado"></button>')
                .attr('data-id-estado', estado.id)
                .text('Pasar a ' + estado.nombre)
                .appendTo($botones);
        });
        $('#sinTransiciones').toggleClass('d-none', ticket.estadosSiguientes.length > 0);


        $(document).trigger('helpdesk:ticketActualizado', [ticket]);
    }

    function enviar(url, datos) {
        bloquearPanel();
        HelpDesk.post(url, datos)
            .done(function (respuesta) {
                aplicarCambios(respuesta.ticket);
                HelpDesk.aviso($mensaje, 'success', respuesta.mensaje);
            })
            .fail(function (xhr) {
                HelpDesk.aviso($mensaje, 'danger', HelpDesk.mensajeError(xhr));
            })
            .always(desbloquearPanel);
    }

    function asignar(idUsuario) {
        if (!idUsuario) {
            HelpDesk.aviso($mensaje, 'warning', 'Seleccione la persona de soporte.');
            return;
        }
        enviar(urlAsignar, { id: idTicket, idUsuarioAsignado: idUsuario });
    }

    $('#btnAsignar').on('click', function () {
        asignar($select.val());
    });

    $('#btnAsignarme').on('click', function () {
        asignar($(this).data('id-usuario'));
    });


    $botones.on('click', '.btn-estado', function () {
        enviar(urlEstado, { id: idTicket, idEstado: $(this).data('id-estado') });
    });

    $('#formEliminar').on('submit', function (e) {
        var confirmado = window.confirm('¿Eliminar el ticket #' + idTicket + ' y todos sus comentarios? Esta acción no se puede deshacer.');
        if (!confirmado) {
            e.preventDefault();
        }
    });

    desbloquearPanel();
});
