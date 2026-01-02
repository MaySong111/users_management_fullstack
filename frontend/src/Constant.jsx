//frontend
export const PATH_PUBLIC = {
  home: "/",
  register: "/register",
  login: "/login",
  changePassword:"/changepassword",
  unauthorized: "/unauthorized",
  notFound: "/404"
};

export const PATH_DASHBOARD = {
  dashboard: "/dashboard",
  usersManagement: "/dashboard/users-management",
  updateRole: "/dashboard/update-role/:userName",
  sendMessage: "/dashboard/send-message",
  inbox: "/dashboard/inbox",
  allMessages: "/dashboard/all-messages",
  systemLogs: "/dashboard/system-logs",
  myLogs: "/dashboard/my-logs",
  owner: "/dashboard/owner",
  admin: "/dashboard/admin",
  manager: "/dashboard/manager",
  user: "/dashboard/user"
};


// 视频4h:22时,这下面的我没懂
// auth routes
export const PATH_AFTER_REGISTER = PATH_PUBLIC.login
export const PATH_AFTER_LOGN = PATH_PUBLIC.dashboard
export const PATH_AFTER_LOGOUT = PATH_PUBLIC.home



// backend
export const BASE_HOST_URL = "https://localhost:7109/api"
export const REGISTER_URL = "/auth/register"
export const LOGIN_URL = "/auth/login"
// export const ME_URL = "/auth/me"
export const USERS_LIST_URL = "/auth/users"
export const UPDATE_ROLE_URL = "/auth/update-role"
export const USERNAMES_LIST_URL = "/auth/usernames"
export const ALL_MESSAGES_URL = "/messages"
export const CREATE_MESSAGE_URL = "/messages/create"
export const MY_MESSAGE_URL = "/messages/mine"
export const LOGS_URL = "/logs"
export const MY_LOGS_URL = "/logs/mine"












