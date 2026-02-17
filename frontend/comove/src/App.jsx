import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Login from "./components/Login/Login";
import Registration from "./components/Registration/Registration";
import Cards from './components/Cards/Cards';
import Home from './components/Home/Home'

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Registration />} />
        <Route path="/cards" element={<Cards />} />
        <Route path="/" element={<Home />} />
      </Routes>
    </BrowserRouter>
  );
}