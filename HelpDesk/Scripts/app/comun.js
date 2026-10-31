var HelpDesk = window.HelpDesk || {};

HelpDesk.token = function () {
    return $('input[name="__RequestVerificationToken"]').first().val();
};

HelpDesk.post = function (url, datos) {
    return $.ajax({
        url: url,
        type: 'POST',
        data: $.extend({ __RequestVerificationToken: HelpDesk.token() }, datos),
        dataType: 'json'
    });
};

HelpDesk.mensajeError = function (xhr) {
    if (xhr && xhr.responseJSON && xhr.responseJSON.mensaje) {
        return xhr.responseJSON.mensaje;
    }
    if (xhr && xhr.status === 401) {
        return 'La sesión expiró. Vuelva a iniciar sesión.';
    }
    if (xhr && xhr.status === 403) {
        return 'No tiene permisos para realizar esta acción.';
    }
    return 'Ocurrió un error inesperado. Intente de nuevo.';
};

HelpDesk.aviso = function ($contenedor, tipo, texto) {
    var $aviso = $('<div class="alert alert-dismissible fade show py-2 mb-0" role="alert"></div>')
        .addClass('alert-' + tipo)
        .text(texto)
        .append('<button type="button" class="btn-close py-2" data-bs-dismiss="alert" aria-label="Cerrar"></button>');
    $contenedor.empty().append($aviso);
};
