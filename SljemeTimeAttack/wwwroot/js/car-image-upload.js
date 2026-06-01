(() => {
    document.querySelectorAll("[data-car-image-upload]").forEach(root => {
        const input = root.querySelector("[data-car-image-input]");
        const preview = root.querySelector("[data-car-image-preview]");
        const name = root.querySelector("[data-car-image-name]");

        input?.addEventListener("change", () => {
            const file = input.files?.[0];
            if (name) {
                name.textContent = file?.name || "No file selected";
            }

            if (!preview || !file) {
                return;
            }

            const imageUrl = URL.createObjectURL(file);
            preview.innerHTML = "";

            const image = document.createElement("img");
            image.src = imageUrl;
            image.alt = file.name;
            image.onload = () => URL.revokeObjectURL(imageUrl);

            preview.appendChild(image);
        });
    });
})();
