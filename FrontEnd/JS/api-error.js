(function () {
    "use strict";

    const paginas = {
        404: "/Errores/404.html",
        500: "/Errores/500.html",
        502: "/Errores/502.html",
        503: "/Errores/503.html",
        504: "/Errores/504.html"
    };

    // Redirige a la pantalla personalizada del codigo HTTP indicado.
    function irAError(status) {

        const pagina = paginas[status];

        if (!pagina) {
            return false;
        }

        window.location.assign(pagina);
        return true;
    }

    // Un TypeError en fetch indica que no hubo comunicacion con el backend.
    function esErrorDeConexion(error) {
        return error instanceof TypeError;
    }

    // Maneja un fallo de comunicacion. Devuelve true si mostro la pantalla.
    function manejarErrorConexion(error, opciones) {

        const config = Object.assign({ redirigir: true }, opciones);

        if (!esErrorDeConexion(error)) {
            return false;
        }

        if (config.redirigir) {
            window.location.assign("/Errores/conexion.html");
        }

        return config.redirigir;
    }

    // Envoltorio de fetch que muestra la pantalla de conexion si falla la red.
    async function fetchApi(url, opciones) {

        try {
            return await fetch(url, opciones);
        } catch (error) {
            manejarErrorConexion(error);
            throw error;
        }
    }

    // Redirige las respuestas 500, 502, 503 y 504 a su pantalla.
    async function manejarRespuesta(respuesta, opciones) {

        const config = Object.assign({ redirigir: true }, opciones);

        if (respuesta.ok) {
            return { ok: true, status: respuesta.status, data: await leerJson(respuesta), mensaje: "" };
        }

        let data = null;

        try {
            data = await respuesta.clone().json();
        } catch (error) {
            data = null;
        }

        const mensaje = (data && (data.message || data.mensaje)) ||
            "No fue posible completar la solicitud.";

        if (config.redirigir &&
            [500, 502, 503, 504].includes(respuesta.status)) {
            irAError(respuesta.status);
        }

        return { ok: false, status: respuesta.status, data: data, mensaje: mensaje };
    }

    async function leerJson(respuesta) {

        try {
            return await respuesta.json();
        } catch (error) {
            return null;
        }
    }

    window.ShopEasyErrores = {
        paginas: paginas,
        irAError: irAError,
        esErrorDeConexion: esErrorDeConexion,
        manejarErrorConexion: manejarErrorConexion,
        manejarRespuesta: manejarRespuesta,
        fetchApi: fetchApi
    };

    document.addEventListener("DOMContentLoaded", () => {

        document.querySelectorAll("[data-reload]").forEach((boton) => {
            boton.addEventListener("click", () => window.location.reload());
        });

    });

})();
