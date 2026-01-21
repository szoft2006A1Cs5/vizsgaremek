document.addEventListener("DOMContentLoaded", () => {
const tabLogin = document.getElementById("tab-login")
const tabRegister = document.getElementById("tab-register")
const panelLogin = document.getElementById("panel-login")
const panelRegister = document.getElementById("panel-register")
const cim = document.getElementById("headline")
const alcim = document.getElementById("subline")
const alapCim = cim.textContent
const alapAlcim = alcim.textContent

function Valt(merre) {
  const login = merre === "login"
  tabLogin.classList.toggle("active", login)
  tabRegister.classList.toggle("active", !login)
  panelLogin.classList.toggle("active", login)
  panelRegister.classList.toggle("active", !login)

  if (login) {
    cim.textContent = alapCim
    alcim.textContent = alapAlcim
  } else {
    cim.textContent = "Regisztráció"
    alcim.textContent = "Hozza létre új fiókját"
  }
}

tabLogin.addEventListener("click", () => Valt("login"))
tabRegister.addEventListener("click", () => Valt("register"))

document.getElementById("goto-register")?.addEventListener("click", (e) => {
  e.preventDefault()
  Valt("register")
})
document.getElementById("goto-login")?.addEventListener("click", (e) => {
  e.preventDefault()
   Valt("login")
})


const regForm = document.querySelector("#panel-register form")
if (!regForm) return
const nevMezo = regForm.querySelector('input[name="teljes_name"]')
const szulMezo = regForm.querySelector('input[name="szuletesi_datum"]')
const telefonMezo = regForm.querySelector('input[name="telefonszam"]')
const nemMezo = document.getElementById("nem")
const emailMezo = regForm.querySelector('input[name="email"]')
const jelszoMezok = regForm.querySelectorAll('input[name="jelszo"]')
const jelszo1Mezo = jelszoMezok[0]
const jelszo2Mezo = jelszoMezok[1]
const feltetelekMezo = document.getElementById("feltetelek")
const nevMinta = /^[A-ZÁÉÍÓÖŐÚÜŰ][a-záéíóöőúüű]+ [A-ZÁÉÍÓÖŐÚÜŰ][a-záéíóöőúüű]+$/
const emailMinta = /^[^\s@]+@[a-zA-Z0-9]{1,6}\.[a-zA-Z]{1,4}$/
const telefonMinta = /^\d{11}$/
const erosJelszoMinta = /^(?=.*[a-záéíóöőúüű])(?=.*[A-ZÁÉÍÓÖŐÚÜŰ])(?=.*\d)(?=.*[^\w\s]).{8,}$/


function errorDiv(mezo) {
  const field = mezo.closest(".field") || mezo.parentElement
  let e = field.previousElementSibling

  if (!e || !e.classList || !e.classList.contains("error")) {
    e = document.createElement("div")
    e.className = "error"
    field.insertAdjacentElement("beforebegin", e)
  }
  return e
}

function SetHiba(mezo, uzenet) {
  errorDiv(mezo).textContent = uzenet || ""
}

function hibakTorlese() {
  regForm.querySelectorAll(".error").forEach(e => e.remove())
}

function nagykoruE(yyyy_mm_dd) {
  if (!yyyy_mm_dd) return false

  const ma = new Date()
  ma.setHours(0, 0, 0, 0)
  
  const szuletett = new Date(yyyy_mm_dd + "T00:00:00")
  if (Number.isNaN(szuletett.getTime())) return false

  const korhatar = new Date(ma)
  korhatar.setFullYear(korhatar.getFullYear() - 18)

  return szuletett <= korhatar
}
if (szulMezo?.type === "date") {
  const ma = new Date()
  szulMezo.max = `${ma.getFullYear()}-${String(ma.getMonth() + 1).padStart(2, "0")}-${String(ma.getDate()).padStart(2, "0")}`
}


regForm.addEventListener("submit", (e) => {
  e.preventDefault()

  hibakTorlese()
  let jo = true


  //nev
  const nev = (nevMezo.value || "").trim()
  if (!nev) {
    SetHiba(nevMezo, "Kötelező *")
    jo = false

  } else if (!nev.includes(" ")) {
    SetHiba(nevMezo, "Vezeték/keresztnév között kötelező a szóköz")
    jo = false

  } else if (!nevMinta.test(nev)) {
    SetHiba(nevMezo, "Vezeték/keresztnév nagybetűvel kezdődjön (pl. Kovács Péter)")
    jo = false
  }


  //szul
  if (!szulMezo.value) {
    SetHiba(szulMezo, "Kötelező *")
    jo = false
  } else if (!nagykoruE(szulMezo.value)) {
    SetHiba(szulMezo, "Legalább 18 évesnek kell lenned")
    jo = false
  }


  //tel
  const tel = (telefonMezo.value || "").trim()
  if (!tel) {
    SetHiba(telefonMezo, "Kötelező *")
    jo = false
  } else if (!telefonMinta.test(tel)) {
    SetHiba(telefonMezo, "11 számjegyből kell állnia")
    jo = false
  }


  //nem
  if (!nemMezo.value) {
    SetHiba(nemMezo, "Kötelező *")
    jo = false
  }


  //email
  const email = (emailMezo.value || "").trim()
  if (!email) {
    SetHiba(emailMezo, "Kötelező *")
    jo = false

  } else if (!email.includes("@")) {
    SetHiba(emailMezo, "Hiányzik az @ jel.")
    jo = false

  } else {
    const atUtan = email.split("@")[1] || ""

    if (!atUtan.includes(".")) {
      SetHiba(emailMezo, "A @ után kötelező a pont (pl. gmail.com)")
      jo = false

    } else {
      const utolsoPontUtan = atUtan.split(".").pop() || ""

      if (utolsoPontUtan.length < 2) {
        SetHiba(emailMezo, "A pont után legalább 2 karakter kell (pl. .com, .hu)")
        jo = false

      } else if (!emailMinta.test(email)) {
        setHiba(emailMezo, "Hibás formátum (pl. pelda@gmail.com)")
        jo = false
      }
    }
  }


  //jelszo  
  const pw1 = jelszo1Mezo.value || ""
  if (!pw1) {
    SetHiba(jelszo1Mezo, "Kötelező *")
    jo = false
  } else if (/\s/.test(pw1)) {
    SetHiba(jelszo1Mezo, "Nem lehet benne szóköz")
    jo = false
  } else if (!erosJelszoMinta.test(pw1)) {
    SetHiba(jelszo1Mezo, "Minimum 8 karakter, kis/nagybetü, szám, speciális karakter")
    jo = false
  }

  const pw2 = jelszo2Mezo.value || ""
  if (!pw2) {
    SetHiba(jelszo2Mezo, "Kötelező *")
    jo = false
  } else if (pw1 !== pw2) {
    SetHiba(jelszo2Mezo, "A két jelszó nem egyezik")
    jo = false
  }

  if (!feltetelekMezo.checked) {
    SetHiba(feltetelekMezo, "Kötelező elfogadni *")
    jo = false
  }

  if (jo) {
    alert("Sikeres regisztráció!")
    regForm.reset()
    hibakTorlese()
  }
  })
})