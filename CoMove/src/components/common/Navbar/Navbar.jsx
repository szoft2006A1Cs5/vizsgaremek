import { useState, useEffect } from "react"
import "./Navbar.css"
import {
    AppShell,
    Flex,
    Group,
    Stack,
    Image,
    Burger,
    Avatar,
    Box,
    LoadingOverlay,
    Title,
    Divider,
    NavLink,
    Popover,
} from "@mantine/core";
import logo from "../../../assets/kepek/logo/comove_logo4.png"
import { useDisclosure } from "@mantine/hooks"
import { Link, useLocation } from "react-router-dom"
import NotificationMenu from "./Notifications/NotificationMenu/NotificationMenu";
import { useLogout, useUser } from "../../../assets/scripts/AuthUser";
import { BACKEND_URL } from "../../../assets/scripts/Config";
import { formatPic } from "../../../assets/scripts/Utilities";

function Navbar({ children }) {
    const [sideNavOpen, sideNav] = useDisclosure(false)
    const [popoverOpen, popover] = useDisclosure(false)
    const [asideOpen, aside] = useDisclosure(false)
    const location = useLocation()

    const links = [
        { to: "/", name: "Főoldal" },
        { to: "/searching", name: "Autók keresése" },
    ]

    const [scrolled, setScrolled] = useState(false)

    useEffect(() => {
        if (!popoverOpen) return
        const onAnyScroll = () => popover.close();
        window.addEventListener("scroll", onAnyScroll, { passive: true, capture: true })
        return () => window.removeEventListener("scroll", onAnyScroll, { capture: true })
    })

    useEffect(() => {
        const onScroll = () => setScrolled(window.scrollY > 16)
        onScroll()
        window.addEventListener("scroll", onScroll, { passive: true })
        return () => window.removeEventListener("scroll", onScroll)
    }, [])

    useEffect(() => {
        sideNav.close();
        popover.close();
        aside.close();
    }, [location.pathname])

    const { data: authUser, isLoading } = useUser();
    const logout = useLogout();

    const closeAll = () => {
        popover.close();
        aside.close();
    }

    const menuItems = (
        <Stack gap={5}>
            {!authUser ? (
                <>
                    <NavLink component={Link} to="/login" label="Bejelentkezés" onClick={closeAll} />
                    <NavLink component={Link} to="/register" label="Regisztráció" onClick={closeAll} />
                </>
            ) : (
                <>
                    <NavLink component={Link} to="/account" label="Beállítások" onClick={closeAll} />
                    <Divider />
                    <NavLink component={Link} to="/rentals" label="Bérléseim" onClick={closeAll} />
                    <NavLink component={Link} to="/vehicles" label="Járműveim" onClick={closeAll} />
                    <Divider />
                    <NavLink label="Kijelentkezés" onClick={() => logout.mutate()} />
                </>
            )}
        </Stack>
    )

    return (
        <AppShell
            header={{ height: 80, zIndex: 50000 }}
            navbar={{
                breakpoint: "sm",
                collapsed: { desktop: true, mobile: !sideNavOpen },
            }}
            aside={{
                width: 300,
                breakpoint: "sm",
                collapsed: { desktop: true, mobile: !asideOpen },
            }}
        >
            <AppShell.Header className={`nav_header ${scrolled ? "nav_scrolled" : ""}`}>
                <div className="nav_bar">
                    <Flex h="100%" w="100%" align="center" justify="space-between">
                        <Group>
                            <Burger
                                hiddenFrom="sm"
                                opened={sideNavOpen}
                                onClick={() => {
                                    if (asideOpen) aside.close();
                                    sideNav.toggle();
                                }}
                                color="white"
                            />
                            <Link to="/" aria-label="CoMove">
                                <Image src={logo} w={50} h={50} />
                            </Link>
                        </Group>

                        <Group gap={20} className="nav_right">
                            <Group visibleFrom="sm" className="nav_links">
                                {links.map((x) => (
                                    <Link key={x.to} to={x.to} className="nav_link">
                                        {x.name}
                                    </Link>
                                ))}
                            </Group>

                            <NotificationMenu />

                            <Box visibleFrom="sm">
                                <Popover
                                    opened={popoverOpen}
                                    onChange={(o) => (o ? popover.open() : popover.close())}
                                    position="bottom"
                                    offset={5}
                                    withArrow
                                    arrowSize={16}
                                    arrowRadius={4}
                                    shadow="md"
                                    withinPortal
                                    zIndex={10000}
                                    styles={{ arrow: { background: "#e6f1ff" } }}
                                >
                                    <Popover.Target>
                                        <Avatar
                                            onClick={() => {
                                                if (sideNavOpen) sideNav.close();
                                                popover.toggle();
                                            }}
                                            className="nav_account"
                                            src={formatPic(authUser?.profilePicPath)}
                                            w={44} h={44} color="white"
                                        />
                                    </Popover.Target>
                                    <Popover.Dropdown className="nav_account_dropdown">
                                        <div className="nav_account_card">
                                            {authUser?.name && (
                                                <>
                                                    <Title size={20} c="black">Üdv {authUser.name}!</Title>
                                                    <Divider my={5} />
                                                </>
                                            )}
                                            {menuItems}
                                        </div>
                                    </Popover.Dropdown>
                                </Popover>
                            </Box>

                            <Avatar
                                hiddenFrom="sm"
                                onClick={() => {
                                    if (sideNavOpen) sideNav.close();
                                    aside.toggle();
                                }}
                                className="nav_account"
                                src={formatPic(authUser?.profilePicPath)}
                                w={44} h={44} color="white"
                            />
                        </Group>
                    </Flex>
                </div>
            </AppShell.Header>

            <AppShell.Navbar bg="transparent" hiddenFrom="sm" bd={0}>
                <div className="nav_glass">
                    <Stack>
                        {links.map((x) => (
                            <Link key={x.to} to={x.to} className="nav_link">
                                {x.name}
                            </Link>
                        ))}
                    </Stack>
                </div>
            </AppShell.Navbar>

            <AppShell.Aside bg='transparent' bd={0} hiddenFrom="sm">
                <div className="nav_glass">
                    {authUser?.name && (
                        <>
                            <Title size={20} c="white" fw='bold' mb={8}>Üdv, {authUser.name}!</Title>
                            <Divider color="lightgray" mb={8} />
                        </>
                    )}
                    {menuItems}
                </div>
            </AppShell.Aside>

            <AppShell.Main>{children}</AppShell.Main>
        </AppShell>
    );
}

export default Navbar;
