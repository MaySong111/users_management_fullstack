import { useState } from "react";
import { LOGIN_URL, PATH_DASHBOARD, PATH_PUBLIC } from "../../Constant";
import { Link, useNavigate } from "react-router-dom";
import { apiClient, setToken, setUserInfo } from "../../http";
import { GoLock } from "react-icons/go";
import { GoUnlock } from "react-icons/go";
import { FaRegUser } from "react-icons/fa";
import { IoEyeOffOutline } from "react-icons/io5";
import { IoEyeOutline } from "react-icons/io5";

export default function LoginPage() {
  const redirect = useNavigate();
  const [formData, setFormData] = useState({
    username: "",
    password: "",
    rememberMe: false,
  });
  const [errors, setErrors] = useState({});
  const [showPassword, setShowPassword] = useState(false);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: "" }));
    }
  };

  const validate = () => {
    const newErrors = {};
    if (!formData.username.trim()) {
      newErrors.username = "username is required";
    }

    if (!formData.password) {
      newErrors.password = "Password is required";
    } else if (formData.password.length < 6) {
      newErrors.password = "Password must be at least 6 characters";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (validate()) {
      console.log("Login data:", formData);
      // alert("Login successful!");
      const data = await apiClient.post(LOGIN_URL, formData);
      console.log("login-backend data:", data);

      if (data.isSucceed) {
        // store token in localStorage
        setToken(data.data.token);
        // 疑问: 那登录后----是用navigate传递参数给 要跳转的页面,url或者state, 还是要将用户的信息存储到localstorage中呢,因为主界面可能会用到这些用户信息的????
        redirect(PATH_DASHBOARD.dashboard);
        setUserInfo(data.data.user);
      } else {
        alert(data.message);
      }
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-purple-500 via-purple-600 to-blue-600 p-4">
      <div className="w-full max-w-md bg-white rounded-2xl shadow-2xl p-8">
        {/* Logo/Header */}
        <div className="text-center mb-8">
          <h2 className="text-3xl font-bold text-gray-800 mb-2">Login</h2>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="space-y-5">
          {/* username */}
          <div className="relative">
            {/* <label className="block text-sm font-medium text-gray-700 mb-2">
              User Name
            </label> */}
            <FaRegUser className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
            <input
              type="text"
              name="username"
              value={formData.username}
              onChange={handleChange}
              className={`w-full px-10 py-3 border rounded-lg text-sm outline-none transition-all ${
                errors.username
                  ? "border-red-500 focus:ring-2 focus:ring-red-200"
                  : "border-gray-300 focus:border-purple-500 focus:ring-2 focus:ring-purple-200"
              }`}
              placeholder="Enter your User Name"
            />
            {errors.username && (
              <p className="text-red-500 text-xs mt-1">{errors.username}</p>
            )}
          </div>

          {/* Password */}
          <div>
            {/* <label className="block text-sm font-medium text-gray-700 mb-2">
              Password
            </label> */}
            <div className="relative">
              {showPassword ? (
                <GoUnlock className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
              ) : (
                <GoLock className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
              )}
              <input
                type={showPassword ? "text" : "password"}
                name="password"
                value={formData.password}
                onChange={handleChange}
                className={`w-full px-10 py-3 border rounded-lg text-sm outline-none transition-all ${
                  errors.password
                    ? "border-red-500 focus:ring-2 focus:ring-red-200"
                    : "border-gray-300 focus:border-purple-500 focus:ring-2 focus:ring-purple-200"
                }`}
                placeholder="Enter your password"
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-500 hover:text-gray-700"
              >
                {/* {showPassword ? "👁️" : "👁️‍🗨️"} */}
                {showPassword ?  <IoEyeOutline />:  <IoEyeOffOutline />}
              </button>
            </div>
            {errors.password && (
              <p className="text-red-500 text-xs mt-1">{errors.password}</p>
            )}
          </div>

          {/* Remember Me & Forgot Password */}
          <div className="flex items-center justify-between">
            <label className="flex items-center gap-2 cursor-pointer">
              <input
                type="checkbox"
                name="rememberMe"
                checked={formData.rememberMe}
                onChange={handleChange}
                className="w-4 h-4 text-purple-600 border-gray-300 rounded focus:ring-2 focus:ring-purple-500"
              />
              <span className="text-sm text-gray-600">Remember me</span>
            </label>
            <Link
              to={PATH_PUBLIC.changePassword}
              className="text-sm text-purple-600 hover:text-purple-700"
            >
              Forgot password?
            </Link>
          </div>

          {/* Submit Button */}
          <button
            type="submit"
            className="w-full bg-gradient-to-r from-purple-600 to-blue-600 text-white py-3 rounded-lg font-medium hover:from-purple-700 hover:to-blue-700 transition-all shadow-lg hover:shadow-xl transform hover:scale-[1.02]"
          >
            Sign In
          </button>
        </form>

        {/* Sign Up Link */}
        <p className="text-center text-sm text-gray-600 mt-6">
          Don't have an account?{" "}
          <Link
            to={PATH_PUBLIC.register}
            className="text-purple-600 hover:text-purple-700 font-medium"
          >
            Sign up
          </Link>
        </p>
      </div>
    </div>
  );
}
