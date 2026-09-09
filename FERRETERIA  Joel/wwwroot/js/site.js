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