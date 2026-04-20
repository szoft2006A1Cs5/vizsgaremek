import { useEffect } from 'react'
import { LoadingOverlay } from '@mantine/core';
import { Outlet, useNavigate } from 'react-router-dom';
import { useUser } from '../../assets/scripts/AuthUser';

function EnsureAuth() {
    const navigate = useNavigate();
    const { data: authUser, isLoading: isLoading, isSuccess: isSuccess, isError: isError } = useUser();

    useEffect(() => {
        if ((!isLoading && !authUser) || isError) navigate("/login")
    }, [authUser, isLoading, isError])
    
    return (
        <>
            { isLoading ?
                <LoadingOverlay visible={isLoading} />     
              : 
                (isSuccess && authUser && <Outlet />)
            }
        </>
    );
}

export default EnsureAuth;