import { useState, useEffect } from "react";
import logo from '../../assets/kepek/logo/comove_logo4.png';
import profilepic from '../../assets/kepek/profilepicture.jpg'
import { useNavigate, useLocation, Link, Route } from "react-router-dom";
import { Avatar, Anchor, Group, Container, Menu, Center, Button, Text } from '@mantine/core';
import classes from './Index.module.css'

function Index() {
    return <>
        <header className={classes.indexMenu}>
            <Container size="md">
                <div>
                    <Group gap={5} visibleFrom="sm">
                        <Menu shadow="md" width={200}>
                            <Menu.Target>
                                <Avatar className={classes.avatar} />
                            </Menu.Target>

                            <Menu.Dropdown>
                                <Menu.Item>Application</Menu.Item>
                                <Menu.Item>Beállítások</Menu.Item>
                            </Menu.Dropdown>
                        </Menu>
                    </Group>
                </div>
            </Container>
        </header>
    </>
}

export default Index;