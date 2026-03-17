import "./Account.css";
import logo from "../../assets/kepek/logo/comove_logo4.png";
import { useLayoutEffect, useEffect, useState } from "react";

function Account() {
  const [loaded, setLoaded] = useState(false);

  useLayoutEffect(() => {
    window.scrollTo(0, 0);
  }, []);

  useEffect(() => {
    const id = requestAnimationFrame(() => setLoaded(true));
    return () => cancelAnimationFrame(id);
  }, []);

  return (
    <div className="account_root account_root">
      <section
        className={`account_hero account_pageAnim ${loaded ? "isLoaded" : ""}`}
      >
        <img className="account_logo" src={logo} alt="CoMove" />
        <h1 className="account_focim">Fiókbeállítások</h1>
        <p className="account_alcim">Módosítsa adatait</p>
      </section>

      <section
        className={`account_page account_pageAnim ${loaded ? "isLoaded" : ""}`}
      >
        <div style={{ maxWidth: "1200px", margin: "0 auto" }}>
          <div>
            <div className="map">
              <p className="account_tag">Teljes Név</p>
              <input type="text" placeholder="Kiss Károly" />

              <p className="account_tag">Teljes Név</p>
              <input type="text" />

              <p className="account_tag">Teljes Név</p>
              <input type="text" />
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}

export default Account;
