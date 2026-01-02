import { Route, Routes } from "react-router-dom";
import Layout from "./components/layout/Layout";
import { PATH_DASHBOARD, PATH_PUBLIC } from "./Constant";
import HomePage from "./pages/public/HomePage";
import RegisterPage from "./pages/public/RegisterPage";
import LoginPage from "./pages/public/LoginPage";
import ChangePassword from "./pages/public/ChangePassword";
import NotFoundPage from "./pages/public/NotFoundPage";
import UnauthorizedPage from "./pages/public/UnauthorizedPage";
import AuthGard from "./AuthGard";
import Dashboard from "./pages/dashboard/Dashboard.jsx";
import SendMessagePage from "./pages/dashboard/SendMessagePage.jsx";
import InboxPage from "./pages/dashboard/InboxPage.jsx";
import MyLogsPage from "./pages/dashboard/MyLogsPage.jsx";
import UserPage from "./pages/dashboard/UserPage.jsx";
import ManagerPage from "./pages/dashboard/ManagerPage.jsx";
import AdminPage from "./pages/dashboard/AdminPage.jsx";
import UpdateRolePage from "./pages/dashboard/UpdateRolePage.jsx";
import AllMessagesPage from "./pages/dashboard/AllMessagesPage.jsx";
import SystemLogPage from "./pages/dashboard/SystemLogPage.jsx";
import UsersManagementPage from "./pages/dashboard/UsersManagementPage.jsx";
import OwnerPage from "./pages/dashboard/OwnerPage.jsx";
// 我定义路由是路由-- 和显示没关系--我一直总之搞错---  这个路由就是 以后 在这个位置  定义路由的地方 显示这个组件

function App() {
  return (
    <>
      {/*  两种路由: 公开路由, 注册,登录页面   第二种才是受保护的页面--那这个是登录后才显示的页面比如有Layout - Sidebar + Header + 中间的content + Footer*/}
      <Routes>
        {/* 第一种: 公开路由 - 没有布局 */}
        <Route path={PATH_PUBLIC.home} element={<HomePage />} />
        <Route path={PATH_PUBLIC.register} element={<RegisterPage />} />
        <Route path={PATH_PUBLIC.login} element={<LoginPage />} />
        <Route path={PATH_PUBLIC.changePassword} element={<ChangePassword />} />
        <Route path={PATH_PUBLIC.notFound} element={<NotFoundPage />} />
        <Route path={PATH_PUBLIC.unauthorized} element={<UnauthorizedPage />} />

        {/* 第二种: protected routes --将后面所有的全部包裹*/}
        {/* element={<AuthGard />} 这里就是调用函数,  没有传递参数, 但是这个定义的函数有形参,--这调用函数就是: 传递了一个undefined的参数--定义的函数的形式参数接收的参数就是undefined--if (requiredRole) {这就不符合了所以也就不会进入到这个判断*/}
        <Route element={<AuthGard />}>
          <Route element={<Layout />}>
            <Route path={PATH_DASHBOARD.dashboard} element={<Dashboard />} />
            <Route
              path={PATH_DASHBOARD.usersManagement}
              element={<UsersManagementPage />}
            />
            <Route
              path={PATH_DASHBOARD.sendMessage}
              element={<SendMessagePage />}
            />
            <Route path={PATH_DASHBOARD.inbox} element={<InboxPage />} />
            <Route
              path={PATH_DASHBOARD.allMessages}
              element={<AllMessagesPage />}
            />
            <Route
              path={PATH_DASHBOARD.systemLogs}
              element={<SystemLogPage />}
            />
            <Route path={PATH_DASHBOARD.myLogs} element={<MyLogsPage />} />
            <Route path={PATH_DASHBOARD.user} element={<UserPage />} />

            {/* protected routes ---required role:manager route*/}
            <Route element={<AuthGard requiredRole="manager" />}>
              <Route path={PATH_DASHBOARD.manager} element={<ManagerPage />} />
            </Route>

            {/* protected routes ---required role:admin route*/}
            <Route element={<AuthGard requiredRole="admin" />}>
              <Route path={PATH_DASHBOARD.admin} element={<AdminPage />} />
              <Route
                path={PATH_DASHBOARD.updateRole}
                element={<UpdateRolePage />}
              />
              <Route
                path={PATH_DASHBOARD.allMessages}
                element={<AllMessagesPage />}
              />
              <Route
                path={PATH_DASHBOARD.systemLogs}
                element={<SystemLogPage />}
              />
              <Route
                path={PATH_DASHBOARD.usersManagement}
                element={<UsersManagementPage />}
              />
            </Route>

            {/* protected routes ---required role:owner route*/}
            <Route element={<AuthGard requiredRole="owner" />}>
              <Route path={PATH_DASHBOARD.owner} element={<OwnerPage />} />
            </Route>
            {/* 404那是 输入的url就没有定义,  未授权那是定义了路由,但是输入这个url的请求后--那这个请求不符合角色显示打不开这个页面*/}
            {/* except all other route path , if not meets all above routes, then this will catch the url */}
            {/* <Route
              path="/*"
              element={<Navigate to={PATH_PUBLIC.notFound} replace />}
            />
            用上面的话会造成很多问题, 我localstorage没有token,也没用户信息--我这个时候输入url: dashboard的,还是不存在的url--都是到登录
            这不对, 应该是 未授权, 不存在的,所以才改成下面的
          {/* * 匹配 所有未定义路径，直接渲染 NotFound 页面, 不要用 Navigate 再跳一次，这样就不会被 AuthGuard 的登录逻辑再次拦截 */}
          </Route>
        </Route>
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </>
  );
}
export default App;
