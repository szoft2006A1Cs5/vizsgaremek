import { useState, useEffect } from 'react'
import style from './Navbar.module.css'
import { Anchor, AppShell, Flex, Group, Stack, Image, Burger, Avatar, Menu, LoadingOverlay, Title, Divider, NavLink } from '@mantine/core';
import logo from '../../assets/kepek/logo/comove_logo4.png';
import { useDisclosure } from "@mantine/hooks";
import { Link, useLocation } from 'react-router-dom';

function Navbar({ children }) {
    const [sideNavOpen, { toggle: sideNavToggle }] = useDisclosure();
    const [authBarOpen, { toggle: authBarToggle }] = useDisclosure();
    const location = useLocation();

    const links = [
        { to: "/", name: "Főoldal" },
        { to: "/search", name: "Autók keresése" }
    ];

    const [authUser, setAuthUser] = useState({});

    useEffect(() => {
        let token = localStorage.getItem("token");

        if (!token) {
            setAuthUser(null);
            return;
        }

        fetch("https://localhost:7245/api/User", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        })
        .then(resp => {
            if (resp.status != 200) {
                localStorage.removeItem("token");
                setAuthUser(null);
                return null;
            }

            return resp.json();
        })
        .then(data => {
            console.log(data);
            setAuthUser(data);
        })
        .catch(_ => {})
    }, [location.pathname])

    return (<>
        <AppShell
            header={{
                height: 80
            }}
            navbar={{
                breakpoint: 'sm',
                collapsed: {
                    desktop: true,
                    mobile: !sideNavOpen
                }
            }}
            aside={{
                width: 300,
                breakpoint: 'sm',
                collapsed: {
                    desktop: !authBarOpen,
                    mobile: !authBarOpen
                }
            }}
        >
            <AppShell.Header className={style.glass} bdrs={999} p={15} m={15} bd={0} >
                <Flex direction="row" gap={5} align="center" justify='space-between'>
                    <Group>
                        <Burger hiddenFrom='sm' opened={sideNavOpen} onClick={() => {
                            if (authBarOpen) authBarToggle();
                            sideNavToggle();
                        }} color='white' />
                        <Link to="/"><Image src={logo} w={50} h={50} /></Link>
                    </Group>

                    <Group gap={20}>
                        <Group visibleFrom='sm'>
                            {links.map(x => <Link to={x.to}><span>{x.name}</span></Link>)}
                        </Group>


                        <Avatar onClick={() => {
                            if (sideNavOpen) sideNavToggle();
                            authBarToggle();
                        }} className={style.account} w={50} h={50} color='white' />

                        {/*
                        <Menu>
                            <Menu.Target>
                                <Avatar className={style.account} w={50} h={50} color='white' />
                            </Menu.Target>

                            <Menu.Dropdown>
                                <LoadingOverlay visible={ authUser && !authUser.name } zIndex={1000} overlayProps={{ radius: "sm", blur: 2 }} />
                                {
                                    !authUser ?
                                    <>
                                        <Menu.Item><Link to="/login">Bejelentkezés</Link></Menu.Item>
                                        <Menu.Item><Link to="/register">Regiszráció</Link></Menu.Item>
                                    </> :
                                    <>
                                        <Menu.Label>Üdv {authUser.name}!</Menu.Label>
                                        <Menu.Item>Beállítások</Menu.Item>
                                        <Menu.Divider></Menu.Divider>
                                        <Menu.Item>Járműveim</Menu.Item>
                                        <Menu.Item>Bérléseim</Menu.Item>
                                        <Menu.Divider></Menu.Divider>
                                        <Menu.Item onClick={() => {
                                            setAuthUser(null);
                                            localStorage.removeItem("token");
                                        }}>Kijelentkezés</Menu.Item>
                                    </>
                                }
                               
                            </Menu.Dropdown>
                        </Menu>
                        */}
                    </Group>
                </Flex>
            </AppShell.Header>

            <AppShell.Navbar bg='transparent' hiddenFrom='sm' bd={0}>
                <div className={`${style.glass} ${style.navbarPopout}`} >
                    <Stack>
                        {links.map(x => <Link to={x.to}><span>{x.name}</span></Link>)}
                    </Stack>
                </div>
            </AppShell.Navbar>

            <AppShell.Aside bg='transparent' bd={0}>
                <div className={`${style.glass} ${style.navbarPopout}`} style={{height: "100%"}} >
                    <LoadingOverlay visible={ authUser && !authUser.name } zIndex={1000} overlayProps={{ radius: "sm", blur: 5 }} bdrs={50} />
                    <Stack className={style.authBarItem}>
                        { !authUser || (authUser && !authUser.name) ?
                            <>
                                <Link to='/login'><NavLink label="Bejelentkezés" /></Link>
                                <Link to='/register'><NavLink label="Regiszráció" /></Link>
                            </> :
                            <>
                                <Title size={20} color='white'>Üdv {authUser.name}!</Title>
                                <Divider />
                                <Link to='/settings'><NavLink label="Beállítások" /></Link>
                                <Divider />
                                <Link to='/rentals'><NavLink label="Bérléseim" /></Link>
                                <Link to='/vehicles'><NavLink label="Járműveim" /></Link>
                                <Divider />
                                <NavLink label="Kijelentkezés" onClick={() => {
                                    setAuthUser(null);
                                    localStorage.removeItem("token");
                                }} />
                            </>
                        }
                    </Stack>
                </div>
            </AppShell.Aside>

            <AppShell.Main>
                {children}
            </AppShell.Main>
        </AppShell>
    </>)
}

export default Navbar;