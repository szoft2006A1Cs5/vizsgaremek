export const BACKEND_URL = `https://${window.location.hostname}:7245`
export const API_URL = `${BACKEND_URL}/api`

export const STATUS_DICT = {
    "renterOffer":          { num: 0, text: "Bérlő ajánlata", color: "blue" },
    "ownerOffer":           { num: 1, text: "Tulaj ajánlata", color: "yellow" },
    "offerAccepted":        { num: 2, text: "Elfogadott ajánlat", color: "green" },
    "renterPickupAccepted": { num: 3, text: "Bérlő átvétel elfogadva", color: "blue" },
    "ownerPickupAccepted":  { num: 4, text: "Tulaj átvétel elfogadva", color: "yellow" },
    "active":               { num: 5, text: "A bérlés aktív", color: "green" },
    "renterFinishAccepted": { num: 6, text: "Bérlő visszahozatal megerősítve", color: "blue" },
    "ownerFinishAccepted":  { num: 7, text: "Tulaj visszahozatal megerősítve", color: "yellow" },
    "finished":             { num: 8, text: "Befejeződött", color: "gray" },
    "cancelled":            { num: 9, text: "Lemondva", color: "red" }
}

export function requiresActionFromMe(status, role) {
    const other = role === "renter" ? "owner" : "renter";
    return !status.startsWith(role) && status !== "cancelled";
}

export function nextStatus(status) {
    return Object.keys(STATUS_DICT).filter(x => status.num < STATUS_DICT[x].num && STATUS_DICT[x].num % 3 === 2)[0];
}