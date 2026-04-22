import { Transition } from "@mantine/core";
import CenteredCard from "../../components/common/CenteredCard/CenteredCard";
import { useLocation } from "react-router-dom";
import { useEffect, useRef, useState } from "react";
import Login from "../../components/LogReg/Login";
import Register from "../../components/LogReg/Register";

function LogReg() {
    const location = useLocation();
    const [path, setPath] = useState(location.pathname);
    const [mounted, setMounted] = useState(true);
    const firstRender = useRef(true);
    const animationDuration = 300;

    useEffect(() => {
        if (firstRender.current) {
            firstRender.current = false;
            setMounted(true);
            return () => {};
        }

        setMounted(false);
        const transitionTimeout = setTimeout(() => {
            setPath(location.pathname);
            setMounted(true);
        }, animationDuration);
        return () => clearTimeout(transitionTimeout);
    }, [location]);

    return (
        <CenteredCard maw={700}>
            <Transition 
                mounted={mounted} 
                transition={path === "/login" ? 'fade-left' : 'fade-right'} 
                duration={animationDuration} 
                timingFunction="ease"
            >
                {(transitionStyle) => (
                    path === '/login'
                    ? <Login style={transitionStyle} />
                    : <Register style={transitionStyle} />
                )}
            </Transition>
        </CenteredCard>
    )
}

export default LogReg;