(() => {
    const language = (navigator.language || "en").toLowerCase();
    const locale = language.startsWith("hr") && window.flatpickr?.l10ns?.hr ? "hr" : "default";

    document.querySelectorAll("[data-datetime-control]").forEach(input => {
        if (!window.flatpickr) {
            return;
        }

        window.flatpickr(input, {
            allowInput: true,
            dateFormat: "Y-m-d H:i",
            enableTime: true,
            locale,
            time_24hr: true
        });
    });
})();
