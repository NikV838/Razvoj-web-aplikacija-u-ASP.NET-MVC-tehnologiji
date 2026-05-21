(() => {
    const getMessage = field => {
        const value = field.value.trim();
        const label = field.closest("div")?.querySelector("label")?.textContent?.trim() || "Field";

        if (field.dataset.inlineRequired !== undefined && value.length === 0) {
            return `${label} is required.`;
        }

        const minLength = Number(field.dataset.inlineMinLength || 0);
        const maxLength = Number(field.dataset.inlineMaxLength || Number.MAX_SAFE_INTEGER);
        if (value.length > 0 && (value.length < minLength || value.length > maxLength)) {
            return `${label} must be between ${minLength} and ${maxLength} characters.`;
        }

        const hasRange = field.dataset.inlineMin !== undefined || field.dataset.inlineMax !== undefined;
        const min = Number(field.dataset.inlineMin || Number.MIN_SAFE_INTEGER);
        const max = Number(field.dataset.inlineMax || Number.MAX_SAFE_INTEGER);
        if (hasRange && value.length > 0 && (Number.isNaN(Number(value)) || Number(value) < min || Number(value) > max)) {
            return `${label} must be between ${min} and ${max}.`;
        }

        return "";
    };

    document.querySelectorAll("[data-inline-create]").forEach(root => {
        const toggle = root.querySelector("[data-inline-toggle]");
        const panel = root.querySelector("[data-inline-panel]");
        const submit = root.querySelector("[data-inline-submit]");
        const status = root.querySelector("[data-inline-status]");
        const fields = Array.from(root.querySelectorAll("[data-inline-field]"));
        const parentForm = root.closest("form");
        const token = parentForm?.querySelector("input[name='__RequestVerificationToken']");
        const targetSelect = parentForm?.querySelector(`select[name="${root.dataset.targetSelect}"]`);

        if (!toggle || !panel || !submit || !parentForm || !targetSelect) {
            return;
        }

        const errorNode = name => root.querySelector(`[data-inline-error-for="${name}"]`);

        const showError = (field, message) => {
            const node = errorNode(field.name);
            if (node) {
                node.textContent = message;
            }

            field.classList.toggle("is-invalid", message.length > 0);
            return message.length === 0;
        };

        const validateField = field => {
            if (field.type === "checkbox") {
                return true;
            }

            return showError(field, getMessage(field));
        };

        const resetMessages = () => {
            fields.forEach(field => showError(field, ""));
            if (status) {
                status.textContent = "";
            }
        };

        const setOpen = open => {
            panel.hidden = !open;
            root.classList.toggle("is-open", open);
            toggle.setAttribute("aria-expanded", open ? "true" : "false");

            if (open) {
                fields.find(field => field.type !== "checkbox")?.focus();
            }
        };

        toggle.addEventListener("click", () => {
            resetMessages();
            setOpen(panel.hidden);
        });

        fields.forEach(field => {
            field.addEventListener("blur", () => validateField(field));
            field.addEventListener("change", () => validateField(field));
        });

        submit.addEventListener("click", async () => {
            resetMessages();
            const isValid = fields.map(validateField).every(Boolean);
            if (!isValid) {
                return;
            }

            const payload = new FormData();
            if (token) {
                payload.append(token.name, token.value);
            }

            fields.forEach(field => {
                payload.append(field.name, field.type === "checkbox" ? field.checked : field.value);
            });

            root.classList.add("is-saving");
            submit.disabled = true;
            if (status) {
                status.textContent = "Saving...";
            }

            try {
                const response = await fetch(root.dataset.createUrl, {
                    method: "POST",
                    body: payload,
                    headers: { "X-Requested-With": "XMLHttpRequest" }
                });
                const result = await response.json();

                if (!response.ok) {
                    Object.entries(result.errors || {}).forEach(([name, errors]) => {
                        const field = fields.find(item => item.name === name);
                        if (field) {
                            showError(field, errors[0] || "Check this field.");
                        }
                    });

                    if (status) {
                        status.textContent = "Check the highlighted fields.";
                    }
                    return;
                }

                const option = new Option(result.text, result.id, true, true);
                targetSelect.add(option);
                targetSelect.value = result.id;
                targetSelect.dispatchEvent(new Event("change", { bubbles: true }));
                targetSelect.dispatchEvent(new Event("blur", { bubbles: true }));

                fields.forEach(field => {
                    if (field.type === "checkbox") {
                        field.checked = false;
                    } else {
                        field.value = "";
                    }
                });

                if (status) {
                    status.textContent = "Added.";
                }

                root.classList.add("is-saved");
                window.setTimeout(() => {
                    root.classList.remove("is-saved");
                    setOpen(false);
                }, 360);
            } catch {
                if (status) {
                    status.textContent = "Save unavailable.";
                }
            } finally {
                root.classList.remove("is-saving");
                submit.disabled = false;
            }
        });
    });
})();
