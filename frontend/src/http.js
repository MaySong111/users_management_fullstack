import { BASE_HOST_URL } from "./Constant";

// 从 localStorage 获取 token
export function getToken() {
  return localStorage.getItem("accesstoken");
}
export function getRoles() {
  const user = GetUserInfo();
  return user?.role || null;
}

// 存储token
export function setToken(token) {
  return localStorage.setItem("accesstoken", token);
}

// 从 localStorage 获取 已登录的用户信息
export function GetUserInfo() {
  return JSON.parse(localStorage.getItem("userInfo"));
}

// 存储已登录的用户信息
export function setUserInfo(userInfo) {
  return localStorage.setItem("userInfo", JSON.stringify(userInfo));
}

// 通用请求函数
async function request(urlPath, { method = "GET", body, headers = {} } = {}) {
  const token = getToken();

  // 默认 headers
  const defaultHeaders = {
    "Content-Type": "application/json",
    ...headers,
  };

  // 如果有 token，添加 Authorization
  if (token) {
    defaultHeaders["Authorization"] = `Bearer ${token}`;
  }

  const config = {
    method,
    headers: defaultHeaders,
  };

  if (body) {
    config.body = JSON.stringify(body);
  }
  
    const res = await fetch(`${BASE_HOST_URL}${urlPath}`, config);

    // HTTP 状态码 → 判断网络层面的错误（401/403/500）
    // 业务状态码（IsSucceed）→ 判断业务逻辑的成功/失败
    // 401 或 403 可以统一处理
    if (res.status === 401 || res.status === 403) {
      // token 过期或无权限，清除 token 或跳转登录
      localStorage.removeItem("token");
      window.location.href = "/login"; // 或者用你的路由跳转
      return;
    }

    if (res.status >= 500) {
      alert("Internal Server Error");
      return;
    }
    // 业务状态（后端返回的)
    const data = await res.json();
    return data
}

// 简化 GET/POST/PUT/DELETE 方法
export const apiClient = {
  get: (urlPath) => request(urlPath),
  post: (urlPath, body) => request(urlPath, { method: "POST", body }),
  put: (urlPath, body) => request(urlPath, { method: "PUT", body }),
  del: (urlPath) => request(urlPath, { method: "DELETE" }),
};
