import { useEffect } from 'react'
import { LoadingOverlay } from '@mantine/core';
import { Outlet, useNavigate } from 'react-router-dom';
import { useUser } from '../../assets/scripts/AuthUser';

function EnsureAuth() {
    const navigate = useNavigate();
    const { data: authUser, isLoading: isLoading, isSuccess: isSuccess } = useUser();

    useEffect(() => {
        if (isSuccess && !authUser) navigate("/login")
    }, [authUser, isSuccess])
    
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