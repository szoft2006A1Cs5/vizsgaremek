import { BrowserRouter, Routes, Route, Outlet, useLocation } from "react-router-dom";
import Navbar from "./components/common/Navbar/Navbar";
import Home from "./pages/Home/Home";
import Searching from "./pages/Searching/Searching";
import MyVehicles from "./pages/MyVehicles/MyVehicles";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import EnsureAuth from "./components/EnsureAuth/EnsureAuth";
import Vehicle from "./pages/Vehicle/Vehicle";
import AddVehicle from "./pages/AddVehicle/AddVehicle";
import EditVehicle from "./pages/EditVehicle/EditVehicle";
import MyRentals from "./pages/MyRentals/MyRentals";
import Rental from "./pages/Rental/Rental";
import Account from "./pages/Account/Account";
import ForgotPassword from "./pages/ForgotPassword/ForgotPassword";
import ResetPassword from "./pages/ResetPassword/ResetPassword";
import LogReg from "./pages/LogReg/LogReg";
import Page404 from "./pages/404/404";
import Footer from "./components/common/Footer/Footer";
import { useEffect } from "react";
import { Stack } from "@mantine/core";
import Admin from "./pages/Admin/Admin";

const queryClient = new QueryClient();

function LayoutWithNavbarFooter() {
  const location = useLocation();

  useEffect(() => {
    requestAnimationFrame(() => {
      window.scrollTo(0, 0)
    })
  }, [location])

  return (
    <>
      <Navbar>
        <Outlet />
      </Navbar>
      <Footer />
    </>
  );
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route path="*" element={<Page404 />} />

          <Route element={<LayoutWithNavbarFooter />}>
            <Route path="/" element={<Home />} />
            <Route path="/search" element={<Searching />} />
            <Route path="/vehicle/:carId" element={<Vehicle />} />

            <Route element={<EnsureAuth />} >
              <Route path="/vehicles" element={<MyVehicles />} />
              <Route path="/vehicle/add" element={<AddVehicle />} />
              <Route path="/vehicle/:carId/edit" element={<EditVehicle />} />
              <Route path="/rentals" element={<MyRentals />} />
              <Route path="/rental/:rentalId" element={<Rental />} />
              <Route path="/account/:userId" element={<Account />} />
              <Route path="/account" element={<Account />} />

              <Route path="/admin" element={<Admin />} />
            </Route>
          </Route>

          <Route path="/forgotpassword" element={<ForgotPassword />} />
          <Route path="/resetpassword" element={<ResetPassword />} />
          <Route path="/login" element={<LogReg />} />
          <Route path="/register" element={<LogReg />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
}