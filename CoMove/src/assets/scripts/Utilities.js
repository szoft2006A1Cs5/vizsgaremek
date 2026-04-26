import { API_URL, BACKEND_URL, RES_URL } from "./Config";

export function formatDate(str) {
    if (!str) return '-';
    return new Date(str).toLocaleDateString('hu-HU', { year: 'numeric', month: '2-digit', day: '2-digit' });
}

export function formatDateTime(str) {
    if (!str) return '-';
    return new Date(str).toLocaleString('hu-HU', { dateStyle: 'short', timeStyle: 'short' });
}

export function formatPrice(n) {
    if (n == null) return '-';
    return Number(n).toLocaleString('hu-HU') + ' Ft';
}

export function formatPic(pic) {
    return pic ? `${RES_URL}/${pic}` : null;
}

export function checkOver18(dateStr) {
    if (!dateStr) return false;
    const date = new Date(dateStr);
    if (isNaN(date)) return false;
    date.setHours(0, 0, 0, 0);

    const today = new Date();
    today.setHours(0, 0, 0, 0);
    today.setFullYear(today.getFullYear() - 18);

    return date <= today;
}

export function trimForm(form) {
    return Object.fromEntries(
        Object.entries(form).map(([key, val]) => {
            return [key, typeof val === "string" ? val.trim() : val]
        })
    );
}

export async function fetchAPI(path, options = {}) {
    try {
        return await fetch(`${API_URL}${path}`, {
            ...options,
            credentials: "include"
        });
    } catch {
        throw new Error("Nem sikerült lekérni az adatokat!");
    }
}