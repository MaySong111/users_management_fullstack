import { useState } from "react";
import { Link } from "react-router-dom";
import { PATH_DASHBOARD } from "../../Constant";

export default function Sidebar({ onCollapse }) {
  const [isCollapsed, setIsCollapsed] = useState(false);

  const menuItems = [
    { icon: "📊", label: "Dashboard", path: PATH_DASHBOARD.dashboard },
    { icon: "📋", label: "Users Management", path: PATH_DASHBOARD.usersManagement },
    { icon: "🔔", label: "Send Message", path: PATH_DASHBOARD.sendMessage },
    { icon: "📚", label: "Inbox", path: PATH_DASHBOARD.inbox },
    { icon: "⭐", label: "All Messages", path: PATH_DASHBOARD.allMessages },
    { icon: "🔧", label: "All Logs", path: PATH_DASHBOARD.systemLogs },
    { icon: "⚙️", label: "My Logs", path: PATH_DASHBOARD.myLogs },
    { icon: "⚙️", label: "Owner Page", path: PATH_DASHBOARD.owner },
    { icon: "⚙️", label: "Admin Page", path: PATH_DASHBOARD.admin },
    { icon: "⚙️", label: "Manager Page", path: PATH_DASHBOARD.manager },
  ];

  const toggleCollapse = () => {
    const newState = !isCollapsed;
    setIsCollapsed(newState);
    if (onCollapse) onCollapse(newState);
  };

  return (
    <div className={`${isCollapsed ? "w-20" : "w-64"} h-screen bg-gray-200 text-gray-800 transition-all duration-300 fixed left-0 top-0 overflow-y-auto z-50 border-r border-gray-300`}>
      
      {/* Logo and Collapse Button */}
      <div className={`flex items-center ${isCollapsed ? "justify-center" : "justify-between"} p-5 border-b border-gray-300`}>
        <div className="w-12 h-12 bg-gray-300 rounded-full flex items-center justify-center font-bold text-xl transition-colors hover:bg-gray-400">
          CV
        </div>
        {!isCollapsed && (
          <button
            className="w-8 h-8 rounded-lg transition-colors hover:bg-gray-400"
            onClick={toggleCollapse}
          >
            ←
          </button>
        )}
      </div>

      {/* Collapse button when sidebar is collapsed */}
      {isCollapsed && (
        <div className="flex justify-center py-3">
          <button
            className="w-8 h-8 rounded-lg transition-colors hover:bg-gray-400"
            onClick={toggleCollapse}
          >
            →
          </button>
        </div>
      )}

      {/* Menu Items */}
      <nav className="py-5">
        {menuItems.map((item, index) => (
          <div key={index} className="mb-2">
            <Link to={item.path}>
              <div
                className={`flex items-center px-5 py-3 cursor-pointer transition-colors rounded-lg ${
                  isCollapsed ? "justify-center" : "gap-3"
                } hover:bg-gray-300`}
              >
                <span className="text-xl">{item.icon}</span>
                {!isCollapsed && <span className="text-sm font-medium">{item.label}</span>}
              </div>
            </Link>
          </div>
        ))}
      </nav>
    </div>
  );
}
