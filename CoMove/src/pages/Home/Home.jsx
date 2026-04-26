import "./Home.css";
import vazkep from "../../assets/kepek/egyeb/vazkep.png";
import icon_1 from "../../assets/kepek/egyeb/icon1.png";
import icon_2 from "../../assets/kepek/egyeb/icon2.png";
import icon_3 from "../../assets/kepek/egyeb/icon3.png";
import vazkep2 from "../../assets/kepek/egyeb/vazkep2.png";
import cucu from "../../assets/kepek/egyeb/mb.jpg";
import lien from '../../assets/kepek/egyeb/dml.jpg'
import aron from '../../assets/kepek/egyeb/gaa.jpg'
import { FaFacebookF, FaInstagram } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { useLayoutEffect, useEffect, useState } from "react";

function Home() {
  const navigate = useNavigate();
  const [loaded, setLoaded] = useState(false);
  useLayoutEffect(() => {
    window.scrollTo(0, 0);
  }, []);
  useEffect(() => {
    const id = requestAnimationFrame(() => setLoaded(true));
    return () => cancelAnimationFrame(id);
  }, []);

  /*rolunk animacio*/
  useEffect(() => {
    const elements = document.querySelectorAll(".scroll-animate");
    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add("show");
          }
        });
      },
      { threshold: 0.2 },
    );
    elements.forEach((el) => observer.observe(el));
    return () => observer.disconnect();
  }, []);

  const [showFloatingTop, setShowFloatingTop] = useState(false);
  useEffect(() => {
    const handleScroll = () => {
      if (window.scrollY > 200) {
        setShowFloatingTop(true);
      } else {
        setShowFloatingTop(false);
      }
    };

    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  return (
    <>
      <div className="home_root">
        {/*Fejlec*/}
        <section
          className={`home_hero home_pageAnim ${loaded ? "isLoaded" : ""}`}
        >
          <div className="home_dots" aria-hidden="true">
            <span></span>
            <span></span>
            <span></span>
            <span></span>
            <span></span>
            <span></span>
            <span></span>
            <span></span>
            <span></span>
          </div>

          <div className="home_container">
            <div className="home_content">
              <div className="home_left">
                <h1>
                  Bérelj olcsón,
                  <br />
                  biztonságosan!
                </h1>

                <p>
                  Találd meg az igényeidnek megfelelő autót, mellyel élvezet
                  lesz a vezetés minden perce.
                </p>

                <button
                  className="home_btn"
                  type="button"
                  onClick={() => navigate("/search")}
                >
                  Béreljen itt
                </button>
              </div>
              <div className="home_right">
                <div className="home_carShadow" aria-hidden="true"></div>
                <img
                  className="home_carImg"
                  src="https://www.pngall.com/wp-content/uploads/8/White-SUV-PNG.png"
                  alt="Autó"
                  loading="lazy"
                />
              </div>
            </div>
          </div>
        </section>

        {/*Rolunk*/}
        <section className="home_rolunk_section scroll-animate">
          <div className="home_rolunk_container scroll-animate">
            <div className="home_rolunk_images">
              <img src={vazkep} alt="CoMove - bemutatkozó kép" loading="lazy" />
            </div>

            <div className="home_rolunk_content">
              <h2 className="home_line">Rólunk</h2>
              <h1>
                Közösségi autóbérlés
                <br />
                <span style={{ color: "#2136bd" }}>egyszerűen</span> és
                biztonságosan
              </h1>
              <p>
                A CoMove egy olyan platform, ahol a tulajdonosok könnyedén
                kiadhatják autóikat, a bérlők pedig pár kattintással
                megtalálhatják az igényeikhez legjobban illő járművet.
              </p>
              <p>
                A CoMove-nál egyszerre lehetsz bérlő és bérbeadó is: ha épp
                autóra van szükséged, bérelsz, ha pedig van szabad kapacitásod,
                kiadhatod a sajátodat. Olcsó, gyors és átlátható — értékelési
                rendszerrel tesszük megbízhatóvá a közösségi
                autóhasználatot.
              </p>
              <div className="home_rolunk_ev">
                <div className="home_rolunk_border">
                  <div className="home_rolunk_evmunka">1</div>
                </div>
                <div className="home_rolunk_evmunka_szoveg">
                  Éve építjük a CoMove közösséget
                </div>
              </div>
            </div>
          </div>
        </section>

        {/*Miert mi*/}
        <section className="home_miert_section">
          <div className="home_hatter">
            <div className="home_projekt_container scroll-animate">
              <div className="home_text-projekt">
                <h2 className="home_line">Miért válaszd a CoMove-ot?</h2>
                <h1>
                  Miért bízhatsz meg bennünk a közösségi{" "}
                  <span style={{ color: "#2136bd" }}>autóbérlés</span> során?
                </h1>
                <p>
                  Célunk, hogy átlátható folyamatokkal, korrekt szabályokkal és
                  megbízható közösséggel segítsük a felhasználókat, legyen szó
                  rövid távú bérlésről vagy hosszabb használatról. A CoMove-nál
                  minden funkciót úgy alakítottunk ki, hogy időt spórolj, pénzt
                  takaríts meg, és nyugodtan intézhesd a bérlést vagy a kiadást.
                </p>
                <div className="home_resz">
                  <img src={icon_1} />
                  <div>
                    <h3>Két szerep, egy platform</h3>
                    <p>
                      A CoMove egyik legnagyobb előnye, hogy egyetlen fiókkal
                      lehetsz bérlő és bérbeadó is. Ha autóra van szükséged,
                      könnyedén szerezhetsz, ha pedig van egy szabad autód,
                      egyszerűen meghirdethetedés bevételt szerezhetsz vele.
                    </p>
                  </div>
                </div>
                <div className="home_resz">
                  <img src={icon_2} />
                  <div>
                    <h3>Gyors és egyszerű használat</h3>
                    <p>
                      Tudjuk, hogy senki sem szeret bonyolult folyamatokat,
                      ezért a CoMove-ot úgy alakítottuk ki, hogy a bérlés és a
                      kiadás gyors és átlátható legyen. A keresés, a foglalás és
                      a kommunikáció egy helyen, egyszerű lépésekben történik.
                    </p>
                  </div>
                </div>
                <div className="home_resz">
                  <img src={icon_3} />
                  <div>
                    <h3>Biztonság és megbízhatóság</h3>
                    <p>
                      A közösségi autóbérlés alapja a bizalom, ezért a CoMove
                      kiemelten kezeli a biztonságot. Az értékelési rendszer
                      segít a megbízható felhasználók kiválasztásában.
                    </p>
                  </div>
                </div>
              </div>
              <div className="home_image-content">
                <img src={vazkep2} alt="Ifopontfoglalas" />
              </div>
            </div>
          </div>
        </section>

        {/*Csapat*/}
        <section className="home_csapattagok_resz">
          <div className="scroll-animate">
            <h2 className="home_kiscim">Csapat tagjai</h2>
            <h1 className="home_focim2">
              A CoMove alapítói és szakmai <br />
              vezetői
            </h1>
            <div className="home_csapat-container">
              <div className="home_csapattag scroll-animate">
                <img src={cucu} alt="Arckép" />
                <div className="home_social-linkek">
                  <a
                    href="https://www.facebook.com/profile.php?id=100058838472197"
                    target="_blank"
                    aria-label="Facebook"
                  >
                    <FaFacebookF />
                  </a>
                  <a
                    href="https://www.instagram.com/bence_megyeri/"
                    target="_blank"
                    aria-label="Instagram"
                  >
                    <FaInstagram />
                  </a>
                </div>
                <h3>Megyeri Bence</h3>
                <p>Frontend Developer</p>
              </div>
              <div className="home_csapattag scroll-animate">
                <img src={lien} alt="Arckép" />
                <div className="home_social-linkek">
                  <a
                    href="https://www.facebook.com/mariah.llianne"
                    target="_blank"
                    aria-label="Facebook"
                  >
                    <FaFacebookF />
                  </a>
                  <a
                    href="https://www.instagram.com/yantoyyyy/"
                    target="_blank"
                    aria-label="Instagram"
                  >
                    <FaInstagram />
                  </a>
                </div>
                <h3>Darunday Mariah Llianne</h3>
                <p>Frontend Developer</p>
              </div>
              <div className="home_csapattag scroll-animate">
                <img src={aron} alt="Arckép" />
                <div className="home_social-linkek">
                  <a
                    href="https://www.facebook.com/aron.geosits"
                    target="_blank"
                    aria-label="Facebook"
                  >
                    <FaFacebookF />
                  </a>
                  <a
                    href="https://www.instagram.com/"
                    target="_blank"
                    aria-label="Instagram"
                  >
                    <FaInstagram />
                  </a>
                </div>
                <h3>Geosits Áron András</h3>
                <p>Backend & Database Engineer</p>
              </div>
            </div>
          </div>
        </section>

        <button
          className={`footerToTop ${showFloatingTop ? "floating" : ""}`}
          onClick={() => window.scrollTo({ top: 0, behavior: "smooth" })}
        >
          ↑
        </button>
      </div>
    </>
  );
}

export default Home;
