import { useMediaQuery } from "@mantine/hooks";
import 'dayjs/locale/hu'

export function useDateInputProps(type = 'dateTime') {
    const isMobile = useMediaQuery('(max-width: 768px)') ?? false;

    return {
        locale: 'hu',
        valueFormat: type === 'dateTime' ? 'YYYY. MM. DD. HH:mm' : 'YYYY. MM. DD.',
        radius: "md",
        size: "sm",
        dropdownType: isMobile ? "modal" : "popover",
        modalProps: isMobile ? { yOffset: 150, lockScroll: false } : {}
    };
}
