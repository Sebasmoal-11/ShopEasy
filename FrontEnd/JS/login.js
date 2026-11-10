
document.addEventListener("DOMContentLoaded", () => {

    const loginForm = document.getElementById("loginForm");
    const emailInput = document.getElementById("Email");
    const passwordInput = document.getElementById("Password");

    const togglePassword = document.getElementById("togglePassword");
    const eyeIcon = document.getElementById("eyeIcon");

    const loginMessage = document.getElementById("loginMessage");
    const loginButton = document.getElementById("loginButton");
    const buttonText = document.getElementById("buttonText");

    // Mostrar u ocultar contraseña
    togglePassword.addEventListener("click", () => {

        const mostrar = passwordInput.type === "password";

        passwordInput.type = mostrar ? "text" : "password";

        eyeIcon.classList.toggle("bi-eye", !mostrar);
        eyeIcon.classList.toggle("bi-eye-slash", mostrar);

        togglePassword.setAttribute(
            "aria-label",
            mostrar ? "Ocultar contraseña" : "Mostrar contraseña"
        );

        togglePassword.setAttribute(
            "aria-pressed",
            String(mostrar)
        );
    });

    // Mostrar mensajes
    function showMessage(message, type) {

        loginMessage.textContent = message;
        loginMessage.className = `alert alert-${type}`;

    }

    function hideMessage() {

        loginMessage.textContent = "";
        loginMessage.className = "alert d-none";

    }

    // Enviar credenciales al backend
    loginForm.addEventListener("submit", async (event) => {

        event.preventDefault();
        hideMessage();

        const correo = emailInput.value.trim();
        const contrasena = passwordInput.value;

        if (!correo || !contrasena) {
            showMessage(
                "Por favor, completa todos los campos.",
                "warning"
            );
            return;
        }

        loginButton.disabled = true;
        buttonText.textContent = "Validando...";

        try {

            const response = await fetch("/api/auth/login", {

                method: "POST",

                headers: {
                    "Content-Type": "application/json"
                },

                body: JSON.stringify({
                    correo: correo,
                    contrasena: contrasena
                })

            });

            // Leer la respuesta incluso si el backend devuelve un error
            const data = await response.json();

            if (!response.ok) {

                showMessage(
                    data.message ||
                    data.mensaje ||
                    "No fue posible iniciar sesión. Verifica tus credenciales.",
                    "danger"
                );

                return;
            }

            // Verificar que el backend haya devuelto el token esperado
            if (!data.token || !data.user) {

                showMessage(
                    "El servidor respondió, pero no devolvió los datos de sesión esperados.",
                    "danger"
                );

                return;
            }

            // Guardar sesión
            sessionStorage.setItem("token", data.token);

            sessionStorage.setItem(
                "usuario",
                JSON.stringify(data.user)
            );

            showMessage(
                `¡Bienvenido, ${data.user.nombreCompleto}!`,
                "success"
            );

            // Más adelante conectaremos aquí el menú principal.

        } catch (error) {

            console.error("Error de inicio de sesión:", error);

            showMessage(
                "No se pudo conectar con el servidor. Inténtalo nuevamente.",
                "danger"
            );

        } finally {

            loginButton.disabled = false;
            buttonText.textContent = "Iniciar sesión";

        }

    });

});
