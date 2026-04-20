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
