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
    <div className="account_root">
      <section
        className={`account_hero account_pageAnim ${
          loaded ? "isLoaded" : ""
        }`}
      >
        <div className="account_hero_content">
          <h1 className="account_focim">Fiókbeállítások</h1>
          <p className="account_alcim">Módosítsa adatait</p>
        </div>
      </section>

      <section
        className={`account_page account_pageAnim ${
          loaded ? "isLoaded" : ""
        }`}
      >
        <div className="account_card">
          <div className="account_form">
            <label>Teljes név</label>
            <input type="text" placeholder="Kiss Károly" />

            <label>Születési idő</label>
            <input type="date" />

            <label>Lakcím</label>
            <input type="text" />

            <label>Telefonszám</label>
            <input type="text" />

            <label>Forgalmiszám</label>
            <input type="text" />

            <label>E-mail cím</label>
            <input type="text" />

            <label>Jelszó</label>
            <input type="password" />

            <label>Jelszó újra</label>
            <input type="password" />

            <div className="account_buttons">
              <button className="btn_cancel">Mégse</button>
              <button className="btn_save">Mentés</button>
            </div>
          </div>

          <div className="account_avatar">
            <div className="avatar_circle"></div>
            <button className="upload_btn">Kép feltöltése</button>
          </div>
        </div>
      </section>
    </div>
  );
}

export default Account;