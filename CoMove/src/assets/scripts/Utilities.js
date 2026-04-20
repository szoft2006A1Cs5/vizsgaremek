import { BACKEND_URL } from "./Config";

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

export function formatProfilePic(pic) {
    if (pic == null) return null;
    return `${BACKEND_URL}/${pic}`;
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

export async function getRespJsonError(resp) {
    return resp.headers.get("Content-Type")?.includes("application/json") ? await resp.json() : null;
}

