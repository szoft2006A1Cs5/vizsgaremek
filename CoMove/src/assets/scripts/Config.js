export const BACKEND_URL = "https://localhost:7245"
export const API_URL = `${BACKEND_URL}/api`

export const STATUS_DICT = {
    "renterOffer":          { text: "Bérlő ajánlata", color: "blue" },
    "ownerOffer":           { text: "Tulaj ajánlata", color: "yellow" },
    "offerAccepted":        { text: "Elfogadott ajánlat", color: "green" },
    "renterPickupAccepted": { text: "Bérlő átvétel elfogadva", color: "blue" },
    "ownerPickupAccepted":  { text: "Tulaj átvétel elfogadva", color: "yellow" },
    "active":               { text: "A bérlés aktív", color: "green" },
    "renterFinishAccepted": { text: "Bérlő visszahozatal megerősítve", color: "blue" },
    "ownerFinishAccepted":  { text: "Tulaj visszahozatal megerősítve", color: "yellow" },
    "finished":             { text: "Befejeződött", color: "gray" },
    "cancelled":            { text: "Lemondva", color: "red" }
}