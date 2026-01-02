import { useState } from "react";
import Sidebar from "./Sidebar";
import Header from "./Header";
import Footer from "./Footer";
import { Outlet } from "react-router-dom";
import useThemeStore from "../../them";

export default function Layout() {
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const mode = useThemeStore((state) => state.mode);

  return (
    <div className={mode === "dark" ? "dark" : ""}>
      <div className="flex min-h-screen bg-gray-50">
        {/* Sidebar - 固定在左边 */}
        <Sidebar onCollapse={setIsSidebarCollapsed} />

        {/* 主内容区 - 根据sidebar状态调整margin */}
        <div
          className={`flex-1 flex flex-col transition-all duration-300 ${
            isSidebarCollapsed ? "ml-20" : "ml-64"
          }`}
        >
          {/* Header */}
          <Header />

          {/* 主要内容区域 */}
          <main className="flex-1 p-6 md:p-8">
            <Outlet />
          </main>

          {/* Footer */}
          <Footer />
        </div>
      </div>
    </div>
  );
}
