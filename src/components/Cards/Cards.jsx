import { useState } from 'react'
import './Cards.css'

function Cards() {
  return (
    <div className="wrap">
        <div className="top">
            <div>
                <h1>Találatok</h1>
            </div>
        </div>

        <section className="grid" id="grid">
            <article className="card selected">
                <div className="img">
                    <img alt="Toyota Corolla" src="kepek/autok/toyotacorolla1.png"/>
                </div>
                <div className="content">
                    <h3 className="title">Toyota Corolla 1.8</h3>
                    <p className="meta">
                        <i className="fas fa-map-marker-alt"></i>&nbsp;&nbsp;Vas vármegye,
                        &nbsp;<span className="owner">Németh Csaba</span>
                    </p>
                </div>
                <div>
                    <div className="stars">
                        <i className="fas fa-star"></i>
                        <i className="fas fa-star"></i>
                        <i className="fas fa-star"></i>
                        <i className="fas fa-star"></i>
                        <i className="far fa-star"></i>
                        &nbsp;<span className="rating-value">4.0</span>
                    </div>
                </div>
                <div className="pillek">
                    <span className="pillmain">Szombathely</span>
                    <span className="pill">2019</span>
                    <span className="pill">Hybrid</span>
                </div>
                <div className="nextrow">
                    <span className="pill">120LE</span>
                    <span className="pill">215 000 km</span>
                    <span className="pill">Automata</span>
                </div>
                <br/>
                <div className="row">
                    <div className="price">
                    <strong>3.000 Ft <span>/ óra</span></strong>
                    </div>
                    <button className="heart" onclick="this.classNameList.toggle('active')">
                    <i className="fas fa-heart"></i>
                    </button>
                </div>
            </article>
        </section>
    </div>
    )
}

export default Cards