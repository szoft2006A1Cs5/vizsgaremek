import { useState } from "react";
import "./Home.css";

function Home(){
    return(
        <>
            <section className="hero">
                <div className="dots">
                    <span></span><span></span><span></span><span></span><span></span><span></span><span></span><span></span>
                </div>
                <div className="container">
                    <header className="nav">
                        <img className="logo" src="kepek/logo/comove_logo4.png" alt="CoMove"/>

                        <nav className="menu">
                            <a href="#">Főoldal</a>
                            <a href="#">Rólunk</a>
                            <a href="#">Foglalások</a>
                        </nav>
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