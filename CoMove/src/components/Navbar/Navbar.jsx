import { useState, useEffect, useEffectEvent } from "react"
import "./Navbar.css"
import {
    AppShell,
    Flex,
    Group,
    Stack,
    Image,
    Burger,
    Avatar,
    LoadingOverlay,
    Title,
    Divider,
    NavLink,
} from "@mantine/core";
import logo from "../../assets/kepek/logo/comove_logo4.png"
import { useDisclosure } from "@mantine/hooks"
import { Link, useLocation } from "react-router-dom"
import { Popover } from "@mantine/core"

function Navbar({ children }) {
    const [sideNavOpen, sideNav] = useDisclosure(false)
    const [authBarOpen, authBar] = useDisclosure(false)
    const location = useLocation()

    const links = [
        { to: "/", name: "Főoldal" },
        { to: "/searching", name: "Autók keresése" },
    ]

    const [scrolled, setScrolled] = useState(false)

    useEffect(() => {
        if (!authBarOpen)
            return

        const onAnyScroll = () => authBar.close();

        window.addEventListener("scroll", onAnyScroll, { passive: true, capture: true })

        return () => window.removeEventListener("scroll", onAnyScroll, { capture: true })
    })

    useEffect(() => {
        const onScroll = () => setScrolled(window.scrollY > 16)
        onScroll()
        window.addEventListener("scroll", onScroll, { passive: true })
        return () => window.removeEventListener("scroll", onScroll)
    }, [])

    const [authUser, setAuthUser] = useState(null)

    useEffect(() => {
        sideNav.close()
        authBar.close()

        const token = localStorage.getItem("token")
        if (!token) {
            setAuthUser(null)
            return
        }

        fetch("https://localhost:7245/api/User", {
            method: "GET",
            headers: { Authorization: `Bearer ${token}` },
        })
            .then((resp) => {
                if (resp.status !== 200) {
                    localStorage.removeItem("token")
                    setAuthUser(null)
                    return null
                }
                return resp.json()
            })
            .then((data) => {
                if (!data) return
                setAuthUser(data)
            })
            .catch(() => {
                setAuthUser(null)
            });
    }, [location.pathname])

    const isLoadingUser = !!localStorage.getItem("token") && (!authUser || !authUser.name)

    return (
        <AppShell
            header={{ height: 80 }}
            navbar={{
                breakpoint: "sm",
                collapsed: { desktop: true, mobile: !sideNavOpen },
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
                                    if (authBarOpen) authBar.close();
                                    sideNav.toggle();
                                }}
                                color="white"
                            />
                            <Link to="/" aria-label="CoMove">
                                <Image src={logo} w={50} h={50} />
                            </Link>
                        </Group>

                        <Group gap={18} className="nav_right">
                            <Group visibleFrom="sm" className="nav_links">
                                {links.map((x) => (
                                    <Link key={x.to} to={x.to} className="nav_link">
                                        {x.name}
                                    </Link>
                                ))}
                            </Group>

                            <Popover
                                opened={authBarOpen}
                                onChange={(o) => (o ? authBar.open() : authBar.close())}
                                position="bottom"
                                offset={5}
                                withArrow
                                arrowSize={16}
                                arrowRadius={4}
                                shadow="md"
                                withinPortal
                                zIndex={10000}
                                styles={{
                                    arrow: {
                                        background: "#e6f1ff",
                                    }
                                }}
                            >
                                <Popover.Target>
                                    <Avatar
                                        onClick={() => {
                                            if (sideNavOpen) sideNav.close();
                                            authBar.toggle();
                                        }}
                                        className="nav_account"
                                        w={44}
                                        h={44}
                                        color="white"
                                    />
                                </Popover.Target>

                                <Popover.Dropdown className="nav_account_dropdown">
                                    <div className="nav_account_card">
                                        <LoadingOverlay visible={isLoadingUser} zIndex={1000} overlayProps={{ radius: "sm", blur: 5 }} />

                                        <Stack gap={5} className="nav_authItems">
                                            {!authUser || !authUser.name ? (
                                                <>
                                                    <Link to="/login" className="nav_navlinkWrap" onClick={() => authBar.close()}>
                                                        <NavLink label="Bejelentkezés" />
                                                    </Link>
                                                    <Link to="/register" className="nav_navlinkWrap" onClick={() => authBar.close()}>
                                                        <NavLink label="Regisztráció" />
                                                    </Link>
                                                    <Link to="/account" className="nav_navlinkWrap" onClick={() => authBar.close()}>
                                                        <NavLink label="Fiókom" />
                                                    </Link>
                                                </>
                                            ) : (
                                                <>
                                                    <Title size={20} c="white">
                                                        Üdv {authUser.name}!
                                                    </Title>
                                                    <Divider />

                                                    <Link to="/settings" className="nav_navlinkWrap" onClick={() => authBar.close()}>
                                                        <NavLink label="Beállítások" />
                                                    </Link>

                                                    <Divider />

                                                    <Link to="/rentals" className="nav_navlinkWrap" onClick={() => authBar.close()}>
                                                        <NavLink label="Bérléseim" />
                                                    </Link>
                                                    <Link to="/vehicles" className="nav_navlinkWrap" onClick={() => authBar.close()}>
                                                        <NavLink label="Járműveim" />
                                                    </Link>

                                                    <Divider />

                                                    <NavLink
                                                        label="Kijelentkezés"
                                                        onClick={() => {
                                                            setAuthUser(null);
                                                            localStorage.removeItem("token");
                                                            authBar.close();
                                                        }}
                                                    />
                                                </>
                                            )}
                                        </Stack>
                                    </div>
                                </Popover.Dropdown>
                            </Popover>
                        </Group>
                    </Flex>
                </div>
            </AppShell.Header>

            <AppShell.Navbar bg="transparent" hiddenFrom="sm" bd={0}>
                <div className="nav_glass nav_popout">
                    <Stack>
                        {links.map((x) => (
                            <Link key={x.to} to={x.to} className={location.pathname === x.to ? "nav_link active" : "nav_link"}>
                                {x.name}
                            </Link>
                        ))}
                    </Stack>
                </div>
            </AppShell.Navbar>



            <AppShell.Main>{children}</AppShell.Main>
        </AppShell>
    );
}

export default Navbar;