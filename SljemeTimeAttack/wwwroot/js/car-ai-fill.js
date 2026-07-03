(function () {
    const panel = document.querySelector("[data-car-ai-panel]");
    if (!panel) {
        return;
    }

    const input = panel.querySelector("[data-car-ai-input]");
    const button = panel.querySelector("[data-car-ai-submit]");
    const status = panel.querySelector("[data-car-ai-status]");
    const endpoint = panel.dataset.carAiUrl || "/Car/ParseCarPrompt";

    const fields = {
        make: document.querySelector('[name="Make"]'),
        model: document.querySelector('[name="Model"]'),
        year: document.querySelector('[name="Year"]'),
        horsepower: document.querySelector('[name="Horsepower"]'),
        weightKg: document.querySelector('[name="WeightKg"]'),
        registrationNumber: document.querySelector('[name="RegistrationNumber"]')
    };

    function setStatus(message, isError) {
        status.textContent = message || "";
        status.classList.toggle("is-error", Boolean(isError));
    }

    function setLoading(isLoading) {
        panel.classList.toggle("is-loading", isLoading);
        button.disabled = isLoading;
        button.textContent = isLoading ? "Parsing..." : "Fill form with AI";
    }

    function getValue(data, pascalName, camelName) {
        return data[camelName] ?? data[pascalName] ?? "";
    }

    function fillField(field, value) {
        if (!field || value === null || value === undefined || value === "") {
            return false;
        }

        field.value = value;
        field.dispatchEvent(new Event("input", { bubbles: true }));
        field.dispatchEvent(new Event("change", { bubbles: true }));
        return true;
    }

    async function fillWithAi() {
        const prompt = input.value.trim();
        if (!prompt) {
            setStatus("Enter a car description first.", true);
            input.focus();
            return;
        }

        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        setLoading(true);
        setStatus("Reading car details...", false);

        try {
            const response = await fetch(endpoint, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": token || ""
                },
                body: JSON.stringify({ prompt })
            });

            const data = await response.json();
            if (!response.ok) {
                throw new Error(data.error || "AI parser could not read the text.");
            }

            let filledCount = 0;
            filledCount += fillField(fields.make, getValue(data, "Make", "make")) ? 1 : 0;
            filledCount += fillField(fields.model, getValue(data, "Model", "model")) ? 1 : 0;
            filledCount += fillField(fields.year, getValue(data, "Year", "year")) ? 1 : 0;
            filledCount += fillField(fields.horsepower, getValue(data, "Horsepower", "horsepower")) ? 1 : 0;
            filledCount += fillField(fields.weightKg, getValue(data, "WeightKg", "weightKg")) ? 1 : 0;
            filledCount += fillField(fields.registrationNumber, getValue(data, "RegistrationNumber", "registrationNumber")) ? 1 : 0;

            if (filledCount === 0) {
                throw new Error("No usable fields were found.");
            }

            const source = getValue(data, "Source", "source") === "ai" ? "AI" : "fallback parser";
            setStatus(`Filled ${filledCount} fields with ${source}. Review before creating.`, false);
        } catch (error) {
            setStatus(error.message || "Something went wrong while parsing.", true);
        } finally {
            setLoading(false);
        }
    }

    button.addEventListener("click", fillWithAi);
})();
