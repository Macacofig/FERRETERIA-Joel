// FERRETERÍA JOEL — Interacciones globales (menú móvil, submenú por dominio, accesibilidad).

document.addEventListener('DOMContentLoaded', function () {
    var burger = document.querySelector('[data-fj-burger]');
    var nav = document.getElementById('fjNav');

    // Menú hamburguesa (pantallas pequeñas)
    if (burger && nav) {
        burger.addEventListener('click', function () {
            var abierto = nav.classList.toggle('fj-nav-open');
            burger.setAttribute('aria-expanded', abierto ? 'true' : 'false');
        });
    }

    // Submenú contextual por dominio (despliegue táctil / móvil)
    document.querySelectorAll('[data-fj-mega-toggle]').forEach(function (toggle) {
        toggle.addEventListener('click', function () {
            var item = toggle.closest('.has-mega');
            if (!item) return;
            var abierto = item.classList.toggle('is-open');
            toggle.setAttribute('aria-expanded', abierto ? 'true' : 'false');
        });
    });

    // Cerrar menú móvil al navegar
    document.querySelectorAll('#fjNav a[href]').forEach(function (link) {
        link.addEventListener('click', function () {
            if (window.innerWidth <= 1024 && nav) {
                nav.classList.remove('fj-nav-open');
                burger && burger.setAttribute('aria-expanded', 'false');
            }
        });
    });

    // Cerrar menú móvil y submenús con Escape
    document.addEventListener('keydown', function (event) {
        if (event.key !== 'Escape') return;
        nav && nav.classList.remove('fj-nav-open');
        burger && burger.setAttribute('aria-expanded', 'false');
        document.querySelectorAll('.has-mega.is-open').forEach(function (item) {
            item.classList.remove('is-open');
            var t = item.querySelector('[data-fj-mega-toggle]');
            t && t.setAttribute('aria-expanded', 'false');
        });
    });

    // Cerrar submenús al hacer clic fuera
    document.addEventListener('click', function (event) {
        var megaAbiertos = document.querySelectorAll('.has-mega.is-open');
        if (!megaAbiertos.length) return;
        if (!event.target.closest('.has-mega')) {
            megaAbiertos.forEach(function (item) {
                item.classList.remove('is-open');
                var t = item.querySelector('[data-fj-mega-toggle]');
                t && t.setAttribute('aria-expanded', 'false');
            });
        }
    });
});

// ---------------------------------------------------------
// Modal de confirmación reutilizable.
// Cualquier <form> con data-fj-confirm="mensaje" intercepta su
// envío, muestra el modal y solo envía si el usuario confirma.
// ---------------------------------------------------------
(function () {
    var modal = document.getElementById('fjConfirmModal');
    if (!modal) return;

    var mensajeEl = document.getElementById('fjConfirmMessage');
    var botonOk = document.getElementById('fjConfirmOk');
    var formularioPendiente = null;

    function abrirModal(msj, form) {
        formularioPendiente = form;
        if (mensajeEl) mensajeEl.textContent = msj;
        modal.classList.add('is-open');
        modal.setAttribute('aria-hidden', 'false');
        document.body.classList.add('fj-modal-open');
        if (botonOk) botonOk.focus();
    }

    function cerrarModal() {
        formularioPendiente = null;
        modal.classList.remove('is-open');
        modal.setAttribute('aria-hidden', 'true');
        document.body.classList.remove('fj-modal-open');
    }

    document.addEventListener('submit', function (event) {
        var form = event.target;
        if (!form || form.nodeName !== 'FORM') return;

        var msj = form.getAttribute('data-fj-confirm');
        if (!msj) return;

        event.preventDefault();
        event.stopImmediatePropagation();
        abrirModal(msj, form);
    });

    if (botonOk) {
        botonOk.addEventListener('click', function () {
            var form = formularioPendiente;
            cerrarModal();
            if (!form) return;
            form.removeAttribute('data-fj-confirm');
            form.submit();
        });
    }

    modal.addEventListener('click', function (event) {
        if (event.target.classList.contains('fj-modal-backdrop') ||
            event.target.closest('[data-fj-modal-close]')) {
            cerrarModal();
        }
    });

    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape') cerrarModal();
    });
})();

// ---------------------------------------------------------
// Normalización de textos de formularios: elimina espacios
// redundantes al inicio/fin antes de enviar.
// ---------------------------------------------------------
(function () {
    document.querySelectorAll('form.fj-form').forEach(function (form) {
        form.addEventListener('submit', function () {
            form.querySelectorAll('input[type="text"], input[type="tel"], input[type="number"], input:not([type]), textarea').forEach(function (campo) {
                if (campo.readOnly) return;
                if (campo.value) campo.value = campo.value.trim();
            });
        });
    });
})();