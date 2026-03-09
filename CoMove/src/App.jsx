import { BrowserRouter, Routes, Route, Outlet } from "react-router-dom";
import Navbar from "./components/Navbar/Navbar";
import Home from "./components/Home/Home";
import Searching from "./components/Searching/Searching";
import Log_Reg from "./components/Log_Reg/Log_Reg";
import Cards from "./components/Cards/Cards";

function LayoutWithNavbar() {
  return (
    <Navbar>
      <Outlet />
    </Navbar>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* NAVBAR-ral rendelkező oldalak */}
        <Route element={<LayoutWithNavbar />}>
          <Route path="/" element={<Home />} />
          <Route path="/searching" element={<Searching />} />
          <Route path="/results" element={<Cards />} />
        </Route>

        <Route path="/login" element={<Log_Reg />} />
        <Route path="/register" element={<Log_Reg />} />
      </Routes>
    </BrowserRouter>
  );
}