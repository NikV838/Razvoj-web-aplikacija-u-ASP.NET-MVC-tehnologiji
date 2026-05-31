(() => {
    const revealButtons = document.querySelectorAll("[data-password-reveal]");

    for (const button of revealButtons) {
        const wrapper = button.closest(".account-password");
        const input = wrapper?.querySelector("input");
        if (!input) continue;

        const reveal = (event) => {
            event.preventDefault();
            input.type = "text";
            button.classList.add("is-revealing");
        };

        const conceal = () => {
            input.type = "password";
            button.classList.remove("is-revealing");
        };

        button.addEventListener("pointerdown", reveal);
        button.addEventListener("pointerup", conceal);
        button.addEventListener("pointerleave", conceal);
        button.addEventListener("pointercancel", conceal);
        button.addEventListener("blur", conceal);
        button.addEventListener("keydown", (event) => {
            if (event.key === " " || event.key === "Enter") reveal(event);
        });
        button.addEventListener("keyup", conceal);
    }
})();
