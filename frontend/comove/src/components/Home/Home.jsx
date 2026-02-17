import { useState, useEffect } from "react";
import "./Home.css";
import logo from '../../assets/kepek/logo/comove_logo4.png';
import profilepic from '../../assets/kepek/profilepicture.jpg'
import Nav from 'react-bootstrap/Nav';
import NavDropdown from 'react-bootstrap/NavDropdown';
import Navbar from 'react-bootstrap/Navbar';
import { useNavigate, useLocation, Link, Route } from "react-router-dom";

function Home(){
    const [authUser, setAuthUser] = useState(null);
    useEffect(() => {
        let token = localStorage.getItem("token");

        if (!token) return;

        fetch("https://localhost:7245/api/User", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        })
        .then(resp => {
            if (resp.status != 200) {
                return null;
            }

            return resp.json();
        })
        .then(data => {
            setAuthUser(data);
        })
        .catch(_ => {})
    }, [])

    return(
        <>
            <section className="hero">
                <div className="dots">
                    <span></span><span></span><span></span><span></span><span></span><span></span><span></span><span></span>
                </div>
                <div className="container">
                    <header className="nav">
                        <img className="logo" src={logo} alt="CoMove"/>

                        <Navbar className="menu">
                            <a href="#">Főoldal</a>
                            <a href="#">Rólunk</a>
                            <a href="#">Foglalások</a>
                            {authUser ?
                            <Navbar.Collapse>
                                <Nav>
                                    <NavDropdown title={authUser.name} menuVariant="dark">
                                        <NavDropdown.Item>Beállítások</NavDropdown.Item>
                                        <NavDropdown.Item onClick={() => {
                                            localStorage.removeItem("token");

                                            setAuthUser(null);
                                        }}>Kijelentkezés</NavDropdown.Item>
                                    </NavDropdown>
                                </Nav>
                            </Navbar.Collapse>
                            : <Link to='/login'>Bejelentkezés</Link> }
                        </Navbar>
                    </header>
                    <div className="content">
                        <div className="left">
                            <h1>Bérelj olcsón, biztonságosan!</h1>
                            <p>
                                Találd meg az igényeidnek megfelelő autót, mellyel élvezet lesz a vezetés minden perce.
                            </p>
                            <a className="btn" href="#">Továbbiak</a>
                        </div>
                        <div className="right">
                            <div className="carShadow" aria-hidden="true"></div>
                            <div className="car">
                                <img src="https://www.pngall.com/wp-content/uploads/8/White-SUV-PNG.png"/>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </>
    )
}

export default Home