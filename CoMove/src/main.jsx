import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { MantineProvider } from "@mantine/core";
import { Notifications } from "@mantine/notifications"
import "@mantine/core/styles.css";
import "@mantine/dates/styles.css";
import '@mantine/notifications/styles.css';

import "./index.css";
import App from "./App.jsx";

createRoot(document.getElementById("root")).render(
  <StrictMode>
    <MantineProvider>
      <Notifications />
      <App />
    </MantineProvider>
  </StrictMode>
);