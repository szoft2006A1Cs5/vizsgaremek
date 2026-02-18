import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Login from "./components/Login/Login";
import Registration from "./components/Registration/Registration";
import Cards from './components/Cards/Cards';
import Home from './components/Home/Home'
import Search from './components/Search/Search'
import '@mantine/core/styles.css';

import { MantineProvider } from '@mantine/core'
import Index from "./components/Index/Index";

export default function App() {
  return (
    <MantineProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Registration />} />
          <Route path="/cards" element={<Cards />} />
          <Route path="/" element={<Home />} />
          <Route path="/search" element={<Search />} />
        </Routes>
      </BrowserRouter>
    </MantineProvider>
  );
}