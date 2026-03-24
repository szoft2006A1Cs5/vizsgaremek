import { useEffect } from 'react'
import { LoadingOverlay } from '@mantine/core';
import { Outlet, useNavigate } from 'react-router-dom';
import { useUser } from '../../assets/scripts/AuthUser';

function EnsureAuth() {
    const navigate = useNavigate();
    const { data: authUser, isLoading: isLoading, isSuccess: isSuccess } = useUser();

    useEffect(() => {
        const auth = JSON.parse(localStorage.getItem("auth"));
        if (!auth) navigate("/login");
    }, []);

    useEffect(() => {
        if (isSuccess && !authUser) navigate("/login")
    }, [authUser])
    
    return (
        <>
            { isLoading ?
                <LoadingOverlay visible={isLoading} />     
              : 
                <Outlet />
            }
        </>
    );
}

export default EnsureAuth;