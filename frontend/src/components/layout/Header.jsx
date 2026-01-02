import { useState } from "react";
import { GetUserInfo } from "../../http";
import { Link } from "react-router-dom";
import { PATH_PUBLIC } from "../../Constant";
import useThemeStore from "../../them";

export default function Header() {
  const userInfo = GetUserInfo();
  const [showDropdown, setShowDropdown] = useState(false);
  const changeMode = useThemeStore((state) => state.changeMode);
  const mode = useThemeStore((state) => state.mode);

  const toggleTheme = () => {
    // 改变全局变量-mode,然后整个layout(sidebar,header, 内容,footer区域)全部颜色改变
    // 用户点击icon--icon改变,并且 颜色也会改变--但是我初始值设置了固定的,比如是白色---然后变成黑色,但是黑色--怎么变回白色呢--一直卡住想不通
    // 我陷入了一个陷阱,那就是改变这个mode变量而已---但是使用这个变量的layout--是根据这个mode的值--然后显示不同的颜色, 而不是mode初始值设置成颜色--哎
    // 这就是定义them.js 文件中: changeMode:()=>set(()=> ({mode: get().mode === "light"? "dark": "light"}))
    changeMode();
  };
  console.log(userInfo);

  return (
    <header className="h-[70px] bg-white border-b border-gray-200 flex items-center justify-between px-8 sticky top-0 z-50">
      <div className="flex-1">
        <h1 className="text-2xl font-semibold text-gray-800">Welcome Back</h1>
      </div>

      <div className="flex items-center gap-5">
        <button
          className="w-10 h-10 bg-gray-100 rounded-lg text-xl hover:bg-gray-200 transition-all hover:scale-105"
          onClick={toggleTheme}
        >
          {mode === "light" ? "☀️" : "🌙"}
        </button>

        <div
          className="relative"
          onMouseEnter={() => setShowDropdown(true)}
          onMouseLeave={() => setShowDropdown(false)}
        >
          {/* 头像 */}
          <div className="w-10 h-10 rounded-full overflow-hidden border-2 border-gray-200 cursor-pointer hover:border-purple-500 transition-colors">
            <img
              src="https://i.pravatar.cc/150?img=12"
              alt="User"
              className="w-full h-full object-cover"
            />
          </div>
          {/* 下拉框 */}
          {showDropdown && (
            <div className="absolute right-0 w-64 bg-white rounded-xl shadow-2xl overflow-hidden animate-[dropdownFade_0.2s_ease]">
              <div className="p-5 flex gap-3 items-center bg-gray-50">
                <div className="w-12 h-12 rounded-full overflow-hidden border-2 border-white">
                  <img
                    src="https://i.pravatar.cc/150?img=12"
                    alt="User"
                    className="w-full h-full object-cover"
                  />
                </div>
                <div className="flex-1">
                  <div className="font-semibold text-sm text-gray-800 mb-1">
                    {userInfo.userName}
                  </div>
                  <div className="text-xs text-gray-500">{userInfo.email}</div>
                </div>
              </div>

              <div className="h-px bg-gray-200 my-2"></div>

              <div
                className="px-5 py-3 flex items-center gap-3 cursor-pointer hover:bg-gray-50 transition-colors text-sm text-gray-700"
                onClick={() => alert("Profile clicked")}
              >
                <span>👤</span>
                <span>Profile</span>
              </div>
              <div
                className="px-5 py-3 flex items-center gap-3 cursor-pointer hover:bg-gray-50 transition-colors text-sm text-gray-700"
                onClick={() => alert("Settings clicked")}
              >
                <span>⚙️</span>
                <span>Settings</span>
              </div>

              <div className="h-px bg-gray-200 my-2"></div>

              {/* <div
                className="px-5 py-3 flex items-center gap-3 cursor-pointer hover:bg-red-50 transition-colors text-sm text-red-600"
                onClick={() => alert("Logout clicked")}
              > */}
              <div className="px-5 py-3 flex items-center gap-3 cursor-pointer hover:bg-red-50 transition-colors text-sm text-red-600">
                <span>🚪</span>
                <span>
                  <Link to={PATH_PUBLIC.login}>Logout</Link>
                </span>
              </div>
            </div>
          )}
        </div>
      </div>
    </header>
  );
}
