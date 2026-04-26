# CoMove - technikumi vizsgaremek

## Röviden a projektről
A CoMove egy közösségi járműkölcsönző szolgáltatás, ez lényegében azt jelenti, hogy a nagy járműflottával rendelkező szolgáltatók helyett, a CoMove-on a felhasználók más felhasználók járműveit bérelhetik ki.

## Műszaki háttér és futtatás
- a backend ASP.NET Core 8.0-án alapul, ezt a `backend/backend.sln` Visual Studio Solution-ből ajánlott elindítani.
- a frontend React-Vite-ot használ, Mantine komponenskönyvtárral, Tanstack Query-vel és React Router-rel. Ezekhez szükséges telepíteni a Node.js 24.x.x verzióját. Az alábbi módon lehet elindítani:
    1. A `CoMove/` mappába lépve adjuk ki egy parancsértelmezőben az `npm install` parancsot a szükséges csomagok letöltéséhez.
    2. Miután azok települtek elindíthatjuk azt fejlesztői módban az `npm run dev` paranccsal vagy statikus JS/HTML fájlokká építhetjük az `npm run build` paranccsal, ekkor a `CoMove/dist/` mappa alá kerülnek a létrejött fájlok.

## Mappa- és fájlstruktúra
- `backend/`: ebben a mappában található a szoftver backend részével kapcsolatos kód, ez az alábbi módon oszlik fel:
    - `backend/`: maga a backend forráskódját és Visual Studio projektjét tartalmazó mappa
    - `backend.UnitTests/`: a backend API végpontjainak metódusait tesztelő egységteszteket tartalmazó VS projekt
    - `loadtest/`: a [locust](https://github.com/locustio/locust)-on alapuló stresszteszteket tartalmazó `locustfile.py`-t tartalmazza
    - `backend.sln`: a Visual Studio Solution, amellyel a backend és egységtesztei indíthatóak, debugolhatóak
- `CoMove/`: ez a mappa tartalmazza a frontendhez tartozó JS/JSX/CSS React-Vite forráskódot:
    - `src/`: a forráskódot tartalmazó mappa:
    - `package.json`: a frontend projekthez szükséges npm csomagokat tartalmazó JSON fájl.
- `comove.sql`: a MySQL szerverre importálandó adatbázis-dump fájl
- `docs/`: a projekt dokumentációját tartalmazó mappa
    - `CoMove szoftveralkalmazás dokumentációja.docx`: Word fájl, a szoftvert és tesztelését részletesen bemutató dokumentáció
    - `ER-modell.png`: az adatbázis ER-modell diagramja
    - `lekepezes.png`: a leképezés után létrejövő adatbázis tábláit és azok kapcsolatait bemutató diagram
    - `stresszteszt.png`: Az egyik futtatott stresszteszt eredményeit mutató kép
    - ezek mellett a mappában még megtalálható pár stresszteszt locust-ból kiexportált eredménye HTML fájlokban
- `.github/workflows/buildtestdeploy.yml`: A CI/CD GitHub Action műveleteket meghatározó fájl, amely felépíti, teszteli, majd közzéteszi (deploy) a projektet a [comove.app](https://comove.app) oldalon.

