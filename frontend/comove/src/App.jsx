import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import Login from "./pages/Login/Login";
import Registration from "./pages/Registration/Registration";
import Cards from './components/Cards/Cards';
import Home from './components/Home/Home'
import Search from './pages/Search/Search'
import Navbar from "./components/Navbar/Navbar";
import '@mantine/core/styles.css';

import { Flex, MantineProvider, NavLink } from '@mantine/core'

export default function App() {

  return (
    
    <MantineProvider>
      <BrowserRouter>
        <Navbar>
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Registration />} />
            <Route path="/cards" element={<Cards />} />
            <Route path="/" element={<Home />} />
            <Route path="/search" element={<Search />} />
          </Routes>
        </Navbar>
      </BrowserRouter>
    </MantineProvider>
  );
}