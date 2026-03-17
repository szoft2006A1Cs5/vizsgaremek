import "./Account.css";
import logo from "../../assets/kepek/logo/comove_logo1.png";
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
        <h1>Fiókbeállítások</h1>
      </section>

      <section
        className={`account_page account_pageAnim ${loaded ? "isLoaded" : ""}`}
      >
        <div style={{ maxWidth: "1200px", margin: "0 auto" }}>
          {/* IDE JÖN MAJD A PROFIL */}
          <div>
            <div className="map"></div>
          </div>
        </div>
      </section>
    </div>
  );
}

export default Account;
