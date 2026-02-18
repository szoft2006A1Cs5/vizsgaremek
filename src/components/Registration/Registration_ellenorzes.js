document.addEventListener("click", function (sz) {
  const szem = sz.target.closest(".end-icon");
  if (!szem) {
    return;
  }

  const input = szem
    .closest(".field")
    .querySelector('input[type="password"], input[type="text"]');

  if (input.type === "password") {
    input.type = "text";
    szem.classList.add("active");
  } else {
    input.type = "password";
    szem.classList.remove("active");
  }
});
