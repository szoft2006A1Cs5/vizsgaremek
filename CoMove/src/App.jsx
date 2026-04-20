import { BrowserRouter, Routes, Route, Outlet } from "react-router-dom";
import Navbar from "./components/common/Navbar/Navbar";
import Home from "./pages/Home/Home";
import Searching from "./pages/Searching/Searching";
import Log_Reg from "./pages/Log_Reg/Log_Reg";
import MyVehicles from "./pages/MyVehicles/MyVehicles";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import EnsureAuth from "./components/EnsureAuth/EnsureAuth";
import Vehicle from "./pages/Vehicle/Vehicle";
import AddVehicle from "./pages/AddVehicle/AddVehicle";
import EditVehicle from "./pages/EditVehicle/EditVehicle";
import MyRentals from "./pages/MyRentals/MyRentals";
import Rental from "./pages/Rental/Rental";

const queryClient = new QueryClient();

function LayoutWithNavbar() {
  return (
    <Navbar>
      <Outlet />
    </Navbar>
  );
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route element={<LayoutWithNavbar />}>
            <Route path="/" element={<Home />} />
            <Route path="/search" element={<Searching />} />
            <Route path="/vehicle/:carId" element={<Vehicle />} />

            <Route element={<EnsureAuth />} >
              <Route path="/vehicles" element={<MyVehicles />} />
              <Route path="/vehicles/add" element={<AddVehicle />} />
              <Route path="/vehicle/:carId/edit" element={<EditVehicle />} />
              <Route path="/rentals" element={<MyRentals />} />
              <Route path="/rental/:rentalId" element={<Rental />} />
            </Route>
          </Route>

          <Route path="/login" element={<Log_Reg />} />
          <Route path="/register" element={<Log_Reg />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
}